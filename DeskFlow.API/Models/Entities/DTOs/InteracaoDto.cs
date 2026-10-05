namespace DeskFlow.API.Models.DTOs;

public class InteracaoDto
{
    public int Id { get; set; }
    public string Autor { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
    public DateTime DataRegistro { get; set; }
}