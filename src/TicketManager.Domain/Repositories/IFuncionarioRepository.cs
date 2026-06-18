using TicketManager.Domain.Entities;

namespace TicketManager.Domain.Repositories;

public interface IFuncionarioRepository
{
    void Adicionar(Funcionario funcionario);
    void Atualizar(Funcionario funcionario);
    Funcionario? ObterPorId(int id);
    Funcionario? ObterPorCpf(string cpf);
    IEnumerable<Funcionario> ObterTodos();
}