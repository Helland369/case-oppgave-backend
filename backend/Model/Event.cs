using System.Text.Json.Serialization;

namespace Backend.Model;

public class Event
{
    [JsonPropertyName("eventId")]
    public string EventId { get; set; } = "";

    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("occurredUtc")]
    public DateTime OccurredUtc { get; set; }

    [JsonPropertyName("recordedUtc")]
    public DateTime RecordedUtc { get; set; }
    
    [JsonPropertyName("course")]
    public string Course { get; set; } = "";

    [JsonPropertyName("year")]
    public int Year { get; set; } = 0;

    [JsonPropertyName("semester")]
    public int Semester { get; set; } = 0;

    [JsonPropertyName("type")]
    public string Type { get; set; } = "";

    [JsonPropertyName("birthdate")]
    public string Birthdate { get; set; } = "";

    [JsonPropertyName("city")]
    public string City { get; set; } = "";
    
    [JsonPropertyName("studentId")]
    public string StudentId { get; set; } = "";
}
