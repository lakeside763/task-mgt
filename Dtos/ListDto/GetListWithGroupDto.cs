using Dtos.GroupDto;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskMgt.Dtos.ListDto
{
    public class GetListWithGroupDto
    {
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public GetGroupDto? Group { get; set; }
        
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