using Synchronizer.Core;
using Synchronizer.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var applicationConfig = new SynchronizerConfig
{
    DbConnection = builder.Configuration.GetConnectionString("Postgres")!,
    MigrationsAssemly = typeof(Synchronizer.DAL.SynchronizerDbContext).Assembly.ToString()
};
builder.Services.AddSynchronizerCore(applicationConfig);

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