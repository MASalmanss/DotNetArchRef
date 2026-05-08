namespace DotNetConsistency.Domain.Exceptions;

public class DataCorruptionException : Exception
{
    public string FieldName { get; }
    public string RawValue { get; }

    public DataCorruptionException(string fieldName, string rawValue)
        : base($"Veritabanında bozuk veri tespit edildi. Alan: '{fieldName}', Değer: '{rawValue}'")
    {
        FieldName = fieldName;
        RawValue = rawValue;
    }
}
