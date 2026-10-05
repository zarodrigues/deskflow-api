using DeskFlow.API.Data;
using DeskFlow.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeskFlow.API.Repositories;

public class ChamadoRepository : IChamadoRepository
{
    private readonly AppDbContext _context;

    public ChamadoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Chamado>> ListarAsync(StatusChamado? status, Prioridade? prioridade, int? categoriaId)
    {
        var query = _context.Chamados.Include(c => c.Categoria).AsQueryable();

        if (status.HasValue)
            query = query.Where(c => c.Status == status.Value);

        if (prioridade.HasValue)
            query = query.Where(c => c.Prioridade == prioridade.Value);

        if (categoriaId.HasValue)
            query = query.Where(c => c.CategoriaId == categoriaId.Value);

        return await query.OrderByDescending(c => c.DataAbertura).ToListAsync();
    }

    public async Task<Chamado?> BuscarPorIdAsync(int id)
    {
        return await _context.Chamados
            .Include(c => c.Categoria)
            .Include(c => c.Interacoes)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

       public async Task<Chamado> AdicionarAsync(Chamado chamado)
    {
        _context.Chamados.Add(chamado);
        await _context.SaveChangesAsync();
        return chamado;
    }

    public async Task AtualizarAsync(Chamado chamado)
    {
        _context.Entry(chamado).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }
}