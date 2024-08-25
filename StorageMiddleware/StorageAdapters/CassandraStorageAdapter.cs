using Cassandra;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StorageAdapters;

public class CassandraStorageAdapter : IStorageAdapter
{
    private readonly ISession _session;
    private readonly ILogger<CassandraStorageAdapter> logger;
    private bool _disposed = false;

    public CassandraStorageAdapter(ILogger<CassandraStorageAdapter> logger)
    {
        this.logger = logger;
        try
        {
            var cluster = Cluster.Builder()
                                .AddContactPoint($"{Environment.GetEnvironmentVariable("CASSANDRA_HOST")}")
                                .Build();

            _session = cluster.Connect(Environment.GetEnvironmentVariable("CASSANDRA_KEYSPACE"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message, ex.StackTrace);
        }
    }

    public async Task CreateAsync<T>(string key, T value)
    {
        try
        {
            var jsonValue = JsonConvert.SerializeObject(value);
            var query = "INSERT INTO test_keyspace.key_value (key, value) VALUES (?, ?) IF NOT EXISTS";

            await _session.ExecuteAsync(new SimpleStatement(query, key, jsonValue)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Custom error: " + ex.Message);
        }
    }

    public async Task<T> ReadAsync<T>(string key)
    {
        var query = "SELECT value FROM test_keyspace.key_value WHERE key = ?";
        var row = await _session.ExecuteAsync(new SimpleStatement(query, key)).ConfigureAwait(false);
        var jsonValue = row.FirstOrDefault()?.GetValue<string>("value");

        return jsonValue == null ? default : JsonConvert.DeserializeObject<T>(jsonValue);
    }

    public async Task UpdateAsync<T>(string key, T value)
    {
        var jsonValue = JsonConvert.SerializeObject(value);
        var query = "UPDATE test_keyspace.key_value SET value = ? WHERE key = ?";
        await _session.ExecuteAsync(new SimpleStatement(query, jsonValue, key)).ConfigureAwait(false);
    }

    public async Task DeleteAsync(string key)
    {
        var query = "DELETE FROM test_keyspace.key_value WHERE key = ?";
        await _session.ExecuteAsync(new SimpleStatement(query, key)).ConfigureAwait(false);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _session?.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~CassandraStorageAdapter()
    {
        Dispose(false);
    }
}
