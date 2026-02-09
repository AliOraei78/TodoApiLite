using DotNet.Testcontainers.Builders;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace TodoApiLite.Api.Tests;

public class TodoApiTests : IAsyncLifetime
{
    // The PostgreSqlBuilder has built-in wait strategies; manual Unix commands are usually unnecessary.
    private readonly PostgreSqlContainer _postgresContainer = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .WithDatabase("todo_db")
        .WithUsername("postgres")
        .WithPassword("password123")
        .Build();

    private TodoDbContext _dbContext = null!;

    public async Task InitializeAsync()
    {
        await _postgresContainer.StartAsync();

        var connectionString = _postgresContainer.GetConnectionString();

        var options = new DbContextOptionsBuilder<TodoDbContext>()
            .UseNpgsql(connectionString)
            // Critical: This must match your Program.cs configuration
            .UseSnakeCaseNamingConvention()
            .Options;

        _dbContext = new TodoDbContext(options);

        // This applies the migration you just created to the test container
        await _dbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        // Cleanup the database context before the container
        if (_dbContext != null)
        {
            await _dbContext.DisposeAsync();
        }
        await _postgresContainer.DisposeAsync();
    }

    [Fact]
    public async Task CanAddAndRetrieveTodo()
    {
        // Arrange
        var todo = new TodoItem
        {
            Title = "Test Todo from Testcontainers",
            IsCompleted = false,
            // Explicitly set CreatedAt to UTC to avoid Postgres timestamp issues
            CreatedAt = DateTime.UtcNow
        };

        // Act
        _dbContext.Todos.Add(todo);
        await _dbContext.SaveChangesAsync();

        // Clear tracking to ensure we fetch fresh from the DB
        _dbContext.ChangeTracker.Clear();

        var savedTodo = await _dbContext.Todos.FindAsync(todo.Id);

        // Assert
        Assert.NotNull(savedTodo);
        Assert.Equal("Test Todo from Testcontainers", savedTodo.Title);
        Assert.False(savedTodo.IsCompleted);
    }
}