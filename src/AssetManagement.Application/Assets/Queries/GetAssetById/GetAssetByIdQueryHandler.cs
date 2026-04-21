using AssetManagement.Application.Common.Models;
using AssetManagement.Domain.Interfaces;
using MediatR;

namespace AssetManagement.Application.Assets.Queries.GetAssetById;

public class GetAssetByIdQueryHandler
    : IRequestHandler<GetAssetByIdQuery, Result<AssetDto>>
{
    private readonly IAssetRepository _assetRepository;

    public GetAssetByIdQueryHandler(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task<Result<AssetDto>> Handle(
        GetAssetByIdQuery request,
        CancellationToken cancellationToken)
    {
        var asset = await _assetRepository.GetByIdAsync(request.Id, cancellationToken);

        if (asset == null)
            return Result<AssetDto>.Failure($"Asset con ID '{request.Id}' non trovato.");

        return Result<AssetDto>.Success(new AssetDto(asset));
    }
}