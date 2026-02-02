using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;

// ──────────────── Simple Model ────────────────

// ──────────────── Application Startup ────────────────
var builder = WebApplication.CreateBuilder(args);

// DbContext using InMemory (for initial testing without PostgreSQL)
builder.Services.AddDbContext<TodoDbContext>(options =>
    options.UseInMemoryDatabase("TodoDb"));

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Todo API Lite",
        Version = "v1",
        Description = "A simple API for managing a daily To-Do list",
        Contact = new OpenApiContact
        {
            Name = "Ali Jenabi",
            Email = "a.jenabi78@example.com"
        }
    });

    // Optional: Enable XML comments (for better Swagger documentation)
    // var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    // c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

// Enable Swagger UI in development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API Lite v1");
        c.RoutePrefix = string.Empty;  // Open Swagger at root (http://localhost:xxxx/)
        c.DocumentTitle = "Todo API Lite - Swagger";
        c.DefaultModelsExpandDepth(-1); // Hide models by default
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
app.Run();

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