using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System.Text.Json;

namespace FlightService.Services;

public interface IAwsSecretsManagerService
{
    Task<string> GetSecretAsync(string secretName);
    Task<DatabaseCredentials?> GetDatabaseCredentialsAsync(string secretName);
}

public class AwsSecretsManagerService : IAwsSecretsManagerService
{
    private readonly IAmazonSecretsManager _secretsManager;
    private readonly ILogger<AwsSecretsManagerService> _logger;

    public AwsSecretsManagerService(
        IAmazonSecretsManager secretsManager,
        ILogger<AwsSecretsManagerService> logger)
    {
        _secretsManager = secretsManager;
        _logger = logger;
    }

    public async Task<string> GetSecretAsync(string secretName)
    {
        try
        {
            var request = new GetSecretValueRequest
            {
                SecretId = secretName
            };

            var response = await _secretsManager.GetSecretValueAsync(request);

            return response.SecretString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving secret {SecretName} from AWS Secrets Manager", secretName);
            throw;
        }
    }

    public async Task<DatabaseCredentials?> GetDatabaseCredentialsAsync(string secretName)
    {
        try
        {
            var secretString = await GetSecretAsync(secretName);
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            var credentials = JsonSerializer.Deserialize<DatabaseCredentials>(secretString, options);
            
            if (credentials != null)
            {
                _logger.LogInformation("Database credentials successfully deserialized");
            }
            else
            {
                _logger.LogWarning("Deserialization returned null credentials");
            }
            
            return credentials;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error parsing database credentials from secure vault");
            throw;
        }
    }
}

public class DatabaseCredentials
{
    public string? username { get; set; }
    public string? password { get; set; }
    public string? engine { get; set; }
    public string? host { get; set; }
    public int port { get; set; }
    public string? dbname { get; set; }
}
