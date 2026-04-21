using AssetManagement.Application.Common.Models;
using MediatR;

namespace AssetManagement.Application.Assets.Queries.GetAllAssets;

// Query per recuperare tutti gli asset — filtro opzionale per categoria
public record GetAllAssetsQuery(string? Category = null) : IRequest<Result<IEnumerable<AssetDto>>>;