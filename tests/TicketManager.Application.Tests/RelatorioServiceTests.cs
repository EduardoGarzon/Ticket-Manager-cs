using Moq;
using TicketManager.Application.Services;
using TicketManager.Domain.Entities;
using TicketManager.Domain.Enums;
using TicketManager.Domain.Repositories;

namespace TicketManager.Application.Tests;

public class RelatorioServiceTests
{
    private readonly Mock<ITicketEntregueRepository> _mockTicketRepo;
    private readonly Mock<IFuncionarioRepository> _mockFuncionarioRepo;
    private readonly RelatorioService _service;

    public RelatorioServiceTests()
    {
        _mockTicketRepo = new Mock<ITicketEntregueRepository>();
        _mockFuncionarioRepo = new Mock<IFuncionarioRepository>();
        _service = new RelatorioService(_mockTicketRepo.Object, _mockFuncionarioRepo.Object);
    }

    [Fact]
    public void GerarPorPeriodo_ComTickets_RetornaTotaisCorretosEOrdenados()
    {
        // Arrange
        var inicio = new DateTime(2025, 1, 1);
        var fim = new DateTime(2025, 1, 31);

        var maria = Funcionario.Reconstituir(1, "Maria Silva", "12345678901", SituacaoCadastro.Ativo, DateTime.Now);
        var joao = Funcionario.Reconstituir(2, "João Souza", "98765432100", SituacaoCadastro.Ativo, DateTime.Now);

        var tickets = new List<TicketEntregue>
        {
            TicketEntregue.Reconstituir(1, 1, 10, SituacaoCadastro.Ativo, new DateTime(2025, 1, 10)),
            TicketEntregue.Reconstituir(2, 1, 5,  SituacaoCadastro.Ativo, new DateTime(2025, 1, 20)),
            TicketEntregue.Reconstituir(3, 2, 8,  SituacaoCadastro.Ativo, new DateTime(2025, 1, 15)),
        };

        _mockTicketRepo.Setup(r => r.ObterPorPeriodo(inicio, fim)).Returns(tickets);
        _mockFuncionarioRepo.Setup(r => r.ObterTodos()).Returns(new List<Funcionario> { maria, joao });

        // Act
        var relatorio = _service.GerarPorPeriodo(inicio, fim);

        // Assert: total geral correto (10 + 5 + 8 = 23)
        Assert.Equal(23, relatorio.TotalGeral);
        Assert.Equal(2, relatorio.Itens.Count);

        // Maria: 10 + 5 = 15
        var itemMaria = relatorio.Itens.First(i => i.FuncionarioId == 1);
        Assert.Equal(15, itemMaria.TotalTickets);

        // João: 8
        var itemJoao = relatorio.Itens.First(i => i.FuncionarioId == 2);
        Assert.Equal(8, itemJoao.TotalTickets);

        // Ordenação por nome: João vem antes de Maria
        Assert.Equal("João Souza", relatorio.Itens[0].NomeFuncionario);
        Assert.Equal("Maria Silva", relatorio.Itens[1].NomeFuncionario);
    }

    [Fact]
    public void GerarPorPeriodo_SemTicketsNoPeriodo_RetornaRelatorioVazio()
    {
        // Arrange
        var inicio = new DateTime(2025, 1, 1);
        var fim = new DateTime(2025, 1, 31);

        _mockTicketRepo.Setup(r => r.ObterPorPeriodo(inicio, fim)).Returns(new List<TicketEntregue>());
        _mockFuncionarioRepo.Setup(r => r.ObterTodos()).Returns(new List<Funcionario>());

        // Act
        var relatorio = _service.GerarPorPeriodo(inicio, fim);

        // Assert
        Assert.Equal(0, relatorio.TotalGeral);
        Assert.Empty(relatorio.Itens);
    }
}