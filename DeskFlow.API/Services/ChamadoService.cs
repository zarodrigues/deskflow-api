using DeskFlow.API.Exceptions;
using DeskFlow.API.Models.DTOs;
using DeskFlow.API.Models.Entities;
using DeskFlow.API.Repositories;

namespace DeskFlow.API.Services;

public class ChamadoService : IChamadoService
{
    private readonly IChamadoRepository _chamadoRepository;
    private readonly ICategoriaRepository _categoriaRepository;

    public ChamadoService(IChamadoRepository chamadoRepository, ICategoriaRepository categoriaRepository)
    {
        _chamadoRepository = chamadoRepository;
        _categoriaRepository = categoriaRepository;
    }

    public async Task<List<ChamadoDto>> ListarAsync(StatusChamado? status, Prioridade? prioridade, int? categoriaId)
    {
        var chamados = await _chamadoRepository.ListarAsync(status, prioridade, categoriaId);
        return chamados.Select(Mapear).ToList();
    }

    public async Task<ChamadoDto> BuscarPorIdAsync(int id)
    {
        var chamado = await _chamadoRepository.BuscarPorIdAsync(id);

        if (chamado == null)
            throw new NaoEncontradoException($"Chamado {id} não encontrado.");

        return Mapear(chamado);
    }

    public async Task<ChamadoDto> AbrirAsync(ChamadoCriarDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Titulo))
            throw new RegraNegocioException("O título é obrigatório.");

        if (string.IsNullOrWhiteSpace(dto.Descricao))
            throw new RegraNegocioException("A descrição é obrigatória.");

        if (string.IsNullOrWhiteSpace(dto.SolicitanteNome))
            throw new RegraNegocioException("O nome do solicitante é obrigatório.");

        var categoria = await _categoriaRepository.BuscarPorIdAsync(dto.CategoriaId);
        if (categoria == null)
            throw new RegraNegocioException("A categoria informada não existe.");

        var chamado = new Chamado
        {
            Titulo = dto.Titulo.Trim(),
            Descricao = dto.Descricao.Trim(),
            Prioridade = dto.Prioridade,
            SolicitanteNome = dto.SolicitanteNome.Trim(),
            CategoriaId = dto.CategoriaId,
            Status = StatusChamado.Aberto,
            DataAbertura = DateTime.Now
        };

        await _chamadoRepository.AdicionarAsync(chamado);

        var salvo = await _chamadoRepository.BuscarPorIdAsync(chamado.Id);
        return Mapear(salvo!);
    }

    private static ChamadoDto Mapear(Chamado c)
    {
        return new ChamadoDto
        {
            Id = c.Id,
            Titulo = c.Titulo,
            Descricao = c.Descricao,
            Prioridade = c.Prioridade,
            Status = c.Status,
            SolicitanteNome = c.SolicitanteNome,
            DataAbertura = c.DataAbertura,
            DataFechamento = c.DataFechamento,
            Solucao = c.Solucao,
            CategoriaId = c.CategoriaId,
            CategoriaNome = c.Categoria?.Nome,
            Interacoes = c.Interacoes.Select(i => new InteracaoDto
            {
                Id = i.Id,
                Autor = i.Autor,
                Mensagem = i.Mensagem,
                DataRegistro = i.DataRegistro
            }).ToList()
        };
    }

    public async Task IniciarAsync(int id)
    {
        var chamado = await _chamadoRepository.BuscarPorIdAsync(id);

        if (chamado == null)
            throw new NaoEncontradoException($"Chamado {id} não encontrado.");

        if (chamado.Status != StatusChamado.Aberto)
            throw new RegraNegocioException("Só é possível iniciar um chamado com status Aberto.");

        chamado.Status = StatusChamado.EmAndamento;

        await _chamadoRepository.AtualizarAsync(chamado);
    }

    public async Task EncerrarAsync(int id, ChamadoEncerrarDto dto)
    {
        var chamado = await _chamadoRepository.BuscarPorIdAsync(id);

        if (chamado == null)
            throw new NaoEncontradoException($"Chamado {id} não encontrado.");

        if (chamado.Status != StatusChamado.EmAndamento)
            throw new RegraNegocioException("Só é possível encerrar um chamado com status Andamento.");

        chamado.Status = StatusChamado.Fechado;
        chamado.Solucao = dto.Solucao.Trim();
        chamado.Solucao = dto.Solucao.Trim();


        await _chamadoRepository.AtualizarAsync(chamado);


    }

    public async Task<InteracaoDto> AdicionarInteracaoAsync(int chamadoId, InteracaoCriarDto dto)
{
    var chamado = await _chamadoRepository.BuscarPorIdAsync(chamadoId);

    if (chamado == null)
        throw new NaoEncontradoException($"Chamado {chamadoId} não encontrado.");

    if (chamado.Status == StatusChamado.Fechado)
        throw new RegraNegocioException("Não é possível adicionar interações a um chamado fechado.");

    if (string.IsNullOrWhiteSpace(dto.Autor))
        throw new RegraNegocioException("O autor é obrigatório.");

    if (string.IsNullOrWhiteSpace(dto.Mensagem))
        throw new RegraNegocioException("A mensagem é obrigatória.");

    var interacao = new Interacao
    {
        ChamadoId = chamadoId,
        Autor = dto.Autor.Trim(),
        Mensagem = dto.Mensagem.Trim(),
        DataRegistro = DateTime.Now
    };

    await _chamadoRepository.AdicionarInteracaoAsync(interacao);

    return new InteracaoDto
    {
        Id = interacao.Id,
        Autor = interacao.Autor,
        Mensagem = interacao.Mensagem,
        DataRegistro = interacao.DataRegistro
    };
}

}