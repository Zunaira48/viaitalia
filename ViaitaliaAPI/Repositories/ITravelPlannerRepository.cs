using ViaitaliaAPI.Models;

namespace ViaitaliaAPI.Repositories
{
    public interface ITravelPlannerRepository
    {
        Task<TravelPlannerResponse> GenerateTravelPlan(TravelPlannerRequest model);
    }
}
