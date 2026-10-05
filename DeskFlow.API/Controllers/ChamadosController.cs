using DeskFlow.API.Models.DTOs;
using DeskFlow.API.Models.Entities;
using DeskFlow.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace DeskFlow.API.Controllers;

[ApiController]
[Route("api/chamados")]
public class ChamadosController : ControllerBase
{
    private readonly IChamadoService _service;

    public ChamadosController(IChamadoService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> Listar(
        [FromQuery] StatusChamado? status,
        [FromQuery] Prioridade? prioridade,
        [FromQuery] int? categoriaId)
    {
        var chamados = await _service.ListarAsync(status, prioridade, categoriaId);
        return Ok(chamados);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var chamado = await _service.BuscarPorIdAsync(id);
        return Ok(chamado);
    }

    [HttpPost]
    public async Task<IActionResult> Abrir(ChamadoCriarDto dto)
    {
        var criado = await _service.AbrirAsync(dto);
        return CreatedAtAction(nameof(BuscarPorId), new { id = criado.Id }, criado);
    }

    [HttpPost("{id}/iniciar")]

    public async Task<IActionResult> Iniciar(int id)

    {
        await _service.IniciarAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/encerrar")]

    public async Task<IActionResult> Encerrar(int id, ChamadoEncerrarDto dto)

    {
        await _service.EncerrarAsync(id, dto);
        return NoContent();
    }

}