namespace Codeforge.Domain.Options;

public sealed class RabbitMqOptions {
	public const string SectionName = "RabbitMQ";
	public required string HostName { get; init; }
	public int Port { get; init; }
	public required string UserName { get; init; }
	public required string Password { get; init; }
	public required string QueueName { get; init; }
}