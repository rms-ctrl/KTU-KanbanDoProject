using LATESTReactKanBanDo.Data.Entities;

namespace LATESTReactKanBanDo.Data.Interfaces
{
    public interface ITaskItemRepository
    {
        Task CreateAsync(TaskItem taskItem, int columnId);
        Task DeleteAsync(TaskItem taskitem);
        Task<TaskItem?> GetAsync(int taskItemId);
        Task<IReadOnlyList<TaskItem>> GetManyAsync(int columnId);
        Task UpdateAsync(TaskItem taskitem);
    }
}
