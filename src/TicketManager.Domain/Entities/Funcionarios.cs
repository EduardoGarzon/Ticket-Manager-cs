using TicketManager.Domain.Enums;
using TicketManager.Domain.Exceptions;

namespace TicketManager.Domain.Entities;

public class Funcionario
{
    public int Id { get; private set; }
    public string Nome { get; private set; }
    public string Cpf { get; private set; }
    public SituacaoCadastro Situacao { get; private set; }
    public DateTime DataAlteracao { get; private set; }

    public Funcionario(string nome, string cpf)
    {
        Nome = ValidarNome(nome);
        Cpf = ValidarCpf(cpf);
        Situacao = SituacaoCadastro.Ativo;
        DataAlteracao = DateTime.Now;
    }

    private Funcionario(int id, string nome, string cpf, SituacaoCadastro situacao, DateTime dataAlteracao)
    {
        Id = id;
        Nome = nome;
        Cpf = cpf;
        Situacao = situacao;
        DataAlteracao = dataAlteracao;
    }

    public static Funcionario Reconstituir(int id, string nome, string cpf, SituacaoCadastro situacao, DateTime dataAlteracao)
    {
        return new Funcionario(id, nome, cpf, situacao, dataAlteracao);
    }

    public void DefinirId(int id)
    {
        if (Id != 0)
            throw new DomainException("O Id deste registro já foi definido e não pode ser alterado.");

        Id = id;
    }

    public void AtualizarNome(string nome)
    {
        Nome = ValidarNome(nome);
        DataAlteracao = DateTime.Now;
    }

    public void AtualizarCpf(string cpf)
    {
        Cpf = ValidarCpf(cpf);
        DataAlteracao = DateTime.Now;
    }

    public void Ativar()
    {
        Situacao = SituacaoCadastro.Ativo;
        DataAlteracao = DateTime.Now;
    }

    public void Inativar()
    {
        Situacao = SituacaoCadastro.Inativo;
        DataAlteracao = DateTime.Now;
    }

    private static string ValidarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new DomainException("O nome do funcionário é obrigatório.");

        return nome.Trim();
    }

    private static string ValidarCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            throw new DomainException("O CPF do funcionário é obrigatório.");

        if (cpf.Length != 11 || !cpf.All(char.IsDigit))
            throw new DomainException("O CPF deve conter exatamente 11 dígitos numéricos.");

        return cpf;
    }
}