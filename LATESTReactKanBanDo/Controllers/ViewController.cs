using LATESTReactKanBanDo.Auth.Model;
using LATESTReactKanBanDo.Data.Dtos;
using LATESTReactKanBanDo.Data.Entities;
using LATESTReactKanBanDo.Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LATESTReactKanBanDo.Controllers
{
    [ApiController]
    [Route("api/views")]
    public class ViewController : ControllerBase
    {
        private readonly IViewsRepository _viewsRepository;
        private readonly IAuthorizationService _authorizationService;

        public ViewController(IViewsRepository viewsRepository, IAuthorizationService authorizationService)
        {
            _viewsRepository = viewsRepository;
            _authorizationService = authorizationService;
        }

        [HttpGet]
        public async Task<IReadOnlyList<ViewDto>> GetMany()
        {
            var views = await _viewsRepository.GetManyAsync();

            return views.Select(x => new ViewDto(x.Id, x.Name, x.Description)).ToList();
        }

        // api/views/{viewId}
        [HttpGet]
        [Route("{viewId}", Name = "GetView")]
        public async Task<ActionResult<ViewDto>> Get(int viewId)
        {
            var view = await _viewsRepository.GetAsync(viewId);

            // 404
            if (view == null)
            {
                return NotFound();
            }

            return new ViewDto(view.Id, view.Name, view.Description);
        }

        // api/views
        [HttpPost]
        [Authorize(Roles = KanbanRoles.KanbanUser)]
        public async Task<ActionResult<ViewDto>> Create(CreateViewDto createViewDto)
        {
            var view = new View
            {
                Name = createViewDto.Name,
                Description = createViewDto.Description,
                UserId = User.FindFirstValue(JwtRegisteredClaimNames.Sub) // Iš token paimti User Id.
            };

            await _viewsRepository.CreateAsync(view);

            // 201
            return Created("", new ViewDto(view.Id, view.Name, view.Description));
        }

        // api/views/{viewId}
        [HttpPut]
        [Route("{viewId}")]
        [Authorize(Roles = KanbanRoles.KanbanUser)]
        public async Task<ActionResult<ViewDto>> Update(int viewId, UpdateViewDto updateViewDto)
        {
            var view = await _viewsRepository.GetAsync(viewId);

            if (view == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, view, PolicyNames.ResourceOwner);

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            view.Description = updateViewDto.Description;
            await _viewsRepository.UpdateAsync(view);

            return Ok(new ViewDto(view.Id, view.Name, view.Description));
        }

        // api/views/{viewId}
        [HttpDelete]
        [Route("{viewId}")]
        public async Task<ActionResult> Remove(int viewId)
        {
            var view = await _viewsRepository.GetAsync(viewId);

            if (view == null)
            {
                return NotFound();
            }

            var authorizationResult = await _authorizationService.AuthorizeAsync(User, view, PolicyNames.ResourceOwner);

            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }

            await _viewsRepository.DeleteAsync(view);

            //204
            return NoContent();
        }
    }
}
