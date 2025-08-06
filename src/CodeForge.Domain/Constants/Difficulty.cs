using System.Text.Json.Serialization;

namespace Codeforge.Domain.Constants;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum Difficulty {
	Easy,
	Medium,
	Hard
}