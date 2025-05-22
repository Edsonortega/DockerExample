using DockerExample.Models;
using DockerExample.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

//postgres connection string
var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? "Host=localhost;Port=5432;Database=taskdb;Username=postgres;Password=postgres";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Endpoints
app.MapGet("/api/tasks", async (AppDbContext db) => await db.TaskItems.ToListAsync());

app.MapGet("/api/tasks/{id}", async (int id, AppDbContext db) =>
    await db.TaskItems.FindAsync(id) is TaskItem task ? Results.Ok(task) : Results.NotFound());

app.MapPost("/api/tasks", async (TaskItem task, AppDbContext db) =>
{
    db.TaskItems.Add(task);
    await db.SaveChangesAsync();
    return Results.Created($"/api/tasks/{task.Id}", task);
});

app.MapPut("/api/tasks/{id}", async (int id, TaskItem input, AppDbContext db) =>
{
    var task = await db.TaskItems.FindAsync(id);
    if (task is null) return Results.NotFound();

    task.Name = input.Name;
    task.Description = input.Description;
    task.IsCompleted = input.IsCompleted;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/api/tasks/{id}", async (int id, AppDbContext db) =>
{
    var task = await db.TaskItems.FindAsync(id);
    if (task is null) return Results.NotFound();

    db.TaskItems.Remove(task);
    await db.SaveChangesAsync();
    return Results.NoContent();
});


app.Run();
