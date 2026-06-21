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

    // -------------------------
    // GerarRelatorioPorPeriodo
    // -------------------------

    [Fact]
    public void GerarRelatorioPorPeriodo_ComTickets_RetornaTotaisCorretosEOrdenados()
    {
        var inicio = new DateTime(2025, 1, 1);
        var fim = new DateTime(2025, 1, 31);

        var maria = Funcionario.Reconstituir(1, "Maria Silva", "12345678901", SituacaoCadastro.Ativo, DateTime.Now);
        var joao = Funcionario.Reconstituir(2, "João Souza", "98765432100", SituacaoCadastro.Inativo, DateTime.Now);

        var tickets = new List<TicketEntregue>
        {
            TicketEntregue.Reconstituir(1, 1, 10, SituacaoCadastro.Ativo, new DateTime(2025, 1, 10)),
            TicketEntregue.Reconstituir(2, 1, 5,  SituacaoCadastro.Ativo, new DateTime(2025, 1, 20)),
            TicketEntregue.Reconstituir(3, 2, 8,  SituacaoCadastro.Ativo, new DateTime(2025, 1, 15)),
        };

        // O serviço ajusta "fim" internamente (fimDoDia) — o mock precisa aceitar
        // qualquer DateTime nesse parâmetro, não o valor exato de "fim".
        _mockTicketRepo.Setup(r => r.ObterPorPeriodo(inicio, It.IsAny<DateTime>())).Returns(tickets);
        _mockFuncionarioRepo.Setup(r => r.ObterTodos()).Returns(new List<Funcionario> { maria, joao });

        var relatorio = _service.GerarRelatorioPorPeriodo(inicio, fim);

        Assert.Equal(23, relatorio.TotalGeral);
        Assert.Equal(2, relatorio.Itens.Count);

        var itemMaria = relatorio.Itens.First(i => i.FuncionarioId == 1);
        Assert.Equal(15, itemMaria.TotalTickets);
        Assert.Equal(SituacaoCadastro.Ativo, itemMaria.SituacaoFuncionario);

        var itemJoao = relatorio.Itens.First(i => i.FuncionarioId == 2);
        Assert.Equal(8, itemJoao.TotalTickets);
        Assert.Equal(SituacaoCadastro.Inativo, itemJoao.SituacaoFuncionario);

        Assert.Equal("João Souza", relatorio.Itens[0].NomeFuncionario);
        Assert.Equal("Maria Silva", relatorio.Itens[1].NomeFuncionario);
    }

    [Fact]
    public void GerarRelatorioPorPeriodo_SemTicketsNoPeriodo_RetornaRelatorioVazio()
    {
        var inicio = new DateTime(2025, 1, 1);
        var fim = new DateTime(2025, 1, 31);

        _mockTicketRepo.Setup(r => r.ObterPorPeriodo(inicio, It.IsAny<DateTime>())).Returns(new List<TicketEntregue>());
        _mockFuncionarioRepo.Setup(r => r.ObterTodos()).Returns(new List<Funcionario>());

        var relatorio = _service.GerarRelatorioPorPeriodo(inicio, fim);

        Assert.Equal(0, relatorio.TotalGeral);
        Assert.Empty(relatorio.Itens);
    }

    [Fact]
    public void GerarRelatorioPorPeriodo_TicketDeFuncionarioInexistente_ExibeComoNaoEncontrado()
    {
        var inicio = new DateTime(2025, 1, 1);
        var fim = new DateTime(2025, 1, 31);

        var tickets = new List<TicketEntregue>
        {
            TicketEntregue.Reconstituir(1, 99, 3, SituacaoCadastro.Ativo, new DateTime(2025, 1, 10))
        };

        _mockTicketRepo.Setup(r => r.ObterPorPeriodo(inicio, It.IsAny<DateTime>())).Returns(tickets);
        _mockFuncionarioRepo.Setup(r => r.ObterTodos()).Returns(new List<Funcionario>());

        var relatorio = _service.GerarRelatorioPorPeriodo(inicio, fim);

        var item = Assert.Single(relatorio.Itens);
        Assert.Equal("Funcionário não encontrado", item.NomeFuncionario);
        Assert.Equal(3, item.TotalTickets);
    }

    // -------------------------
    // GerarRelatorioCompleto
    // -------------------------

    [Fact]
    public void GerarRelatorioCompleto_ComFuncionariosETickets_RetornaItensETotalGeral()
    {
        var maria = Funcionario.Reconstituir(1, "Maria Silva", "12345678901", SituacaoCadastro.Ativo, DateTime.Now);
        var joao = Funcionario.Reconstituir(2, "João Souza", "98765432100", SituacaoCadastro.Ativo, DateTime.Now);

        var tickets = new List<TicketEntregue>
        {
            TicketEntregue.Reconstituir(1, 1, 10, SituacaoCadastro.Ativo, DateTime.Now),
            TicketEntregue.Reconstituir(2, 1, 5,  SituacaoCadastro.Ativo, DateTime.Now),
        };

        _mockFuncionarioRepo.Setup(r => r.ObterTodos()).Returns(new List<Funcionario> { maria, joao });
        _mockTicketRepo.Setup(r => r.ObterTodos()).Returns(tickets);

        var resultado = _service.GerarRelatorioCompleto();
        var itens = resultado.Itens.ToList();

        Assert.Equal(15, resultado.TotalGeral);
        Assert.Equal(2, itens.Count);

        var itemMaria = itens.First(i => i.Funcionario.Id == 1);
        Assert.Equal(15, itemMaria.TotalTicketsFuncionario);
        Assert.Equal(2, itemMaria.Tickets.Count);

        var itemJoao = itens.First(i => i.Funcionario.Id == 2);
        Assert.Equal(0, itemJoao.TotalTicketsFuncionario);
        Assert.Empty(itemJoao.Tickets);
    }

    [Fact]
    public void GerarRelatorioCompleto_FuncionarioSemTickets_RetornaListaVaziaETotalZero()
    {
        var maria = Funcionario.Reconstituir(1, "Maria Silva", "12345678901", SituacaoCadastro.Ativo, DateTime.Now);

        _mockFuncionarioRepo.Setup(r => r.ObterTodos()).Returns(new List<Funcionario> { maria });
        _mockTicketRepo.Setup(r => r.ObterTodos()).Returns(new List<TicketEntregue>());

        var resultado = _service.GerarRelatorioCompleto();
        var item = Assert.Single(resultado.Itens);

        Assert.Equal(0, item.TotalTicketsFuncionario);
        Assert.Empty(item.Tickets);
        Assert.Equal(0, resultado.TotalGeral);
    }
}