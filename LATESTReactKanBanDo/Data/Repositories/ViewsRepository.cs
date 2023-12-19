using LATESTReactKanBanDo.Data.Entities;
using LATESTReactKanBanDo.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LATESTReactKanBanDo.Data.Repositories
{
    public class ViewsRepository : IViewsRepository
    {
        private readonly KanbanDoDbContext _kanbanDbContext;

        public ViewsRepository(KanbanDoDbContext kanbanDoDbContext)
        {
            _kanbanDbContext = kanbanDoDbContext;
        }

        public async Task<View?> GetAsync(int viewId)
        {
            return await _kanbanDbContext.Views.FirstOrDefaultAsync(x => x.Id == viewId);
        }

        public async Task<IReadOnlyList<View>> GetManyAsync()
        {
            return await _kanbanDbContext.Views.ToListAsync();
        }

        public async Task<IReadOnlyList<View>> GetManyForUserAsync(string userId, bool isAdmin = false)
        {
            if (isAdmin)
            {
                return await _kanbanDbContext.Views.ToListAsync();
            }
            else
            {
                return await _kanbanDbContext.Views
                                             .Where(v => v.UserId == userId)
                                             .ToListAsync();
            }
        }

        public async Task CreateAsync(View view)
        {
            _kanbanDbContext.Views.Add(view);
            await _kanbanDbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(View view)
        {
            _kanbanDbContext.Views.Update(view);
            await _kanbanDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(View view)
        {
            _kanbanDbContext.Views.Remove(view);
            await _kanbanDbContext.SaveChangesAsync();
        }
    }
}
