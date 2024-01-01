using Microsoft.EntityFrameworkCore;
using TaskManager;
using TaskManager.BL;
using TaskManager.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApiContext>(
    opt => opt.UseInMemoryDatabase("kit-ar interview"));

builder.Services.AddCors();
builder.Services.AddTransient<IApiContext, ApiContext>();
builder.Services.AddTransient<IProjectsLogic, ProjectsLogic>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

app.UseSwagger();
app.UseSwaggerUI();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApiContext>();
    context.Database.EnsureCreated();
    SeedDatabase(context);
}

app.UseAuthorization();

app.MapControllers();

app.Run();


void SeedDatabase(ApiContext context)
{
    var testProject1 = new Project
    {
        Id = 1,
        Name = "Kit-ar Interview",
        DeadLine = DateTime.UtcNow.AddDays(60),
        TimeSpend = 1124,
        ProjectIsCompleted = true,
        DeveloperId = 1
    };

    context.Projects.Add(testProject1);

    context.SaveChanges();
}