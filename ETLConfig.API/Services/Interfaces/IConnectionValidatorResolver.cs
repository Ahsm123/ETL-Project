namespace ETLConfig.API.Services.Interfaces;

public interface IConnectionValidatorResolver
{
    IConnectionValidator? Resolve(string type);
}
