namespace LATESTReactKanBanDo.Data.Dtos
{
    public record ViewDto(int Id, string Name, string Description);
    public record CreateViewDto(string Name, string Description);
    public record UpdateViewDto(string Description);
}
