using TaskMgt;
using TaskMgt.GroupService;
using TaskMgt.Routes;
using TaskMgt.Services.ListService;
using TaskMgt.Services.TaskService;

var builder = WebApplication.CreateBuilder(args);

DotNetEnv.Env.Load();

// Access environment variables
var mongoDbUrl = Environment.GetEnvironmentVariable("MongoDbURL");
var redisUrl = Environment.GetEnvironmentVariable("RedisURL");

builder.Configuration["ConnectionStrings:MongoDb"] = mongoDbUrl;

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = redisUrl;
    options.InstanceName ="TaskMgt_";
});
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IListService, ListService>();
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        policy =>
        {
            policy.AllowAnyOrigin()    // Allows requests from any origin
                  .AllowAnyHeader()    // Allows any headers
                  .AllowAnyMethod();   // Allows any HTTP method (GET, POST, etc.)
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAllOrigins");

app.MapGroupRoutes();
app.MapListRoutes();
app.MapTaskRoutes();

app.Run();