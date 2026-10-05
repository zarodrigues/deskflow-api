using DeskFlow.API.Exceptions;
using DeskFlow.API.Models.DTOs;
using DeskFlow.API.Models.Entities;
using DeskFlow.API.Repositories;

namespace DeskFlow.API.Services;

public class CategoriaService : ICategoriaService
{
    private readonly ICategoriaRepository _repository;

    public CategoriaService(ICategoriaRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<CategoriaDto>> ListarAsync()
    {
        var categorias = await _repository.ListarAsync();
        return categorias.Select(c => new CategoriaDto { Id = c.Id, Nome = c.Nome }).ToList();
    }

    public async Task<CategoriaDto> BuscarPorIdAsync(int id)
    {
        var categoria = await _repository.BuscarPorIdAsync(id);

        if (categoria == null)
            throw new NaoEncontradoException($"Categoria {id} não encontrada.");

        return new CategoriaDto { Id = categoria.Id, Nome = categoria.Nome };
    }

    public async Task<CategoriaDto> CriarAsync(CategoriaDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new RegraNegocioException("O nome da categoria é obrigatório.");

        var categoria = new Categoria { Nome = dto.Nome.Trim() };

        await _repository.AdicionarAsync(categoria);

        return new CategoriaDto { Id = categoria.Id, Nome = categoria.Nome };
    }

    public async Task AtualizarAsync(int id, CategoriaDto dto)
    {
        var categoria = await _repository.BuscarPorIdAsync(id);

        if (categoria == null)
            throw new NaoEncontradoException($"Categoria {id} não encontrada.");

        if (string.IsNullOrWhiteSpace(dto.Nome))
            throw new RegraNegocioException("O nome da categoria é obrigatório.");

        categoria.Nome = dto.Nome.Trim();

        await _repository.AtualizarAsync(categoria);
    }

    public async Task RemoverAsync(int id)
{
    var categoria = await _repository.BuscarPorIdAsync(id);

    if (categoria == null)
        throw new NaoEncontradoException($"Categoria {id} não encontrada.");

    if (await _repository.PossuiChamadosAsync(id))
        throw new RegraNegocioException("Não é possível remover uma categoria que possui chamados.");

    await _repository.RemoverAsync(categoria);

}

}