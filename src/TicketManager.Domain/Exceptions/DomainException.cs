namespace TicketManager.Domain.Exceptions;

// Retorna erros de regras de negócio.
public class DomainException : Exception
{
    public DomainException(string mensagem) : base(mensagem)
    {
    }
}