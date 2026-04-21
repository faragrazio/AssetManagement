using AssetManagement.Application.Common.Models;
using AssetManagement.Domain.Exceptions;
using AssetManagement.Domain.Interfaces;
using MediatR;

namespace AssetManagement.Application.Assets.Commands.UpdateAsset;

public class UpdateAssetCommandHandler : IRequestHandler<UpdateAssetCommand, Result>
{
    private readonly IAssetRepository _assetRepository;

    public UpdateAssetCommandHandler(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task<Result> Handle(
        UpdateAssetCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Cerca l'asset — se non esiste restituisce Failure
        var asset = await _assetRepository.GetByIdAsync(request.Id, cancellationToken);

        if (asset == null)
            return Result.Failure($"Asset con ID '{request.Id}' non trovato.");

        // 2. Chiama il metodo dell'entità — applica le regole di business
        asset.Update(request.Name, request.Category, request.Location);

        // 3. Marca come modificato e salva
        _assetRepository.Update(asset);
        await _assetRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}