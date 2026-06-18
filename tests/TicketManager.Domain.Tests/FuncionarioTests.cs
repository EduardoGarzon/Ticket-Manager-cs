using TicketManager.Domain.Entities;
using TicketManager.Domain.Enums;
using TicketManager.Domain.Exceptions;

namespace TicketManager.Domain.Tests;

public class FuncionarioTests
{
    // -------------------------
    // Construtor
    // -------------------------

    [Fact]
    public void Construtor_DadosValidos_CriaFuncionarioCorreto()
    {
        // Arrange
        var nome = "Maria da Silva";
        var cpf = "12345678901";

        // Act
        var funcionario = new Funcionario(nome, cpf);

        // Assert
        Assert.Equal("Maria da Silva", funcionario.Nome);
        Assert.Equal("12345678901", funcionario.Cpf);
        Assert.Equal(SituacaoCadastro.Ativo, funcionario.Situacao);
        Assert.Equal(0, funcionario.Id);
    }

    [Fact]
    public void Construtor_SemInformarSituacao_CriaComoAtivo()
    {
        var funcionario = new Funcionario("João Silva", "12345678901");
        Assert.Equal(SituacaoCadastro.Ativo, funcionario.Situacao);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Construtor_NomeEmBrancoOuVazio_LancaDomainException(string nome)
    {
        var exception = Assert.Throws<DomainException>(() => new Funcionario(nome, "12345678901"));
        Assert.Equal("O nome do funcionário é obrigatório.", exception.Message);
    }

    [Theory]
    [InlineData("1234567890")]    // 10 dígitos
    [InlineData("123456789012")]  // 12 dígitos
    [InlineData("1234567890a")]   // caractere não numérico
    [InlineData("")]
    public void Construtor_CpfInvalido_LancaDomainException(string cpf)
    {
        Assert.Throws<DomainException>(() => new Funcionario("Maria da Silva", cpf));
    }

    // -------------------------
    // AtualizarNome
    // -------------------------

    [Fact]
    public void AtualizarNome_NomeValido_AtualizaPropriedade()
    {
        var funcionario = new Funcionario("Maria da Silva", "12345678901");
        funcionario.AtualizarNome("Ana Paula");
        Assert.Equal("Ana Paula", funcionario.Nome);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void AtualizarNome_NomeEmBrancoOuVazio_LancaDomainException(string nome)
    {
        var funcionario = new Funcionario("Maria da Silva", "12345678901");
        Assert.Throws<DomainException>(() => funcionario.AtualizarNome(nome));
    }

    // -------------------------
    // AtualizarCpf
    // -------------------------

    [Fact]
    public void AtualizarCpf_CpfValido_AtualizaPropriedade()
    {
        var funcionario = new Funcionario("Maria da Silva", "12345678901");
        funcionario.AtualizarCpf("98765432100");
        Assert.Equal("98765432100", funcionario.Cpf);
    }

    [Theory]
    [InlineData("1234567890")]
    [InlineData("123456789012")]
    [InlineData("1234567890a")]
    public void AtualizarCpf_CpfInvalido_LancaDomainException(string cpf)
    {
        var funcionario = new Funcionario("Maria da Silva", "12345678901");
        Assert.Throws<DomainException>(() => funcionario.AtualizarCpf(cpf));
    }

    // -------------------------
    // Ativar / Inativar
    // -------------------------

    [Fact]
    public void Inativar_FuncionarioAtivo_MudaSituacaoParaInativo()
    {
        var funcionario = new Funcionario("Maria da Silva", "12345678901");
        funcionario.Inativar();
        Assert.Equal(SituacaoCadastro.Inativo, funcionario.Situacao);
    }

    [Fact]
    public void Ativar_FuncionarioInativo_MudaSituacaoParaAtivo()
    {
        var funcionario = new Funcionario("Maria da Silva", "12345678901");
        funcionario.Inativar();
        funcionario.Ativar();
        Assert.Equal(SituacaoCadastro.Ativo, funcionario.Situacao);
    }

    // -------------------------
    // DefinirId
    // -------------------------

    [Fact]
    public void DefinirId_IdValido_AtribuiIdAoFuncionario()
    {
        var funcionario = new Funcionario("Maria da Silva", "12345678901");
        funcionario.DefinirId(42);
        Assert.Equal(42, funcionario.Id);
    }

    [Fact]
    public void DefinirId_ChamadoDuasVezes_LancaDomainException()
    {
        var funcionario = new Funcionario("Maria da Silva", "12345678901");
        funcionario.DefinirId(42);
        Assert.Throws<DomainException>(() => funcionario.DefinirId(99));
    }

    // -------------------------
    // Reconstituir
    // -------------------------

    [Fact]
    public void Reconstituir_DadosValidos_CriaFuncionarioComTodosOsDados()
    {
        var dataAlteracao = DateTime.Now;
        var funcionario = Funcionario.Reconstituir(5, "Carlos", "11122233344", SituacaoCadastro.Inativo, dataAlteracao);

        Assert.Equal(5, funcionario.Id);
        Assert.Equal("Carlos", funcionario.Nome);
        Assert.Equal("11122233344", funcionario.Cpf);
        Assert.Equal(SituacaoCadastro.Inativo, funcionario.Situacao);
        Assert.Equal(dataAlteracao, funcionario.DataAlteracao);
    }
}