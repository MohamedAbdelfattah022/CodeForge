using Codeforge.Application.Dtos;
using Codeforge.Domain.Entities;

namespace Codeforge.Application.Mappings;

public static class TagsMapping {
	public static TagDto ToDto(this Tag tag) {
		return new TagDto
			{
				Id = tag.Id,
				Name = tag.Name
			};
	}
}