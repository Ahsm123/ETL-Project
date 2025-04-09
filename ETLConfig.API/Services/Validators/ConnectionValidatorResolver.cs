using ETLConfig.API.Services.Interfaces;

namespace ETLConfig.API.Services.Validators;

public class ConnectionValidatorResolver : IConnectionValidatorResolver
{
    private readonly IEnumerable<IConnectionValidator> _validators;

    public ConnectionValidatorResolver(IEnumerable<IConnectionValidator> validators)
    {
        _validators = validators;
    }

    public IConnectionValidator? Resolve(string type)
    {
        return _validators.FirstOrDefault(v =>
            v.GetType().Name.ToLower().Contains(type.ToLower()));
    }
}

