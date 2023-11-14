using LATESTReactKanBanDo.Data.Entities;
using LATESTReactKanBanDo.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LATESTReactKanBanDo.Data.Repositories
{
    public class ColumnRepository : IColumnRepository
    {
        private readonly KanbanDoDbContext _kanbanDbContext;

        public ColumnRepository(KanbanDoDbContext kanbanDoDbContext)
        {
            _kanbanDbContext = kanbanDoDbContext;
        }

        public async Task<Column?> GetAsync(int ColumnId)
        {
            return await _kanbanDbContext.Columns.FirstOrDefaultAsync(x => x.Id == ColumnId);
        }

        public async Task<IReadOnlyList<Column>> GetManyAsync(int viewId)
        {
            return await _kanbanDbContext.Columns.Where(c => c.View.Id == viewId).ToListAsync();
        }

        public async Task CreateAsync(Column column, int viewId)
        {
            var view = await _kanbanDbContext.Views.FindAsync(viewId);

            if (view != null)
            {
                column.View = view;
                _kanbanDbContext.Columns.Add(column);
                await _kanbanDbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(Column column)
        {
            _kanbanDbContext.Columns.Update(column);
            await _kanbanDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Column column)
        {
            _kanbanDbContext.Columns.Remove(column);
            await _kanbanDbContext.SaveChangesAsync();
        }
    }
}
