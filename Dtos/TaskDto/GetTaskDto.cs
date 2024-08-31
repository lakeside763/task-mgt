using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TaskMgt.Dtos.TaskDto
{
    public class GetTaskDto
    {
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("listId"), BsonRepresentation(BsonType.ObjectId)]
        public required string ListId { get; set; }

        [BsonElement("name")]
        public required string Name { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("status")]
        public TaskStatus? Status { get; set; }

        [BsonElement("priority")]
        public TaskPriority? Priority { get; set; }

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("startTime")]
        public string? StartTime { get; set; }

        [BsonElement("endTime")]
        public string? EndTime { get; set; }

        [BsonElement("dueDate")]
        public DateTime DueDate { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }
}