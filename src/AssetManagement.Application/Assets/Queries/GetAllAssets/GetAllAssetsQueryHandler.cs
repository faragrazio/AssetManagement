using AssetManagement.Application.Common.Models;
using AssetManagement.Domain.Interfaces;
using MediatR;

namespace AssetManagement.Application.Assets.Queries.GetAllAssets;

public class GetAllAssetsQueryHandler
    : IRequestHandler<GetAllAssetsQuery, Result<IEnumerable<AssetDto>>>
{
    private readonly IAssetRepository _assetRepository;

    public GetAllAssetsQueryHandler(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task<Result<IEnumerable<AssetDto>>> Handle(
        GetAllAssetsQuery request,
        CancellationToken cancellationToken)
    {
        // Se è specificata una categoria, filtra — altrimenti prendi tutto
        var assets = request.Category != null
            ? await _assetRepository.GetByCategoryAsync(request.Category, cancellationToken)
            : await _assetRepository.GetAllAsync(cancellationToken);

        var dtos = assets.Select(a => new AssetDto(a));

        return Result<IEnumerable<AssetDto>>.Success(dtos);
    }
}