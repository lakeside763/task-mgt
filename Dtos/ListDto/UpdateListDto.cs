using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskMgt.Dtos.ListDto
{
    public class UpdateListDto
    {
        [BsonElement("name")]
        public string? Name { get; set; }
        
        [BsonElement("description")]
        public string? Description { get; set; }
    }
}