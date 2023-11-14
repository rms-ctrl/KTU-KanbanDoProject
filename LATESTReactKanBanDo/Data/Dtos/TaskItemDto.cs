namespace LATESTReactKanBanDo.Data.Dtos
{
    public record TaskItemDto(int Id, string Name, string Description);
    public record CreateTaskItemDto(string Name, string Description);
    public record UpdateTaskItemDto(string Name, string Description);
}
