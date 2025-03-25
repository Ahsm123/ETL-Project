namespace ExtractAPI.Services;

public interface IExtractService
{
    Task ExtractAsync(string configId);
}
