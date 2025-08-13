using System.Text;
using System.Text.Json;
using Codeforge.Domain.Interfaces;
using Codeforge.Domain.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Codeforge.Infrastructure.Messaging;

public sealed class MessageConsumer(
	IOptions<RabbitMqOptions> rabbitMqOptions,
	ILogger<MessageConsumer> logger) : IMessageConsumer, IAsyncDisposable {
	private IConnection? _connection;
	private IChannel? _channel;

	public async Task ConsumeAsync<TMessage>(string queueName, Func<TMessage, Task> messageHandler, CancellationToken cancellationToken = default) {
		var factory = new ConnectionFactory
			{
				HostName = rabbitMqOptions.Value.HostName,
				Port = rabbitMqOptions.Value.Port,
				UserName = rabbitMqOptions.Value.UserName,
				Password = rabbitMqOptions.Value.Password
			};

		_connection = await factory.CreateConnectionAsync(cancellationToken);
		_channel = await _connection.CreateChannelAsync(cancellationToken: cancellationToken);

		await _channel.QueueDeclareAsync(queue: queueName, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);

		var consumer = new AsyncEventingBasicConsumer(_channel);
		consumer.ReceivedAsync += async (sender, args) => {
			try {
				var json = Encoding.UTF8.GetString(args.Body.ToArray());
				var message = JsonSerializer.Deserialize<TMessage>(json);


				if (message is not null) {
					logger.LogInformation("Processing message from queue {Queue}: {Payload}", queueName, json);
					await messageHandler(message);
				}

				await _channel.BasicAckAsync(args.DeliveryTag, false, cancellationToken);
			}
			catch (Exception ex) {
				logger.LogError(ex, "Error processing message from queue {Queue}", queueName);
				await _channel.BasicNackAsync(args.DeliveryTag, false, true, cancellationToken);
			}
		};

		await _channel.BasicQosAsync(0, prefetchCount: 1, global: false, cancellationToken);
		await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: cancellationToken);

		logger.LogInformation("Started consuming from queue {Queue}", queueName);
	}

	public async ValueTask DisposeAsync() {
		if (_channel != null) await _channel.CloseAsync();
		if (_connection != null) await _connection.CloseAsync();
	}
}