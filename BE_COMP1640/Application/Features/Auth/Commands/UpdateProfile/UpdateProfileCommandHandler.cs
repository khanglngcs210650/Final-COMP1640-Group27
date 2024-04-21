using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Auth.Commands.UpdateProfile;

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, ErrorOr<SuccessResult>>
{
    private readonly ICurrentUserProvider _currentUserProvider;
    private readonly IApplicationDbContext _context;
    private readonly IFileService _fileService;
    private readonly IMapper _mapper;

    public UpdateProfileCommandHandler(ICurrentUserProvider currentUserProvider,
        IApplicationDbContext context,
        IFileService fileService,
        IMapper mapper)
    {
        _currentUserProvider = currentUserProvider;
        _context = context;
        _fileService = fileService;
        _mapper = mapper;
    }

    public async Task<ErrorOr<SuccessResult>> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _currentUserProvider.GetCurrentUser();
        //check if current user is null
        if (currentUser == null) return Error.Unauthorized(description: "You must login first");

        var userEntity = await _context.Users
            .Include(u => u.Avatar)
            .FirstOrDefaultAsync(c => c.Id == currentUser.Id, cancellationToken);

        if (userEntity == null) return Error.Unauthorized(description: "User not found");

        _mapper.Map(request, userEntity);

        if (request.AvatarFile != null)
        {
            if (userEntity.AvatarId == null)
            {
                var avatar = await _fileService.SaveFileAsync(request.AvatarFile, "Avatars");

                await _context.Media.AddAsync(avatar, cancellationToken);

                userEntity.AvatarId = avatar.Id;
            }
            else
            {
                await _fileService.UpdateFileAsync(request.AvatarFile, "Avatars", userEntity.Avatar);
            }
        }

        await _context.SaveChangesAsync(cancellationToken);


        return new SuccessResult(title: "Updated profile successfully!");
    }
}