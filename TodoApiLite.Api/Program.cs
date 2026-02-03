using Microsoft.EntityFrameworkCore;

// ──────────────── Application Startup ────────────────
var builder = WebApplication.CreateBuilder(args);

// DbContext with InMemory (for local testing)
// Later, in Docker, we will switch to a real PostgreSQL database
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseInMemoryDatabase("TodoDb"));

// Add Swagger (already existed)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Allow Swagger in Development AND Production (or just remove the 'if' entirely)
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API Lite v1");
        c.RoutePrefix = string.Empty; // This makes Swagger load at the root URL (/)
    });
}

// ──────────────── Minimal API Endpoints ────────────────
app.MapGet("/todos", async (TodoDbContext db) =>
{
    Console.WriteLine("GET /todos requested");
    return await db.Todos.ToListAsync();
})
.WithName("GetAllTodos");

app.MapGet("/todos/{id}", async (int id, TodoDbContext db) =>
{
    Console.WriteLine($"GET /todos/{id} requested");
    var todo = await db.Todos.FindAsync(id);
    return todo is null ? Results.NotFound() : Results.Ok(todo);
})
.WithName("GetTodoById");

app.MapPost("/todos", async (TodoItem todo, TodoDbContext db) =>
{
    Console.WriteLine($"POST /todos - Title: {todo.Title}");
    db.Todos.Add(todo);
    await db.SaveChangesAsync();
    return Results.Created($"/todos/{todo.Id}", todo);
})
.WithName("CreateTodo");

app.MapPut("/todos/{id}", async (int id, TodoItem input, TodoDbContext db) =>
{
    Console.WriteLine($"PUT /todos/{id} requested");
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
    Console.WriteLine($"DELETE /todos/{id} requested");
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
}