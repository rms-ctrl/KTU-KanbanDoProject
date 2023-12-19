using Microsoft.AspNetCore.Identity;

namespace LATESTReactKanBanDo.Auth.Model
{
    public class KanbanRestUser : IdentityUser
    {
        public bool ForceRelogin { get; set; }

        public string RefreshToken { get; set; }

        public DateTime RefreshTokenExpiryTime { get; set; }
    }
}
