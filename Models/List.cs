using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskMgt.Models
{
  public class List 
  {
    [BsonId]
    [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("groupId"), BsonRepresentation(BsonType.ObjectId)]
    public required string GroupId { get; set; }
    
    [BsonElement("name")]
    public required string Name { get; set; }
    
    [BsonElement("description")]
    public required string Description { get; set; }
    
    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
    public void Update()
    {
      UpdatedAt = DateTime.Now;
    }
  }
}