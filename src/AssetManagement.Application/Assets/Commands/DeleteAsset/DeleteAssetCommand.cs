using AssetManagement.Application.Common.Models;
using MediatR;

namespace AssetManagement.Application.Assets.Commands.DeleteAsset;

// Elimina un asset per ID
// Restituisce Result senza valore — solo successo/fallimento
public record DeleteAssetCommand(int Id) : IRequest<Result>;