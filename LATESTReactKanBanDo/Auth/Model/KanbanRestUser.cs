using Microsoft.AspNetCore.Identity;

namespace LATESTReactKanBanDo.Auth.Model
{
    public class KanbanRestUser : IdentityUser
    {
        public bool ForceRelogin { get; set; }
    }
}
