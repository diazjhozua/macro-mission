using MacroMission.Application.Common.Interfaces;
using MacroMission.Application.Common.Messaging;
using MacroMission.Application.Foods.Commands.CreateFood;
using MacroMission.Application.Foods.Results;
using MacroMission.Domain.Common;
using MacroMission.Domain.Foods;

namespace MacroMission.Application.Foods.Queries.SearchFoods;

internal sealed class SearchFoodsQueryHandler(
    IFoodRepository foodRepository) : IQueryHandler<SearchFoodsQuery, List<FoodResult>>
{
    public async Task<Result<List<FoodResult>>> Handle(
        SearchFoodsQuery query,
        CancellationToken cancellationToken)
    {
        List<Food> foods = await foodRepository.SearchAsync(
            query.Term,
            query.UserId,
            query.Page,
            query.PageSize,
            cancellationToken);

        return Result<List<FoodResult>>.Success(
            foods.Select(CreateFoodCommandHandler.ToResult).ToList());
    }
}
