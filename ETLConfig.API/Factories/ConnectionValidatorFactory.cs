using ETLConfig.API.Services.Interfaces;
using ETLConfig.API.Services.Validators;

namespace ETLConfig.API.Factories;

public class ConnectionValidatorFactory
{
    private readonly MsSqlConnectionValidator _sqlValidator = new();
    private readonly MySqlServerConnectionValidator _mySqlValidator = new();

    public IConnectionValidator? GetValidator(string type)
    {
        return type.ToLower() switch
        {
            "mssql" => _sqlValidator,
            "mysql" => _mySqlValidator,
            _ => null
        };
    }
}
