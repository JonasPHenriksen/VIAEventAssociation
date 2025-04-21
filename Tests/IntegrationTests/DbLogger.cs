using EfcDataAccess.Context;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace IntegrationTests;

public class DbLogger
{
    
    private readonly ITestOutputHelper _output;

    public DbLogger(ITestOutputHelper output)
    {
        _output = output;
    }
    
    public async Task LogDatabaseContentsAsync(MyDbContext context)
    {
        var connection = context.Database.GetDbConnection();
        await connection.OpenAsync();

        // Get the table names
        var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table';";
        
        using (var reader = await command.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var tableName = reader.GetString(0);
                // Log data from each table
                await LogTableContentsAsync(connection, tableName);
            }
        }
    }
    private async Task LogTableContentsAsync(System.Data.Common.DbConnection connection, string tableName)
    {
        var command = connection.CreateCommand();
        command.CommandText = $"SELECT * FROM {tableName}";
        
        using (var reader = await command.ExecuteReaderAsync())
        {
            _output.WriteLine($"Table: {tableName}");
            
            // Read and log each row of data
            while (await reader.ReadAsync())
            {
                var row = new List<string>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var columnName = reader.GetName(i);
                    var value = reader.IsDBNull(i) ? "NULL" : reader.GetValue(i).ToString();
                    row.Add($"{columnName}: {value}");
                }
                _output.WriteLine(string.Join(", ", row));
            }
        }
    }
}