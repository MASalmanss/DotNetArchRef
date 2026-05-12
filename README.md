# DotNetArchRef

> .NET 8 Web API referans projesi. Domain-Driven Design, Clean Architecture ve enterprise pattern'ların production kalitesinde nasıl bir arada çalıştığını göstermek için tasarlanmıştır.

---

## Mimari Genel Bakış

Proje **Clean Architecture** prensiplerine göre dört katmana ayrılmıştır. Bağımlılık oku yalnızca içe doğru akar — hiçbir iç katman dış katmanı referans almaz.

```
DotNetArchRef.Domain          ← Çekirdek; sıfır dış bağımlılık
DotNetArchRef.Application     ← Use case koordinasyonu; yalnızca Domain'e bağımlı
DotNetArchRef.Infrastructure  ← EF Core, cache, logging, event dispatch; Application'a bağımlı
DotNetArchRef.Api             ← Controller'lar, filtreler, validatörler; tüm katmanlara bağımlı
```

---

## Uygulanan Pattern'lar

### 1. Domain-Driven Design (DDD)

#### Value Objects

`string` veya `decimal` yerine anlam taşıyan tipler kullanılır. Her Value Object kendi geçerlilik kuralını içselleştirir; geçersiz bir örnek hiçbir zaman oluşturulamaz.

```csharp
public sealed record ISBN
{
    public string Value { get; }
    private ISBN(string value) => Value = value;

    public static ISBN Create(string value)
    {
        if (!Regex.IsMatch(value, @"^[0-9\-]{10,20}$"))
            throw new DomainException("ISBN yalnızca rakam ve tire içermeli, 10-20 karakter olmalıdır.");
        return new ISBN(value);
    }

    // EF Core Value Converter için — DB'den gelen veri DomainException değil
    // DataCorruptionException fırlatır; ikisi farklı HTTP koduna düşer
    public static ISBN FromDatabase(string value) { ... }
}
```

Aynı yapı `Email` ve `Money` için de geçerlidir. `Money.Amount` hiçbir zaman negatif olamaz; `Email` geçersiz bir formatta asla var olamaz.

#### Rich Domain Model

Entity'ler `private set` kullanır. Durum değişikliği yalnızca factory method ve domain metotları aracılığıyla olur:

```csharp
public class Book : BaseEntity
{
    public string Title { get; private set; }
    public ISBN ISBN { get; private set; }
    public Money Price { get; private set; }

    private Book() { }  // EF Core için

    public static Book Create(string title, ISBN isbn, Money price, int authorId) { ... }
    public void UpdateDetails(string title, Money price) { ... }
}
```

#### Aggregate Root

`Author`, kendi `Book` koleksiyonunu yöneten Aggregate Root'tur. Kitap oluşturma işlemi doğrudan `BookRepository`'ye gitmez; `Author` üzerinden geçer. Bu sayede domain kuralları merkezi bir noktada korunur:

```csharp
public class Author : BaseEntity
{
    private readonly List<Book> _books = [];
    public IReadOnlyCollection<Book> Books => _books.AsReadOnly();

    public Book AddBook(string title, ISBN isbn, Money price)
    {
        if (_books.Count >= 20)
            throw new DomainException("Bir yazar en fazla 20 kitaba sahip olabilir.");

        var book = Book.Create(title, isbn, price, Id);
        _books.Add(book);

        AddDomainEvent(new BookAddedEvent(this, book));  // yan etki tetiklenir
        return book;
    }
}
```

Servis kodu `author.AddBook(...)` çağırır — kapasite kontrolünü bilmek zorunda değildir.

#### Domain Events

Entity'ler, gerçekleştirdikleri önemli olayları `BaseEntity` üzerindeki bir liste aracılığıyla kayıt altına alır. `DbContext.SaveChangesAsync` override edilerek kayıt sonrası bu event'ler toplanır ve dispatch edilir.

```csharp
public abstract class BaseEntity
{
    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyList<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    protected void AddDomainEvent(IDomainEvent e) => _domainEvents.Add(e);
    public void ClearDomainEvents() => _domainEvents.Clear();
}
```

Tanımlı event'ler:

| Event | Tetikleyen | Handler |
|---|---|---|
| `AuthorCreatedEvent` | `Author.Create()` | `AuthorCreatedEventHandler` → log |
| `BookAddedEvent` | `Author.AddBook()` | `BookAddedEventHandler` → log |

`DomainEventDispatcher`, DI container'ı message bus olarak kullanır. `Author`, `BookAddedEvent` fırlattıktan sonra kimin dinlediğini bilmez — yeni bir handler eklemek için hiçbir entity veya servis değiştirilmez.

---

### 2. Result Pattern

Servisler exception fırlatmak yerine `Result<T>` döner. Kontrol akışı tip sistemiyle sağlanır; try-catch bloğu yoktur.

```csharp
public async Task<Result<BookDto>> GetByIdAsync(int id, CancellationToken ct)
{
    var book = await _uow.Books.GetByIdAsync(id, ct);
    if (book is null)
        return Error.NotFound($"Book with id {id} not found.");  // implicit Error → Result<T>

    return BookMapper.ToDto(book, author?.Name ?? string.Empty); // implicit T → Result<T>
}
```

`Error` tipi dört kategori tanır:

| ErrorType | HTTP Status |
|---|---|
| `NotFound` | 404 |
| `Conflict` | 409 |
| `Validation` | 422 |
| `Unexpected` | 500 |

---

### 3. Decorator Pattern — Cache ve Logging

Cross-cutting concern'ler (cache, logging) servis implementasyonuna dokunulmadan eklenir. Her concern kendi decorator sınıfında yaşar; zincir DI container'da kurulur.

```
Controller
    ↓
BookServiceLoggingDecorator   ← her metod başında/sonunda Serilog ile log
    ↓
BookServiceCacheDecorator     ← IMemoryCache; GET → cache; POST/PUT/DELETE → invalidate
    ↓
BookService                   ← asıl iş mantığı; cache veya log bilgisi yok
    ↓
Repository → Database
```

DI kaydı zinciri açıkça kurar:

```csharp
services.AddScoped<IBookService>(sp =>
{
    IBookService cached = new BookServiceCacheDecorator(
        sp.GetRequiredService<BookService>(),
        sp.GetRequiredService<IMemoryCache>());

    return new BookServiceLoggingDecorator(cached,
        sp.GetRequiredService<ILogger<BookServiceLoggingDecorator>>());
});
```

Cache stratejisi: `GetAll` ve `GetById` sonuçları 5 dakika cache'lenir. `Create`, `Update`, `Delete` başarılı olursa ilgili entry'ler invalidate edilir. `GetPagedAsync` pass-through — kombinasyon sayısı fazla, stale veri riski yüksek.

---

### 4. Unit of Work

`SaveChangesAsync`, repository implementasyonlarından kaldırılmıştır. Her use case tek bir `CommitAsync` çağrısıyla transaction'ı sonlandırır:

```csharp
await _uow.Books.AddAsync(book, ct);
await _uow.CommitAsync(ct);  // tek nokta — DbContext.SaveChangesAsync + Domain Event dispatch
```

---

### 5. Repository Pattern

Generic `IReadRepository<T>` ve `IWriteRepository<T>` interface'leri, domain-spesifik interface'lerle genişletilir. Application katmanı yalnızca soyutlamalara bağımlıdır:

```
IReadRepository<T>   ← GetAllAsync, GetByIdAsync, GetPagedAsync
IWriteRepository<T>  ← AddAsync, Update, Delete
IRepository<T>       ← her ikisini birleştirir
IBookRepository      ← GetByIsbnAsync ekler
IAuthorRepository    ← GetByEmailAsync ekler
```

---

### 6. Specification Pattern

Filtreleme ve sıralama mantığı servis katmanından ayrılır. `ISpecification<T>`, strongly-typed `ApplyOrdering` metodu ile boxing sorununu ortadan kaldırır — `Expression<Func<T, object>>` kullanılmaz, EF Core her expression'ı SQL'e çevirebilir:

```csharp
public override IQueryable<Book> ApplyOrdering(IQueryable<Book> query)
    => (_orderBy, _descending) switch
    {
        ("price", false) => query.OrderBy(b => b.Price.Amount),
        ("price", true)  => query.OrderByDescending(b => b.Price.Amount),
        ("title", false) => query.OrderBy(b => b.Title),
        _                => query
    };
```

---

### 7. Structured Logging — Serilog

`Serilog.AspNetCore` entegrasyonu ile her servis çağrısı structured log üretir. `BookServiceLoggingDecorator`, hem başarı hem hata senaryolarını farklı severity'de kaydeder:

```
[INF] GetAllBooks completed: 15 books returned
[WRN] GetBookById failed: 42 - NotFound: Book with id 42 not found.
[INF] Domain Event: AuthorCreated — Id=3, Name=Orhan Pamuk, Email=o@pamuk.com
```

Loglar konsola ve `logs/app-TARIH.log` dosyasına yazılır (günlük rolling).

---

### 8. FluentValidation + Global ValidationFilter

Tüm validatörler `Api` katmanındadır — `Application` katmanında FluentValidation bağımlılığı yoktur. `ValidationFilter`, controller metoduna ulaşmadan önce `IValidator<T>`'yi otomatik çözümler:

```csharp
RuleFor(x => x.ISBN)
    .NotEmpty().WithMessage("ISBN boş bırakılamaz.")
    .Matches(@"^[0-9\-]{10,20}$").WithMessage("ISBN yalnızca rakam ve tire içermeli.");
```

---

### 9. Chain-of-Responsibility Exception Handler

Business logic exception fırlatmaz. Ancak infrastructure hataları (DB bağlantısı, data corruption) middleware zinciri tarafından yakalanır:

```
DomainExceptionHandler          → DomainException           → 422
DataCorruptionExceptionHandler  → DataCorruptionException    → 500
NotFoundExceptionHandler        → KeyNotFoundException       → 404
ConflictExceptionHandler        → DbUpdateException          → 409
DefaultExceptionHandler         → her şey                   → 500
```

`DataCorruptionException` ayrımı kritiktir: DB'den gelen bozuk veri 422 döndürmez — bu bir client hatası değil, infrastructure hatasıdır.

---

## Proje Yapısı

```
src/
├── DotNetArchRef.Domain/
│   ├── Common/
│   │   ├── BaseEntity.cs           ← Id, audit fields, domain event listesi
│   │   ├── IDomainEvent.cs
│   │   ├── IDomainEventHandler.cs
│   │   └── IDomainEventDispatcher.cs
│   ├── Entities/
│   │   ├── Author.cs               ← Aggregate Root; AddBook(), domain event
│   │   └── Book.cs                 ← Rich model; private setters
│   ├── Events/
│   │   ├── AuthorCreatedEvent.cs
│   │   └── BookAddedEvent.cs
│   ├── ValueObjects/
│   │   ├── ISBN.cs                 ← Create() + FromDatabase()
│   │   ├── Email.cs
│   │   └── Money.cs
│   └── Exceptions/
│       ├── DomainException.cs
│       └── DataCorruptionException.cs
│
├── DotNetArchRef.Application/
│   ├── Common/
│   │   ├── Result.cs               ← Result<T> + implicit conversions
│   │   ├── Error.cs                ← factory methods: NotFound, Conflict...
│   │   ├── ErrorType.cs
│   │   └── PagedResult.cs
│   ├── DTOs/
│   ├── Interfaces/
│   │   ├── IReadRepository.cs
│   │   ├── IWriteRepository.cs
│   │   ├── IBookRepository.cs
│   │   ├── IAuthorRepository.cs
│   │   └── IUnitOfWork.cs
│   ├── Mappers/
│   ├── Services/
│   │   ├── BookService.cs
│   │   └── AuthorService.cs
│   └── Specifications/
│       ├── ISpecification.cs
│       ├── BaseSpecification.cs
│       └── Books/BooksByPriceRangeSpec.cs
│
├── DotNetArchRef.Infrastructure/
│   ├── Cache/
│   │   ├── CacheKeys.cs
│   │   ├── BookServiceCacheDecorator.cs
│   │   └── AuthorServiceCacheDecorator.cs
│   ├── Data/
│   │   ├── AppDbContext.cs          ← SaveChangesAsync override + event dispatch
│   │   └── Configurations/
│   ├── EventHandlers/
│   │   ├── AuthorCreatedEventHandler.cs
│   │   └── BookAddedEventHandler.cs
│   ├── Logging/
│   │   ├── BookServiceLoggingDecorator.cs
│   │   └── AuthorServiceLoggingDecorator.cs
│   ├── Persistence/
│   │   ├── UnitOfWork.cs
│   │   └── DomainEventDispatcher.cs ← DI container'ı in-process message bus olarak kullanır
│   └── Repositories/
│
└── DotNetArchRef.Api/
    ├── Controllers/
    ├── ExceptionHandlers/
    ├── Extensions/
    │   ├── ResultExtensions.cs
    │   ├── ServiceCollectionExtensions.cs
    │   └── ApplicationBuilderExtensions.cs
    ├── Filters/ValidationFilter.cs
    ├── Validators/
    └── Program.cs
```

---

## Test

```bash
dotnet test
```

| Proje | Kapsam | Test Sayısı |
|---|---|---|
| `Domain.Tests` | ISBN, Email, Money value object'leri; Book ve Author factory'leri; DomainException, DataCorruptionException | 46 |
| `Application.Tests` | Result\<T\>, Result, Error tipleri; PagedResult hesaplamaları | 20 |

Testler harici bağımlılık gerektirmez — saf domain nesneleri üzerinde çalışır.

---

## Çalıştırma

```bash
dotnet restore
dotnet run --project src/DotNetArchRef.Api
```

Swagger UI: `http://localhost:<port>/swagger`

### Örnek İstekler

```bash
# Validasyon hatası → 422
POST /api/authors
{ "name": "", "email": "geçersiz" }

# Conflict → 409 (aynı e-posta)
POST /api/authors { "name": "A", "email": "a@b.com" }
POST /api/authors { "name": "B", "email": "a@b.com" }

# Bulunamadı → 404
GET /api/books/9999

# Sayfalama
GET /api/books/paged?page=1&pageSize=10

# Specification ile filtreleme + sıralama
GET /api/books/search?minPrice=20&maxPrice=100&orderBy=price&descending=false&page=1&pageSize=10

# Kitap oluştur — Author Aggregate Root üzerinden, BookAddedEvent fırlatılır
POST /api/books
{ "title": "Dune", "isbn": "978-0-441-17271-9", "price": 89.90, "authorId": 1 }
```

---

## Teknoloji Yığını

| Teknoloji | Versiyon | Kullanım |
|---|---|---|
| .NET | 8.0 | Hedef framework |
| ASP.NET Core | 8.0 | Web API |
| Entity Framework Core | 8.0.8 | ORM + Migrations + Value Converters |
| SQLite | 8.0.8 | Veritabanı |
| FluentValidation | 11.9.2 | Input validasyonu |
| Serilog | 8.0.2 | Structured logging (Console + File) |
| Swashbuckle | 6.4.0 | Swagger / OpenAPI |
| xUnit | 2.x | Unit testler |
| FluentAssertions | 6.12.0 | Test assertion'ları |

---

## Yol Haritası

### CQRS + MediatR
`BookService` şu an hem okuma hem yazma operasyonlarını yönetiyor. Doğal evrim noktası CQRS'tir — her handler tek bir iş yapar, MediatR Pipeline Behavior'ları `LoggingDecorator` ve `CacheDecorator`'ın yerini alır.

### Event Sourcing
Mevcut entity state yerine event dizisi persist edilir. Audit log, zaman yolculuğu ve replay senaryoları için uygundur. Domain Events altyapısı bu geçişin temelini oluşturmaktadır.
