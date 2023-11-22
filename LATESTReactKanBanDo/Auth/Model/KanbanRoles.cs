namespace LATESTReactKanBanDo.Auth.Model
{
    public static class KanbanRoles
    {
        public const string Admin = nameof(Admin);
        public const string KanbanUser = nameof(KanbanUser);

        public static readonly IReadOnlyCollection<string> All = new[] {Admin, KanbanUser };
    }
}
