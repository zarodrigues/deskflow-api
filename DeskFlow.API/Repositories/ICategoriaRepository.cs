using DeskFlow.API.Models.Entities;

namespace DeskFlow.API.Repositories;

public interface ICategoriaRepository
{
    Task<List<Categoria>> ListarAsync();
    Task<Categoria?> BuscarPorIdAsync(int id);
    Task<Categoria> AdicionarAsync(Categoria categoria);
    Task AtualizarAsync(Categoria categoria);
    Task RemoverAsync(Categoria categoria);
    Task<bool> PossuiChamadosAsync(int id);
}