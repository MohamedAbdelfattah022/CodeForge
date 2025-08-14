using Codeforge.Application.Dtos;
using MediatR;

namespace Codeforge.Application.Users.Queries.GetUserProfile;

public class GetUserProfileQuery : IRequest<UserProfileDto> { }