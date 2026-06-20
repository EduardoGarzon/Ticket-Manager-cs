using TicketManager.Domain.Entities;
using TicketManager.Domain.Exceptions;
using TicketManager.Domain.Repositories;

namespace TicketManager.Application.Services;

public class FuncionarioService
{
    private readonly IFuncionarioRepository _funcionarioRepository;

    public FuncionarioService(IFuncionarioRepository funcionarioRepository)
    {
        _funcionarioRepository = funcionarioRepository;
    }

    public Funcionario Cadastrar(string nome, string cpf)
    {
        var existente = _funcionarioRepository.ObterPorCpf(cpf);
        if (existente is not null)
            throw new DomainException("Já existe um funcionário cadastrado com este CPF.");

        var funcionario = new Funcionario(nome, cpf);
        _funcionarioRepository.Adicionar(funcionario);

        return funcionario;
    }

    public void Editar(int id, string nome, string cpf)
    {
        var funcionario = _funcionarioRepository.ObterPorId(id)
            ?? throw new DomainException("Funcionário não encontrado.");

        var outroComMesmoCpf = _funcionarioRepository.ObterPorCpf(cpf);
        if (outroComMesmoCpf is not null && outroComMesmoCpf.Id != id)
            throw new DomainException("Já existe outro funcionário cadastrado com este CPF.");

        funcionario.AtualizarNome(nome);
        funcionario.AtualizarCpf(cpf);

        _funcionarioRepository.Atualizar(funcionario);
    }

    public void Ativar(int id)
    {
        var funcionario = _funcionarioRepository.ObterPorId(id)
            ?? throw new DomainException("Funcionário não encontrado.");

        funcionario.Ativar();
        _funcionarioRepository.Atualizar(funcionario);
    }

    public void Inativar(int id)
    {
        var funcionario = _funcionarioRepository.ObterPorId(id)
            ?? throw new DomainException("Funcionário não encontrado.");

        funcionario.Inativar();
        _funcionarioRepository.Atualizar(funcionario);
    }

    public Funcionario ObterFuncionarioPorId(int funcionarioId)
    {
        return _funcionarioRepository.ObterPorId(funcionarioId);
    }
    
    public Funcionario ObterFuncionarioPorCpf(string funcionarioCPF)
    {
        return _funcionarioRepository.ObterPorCpf(funcionarioCPF);
    }

    public IEnumerable<Funcionario> ListarTodos()
    {
        return _funcionarioRepository.ObterTodos();
    }

    public IEnumerable<Funcionario> ListarTodosAtivos()
    {
        return _funcionarioRepository.ObterTodosAtivos();
    }

    public IEnumerable<Funcionario> ListarTodosInativos()
    {
        return _funcionarioRepository.ObterTodosInativos();
    }
}