using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskMgt.Dtos.TaskDto
{
    public class UpdateTaskDto
    {
        
        [BsonElement("name")]
        public string? Name { get; set; }
        
        [BsonElement("description")]
        public string? Description { get; set; }

        public TaskStatus? Status { get; set; }

        [BsonElement("priority")]
        public TaskPriority? Priority { get; set; }
    }
}