using DeskFlow.API.Models.DTOs;

namespace DeskFlow.API.Services;

public interface ICategoriaService
{
    Task<List<CategoriaDto>> ListarAsync();
    Task<CategoriaDto> BuscarPorIdAsync(int id);
    Task<CategoriaDto> CriarAsync(CategoriaDto dto);
    Task AtualizarAsync(int id, CategoriaDto dto);
    Task RemoverAsync(int id);
}