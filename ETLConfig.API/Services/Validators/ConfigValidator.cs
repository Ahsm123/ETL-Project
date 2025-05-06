using ETL.Domain.Config;
using ETLConfig.API.Services.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace ETLConfig.API.Services.Validators;

public class ConfigValidator : IConfigValidator
{
    public void Validate(ConfigFile config)
    {
        ValidateObject(config);
        ValidateObject(config.ExtractConfig);
        ValidateObject(config.TransformConfig);
        ValidateObject(config.LoadTargetConfig);

        if (config.ExtractConfig?.SourceInfo is null)
            throw new ValidationException("Missing SourceInfo");

        if (config.LoadTargetConfig?.TargetInfo is null)
            throw new ValidationException("Missing TargetInfo");

        ValidateObject(config.ExtractConfig.SourceInfo);
        ValidateObject(config.LoadTargetConfig.TargetInfo);
    }

    private void ValidateObject(object? obj)
    {
        if (obj == null)
            throw new ValidationException("Missing or null configuration section.");

        var context = new ValidationContext(obj);
        Validator.ValidateObject(obj, context, validateAllProperties: true);
    }
}
