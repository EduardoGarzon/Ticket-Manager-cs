using Moq;
using TicketManager.Application.Services;
using TicketManager.Domain.Entities;
using TicketManager.Domain.Enums;
using TicketManager.Domain.Exceptions;
using TicketManager.Domain.Repositories;

namespace TicketManager.Application.Tests;

public class FuncionarioServiceTests
{
    private readonly Mock<IFuncionarioRepository> _mockRepo;
    private readonly FuncionarioService _service;

    public FuncionarioServiceTests()
    {
        _mockRepo = new Mock<IFuncionarioRepository>();
        _service = new FuncionarioService(_mockRepo.Object);
    }

    // -------------------------
    // Cadastrar
    // -------------------------

    [Fact]
    public void Cadastrar_CpfNaoCadastrado_AdicionaERetornaFuncionario()
    {
        _mockRepo.Setup(r => r.ObterPorCpf("12345678901"))
                 .Returns((Funcionario?)null);

        _mockRepo.Setup(r => r.Adicionar(It.IsAny<Funcionario>()))
                 .Callback<Funcionario>(f => f.DefinirId(1));

        var resultado = _service.Cadastrar("Maria Silva", "12345678901");

        Assert.Equal("Maria Silva", resultado.Nome);
        Assert.Equal("12345678901", resultado.Cpf);
        Assert.Equal(SituacaoCadastro.Ativo, resultado.Situacao);
        Assert.Equal(1, resultado.Id);

        _mockRepo.Verify(r => r.Adicionar(It.IsAny<Funcionario>()), Times.Once);
    }

    [Fact]
    public void Cadastrar_CpfJaCadastrado_LancaDomainException()
    {
        var existente = Funcionario.Reconstituir(1, "Outro", "12345678901", SituacaoCadastro.Ativo, DateTime.Now);
        _mockRepo.Setup(r => r.ObterPorCpf("12345678901")).Returns(existente);

        Assert.Throws<DomainException>(() => _service.Cadastrar("Maria Silva", "12345678901"));

        _mockRepo.Verify(r => r.Adicionar(It.IsAny<Funcionario>()), Times.Never);
    }

    // -------------------------
    // Editar
    // -------------------------

    [Fact]
    public void Editar_FuncionarioExistente_AtualizaEChamaRepositorio()
    {
        var funcionario = Funcionario.Reconstituir(1, "Maria Silva", "12345678901", SituacaoCadastro.Ativo, DateTime.Now);
        _mockRepo.Setup(r => r.ObterPorId(1)).Returns(funcionario);
        _mockRepo.Setup(r => r.ObterPorCpf("98765432100")).Returns((Funcionario?)null);

        _service.Editar(1, "Maria Souza", "98765432100");

        Assert.Equal("Maria Souza", funcionario.Nome);
        Assert.Equal("98765432100", funcionario.Cpf);

        _mockRepo.Verify(r => r.Atualizar(funcionario), Times.Once);
    }

    [Fact]
    public void Editar_FuncionarioNaoEncontrado_LancaDomainException()
    {
        _mockRepo.Setup(r => r.ObterPorId(99)).Returns((Funcionario?)null);

        Assert.Throws<DomainException>(() => _service.Editar(99, "Maria Silva", "12345678901"));
    }

    [Fact]
    public void Editar_CpfPertenceAOutroFuncionario_LancaDomainException()
    {
        var funcionario = Funcionario.Reconstituir(1, "Maria Silva", "12345678901", SituacaoCadastro.Ativo, DateTime.Now);
        var outro = Funcionario.Reconstituir(2, "João", "98765432100", SituacaoCadastro.Ativo, DateTime.Now);

        _mockRepo.Setup(r => r.ObterPorId(1)).Returns(funcionario);
        _mockRepo.Setup(r => r.ObterPorCpf("98765432100")).Returns(outro);

        Assert.Throws<DomainException>(() => _service.Editar(1, "Maria Silva", "98765432100"));
        _mockRepo.Verify(r => r.Atualizar(It.IsAny<Funcionario>()), Times.Never);
    }

    [Fact]
    public void Editar_CpfPertenceAoProprioFuncionario_NaoLancaExcecao()
    {
        var funcionario = Funcionario.Reconstituir(1, "Maria Silva", "12345678901", SituacaoCadastro.Ativo, DateTime.Now);
        _mockRepo.Setup(r => r.ObterPorId(1)).Returns(funcionario);
        _mockRepo.Setup(r => r.ObterPorCpf("12345678901")).Returns(funcionario);

        var exception = Record.Exception(() => _service.Editar(1, "Maria Souza", "12345678901"));
        Assert.Null(exception);
    }

    // -------------------------
    // Ativar / Inativar
    // -------------------------

    [Fact]
    public void Inativar_FuncionarioAtivo_MudaSituacaoEAtualiza()
    {
        var funcionario = Funcionario.Reconstituir(1, "Maria", "12345678901", SituacaoCadastro.Ativo, DateTime.Now);
        _mockRepo.Setup(r => r.ObterPorId(1)).Returns(funcionario);

        _service.Inativar(1);

        Assert.Equal(SituacaoCadastro.Inativo, funcionario.Situacao);
        _mockRepo.Verify(r => r.Atualizar(funcionario), Times.Once);
    }

    [Fact]
    public void Ativar_FuncionarioInativo_MudaSituacaoEAtualiza()
    {
        var funcionario = Funcionario.Reconstituir(1, "Maria", "12345678901", SituacaoCadastro.Inativo, DateTime.Now);
        _mockRepo.Setup(r => r.ObterPorId(1)).Returns(funcionario);

        _service.Ativar(1);

        Assert.Equal(SituacaoCadastro.Ativo, funcionario.Situacao);
        _mockRepo.Verify(r => r.Atualizar(funcionario), Times.Once);
    }

    [Fact]
    public void Ativar_FuncionarioNaoEncontrado_LancaDomainException()
    {
        _mockRepo.Setup(r => r.ObterPorId(99)).Returns((Funcionario?)null);
        Assert.Throws<DomainException>(() => _service.Ativar(99));
    }

    // -------------------------
    // ObterFuncionarioPorId
    // -------------------------

    [Fact]
    public void ObterFuncionarioPorId_FuncionarioExistente_RetornaFuncionario()
    {
        var funcionario = Funcionario.Reconstituir(1, "Maria", "12345678901", SituacaoCadastro.Ativo, DateTime.Now);
        _mockRepo.Setup(r => r.ObterPorId(1)).Returns(funcionario);

        var resultado = _service.ObterFuncionarioPorId(1);

        Assert.Equal(funcionario, resultado);
    }

    [Fact]
    public void ObterFuncionarioPorId_FuncionarioNaoEncontrado_RetornaNull()
    {
        _mockRepo.Setup(r => r.ObterPorId(99)).Returns((Funcionario?)null);

        var resultado = _service.ObterFuncionarioPorId(99);

        Assert.Null(resultado);
    }

    // -------------------------
    // ObterFuncionarioPorCpf
    // -------------------------

    [Fact]
    public void ObterFuncionarioPorCpf_FuncionarioExistente_RetornaFuncionario()
    {
        var funcionario = Funcionario.Reconstituir(1, "Maria", "12345678901", SituacaoCadastro.Ativo, DateTime.Now);
        _mockRepo.Setup(r => r.ObterPorCpf("12345678901")).Returns(funcionario);

        var resultado = _service.ObterFuncionarioPorCpf("12345678901");

        Assert.Equal(funcionario, resultado);
    }

    [Fact]
    public void ObterFuncionarioPorCpf_FuncionarioNaoEncontrado_RetornaNull()
    {
        _mockRepo.Setup(r => r.ObterPorCpf("00000000000")).Returns((Funcionario?)null);

        var resultado = _service.ObterFuncionarioPorCpf("00000000000");

        Assert.Null(resultado);
    }

    // -------------------------
    // ListarTodos / ListarTodosAtivos / ListarTodosInativos
    // -------------------------

    [Fact]
    public void ListarTodos_RetornaTodosOsFuncionariosDoRepositorio()
    {
        var lista = new List<Funcionario>
        {
            Funcionario.Reconstituir(1, "Maria", "12345678901", SituacaoCadastro.Ativo, DateTime.Now),
            Funcionario.Reconstituir(2, "João", "98765432100", SituacaoCadastro.Inativo, DateTime.Now)
        };
        _mockRepo.Setup(r => r.ObterTodos()).Returns(lista);

        var resultado = _service.ListarTodos();

        Assert.Equal(lista, resultado);
    }

    [Fact]
    public void ListarTodosAtivos_RetornaApenasAtivosDoRepositorio()
    {
        var ativos = new List<Funcionario>
        {
            Funcionario.Reconstituir(1, "Maria", "12345678901", SituacaoCadastro.Ativo, DateTime.Now)
        };
        _mockRepo.Setup(r => r.ObterTodosAtivos()).Returns(ativos);

        var resultado = _service.ListarTodosAtivos();

        Assert.Equal(ativos, resultado);
    }

    [Fact]
    public void ListarTodosInativos_RetornaApenasInativosDoRepositorio()
    {
        var inativos = new List<Funcionario>
        {
            Funcionario.Reconstituir(2, "João", "98765432100", SituacaoCadastro.Inativo, DateTime.Now)
        };
        _mockRepo.Setup(r => r.ObterTodosInativos()).Returns(inativos);

        var resultado = _service.ListarTodosInativos();

        Assert.Equal(inativos, resultado);
    }
}