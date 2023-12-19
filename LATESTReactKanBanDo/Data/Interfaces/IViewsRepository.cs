using LATESTReactKanBanDo.Data.Entities;

namespace LATESTReactKanBanDo.Data.Interfaces
{
    public interface IViewsRepository
    {
        Task CreateAsync(View view);
        Task DeleteAsync(View view);
        Task<View?> GetAsync(int viewId);
        Task<IReadOnlyList<View>> GetManyAsync();
        Task UpdateAsync(View view);
        Task<IReadOnlyList<View>> GetManyForUserAsync(string userId, bool isAdmin = false);

    }
}
