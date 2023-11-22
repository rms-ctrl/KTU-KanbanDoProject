using LATESTReactKanBanDo.Auth.Model;
using Microsoft.AspNetCore.Identity;

namespace LATESTReactKanBanDo.Data
{
    public class AuthDbSeeder
    {
        private readonly UserManager<KanbanRestUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthDbSeeder(UserManager<KanbanRestUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAsync()
        {
            await AddDefaultRoles();
            await AddAdminUser();
        }

        private async Task AddAdminUser()
        {
            var newAdminUser = new KanbanRestUser()
            {
                UserName = "admin",
                Email = "domas.ald@gmail.com"
            };

            var existingAdminUser = await _userManager.FindByNameAsync(newAdminUser.UserName);
            if (existingAdminUser == null)
            {
                var createAdminUserResult = await _userManager.CreateAsync(newAdminUser, "Test.1234");
                if (createAdminUserResult.Succeeded)
                {
                    await _userManager.AddToRolesAsync(newAdminUser, KanbanRoles.All);
                }
            }
        }

        private async Task AddDefaultRoles()
        {
            foreach(var role in KanbanRoles.All)
            {
                var roleExists = await _roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
