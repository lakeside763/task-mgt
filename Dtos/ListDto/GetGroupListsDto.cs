using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using TaskMgt.Models;

namespace TaskMgt.Dtos.ListDto
{
    public class GetGroupListsDto
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        [BsonElement("name")]
        public required string Name { get; set; }
        
        [BsonElement("description")]
        public string? Description { get; set; }
        
        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        public List<GetListDto>? Lists {get; set; }
    }
}