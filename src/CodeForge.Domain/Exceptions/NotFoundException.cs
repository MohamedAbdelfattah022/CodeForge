namespace CodeForge.Domain.Exceptions;

public class NotFoundException : Exception {
	public NotFoundException(string resourceType, string resourceId) : base($"Resource {resourceType} with id {resourceId} not found.") { }
	public NotFoundException(string resourceType) : base($"Resource {resourceType} not found.") { }
}