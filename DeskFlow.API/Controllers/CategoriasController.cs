using DeskFlow.API.Models.DTOs;
using DeskFlow.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeskFlow.API.Controllers;

[ApiController]
[Route("api/categorias")]
public class CategoriasController : ControllerBase
{
    private readonly ICategoriaService _service;

    public CategoriasController(ICategoriaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var categorias = await _service.ListarAsync();
        return Ok(categorias);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var categoria = await _service.BuscarPorIdAsync(id);
        return Ok(categoria);
    }

    [HttpPost]
    public async Task<IActionResult> Criar(CategoriaDto dto)
    {
        var criada = await _service.CriarAsync(dto);
        return CreatedAtAction(nameof(BuscarPorId), new { id = criada.Id }, criada);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(int id, CategoriaDto dto)
    {
        await _service.AtualizarAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remover(int id)

    {
        await _service.RemoverAsync(id);
        return NoContent();
    }

}