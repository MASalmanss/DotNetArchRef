# DotNetArchRef

> Bir .NET 8 Web API referans projesi. Domain-Driven Design, Clean Architecture ve enterprise kalıpların tek bir kod tabanında nasıl birlikte çalıştığını göstermek amacıyla yazılmıştır.

---

## Mimari Genel Bakış

Proje, **Clean Architecture** prensiplerine göre dört katmana ayrılmıştır. Bağımlılık oku yalnızca içe doğru akar — hiçbir iç katman dış katmanı referans almaz.

```
DotNetArchRef.Domain          ← Çekirdek; sıfır bağımlılık
DotNetArchRef.Application     ← Use case'ler; yalnızca Domain'e bağımlı
DotNetArchRef.Infrastructure  ← EF Core, repo implementasyonları; Application'a bağımlı
DotNetArchRef.Api             ← Controller'lar, filtreler, validatörler; tüm katmanlara bağımlı
```

---

## Katman Detayları

### Domain

Domain katmanı framework veya kütüphane bağımlılığı içermez. İş kuralları buradadır ve buradan çıkmaz.

#### Value Objects

`string` veya `decimal` yerine anlam taşıyan tipler kullanılır. Her Value Object kendi geçerlilik kuralını içselleştirir; geçersiz bir örnek hiçbir zaman oluşturulamaz.

```csharp
public sealed record ISBN
{
    public string Value { get; }
    private ISBN(string value) => Value = value;

    public static ISBN Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("ISBN boş olamaz.");

        if (!Regex.IsMatch(value, @"^[0-9\-]{10,20}$"))
            throw new DomainException("ISBN yalnızca rakam ve tire içermeli, 10-20 karakter uzunluğunda olmalıdır.");

        return new ISBN(value);
    }
}
```

Aynı yapı `Email` ve `Money` için de geçerlidir. `Money.Amount` hiçbir zaman negatif olamaz; `Email` geçersiz bir formatta asla var olamaz.

#### Rich Domain Model

Entity'ler `private set` kullanır. Dışarıdan setter çağrısı derleme hatasıdır. Durum değişikliği yalnızca factory method ve domain metotları aracılığıyla olur:

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

`Author` entity'si, kitap koleksiyonunu backing field ile encapsulate eder:

```csharp
private readonly List<Book> _books = [];
public IReadOnlyCollection<Book> Books => _books.AsReadOnly();
```

#### DomainException

Domain invariant ihlalleri için özel exception sınıfı. Middleware seviyesinde `422 Unprocessable Entity` olarak yakalanır — business error ile infrastructure error arasında net bir ayrım sağlar.

---

### Application

Use case koordinasyonundan sorumludur. EF Core, HTTP veya herhangi bir framework bilgisi yoktur.

#### Result Pattern

Servisler exception fırlatmak yerine `Result<T>` döner. Kontrol akışı artık exception ile değil, tip sistemiyle sağlanır.

```csharp
// Implicit conversion — servis kodu temiz kalır
public async Task<Result<BookDto>> GetByIdAsync(int id, CancellationToken ct)
{
    var book = await _uow.Books.GetByIdAsync(id, ct);
    if (book is null)
        return Error.NotFound($"Book with id {id} not found.");  // implicit Error → Result<T>

    return BookMapper.ToDto(book, author?.Name ?? string.Empty); // implicit T → Result<T>
}
```

`Error` tipi dört kategori tanır — her biri doğrudan bir HTTP durum koduna karşılık gelir:

| ErrorType | HTTP Status |
|-----------|------------|
| `NotFound` | 404 |
| `Conflict` | 409 |
| `Validation` | 422 |
| `Unexpected` | 500 |

#### Unit of Work

`SaveChangesAsync`, repository implementasyonlarından kaldırılmıştır. Her use case, tek bir `CommitAsync` çağrısıyla transaction'ı sonlandırır. Repository'ler `IUnitOfWork` üzerinden erişilir:

```csharp
public class BookService : IBookService
{
    private readonly IUnitOfWork _uow;

    public async Task<Result<BookDto>> CreateAsync(CreateBookRequest request, CancellationToken ct)
    {
        // ...iş kuralları kontrolleri...
        await _uow.Books.AddAsync(book, ct);
        await _uow.CommitAsync(ct);  // tek nokta
        return BookMapper.ToDto(book, author.Name);
    }
}
```

#### Repository Soyutlaması

Generic `IReadRepository<T>` ve `IWriteRepository<T>` interface'leri, domain-spesifik `IBookRepository` ve `IAuthorRepository` ile genişletilir. Application katmanı yalnızca soyutlamalara bağımlıdır; EF Core sızıntısı yoktur.

```
IReadRepository<T>   ← GetAllAsync, GetByIdAsync, GetPagedAsync
IWriteRepository<T>  ← AddAsync, Update, Delete
IRepository<T>       ← her ikisini birleştirir
IBookRepository      ← GetByIsbnAsync ekler
IAuthorRepository    ← GetByEmailAsync ekler
```

#### Sayfalama

`IQueryable<T>` Application katmanına sızdırılmaz. Bunun yerine `GetPagedAsync(page, pageSize)` Infrastructure içinde `Skip/Take` uygular ve `PagedResult<T>` döner:

```csharp
public record PagedResult<T>(
    IEnumerable<T> Items,
    int TotalCount,
    int Page,
    int PageSize)
{
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}
```

---

### Infrastructure

#### EF Core Value Converter

Value Object'ler EF Core tarafından şeffaf biçimde persist edilir. Domain tipi ile veritabanı kolonu arasındaki dönüşüm konfigürasyonda tanımlanır; servis kodu `.Value` çağırmaz:

```csharp
builder.Property(b => b.ISBN)
    .HasConversion(
        isbn => isbn.Value,
        value => ISBN.Create(value))
    .IsRequired()
    .HasMaxLength(20);

builder.Property(b => b.Price)
    .HasConversion(
        money => money.Amount,
        amount => Money.Create(amount))
    .HasPrecision(18, 2);
```

#### Unit of Work Implementasyonu

`UnitOfWork` sınıfı `AppDbContext`'i wrap eder. `CommitAsync` çağrısı doğrudan `context.SaveChangesAsync`'e delege edilir:

```csharp
public sealed class UnitOfWork : IUnitOfWork
{
    public IBookRepository Books { get; }
    public IAuthorRepository Authors { get; }

    public Task<int> CommitAsync(CancellationToken ct = default)
        => _context.SaveChangesAsync(ct);
}
```

---

### Api

#### Program.cs — 12 Satır

Tüm kayıt mantığı extension method'lara taşınmıştır. `Program.cs` ne yaptığını açıkça ifade eder, nasıl yaptığını değil:

```csharp
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPresentation();

app.UseGlobalExceptionHandler();
app.UseSwaggerInDevelopment();
app.MapControllers();
```

#### Global ValidationFilter

Controller metodlarına `[FromBody]` argümanı ulaşmadan önce `IValidator<T>` otomatik olarak çözülür ve çalıştırılır. Her controller'da tekrar eden validasyon kodu yoktur:

```csharp
public class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            var validatorType = typeof(IValidator<>).MakeGenericType(argument!.GetType());
            if (_serviceProvider.GetService(validatorType) is not IValidator validator) continue;

            var result = await validator.ValidateAsync(new ValidationContext<object>(argument), ...);
            if (!result.IsValid)
            {
                context.Result = new UnprocessableEntityObjectResult(...);
                return;
            }
        }
        await next();
    }
}
```

Yeni bir DTO ve validator tanımlandığında filtre onu otomatik olarak yakalar — hiçbir controller değişikliği gerekmez.

#### FluentValidation

Tüm validatörler `Api` katmanındadır. `Application` katmanında FluentValidation bağımlılığı yoktur. Validasyon mesajları `WithMessage` ile açıkça tanımlanmıştır:

```csharp
RuleFor(x => x.ISBN)
    .NotEmpty().WithMessage("ISBN boş bırakılamaz.")
    .MaximumLength(20).WithMessage("ISBN en fazla {MaxLength} karakter olabilir.")
    .Matches(@"^[0-9\-]{10,20}$").WithMessage("ISBN yalnızca rakam ve tire içermeli, 10-20 karakter uzunluğunda olmalıdır.");
```

#### Chain-of-Responsibility Exception Handler

`IExceptionHandler` zinciri, infrastructure hataları için son savunma hattıdır. Business logic exception fırlatmaz; ancak veritabanı bağlantısı gibi beklenmedik hatalar middleware tarafından yakalanır:

```
DomainExceptionHandler     → DomainException      → 422
NotFoundExceptionHandler   → KeyNotFoundException  → 404
ConflictExceptionHandler   → DbUpdateException     → 409
DefaultExceptionHandler    → her şey              → 500
```

#### ResultExtensions

Controller'lar `Result<T>` üzerinden doğrudan HTTP yanıtına dönüşüm yapar. HTTP bilgisi servise sızmaz:

```csharp
[HttpPost]
public async Task<IActionResult> Create(CreateBookRequest request, CancellationToken ct)
{
    var result = await _bookService.CreateAsync(request, ct);
    return result.ToCreatedResult(this, nameof(GetById), book => new { id = book.Id });
}
```

---

## Proje Yapısı

```
src/
├── DotNetArchRef.Domain/
│   ├── Common/BaseEntity.cs
│   ├── Entities/
│   │   ├── Book.cs
│   │   └── Author.cs
│   ├── ValueObjects/
│   │   ├── ISBN.cs
│   │   ├── Email.cs
│   │   └── Money.cs
│   └── Exceptions/DomainException.cs
│
├── DotNetArchRef.Application/
│   ├── Common/
│   │   ├── Result.cs
│   │   ├── Error.cs
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
│   └── Services/
│
├── DotNetArchRef.Infrastructure/
│   ├── Data/
│   │   ├── AppDbContext.cs
│   │   └── Configurations/
│   ├── Persistence/UnitOfWork.cs
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

# Conflict → 409
POST /api/authors
{ "name": "A", "email": "a@b.com" }
POST /api/authors
{ "name": "B", "email": "a@b.com" }  # aynı e-posta

# Bulunamadı → 404
GET /api/books/9999

# Sayfalama
GET /api/books/paged?page=1&pageSize=10

# Specification ile filtreleme + sıralama
GET /api/books/search?minPrice=20&maxPrice=100&orderBy=price&descending=false&page=1&pageSize=10
GET /api/authors/search?name=Robert&orderBy=name
```

---

## Test

Proje iki test projesiyle gelir. Testler harici bağımlılık gerektirmez — saf C# objeleri üzerinde çalışır.

```bash
dotnet test
```

| Proje | Kapsam |
|-------|--------|
| `Domain.Tests` | Value Objects (ISBN, Email, Money), Entity factory'leri, DomainException, DataCorruptionException |
| `Application.Tests` | Result\<T\>, Result, Error tipleri, PagedResult hesaplamaları |

---

## Yol Haritası

### CQRS / MediatR

Şu an `BookService` ve `AuthorService` hem okuma hem yazma operasyonlarını yönetiyor. Servisler büyüdüğünde doğal evrim noktası CQRS'tir:

```
// Mevcut yapı
BookService.GetAllAsync()
BookService.CreateAsync()

// CQRS ile evrim
GetAllBooksQuery      → GetAllBooksQueryHandler
CreateBookCommand     → CreateBookCommandHandler
```

Her handler tek bir iş yapar, tek bir dosyada yaşar. `IMediator.Send()` ile controller'lar servise değil handler'a bağlanır. MediatR bu geçişi kolaylaştırır.

Bu yapı CQRS'e hazırdır — `IUnitOfWork`, `Result<T>`, tek sorumluluklu handler'lar için gereken altyapı mevcuttur.

### Domain Events

`Book.Create()` veya `Author.Create()` çağrıldığında yan etki tetikleme ihtiyacı doğduğunda (e-posta, audit log, cache invalidation) Domain Events mekanizması kullanılacaktır:

```csharp
// Entity içinde event üretimi
public static Book Create(...)
{
    var book = new Book { ... };
    book._events.Add(new BookCreatedEvent(book.Id));
    return book;
}

// UnitOfWork.CommitAsync içinde dispatch
await _dispatcher.DispatchAsync(entity.DomainEvents);
await _context.SaveChangesAsync(ct);
```

Domain Events, `BookCreatedEvent`'i dinleyen handler'ları servis bilmeden tetikler. Katman sınırları korunur.

Bu yapı bu geçişe hazırdır — `UnitOfWork` merge noktası olarak zaten merkezdedir.

---

## Teknoloji Yığını

| Teknoloji | Versiyon | Kullanım |
|-----------|----------|----------|
| .NET | 8.0 | Hedef framework |
| ASP.NET Core | 8.0 | Web API |
| Entity Framework Core | 8.0.8 | ORM + Migrations |
| SQLite | 8.0.8 | Veritabanı |
| FluentValidation | 11.9.2 | Input validasyonu |
| Swashbuckle | 6.4.0 | Swagger / OpenAPI |
| xUnit | 2.x | Unit testler |
| FluentAssertions | 6.12.0 | Test assertion'ları |
