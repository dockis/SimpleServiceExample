using SimpleService.Repositories;
using SimpleService.Repositories.Providers;
using SimpleService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// builder.Services.AddControllers();
builder.Services.AddControllers(options =>
    {
        options.RespectBrowserAcceptHeader = true;
    })
    .AddXmlSerializerFormatters()
    .AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddLogging();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IFileSystemProvider, FileSystemProvider>();
builder.Services.AddScoped<IDocumentRepository, FileSystemDocumentRepository>();
builder.Services.AddScoped<ICacheRepository, DocumentMemoryCacheRepository>();
builder.Services.AddScoped<DocumentService, DocumentService>();

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