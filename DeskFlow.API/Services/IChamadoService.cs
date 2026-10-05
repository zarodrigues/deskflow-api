using DeskFlow.API.Models.DTOs;
using DeskFlow.API.Models.Entities;

namespace DeskFlow.API.Services;

public interface IChamadoService
{
    Task<List<ChamadoDto>> ListarAsync(StatusChamado? status, Prioridade? prioridade, int? categoriaId);
    Task<ChamadoDto> BuscarPorIdAsync(int id);
    Task<ChamadoDto> AbrirAsync(ChamadoCriarDto dto);
    Task IniciarAsync(int id);
    Task EncerrarAsync(int id, ChamadoEncerrarDto dto);
    Task<InteracaoDto> AdicionarInteracaoAsync(int chamadoId, InteracaoCriarDto dto);
}