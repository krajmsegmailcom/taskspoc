using Tasks.Application.Services;
using Tasks.Core.Entities.Interfaces;
using Tasks.Infrastructure.Data;
using Tasks.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
DatabaseStartup.SetConfig(builder.Configuration);
DatabaseStartup.ConfigureServices(builder.Services);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddScoped<IRepository<Tasks.Core.Entities.Task>, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();

// Use the authentication middleware
app.UseAuthentication(); // <-- Add this line
app.UseAuthorization();

app.MapControllers(); // Map controller routes
app.Run();


