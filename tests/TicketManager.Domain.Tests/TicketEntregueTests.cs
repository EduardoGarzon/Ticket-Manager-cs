using TicketManager.Domain.Entities;
using TicketManager.Domain.Enums;
using TicketManager.Domain.Exceptions;

namespace TicketManager.Domain.Tests;

public class TicketEntregueTests
{
    // -------------------------
    // Construtor
    // -------------------------

    [Fact]
    public void Construtor_DadosValidos_CriaTicketCorreto()
    {
        var ticket = new TicketEntregue(1, 5);

        Assert.Equal(1, ticket.FuncionarioId);
        Assert.Equal(5, ticket.Quantidade);
        Assert.Equal(SituacaoCadastro.Ativo, ticket.Situacao);
        Assert.Equal(0, ticket.Id);
    }

    [Fact]
    public void Construtor_SemInformarSituacao_CriaComoAtivo()
    {
        var ticket = new TicketEntregue(1, 5);
        Assert.Equal(SituacaoCadastro.Ativo, ticket.Situacao);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Construtor_FuncionarioIdInvalido_LancaDomainException(int funcionarioId)
    {
        Assert.Throws<DomainException>(() => new TicketEntregue(funcionarioId, 5));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void Construtor_QuantidadeInvalida_LancaDomainException(int quantidade)
    {
        Assert.Throws<DomainException>(() => new TicketEntregue(1, quantidade));
    }

    // -------------------------
    // AtualizarQuantidade
    // -------------------------

    [Fact]
    public void AtualizarQuantidade_QuantidadeValida_AtualizaPropriedade()
    {
        var ticket = new TicketEntregue(1, 5);
        ticket.AtualizarQuantidade(10);
        Assert.Equal(10, ticket.Quantidade);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    public void AtualizarQuantidade_QuantidadeInvalida_LancaDomainException(int quantidade)
    {
        var ticket = new TicketEntregue(1, 5);
        Assert.Throws<DomainException>(() => ticket.AtualizarQuantidade(quantidade));
    }

    // -------------------------
    // Ativar / Inativar
    // -------------------------

    [Fact]
    public void Inativar_TicketAtivo_MudaSituacaoParaInativo()
    {
        var ticket = new TicketEntregue(1, 5);
        ticket.Inativar();
        Assert.Equal(SituacaoCadastro.Inativo, ticket.Situacao);
    }

    [Fact]
    public void Ativar_TicketInativo_MudaSituacaoParaAtivo()
    {
        var ticket = new TicketEntregue(1, 5);
        ticket.Inativar();
        ticket.Ativar();
        Assert.Equal(SituacaoCadastro.Ativo, ticket.Situacao);
    }

    // -------------------------
    // DefinirId
    // -------------------------

    [Fact]
    public void DefinirId_IdValido_AtribuiIdAoTicket()
    {
        var ticket = new TicketEntregue(1, 5);
        ticket.DefinirId(10);
        Assert.Equal(10, ticket.Id);
    }

    [Fact]
    public void DefinirId_ChamadoDuasVezes_LancaDomainException()
    {
        var ticket = new TicketEntregue(1, 5);
        ticket.DefinirId(10);
        Assert.Throws<DomainException>(() => ticket.DefinirId(20));
    }

    // -------------------------
    // Reconstituir
    // -------------------------

    [Fact]
    public void Reconstituir_DadosValidos_CriaTicketComTodosOsDados()
    {
        var dataEntrega = DateTime.Now;
        var ticket = TicketEntregue.Reconstituir(3, 1, 7, SituacaoCadastro.Inativo, dataEntrega);

        Assert.Equal(3, ticket.Id);
        Assert.Equal(1, ticket.FuncionarioId);
        Assert.Equal(7, ticket.Quantidade);
        Assert.Equal(SituacaoCadastro.Inativo, ticket.Situacao);
        Assert.Equal(dataEntrega, ticket.DataEntrega);
    }
}