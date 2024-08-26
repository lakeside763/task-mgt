using MongoDB.Driver;
using TaskMgt.Models;

public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public MongoDbContext(IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("MongoDb"));
            _database = client.GetDatabase("task_mgt");
    }

    public IMongoCollection<Group> Groups => _database.GetCollection<Group>("groups");
    public IMongoCollection<List> Lists => _database.GetCollection<List>("lists");
    public IMongoCollection<TodoTask> Tasks => _database.GetCollection<TodoTask>("tasks");
}
