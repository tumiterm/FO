using ForekOnline.Application.Common.Interfaces;
using ForekOnline.Application.Common.Services;
using ForekOnline.Infrastructure.DI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("ElecPoeApp", p =>
        p.WithOrigins("https://localhost:7060") // main site origin
         .AllowAnyHeader()
         .AllowAnyMethod()
    //.AllowCredentials() // uncomment only if you need to send cookies/auth
    );
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddForekInfrastructure(builder.Configuration, isAPI: true);

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("ElecPoeApp");

app.UseAuthorization();

app.MapControllers();

app.Run();
