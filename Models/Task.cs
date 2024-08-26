using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskMgt.Models
{
  public class TodoTask
  {
      [BsonId]
      [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
      public string? Id { get; set; }

      [BsonElement("listId"), BsonRepresentation(BsonType.ObjectId)]
      public required string ListId { get; set; }

      [BsonElement("name")]
      public required string Name { get; set; }

      [BsonElement("description")]
      public string? Description { get; set; }

      [BsonElement("status")]
      public required string Status { get; set; }

      [BsonElement("dueDate")]
      public DateTime DueDate { get; set; }

      [BsonElement("createdAt")]
      public DateTime CreatedAt { get; set; }

      [BsonElement("updatedAt")]
      public DateTime UpdatedAt { get; set; }
      
      public void Update()
      {
        UpdatedAt = DateTime.Now;
      }
  }
}