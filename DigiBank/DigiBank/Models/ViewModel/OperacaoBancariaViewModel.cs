using System.ComponentModel.DataAnnotations;

public class OperacaoBancariaViewModel
{
    [Required]
    public string NumeroConta { get; set; }

    [Required]
    public string TipoOperacao { get; set; }

    [Required]
    [Range(0.01, double.MaxValue)]
    public decimal Valor { get; set; }

    public string? ContaDestino { get; set; } // Validação será feita no serviço

    public string? Descricao { get; set; }
}