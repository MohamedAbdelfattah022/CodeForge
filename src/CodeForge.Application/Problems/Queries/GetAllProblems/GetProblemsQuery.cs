using System.ComponentModel.DataAnnotations;
using CodeForge.Application.Dtos;
using CodeForge.Application.Shared;
using MediatR;

namespace CodeForge.Application.Problems.Queries.GetAllProblems;

public class GetProblemsQuery : IRequest<PaginationResult<ProblemDto>> {
	public int PageNumber { get; set; } = 1;
	public int PageSize { get; set; } = 5;
}