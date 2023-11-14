using LATESTReactKanBanDo.Data.Entities;
using LATESTReactKanBanDo.Data.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LATESTReactKanBanDo.Data.Repositories
{
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly KanbanDoDbContext _kanbanDbContext;

        public TaskItemRepository(KanbanDoDbContext kanbanDoDbContext)
        {
            _kanbanDbContext = kanbanDoDbContext;
        }

        public async Task<TaskItem?> GetAsync(int taskItemId)
        {
            return await _kanbanDbContext.Tasks.FirstOrDefaultAsync(x => x.Id == taskItemId);
        }

        public async Task<IReadOnlyList<TaskItem>> GetManyAsync(int columnId)
        {
            return await _kanbanDbContext.Tasks.Where(t => t.Column.Id == columnId).ToListAsync();
        }

        public async Task CreateAsync(TaskItem taskItem, int columnId)
        {
            var column = await _kanbanDbContext.Columns.FindAsync(columnId);

            if (column != null)
            {
                taskItem.Column = column;
                _kanbanDbContext.Tasks.Add(taskItem);
                await _kanbanDbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateAsync(TaskItem taskitem)
        {
            _kanbanDbContext.Tasks.Update(taskitem);
            await _kanbanDbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(TaskItem taskitem)
        {
            _kanbanDbContext.Tasks.Remove(taskitem);
            await _kanbanDbContext.SaveChangesAsync();
        }
    }
}
