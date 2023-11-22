using LATESTReactKanBanDo.Auth.Model;
using LATESTReactKanBanDo.Data.Dtos;
using LATESTReactKanBanDo.Data.Entities;
using LATESTReactKanBanDo.Data.Interfaces;
using LATESTReactKanBanDo.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LATESTReactKanBanDo.Controllers
{
    [ApiController]
    [Route("api/views/{viewId}/columns/{columnId}/tasks")]
    public class TaskItemController : ControllerBase
    {
        private readonly IColumnRepository _columnsRepository;
        private readonly ITaskItemRepository _taskItemRepository;
        private readonly IViewsRepository _viewsRepository;

        public TaskItemController(ITaskItemRepository taskItemRepository, IColumnRepository columnsRepository, IViewsRepository viewsRepository)
        {
            _taskItemRepository = taskItemRepository;
            _columnsRepository = columnsRepository;
            _viewsRepository = viewsRepository;
        }

        [HttpGet]
        public async Task<IReadOnlyList<TaskItemDto>> GetMany(int columnId)
        {
            var tasks = await _taskItemRepository.GetManyAsync(columnId);

            return tasks.Select(x => new TaskItemDto(x.Id, x.Name, x.Description)).ToList();
        }

        [HttpGet]
        [Route("{taskId}", Name = "GetTask")]
        public async Task<ActionResult<TaskItemDto>> Get(int columnId, int taskId)
        {
            var taskItem = await _taskItemRepository.GetAsync(taskId);
            var column = await _columnsRepository.GetAsync(columnId);

            if (taskItem == null || column == null)
            {
                return NotFound("Task/Column not found");
            }

            return new TaskItemDto(taskItem.Id, taskItem.Name, taskItem.Description);
        }

        [HttpPost]
        [Authorize(Roles = KanbanRoles.KanbanUser)]
        public async Task<ActionResult<TaskItemDto>> Create(int viewId, int columnId, CreateTaskItemDto createTaskItemDto)
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

            var taskItem = new TaskItem { Name = createTaskItemDto.Name, Description = createTaskItemDto.Description };

            await _taskItemRepository.CreateAsync(taskItem, columnId);

            return Created("", new TaskItemDto(taskItem.Id, taskItem.Name, taskItem.Description));
        }

        [HttpPut]
        [Route("{taskId}")]
        [Authorize(Roles = KanbanRoles.KanbanUser)]
        public async Task<ActionResult<TaskItemDto>> Update(int viewId, int taskId, UpdateTaskItemDto updateTaskItemDto)
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

            var taskItem = await _taskItemRepository.GetAsync(taskId);
            if (taskItem == null)
            {
                return NotFound("Task item not found.");
            }

            taskItem.Name = updateTaskItemDto.Name;
            taskItem.Description = updateTaskItemDto.Description;

            await _taskItemRepository.UpdateAsync(taskItem);

            return Ok(new TaskItemDto(taskItem.Id, taskItem.Name, taskItem.Description));
        }

        [HttpDelete]
        [Route("{taskId}")]
        [Authorize(Roles = KanbanRoles.KanbanUser)]
        public async Task<ActionResult> Remove(int viewId, int taskId)
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

            var taskItem = await _taskItemRepository.GetAsync(taskId);
            if (taskItem == null)
            {
                return NotFound("Task item not found.");
            }

            await _taskItemRepository.DeleteAsync(taskItem);

            return NoContent();
        }

    }

}
