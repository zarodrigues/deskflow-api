using DeskFlow.API.Models.Entities;

namespace DeskFlow.API.Models.DTOs;

public class ChamadoDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public Prioridade Prioridade { get; set; }
    public StatusChamado Status { get; set; }
    public string SolicitanteNome { get; set; } = string.Empty;
    public DateTime DataAbertura { get; set; }
    public DateTime? DataFechamento { get; set; }
    public string? Solucao { get; set; }
    public int CategoriaId { get; set; }
    public string? CategoriaNome { get; set; }
    public List<InteracaoDto> Interacoes { get; set; } = new();
}