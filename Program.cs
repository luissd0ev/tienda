using Compras.Helpers;
using Compras.Models;
using Compras.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Registro del DbContext con la cadena de conexión
builder.Services.AddDbContext<MarsystemsDemoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("MarsystemsDemoConnection")));

builder.Services.AddCors(p => p.AddPolicy("corsapp", builder =>
{
    builder.AllowAnyMethod().AllowAnyHeader().AllowCredentials().AllowAnyOrigin().WithOrigins("http://localhost:4200");
}));

builder.Services.AddScoped<ITiendaRepository, TiendaRepository>();


builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<Funcionalidades>();
var app = builder.Build();
app.UseCors("corsapp");



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
