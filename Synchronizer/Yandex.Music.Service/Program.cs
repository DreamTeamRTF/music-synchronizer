using Autofac;
using Autofac.Extensions.DependencyInjection;
using Yandex.Music.Service.Configuration;
using Yandex.Music.Service.DI;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Services.AddLogging(b => b.AddConsole());
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var config = new YandexServiceConfig
{
    TestToken = builder.Configuration.GetSection("Auth")["TestToken"]!,
    TestUserId = Convert.ToInt64(builder.Configuration.GetSection("Auth")["TestUserId"]),
    TestLogin = builder.Configuration.GetSection("Auth")["TestLogin"]!
};

builder.Host.ConfigureContainer<ContainerBuilder>(b => b.RegisterModule(new YandexServiceModule(config)));

var app = builder.Build();

// Configure the HTTP request pipeline.
/* if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
} */
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
