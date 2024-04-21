using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using Domain.Entities;
using ErrorOr;
using MediatR;

namespace Application.Features.Faculties.Commands.CreateFaculty;

public class CreateFacultyCommandHandler : IRequestHandler<CreateFacultyCommand, ErrorOr<SuccessResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateFacultyCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<ErrorOr<SuccessResult>> Handle(CreateFacultyCommand request, CancellationToken cancellationToken)
    {
        var facultyEntity = _mapper.Map<Faculty>(request);
        await _context.Faculties.AddAsync(facultyEntity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return new SuccessResult(title: "Created faculty successfully");
    }
}