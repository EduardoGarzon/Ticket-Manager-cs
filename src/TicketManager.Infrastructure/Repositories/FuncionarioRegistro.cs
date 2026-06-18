namespace TicketManager.Infrastructure.Repositories;

internal class FuncionarioRegistro
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public string Situacao { get; set; } = string.Empty;
    public DateTime DataAlteracao { get; set; }
}