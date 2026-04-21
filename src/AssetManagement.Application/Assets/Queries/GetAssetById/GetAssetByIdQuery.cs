using AssetManagement.Application.Common.Models;
using MediatR;

namespace AssetManagement.Application.Assets.Queries.GetAssetById;

// Query per recuperare un singolo asset per ID
public record GetAssetByIdQuery(int Id) : IRequest<Result<AssetDto>>;