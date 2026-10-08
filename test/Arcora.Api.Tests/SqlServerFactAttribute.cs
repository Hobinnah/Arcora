namespace Arcora.Api.Tests;

public sealed class SqlServerFactAttribute : FactAttribute
{
    public SqlServerFactAttribute()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ARCORA_GUARD_TEST_SQLSERVER")))
            Skip = "Set ARCORA_GUARD_TEST_SQLSERVER to an isolated SQL Server test database connection.";
    }
}
