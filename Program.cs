
using Microsoft.OpenApi.Models;
using ToDoAPI;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("OpenPolicy",
                          policy =>
                          {
                              policy.WithOrigins("http://localhost:3000")
                                                  .AllowAnyHeader()
                                                  .AllowAnyMethod();
                          });
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Todo API", Description = "Keep track of your tasks", Version = "v1" });
});

builder.Services.AddDbContext<ToDoDbContext>();
var app = builder.Build();

app.UseCors("OpenPolicy");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
   c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
});

app.MapGet("/", () => "Hello World!");

app.MapGet("/items", (ToDoDbContext context) =>
{
    return context.Items.ToList();
});

app.MapPost("/items", async(ToDoDbContext context, Item item)=>{
    context.Add(item);
    await context.SaveChangesAsync();
    return item;
});

app.MapPut("/items/{id}", async(ToDoDbContext context, [FromBody]Item item, int id)=>{
    var existItem = await context.Items.FindAsync(id);
    if(existItem is null) return Results.NotFound();

    existItem.Name = item.Name;
    existItem.IsComplete = item.IsComplete;

    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/items/{id}", async(ToDoDbContext context, int id)=>{
    var existItem = await context.Items.FindAsync(id);
    if(existItem is null) return Results.NotFound();

    context.Items.Remove(existItem);
    await context.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();