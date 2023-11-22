using LATESTReactKanBanDo.Auth.Model;
using System.ComponentModel.DataAnnotations;

namespace LATESTReactKanBanDo.Data.Entities
{
    public class View : IUserOwnedResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Required]
        public string UserId { get; set; }
        public KanbanRestUser User { get; set; }
    }
}
