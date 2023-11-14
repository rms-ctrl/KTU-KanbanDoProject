using LATESTReactKanBanDo.Data.Dtos;
using LATESTReactKanBanDo.Data.Entities;
using LATESTReactKanBanDo.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LATESTReactKanBanDo.Controllers
{
    [ApiController]
    [Route("api/views/{viewId}/columns/{columnId}/tasks")]
    public class TaskItemController : ControllerBase
    {
        private readonly IColumnRepository _columnsRepository;
        private readonly ITaskItemRepository _taskItemRepository;

        public TaskItemController(ITaskItemRepository taskItemRepository, IColumnRepository columnsRepository)
        {
            _taskItemRepository = taskItemRepository;
            _columnsRepository = columnsRepository;
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
        public async Task<ActionResult<TaskItemDto>> Create(int columnId, CreateTaskItemDto createTaskItemDto)
        {
            var taskItem = new TaskItem { Name = createTaskItemDto.Name, Description = createTaskItemDto.Description };

            await _taskItemRepository.CreateAsync(taskItem, columnId);

            return Created("", new TaskItemDto(taskItem.Id, taskItem.Name, taskItem.Description));
        }

        [HttpPut]
        [Route("{taskId}")]
        public async Task<ActionResult<TaskItemDto>> Update(int taskId, UpdateTaskItemDto updateTaskItemDto)
        {
            var taskItem = await _taskItemRepository.GetAsync(taskId);

            if (taskItem == null)
            {
                return NotFound();
            }

            taskItem.Name = updateTaskItemDto.Name;
            taskItem.Description = updateTaskItemDto.Description;

            await _taskItemRepository.UpdateAsync(taskItem);

            return Ok(new TaskItemDto(taskItem.Id, taskItem.Name, taskItem.Description));
        }

        [HttpDelete]
        [Route("{taskId}")]
        public async Task<ActionResult> Remove(int taskId)
        {
            var taskItem = await _taskItemRepository.GetAsync(taskId);

            if (taskItem == null)
            {
                return NotFound();
            }

            await _taskItemRepository.DeleteAsync(taskItem);

            return NoContent();
        }
    }

}
