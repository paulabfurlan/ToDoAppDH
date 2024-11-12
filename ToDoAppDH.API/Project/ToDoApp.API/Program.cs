using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Serilog;
using ToDoApp.API.Data;
using ToDoApp.API.Mappings.V1;
using ToDoApp.API.Middlewares;
using ToDoApp.API.Repositories.V1;
using Web_API_Versioning.API;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var logger = new LoggerConfiguration()
	.WriteTo.Console()
	.WriteTo.File("Logs/ToDoApp_Log.txt", rollingInterval: RollingInterval.Day)
	.MinimumLevel.Information()
	.CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddControllers();

builder.Services.AddApiVersioning(options =>
{
	options.AssumeDefaultVersionWhenUnspecified = true;
	options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
	options.ReportApiVersions = true;
});

builder.Services.AddVersionedApiExplorer(options =>
{
	options.GroupNameFormat = "'v'VVV";
	options.SubstituteApiVersionInUrl = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
});
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

// Inject the DB dependencies
builder.Services.AddDbContext<ToDoAppDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("ToDoAppConnectionString")));

// Inject the Repository dependencies
builder.Services.AddScoped<IUserRepository, SQLUserRepository>();

// Inject the AutoMapper dependencies
builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

var app = builder.Build();

var versionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI(options =>
	{
		foreach (var description in versionDescriptionProvider.ApiVersionDescriptions)
		{
			options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
				description.GroupName.ToUpperInvariant());
		}
	});
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
