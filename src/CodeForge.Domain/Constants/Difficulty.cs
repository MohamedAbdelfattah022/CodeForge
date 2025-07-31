using System.Text.Json.Serialization;

namespace CodeForge.Domain.Constants;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Difficulty {
	Easy,
	Medium,
	Hard
}