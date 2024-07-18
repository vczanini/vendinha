using vendinha.Extensions;
using vendinha.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using vendinha.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add NHibernate configuration
builder.Services.AddNHibernate(builder.Configuration.GetConnectionString("PostgreSqlConnection"));

// Register the repository with its dependencies
builder.Services.AddScoped<ClienteRepository>();
builder.Services.AddScoped<IRepository<Clientes>>(provider => provider.GetService<ClienteRepository>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
