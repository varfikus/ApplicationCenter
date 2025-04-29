using ApplicationCenter.Api.Services;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using System;
using ApplicationCenter.Api.Data;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System.Reflection;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationCenterContext>((serviceProvider, options) =>
{
    var dataSourceBuilder = new NpgsqlDataSourceBuilder(builder.Configuration.GetConnectionString("DefaultConnection"));
    dataSourceBuilder.EnableDynamicJson(); 
    var dataSource = dataSourceBuilder.Build();

    options.UseNpgsql(dataSource);
});

builder.Services.AddControllers(options =>
{
    options.InputFormatters.Add(new XmlSerializerInputFormatter(options));
    options.OutputFormatters.Add(new XmlSerializerOutputFormatter());
});

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddScoped<XmlHelper>();
builder.Services.AddScoped<FileStorageService>();

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDefaultFiles();     
app.UseStaticFiles();      

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ApplicationCenterContext>();

    dbContext.Database.Migrate();

    var sqlFilePath = Path.Combine(app.Environment.ContentRootPath, "seed");
    if (File.Exists(sqlFilePath))
    {
        var sql = File.ReadAllText(sqlFilePath);
        var connection = dbContext.Database.GetDbConnection();

        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.CommandType = CommandType.Text;
        command.ExecuteNonQuery();
        connection.Close();
    }
}

app.MapControllers();
app.MapFallbackToFile("/admin", "admin/index.html");

app.Run();