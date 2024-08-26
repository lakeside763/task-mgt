using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskMgt.Dtos.ListDto
{
    public class GetListDto
    {
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        [BsonElement("groupId"), BsonRepresentation(BsonType.ObjectId)]
        public string? GroupId { get; set; }
        
        [BsonElement("name")]
        public string? Name { get; set; }
        
        [BsonElement("description")]
        public string? Description { get; set; }
        
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}