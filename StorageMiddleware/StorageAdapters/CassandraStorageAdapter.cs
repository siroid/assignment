using System;
using System.Linq;
using System.Threading.Tasks;
using Cassandra;
using Newtonsoft.Json;

public class CassandraStorageAdapter : IStorageAdapter
{
    private readonly ISession _session;
    private bool _disposed = false;

    public CassandraStorageAdapter()
    {
        var cluster = Cluster.Builder()
                             .AddContactPoint("localhost") // Change to your Cassandra server address
                             .Build();
        _session = cluster.Connect("my_keyspace"); // Replace 'my_keyspace' with your keyspace
    }

    public async Task CreateAsync<T>(string key, T value)
    {
        var jsonValue = JsonConvert.SerializeObject(value);
        var query = "INSERT INTO my_table (key, value) VALUES (?, ?) IF NOT EXISTS"; // Replace 'my_table' with your table name
        await _session.ExecuteAsync(new SimpleStatement(query, key, jsonValue)).ConfigureAwait(false);
    }

    public async Task<T> ReadAsync<T>(string key)
    {
        var query = "SELECT value FROM my_table WHERE key = ?"; // Replace 'my_table' with your table name
        var row = await _session.ExecuteAsync(new SimpleStatement(query, key)).ConfigureAwait(false);
        var jsonValue = row.FirstOrDefault()?.GetValue<string>("value");
        return jsonValue == null ? default : JsonConvert.DeserializeObject<T>(jsonValue);
    }

    public async Task UpdateAsync<T>(string key, T value)
    {
        var jsonValue = JsonConvert.SerializeObject(value);
        var query = "UPDATE my_table SET value = ? WHERE key = ?"; // Replace 'my_table' with your table name
        await _session.ExecuteAsync(new SimpleStatement(query, jsonValue, key)).ConfigureAwait(false);
    }

    public async Task DeleteAsync(string key)
    {
        var query = "DELETE FROM my_table WHERE key = ?"; // Replace 'my_table' with your table name
        await _session.ExecuteAsync(new SimpleStatement(query, key)).ConfigureAwait(false);
    }

    // Implement IDisposable
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
