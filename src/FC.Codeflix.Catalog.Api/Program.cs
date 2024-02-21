using FC.Codeflix.Catalog.Api.Configurations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services
    .AddConnection(builder.Configuration)
    .AddUseCases()
    .AddAndConfigureControllers();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }
