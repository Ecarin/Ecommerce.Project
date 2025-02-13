using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
        });
});


// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
#if DEBUG
app.UseSwagger();
app.UseSwaggerUI();
#endif

var currentPath = Directory.GetCurrentDirectory();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(currentPath),
});


app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
