using System.Text;
using System.Text.Json;
using Codeforge.Domain.Interfaces;
using Codeforge.Domain.Options;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace Codeforge.Infrastructure.Messaging;

public class MessageProducer(IOptions<RabbitMqOptions> rabbitMqOptions) : IMessageProducer {
	private readonly RabbitMqOptions _rabbitMqOptions = rabbitMqOptions.Value;

	public async Task PublishAsync<TMessage>(TMessage message) {
		var factory = new ConnectionFactory
			{
				HostName = _rabbitMqOptions.HostName,
				UserName = _rabbitMqOptions.UserName,
				Password = _rabbitMqOptions.Password
			};

		await using var connection = await factory.CreateConnectionAsync();
		await using var channel = await connection.CreateChannelAsync();

		await channel.QueueDeclareAsync(
			_rabbitMqOptions.QueueName,
			true,
			false,
			false,
			null);

		var jsonString = JsonSerializer.Serialize(message);
		var body = Encoding.UTF8.GetBytes(jsonString);

		await channel.BasicPublishAsync(
			string.Empty,
			_rabbitMqOptions.QueueName,
			basicProperties: new BasicProperties { Persistent = true },
			mandatory: true,
			body: body
		);
	}
}