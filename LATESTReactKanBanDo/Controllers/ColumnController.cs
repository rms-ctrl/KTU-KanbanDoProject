using LATESTReactKanBanDo.Auth.Model;
using LATESTReactKanBanDo.Data.Dtos;
using LATESTReactKanBanDo.Data.Entities;
using LATESTReactKanBanDo.Data.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LATESTReactKanBanDo.Controllers
{
    [ApiController]
    [Route("api/views/{viewId}/columns")]
    public class ColumnController : ControllerBase
    {
        private readonly IViewsRepository _viewsRepository;
        private readonly IColumnRepository _columnsRepository;

        public ColumnController(IColumnRepository columnsRepository, IViewsRepository viewsRepository)
        {
            _columnsRepository = columnsRepository;
            _viewsRepository = viewsRepository;
        }

        [HttpGet]
        public async Task<IReadOnlyList<ColumnDto>> GetMany(int viewId)
        {
            var columns = await _columnsRepository.GetManyAsync(viewId);

            return columns.Select(x => new ColumnDto(x.Id, x.Name)).ToList();
        }

        [HttpGet]
        [Route("{ColumnId}", Name = "GetColumn")]
        public async Task<ActionResult<ColumnDto>> Get(int viewId, int ColumnId)
        {
            var column = await _columnsRepository.GetAsync(ColumnId);
            var view = await _viewsRepository.GetAsync(viewId);

            // 404
            if (column == null || view == null)
            {
                return NotFound("Column/View not found");
            }

            return new ColumnDto(column.Id, column.Name);
        }

        [HttpPost]
        [Authorize(Roles = KanbanRoles.KanbanUser)]
        public async Task<ActionResult<ColumnDto>> Create(int viewId, CreateColumnDto createColumnDto)
        {
            var view = await _viewsRepository.GetAsync(viewId);
            if (view == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (view.UserId != userId)
            {
                return Forbid();
            }

            var column = new Column { Name = createColumnDto.Name };

            await _columnsRepository.CreateAsync(column, viewId);

            // 201
            return Created("", new ColumnDto(column.Id, column.Name));
        }

        [HttpPut]
        [Route("{columnId}")]
        [Authorize(Roles = KanbanRoles.KanbanUser)]
        public async Task<ActionResult<ColumnDto>> Update(int viewId, int columnId, UpdateColumnDto updateColumnDto)
        {
            var view = await _viewsRepository.GetAsync(viewId);
            if (view == null)
            {
                return NotFound("View not found.");
            }

            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (view.UserId != userId)
            {
                return Forbid();
            }

            var column = await _columnsRepository.GetAsync(columnId);
            if (column == null)
            {
                return NotFound("Column not found.");
            }

            column.Name = updateColumnDto.Name;

            await _columnsRepository.UpdateAsync(column);

            return Ok(new ColumnDto(column.Id, column.Name));
        }

        [HttpDelete]
        [Route("{columnId}")]
        [Authorize(Roles = KanbanRoles.KanbanUser)]
        public async Task<ActionResult> Remove(int viewId, int columnId)
        {
            var view = await _viewsRepository.GetAsync(viewId);
            if (view == null)
            {
                return NotFound("View not found.");
            }

            var userId = User.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (view.UserId != userId)
            {
                return Forbid();
            }

            var column = await _columnsRepository.GetAsync(columnId);
            if (column == null)
            {
                return NotFound("Column not found.");
            }

            await _columnsRepository.DeleteAsync(column);

            return NoContent(); // 204 No Content
        }
    }
}
