using LATESTReactKanBanDo.Data.Dtos;
using LATESTReactKanBanDo.Data.Entities;
using LATESTReactKanBanDo.Data.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<ColumnDto>> Create(int viewId, CreateColumnDto createColumnDto)
        {
            var column = new Column { Name = createColumnDto.Name };

            await _columnsRepository.CreateAsync(column, viewId);

            // 201
            return Created("", new ColumnDto(column.Id, column.Name));
        }

        [HttpPut]
        [Route("{ColumnId}")]
        public async Task<ActionResult<ColumnDto>> Update(int ColumnId, UpdateColumnDto updateColumnDto)
        {
            var column = await _columnsRepository.GetAsync(ColumnId);

            if (column == null)
            {
                return NotFound();
            }

            column.Name = updateColumnDto.Name;
            await _columnsRepository.UpdateAsync(column);

            return Ok(new ColumnDto(column.Id, column.Name));
        }

        // api/Columns/{ColumnId}
        [HttpDelete]
        [Route("{ColumnId}")]
        public async Task<ActionResult> Remove(int ColumnId)
        {
            var column = await _columnsRepository.GetAsync(ColumnId);

            if (column == null)
            {
                return NotFound();
            }

            await _columnsRepository.DeleteAsync(column);

            //204
            return NoContent();
        }
    }
}
