using AssetManagement.Application.Common.Models;
using MediatR;

namespace AssetManagement.Application.Assets.Commands.UpdateAsset;

// Aggiorna i dati descrittivi di un asset esistente
// Restituisce Result (senza valore — solo successo/fallimento)
public record UpdateAssetCommand(int Id, string Name, string Category, string Location) : IRequest<Result>;