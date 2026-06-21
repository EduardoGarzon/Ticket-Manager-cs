using Moq;
using TicketManager.Application.Services;
using TicketManager.Domain.Entities;
using TicketManager.Domain.Enums;
using TicketManager.Domain.Exceptions;
using TicketManager.Domain.Repositories;

namespace TicketManager.Application.Tests;

public class TicketEntregueServiceTests
{
    private readonly Mock<ITicketEntregueRepository> _mockTicketRepo;
    private readonly Mock<IFuncionarioRepository> _mockFuncionarioRepo;
    private readonly TicketEntregueService _service;

    public TicketEntregueServiceTests()
    {
        _mockTicketRepo = new Mock<ITicketEntregueRepository>();
        _mockFuncionarioRepo = new Mock<IFuncionarioRepository>();
        _service = new TicketEntregueService(_mockTicketRepo.Object, _mockFuncionarioRepo.Object);
    }

    // -------------------------
    // RegistrarEntrega
    // -------------------------

    [Fact]
    public void RegistrarEntrega_FuncionarioAtivoEQuantidadeValida_CriaERetornaTicket()
    {
        var funcionario = Funcionario.Reconstituir(1, "Maria", "12345678901", SituacaoCadastro.Ativo, DateTime.Now);
        _mockFuncionarioRepo.Setup(r => r.ObterPorId(1)).Returns(funcionario);
        _mockTicketRepo.Setup(r => r.Adicionar(It.IsAny<TicketEntregue>()))
                       .Callback<TicketEntregue>(t => t.DefinirId(10));

        var ticket = _service.RegistrarEntrega(1, 5);

        Assert.Equal(1, ticket.FuncionarioId);
        Assert.Equal(5, ticket.Quantidade);
        Assert.Equal(SituacaoCadastro.Ativo, ticket.Situacao);
        Assert.Equal(10, ticket.Id);
        _mockTicketRepo.Verify(r => r.Adicionar(It.IsAny<TicketEntregue>()), Times.Once);
    }

    [Fact]
    public void RegistrarEntrega_FuncionarioNaoEncontrado_LancaDomainException()
    {
        _mockFuncionarioRepo.Setup(r => r.ObterPorId(99)).Returns((Funcionario?)null);

        Assert.Throws<DomainException>(() => _service.RegistrarEntrega(99, 5));
        _mockTicketRepo.Verify(r => r.Adicionar(It.IsAny<TicketEntregue>()), Times.Never);
    }

    [Fact]
    public void RegistrarEntrega_FuncionarioInativo_LancaDomainException()
    {
        var inativo = Funcionario.Reconstituir(1, "Maria", "12345678901", SituacaoCadastro.Inativo, DateTime.Now);
        _mockFuncionarioRepo.Setup(r => r.ObterPorId(1)).Returns(inativo);

        var ex = Assert.Throws<DomainException>(() => _service.RegistrarEntrega(1, 5));
        Assert.Equal("Não é possível registrar tickets para um funcionário inativo.", ex.Message);
        _mockTicketRepo.Verify(r => r.Adicionar(It.IsAny<TicketEntregue>()), Times.Never);
    }

    // -------------------------
    // EditarQuantidade
    // -------------------------

    [Fact]
    public void EditarQuantidade_TicketExistente_AtualizaEChamaRepositorio()
    {
        var ticket = TicketEntregue.Reconstituir(1, 1, 5, SituacaoCadastro.Ativo, DateTime.Now);
        _mockTicketRepo.Setup(r => r.ObterPorId(1)).Returns(ticket);

        _service.EditarQuantidade(1, 10);

        Assert.Equal(10, ticket.Quantidade);
        _mockTicketRepo.Verify(r => r.Atualizar(ticket), Times.Once);
    }

    [Fact]
    public void EditarQuantidade_TicketNaoEncontrado_LancaDomainException()
    {
        _mockTicketRepo.Setup(r => r.ObterPorId(99)).Returns((TicketEntregue?)null);

        Assert.Throws<DomainException>(() => _service.EditarQuantidade(99, 5));
    }

    // -------------------------
    // Inativar / Ativar
    // -------------------------

    [Fact]
    public void Inativar_TicketAtivo_MudaSituacaoEAtualiza()
    {
        var ticket = TicketEntregue.Reconstituir(1, 1, 5, SituacaoCadastro.Ativo, DateTime.Now);
        _mockTicketRepo.Setup(r => r.ObterPorId(1)).Returns(ticket);

        _service.Inativar(1);

        Assert.Equal(SituacaoCadastro.Inativo, ticket.Situacao);
        _mockTicketRepo.Verify(r => r.Atualizar(ticket), Times.Once);
    }

    [Fact]
    public void Ativar_TicketInativo_MudaSituacaoEAtualiza()
    {
        var ticket = TicketEntregue.Reconstituir(1, 1, 5, SituacaoCadastro.Inativo, DateTime.Now);
        _mockTicketRepo.Setup(r => r.ObterPorId(1)).Returns(ticket);

        _service.Ativar(1);

        Assert.Equal(SituacaoCadastro.Ativo, ticket.Situacao);
        _mockTicketRepo.Verify(r => r.Atualizar(ticket), Times.Once);
    }

    // -------------------------
    // ListarPorFuncionario
    // -------------------------

    [Fact]
    public void ListarPorFuncionario_FuncionarioExistente_RetornaTicketsDoRepositorio()
    {
        var funcionario = Funcionario.Reconstituir(1, "Maria", "12345678901", SituacaoCadastro.Ativo, DateTime.Now);
        _mockFuncionarioRepo.Setup(r => r.ObterPorId(1)).Returns(funcionario);

        var tickets = new List<TicketEntregue>
        {
            TicketEntregue.Reconstituir(1, 1, 5, SituacaoCadastro.Ativo, DateTime.Now)
        };
        _mockTicketRepo.Setup(r => r.ObterPorFuncionario(1)).Returns(tickets);

        var resultado = _service.ListarPorFuncionario(1);

        Assert.Equal(tickets, resultado);
    }

    [Fact]
    public void ListarPorFuncionario_FuncionarioNaoEncontrado_LancaDomainException()
    {
        _mockFuncionarioRepo.Setup(r => r.ObterPorId(99)).Returns((Funcionario?)null);

        Assert.Throws<DomainException>(() => _service.ListarPorFuncionario(99));

        _mockTicketRepo.Verify(r => r.ObterPorFuncionario(It.IsAny<int>()), Times.Never);
    }
}