namespace LATESTReactKanBanDo.Data.Dtos
{
    public record ColumnDto(int Id, string Name);
    public record CreateColumnDto(string Name);
    public record UpdateColumnDto(string Name);
}
