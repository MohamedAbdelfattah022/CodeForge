using Codeforge.Application.Dtos;
using Codeforge.Application.Mappings;
using Codeforge.Domain.Entities;
using Codeforge.Domain.Exceptions;
using Codeforge.Domain.Interfaces;
using Codeforge.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Codeforge.Application.Contests.Queries.GetContestStandings;

public class GetStandingsQueryHandler(
	ILogger<GetStandingsQueryHandler> logger,
	IContestsRepository contestsRepository,
	IRedisCacheService redisCache) : IRequestHandler<GetStandingsQuery, List<StandingDto>> {
	public async Task<List<StandingDto>> Handle(GetStandingsQuery request, CancellationToken cancellationToken) {
		logger.LogInformation("GetStandingsQueryHandler.Handle called with request: {@Request}", request);

		var cacheKey = $"standings:{request.ContestId}";
		var standings = await redisCache.GetAsync<List<Standing>>(cacheKey);
		if (standings is null) {
			standings = await contestsRepository.GetStandingsAsync(request.ContestId);
			await redisCache.SetAsync(cacheKey, standings, TimeSpan.FromMinutes(10));
		}

		if (standings is null) throw new NotFoundException(nameof(Standing), request.ContestId.ToString());

		return standings.Select(s => s.ToDto()).ToList();
	}
}