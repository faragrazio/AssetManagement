using AssetManagement.Application.Common.Models;
using AssetManagement.Domain.Interfaces;
using MediatR;

namespace AssetManagement.Application.Assets.Commands.DeleteAsset;

public class DeleteAssetCommandHandler : IRequestHandler<DeleteAssetCommand, Result>
{
    private readonly IAssetRepository _assetRepository;

    public DeleteAssetCommandHandler(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task<Result> Handle(
        DeleteAssetCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Cerca l'asset
        var asset = await _assetRepository.GetByIdAsync(request.Id, cancellationToken);

        if (asset == null)
            return Result.Failure($"Asset con ID '{request.Id}' non trovato.");

        // 2. Elimina e salva
        _assetRepository.Remove(asset);
        await _assetRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}