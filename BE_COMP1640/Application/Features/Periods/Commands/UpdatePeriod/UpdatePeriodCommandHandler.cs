using Application.Common.Interfaces;
using Application.Common.Models;
using AutoMapper;
using ErrorOr;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Periods.Commands.UpdatePeriod;

public class UpdatePeriodCommandHandler : IRequestHandler<UpdatePeriodCommand, ErrorOr<SuccessResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public UpdatePeriodCommandHandler(IApplicationDbContext context,
        IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }


    public async Task<ErrorOr<SuccessResult>> Handle(UpdatePeriodCommand request, CancellationToken cancellationToken)
    {
        var periodEntity = await _context.Periods.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        _mapper.Map(request, periodEntity);

        await _context.SaveChangesAsync(cancellationToken);

        return new SuccessResult(title: "Updated a period successfully");
    }
}