using DeskFlow.API.Models.Entities;

namespace DeskFlow.API.Repositories;

public interface IChamadoRepository
{
    Task<List<Chamado>> ListarAsync(StatusChamado? status, Prioridade? prioridade, int? categoriaId);
    Task<Chamado?> BuscarPorIdAsync(int id);
    Task<Chamado> AdicionarAsync(Chamado chamado);
    Task AtualizarAsync(Chamado chamado);

}