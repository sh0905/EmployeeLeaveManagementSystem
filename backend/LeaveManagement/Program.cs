using LeaveManagement.Data;
using LeaveManagement.Services;
using Microsoft.EntityFrameworkCore;
 
var builder = WebApplication.CreateBuilder(args);
 
// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
 
// Configure Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
 
// Register Services
builder.Services.AddScoped<ILeaveService, LeaveService>();
 
// Configure CORS - CRITICAL for frontend-backend communication
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        builder => builder
            .WithOrigins("http://localhost:3000") // React dev server
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});
 
var app = builder.Build();
 
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbContext>();
    DbInitializer.Initialize(context);
}
 
// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}
 
// Enable CORS - Must be before UseAuthorization
app.UseCors("AllowReactApp");
 
app.UseHttpsRedirection();
app.UseAuthorization();
 
app.MapControllers();
 
app.Run();