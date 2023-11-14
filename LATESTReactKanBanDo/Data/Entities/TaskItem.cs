namespace LATESTReactKanBanDo.Data.Entities
{
    public class TaskItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public Column Column { get; set; }
    }
}
