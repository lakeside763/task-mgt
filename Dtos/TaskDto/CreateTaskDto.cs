using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskMgt.Dtos.TaskDto
{
    public class CreateTaskDto
    {
        [BsonElement("listId"), BsonRepresentation(BsonType.ObjectId)]
        public required string ListId { get; set; }
        
        [BsonElement("name")]
        public required string Name { get; set; }
        
        [BsonElement("description")]
        public string? Description { get; set; }
        
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}