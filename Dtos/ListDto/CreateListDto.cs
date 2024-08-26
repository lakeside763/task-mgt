using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskMgt.Dtos.ListDto
{
    public class CreateListDto
    {
        
        [BsonElement("groupId"), BsonRepresentation(BsonType.ObjectId)]
        public required string GroupId { get; set; }
        
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