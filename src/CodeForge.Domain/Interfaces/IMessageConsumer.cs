namespace Codeforge.Domain.Interfaces;

public interface IMessageConsumer {
	Task ConsumeAsync<TMessage>(
		string queueName,
		Func<TMessage, Task> messageHandler,
		CancellationToken cancellationToken = default);
}