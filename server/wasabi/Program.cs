using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using wasabi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options=>{
    options.UseSqlServer(@"Server=LAPTOP-A7INMDKJ;Database=dbone;Trusted_Connection=True;");
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "wasabi_allowed_origins",
                      builder =>
                      {
                          builder.WithOrigins("http://localhost:3000")
                                                .AllowAnyHeader()
                                                  .AllowAnyMethod();
                      });
});
//

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//
app.UseCors("wasabi_allowed_origins");
//


app.UseAuthorization();

app.MapControllers();

app.Run();
