using ETL.Domain.Config;

namespace ETLConfig.API.Services.Interfaces;
public interface IConfigValidator
{
    void Validate(ConfigFile config);
}
