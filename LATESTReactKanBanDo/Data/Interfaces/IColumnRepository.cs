using LATESTReactKanBanDo.Data.Entities;

namespace LATESTReactKanBanDo.Data.Interfaces
{
    public interface IColumnRepository
    {
        Task CreateAsync(Column column, int viewId);
        Task DeleteAsync(Column column);
        Task<Column?> GetAsync(int ColumnId);
        Task<IReadOnlyList<Column>> GetManyAsync(int viewId);
        Task UpdateAsync(Column column);
    }
}
