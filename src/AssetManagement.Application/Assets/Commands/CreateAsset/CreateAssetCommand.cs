using AssetManagement.Application.Common.Models;
using MediatR;

namespace AssetManagement.Application.Assets.Commands.CreateAsset;

// La "lettera" che il Controller spedisce a MediatR
// IRequest<Result<int>> significa: "questa request restituisce un Result<int>"
// int = ID dell'asset appena creato
public record CreateAssetCommand(string Name, string SerialNumber, string Category, string Location, DateTime PurchaseDate) : IRequest<Result<int>>;