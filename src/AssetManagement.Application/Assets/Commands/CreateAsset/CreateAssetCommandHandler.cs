using AssetManagement.Application.Common.Models;
using AssetManagement.Domain.Entities;
using AssetManagement.Domain.Interfaces;
using MediatR;

namespace AssetManagement.Application.Assets.Commands.CreateAsset;

// L'impiegato che esegue il lavoro per CreateAssetCommand
public class CreateAssetCommandHandler
    : IRequestHandler<CreateAssetCommand, Result<int>>
{
    // Repository iniettato dal DI container
    private readonly IAssetRepository _assetRepository;

    public CreateAssetCommandHandler(IAssetRepository assetRepository)
    {
        _assetRepository = assetRepository;
    }

    public async Task<Result<int>> Handle(
        CreateAssetCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Verifica che non esista già un asset con lo stesso SerialNumber
        var exists = await _assetRepository.SerialNumberExistsAsync(
            request.SerialNumber, cancellationToken: cancellationToken);

        if (exists)
            return Result<int>.Failure(
                $"Esiste già un asset con numero seriale '{request.SerialNumber}'.");

        // 2. Crea la nuova entità — le regole di business vengono applicate nel costruttore
        var asset = new Asset(
            request.Name,
            request.SerialNumber,
            request.Category,
            request.Location,
            request.PurchaseDate
        );

        // 3. Salva nel DB
        await _assetRepository.AddAsync(asset, cancellationToken);
        await _assetRepository.SaveChangesAsync(cancellationToken);

        // 4. Restituisce l'ID dell'asset appena creato
        return Result<int>.Success(asset.Id);
    }
}