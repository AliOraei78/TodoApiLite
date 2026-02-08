using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL;  

// ──────────────── Application Startup ────────────────
var builder = WebApplication.CreateBuilder(args);

// DbContext with PostgreSQL (using connection string from configuration or env vars)
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")).UseSnakeCaseNamingConvention());

// Add Swagger (already existed)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply migrations automatically (add this)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<TodoDbContext>();

    try
    {
        dbContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Migration failed: " + ex.Message);
        throw;
    }
}

// Allow Swagger in Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API Lite v1");
        c.RoutePrefix = string.Empty;
    });
}

// ──────────────── Minimal API Endpoints ────────────────
app.MapGet("/todos", async (TodoDbContext db) =>
    await db.Todos.ToListAsync())
    .WithName("GetAllTodos");

app.MapGet("/todos/{id}", async (int id, TodoDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    return todo is null ? Results.NotFound() : Results.Ok(todo);
})
    .WithName("GetTodoById");

app.MapPost("/todos", async (TodoItem todo, TodoDbContext db) =>
{
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo);
})
    .WithName("CreateTodo");

app.MapPut("/todos/{id}", async (int id, TodoItem input, TodoDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    todo.Title = input.Title;
    todo.IsCompleted = input.IsCompleted;

    await db.SaveChangesAsync();
    return Results.NoContent();
})
    .WithName("UpdateTodo");

app.MapDelete("/todos/{id}", async (int id, TodoDbContext db) =>
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return Results.NotFound();

    db.Todos.Remove(todo);
    await db.SaveChangesAsync();
    return Results.NoContent();
})
    .WithName("DeleteTodo");

// ──────────────── Run ────────────────
app.Run("http://0.0.0.0:8080");  // Port 8080 for Docker


// ──────────────── Simple Model ────────────────
public class TodoItem
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// ──────────────── Simple DbContext ────────────────
public class TodoDbContext : DbContext
{
    public DbSet<TodoItem> Todos { get; set; }

    public TodoDbContext(DbContextOptions<TodoDbContext> options) : base(options) { }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>().ToTable("todos");  // lowercase

        // Optional: also fix column names if you want snake_case style
        // modelBuilder.Entity<TodoItem>().Property(t => t.IsCompleted).HasColumnName("is_completed");
        // etc.
    }
}

// Test CI/CD - Day 12
