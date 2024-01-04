using Autofac;
using Autofac.Extensions.DependencyInjection;
using VK.Music.Service;
using VK.Music.Service.Configuration;
using VkNet.AudioBypassService.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
builder.Services.AddLogging(b => b.AddConsole());
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddAudioBypass();

var config = new VkServiceConfig
{
    ApplicationId = Convert.ToUInt64(builder.Configuration.GetSection("Application")["ApplicationId"]),
    TestToken = builder.Configuration.GetSection("Auth")["TestToken"]!,
    TestUserId = Convert.ToInt64(builder.Configuration.GetSection("Auth")["TestUserId"]),
    TestLogin = builder.Configuration.GetSection("Auth")["TestLogin"]!
};

builder.Host.ConfigureContainer<ContainerBuilder>(b => b.RegisterModule(new VkServiceModule(config)));

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