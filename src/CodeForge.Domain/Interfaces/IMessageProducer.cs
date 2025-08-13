namespace Codeforge.Domain.Interfaces;

public interface IMessageProducer {
	Task PublishAsync<TMessage>(TMessage message);
}