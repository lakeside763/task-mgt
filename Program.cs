using TaskMgt;
using TaskMgt.GroupService;
using TaskMgt.Routes;
using TaskMgt.Services.ListService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = "localhost:6379";
    options.InstanceName ="TaskMgt_";
});
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IGroupService, GroupService>();
builder.Services.AddScoped<IListService, ListService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGroupRoutes();
app.MapListRoutes();
app.MapTaskRoutes();

app.Run();