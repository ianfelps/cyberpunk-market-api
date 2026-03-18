using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using cyberpunk_market_api.src.constants;
using cyberpunk_market_api.src.dtos.Category;
using cyberpunk_market_api.src.interfaces;
using cyberpunk_market_api.src.responses;
using Swashbuckle.AspNetCore.Annotations;

namespace cyberpunk_market_api.src.controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Listar categorias", Description = "Retorna a lista paginada de categorias. Filtros: name. Query: page, pageSize.")]
    public async Task<ActionResult<ApiResponse<PagedResponse<CategoryResponse>>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = PaginationConstants.DefaultPageSize,
        [FromQuery] string? name = null)
    {
        var result = await _categoryService.GetAllAsync(page, pageSize, name);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Buscar categoria por ID", Description = "Busca uma categoria específica pelo seu identificador único.")]
    public async Task<ActionResult<ApiResponse<CategoryResponse?>>> GetById(Guid id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        if (!result.success)
            return NotFound(result);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Criar categoria", Description = "Cria uma nova categoria (requer permissão de administrador).")]
    public async Task<ActionResult<ApiResponse<CategoryResponse>>> Create([FromBody] CreateCategoryDto dto)
    {
        var result = await _categoryService.CreateAsync(dto);
        if (!result.success)
            return BadRequest(result);
        return StatusCode(201, result);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Atualizar categoria", Description = "Atualiza os dados de uma categoria (requer permissão de administrador).")]
    public async Task<ActionResult<ApiResponse<CategoryResponse?>>> Update(Guid id, [FromBody] UpdateCategoryDto dto)
    {
        var result = await _categoryService.UpdateAsync(id, dto);
        if (!result.success)
            return BadRequest(result);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Remover categoria", Description = "Remove uma categoria (requer permissão de administrador). Não é possível remover categorias que possuem produtos vinculados.")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(Guid id)
    {
        var result = await _categoryService.DeleteAsync(id);
        if (!result.success)
            return BadRequest(result);
        return Ok(result);
    }
}
