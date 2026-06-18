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
        // Arrange: CPF ainda não existe no banco (dublê devolve null)
        _mockRepo.Setup(r => r.ObterPorCpf("12345678901"))
                 .Returns((Funcionario?)null);

        // Callback: simula o que o repositório real faz — definir o Id após INSERT
        _mockRepo.Setup(r => r.Adicionar(It.IsAny<Funcionario>()))
                 .Callback<Funcionario>(f => f.DefinirId(1));

        // Act
        var resultado = _service.Cadastrar("Maria Silva", "12345678901");

        // Assert: objeto retornado tem os dados certos e Id foi atribuído
        Assert.Equal("Maria Silva", resultado.Nome);
        Assert.Equal("12345678901", resultado.Cpf);
        Assert.Equal(SituacaoCadastro.Ativo, resultado.Situacao);
        Assert.Equal(1, resultado.Id);

        // Verify: Adicionar foi chamado exatamente uma vez
        _mockRepo.Verify(r => r.Adicionar(It.IsAny<Funcionario>()), Times.Once);
    }

    [Fact]
    public void Cadastrar_CpfJaCadastrado_LancaDomainException()
    {
        // Arrange: CPF já existe — dublê devolve um funcionário existente
        var existente = Funcionario.Reconstituir(1, "Outro", "12345678901", SituacaoCadastro.Ativo, DateTime.Now);
        _mockRepo.Setup(r => r.ObterPorCpf("12345678901")).Returns(existente);

        // Act & Assert: serviço deve lançar exceção
        Assert.Throws<DomainException>(() => _service.Cadastrar("Maria Silva", "12345678901"));

        // Verify: Adicionar jamais deve ter sido chamado
        _mockRepo.Verify(r => r.Adicionar(It.IsAny<Funcionario>()), Times.Never);
    }

    // -------------------------
    // Editar
    // -------------------------

    [Fact]
    public void Editar_FuncionarioExistente_AtualizaEChamaRepositorio()
    {
        // Arrange
        var funcionario = Funcionario.Reconstituir(1, "Maria Silva", "12345678901", SituacaoCadastro.Ativo, DateTime.Now);
        _mockRepo.Setup(r => r.ObterPorId(1)).Returns(funcionario);
        _mockRepo.Setup(r => r.ObterPorCpf("98765432100")).Returns((Funcionario?)null);

        // Act
        _service.Editar(1, "Maria Souza", "98765432100");

        // Assert: propriedades foram atualizadas
        Assert.Equal("Maria Souza", funcionario.Nome);
        Assert.Equal("98765432100", funcionario.Cpf);

        // Verify: Atualizar foi chamado exatamente uma vez
        _mockRepo.Verify(r => r.Atualizar(funcionario), Times.Once);
    }

    [Fact]
    public void Editar_FuncionarioNaoEncontrado_LancaDomainException()
    {
        // Arrange: Id não existe no banco
        _mockRepo.Setup(r => r.ObterPorId(99)).Returns((Funcionario?)null);

        // Act & Assert
        Assert.Throws<DomainException>(() => _service.Editar(99, "Maria Silva", "12345678901"));
    }

    [Fact]
    public void Editar_CpfPertenceAOutroFuncionario_LancaDomainException()
    {
        // Arrange: funcionário 1 tenta usar CPF que já é do funcionário 2
        var funcionario = Funcionario.Reconstituir(1, "Maria Silva", "12345678901", SituacaoCadastro.Ativo, DateTime.Now);
        var outro = Funcionario.Reconstituir(2, "João", "98765432100", SituacaoCadastro.Ativo, DateTime.Now);

        _mockRepo.Setup(r => r.ObterPorId(1)).Returns(funcionario);
        _mockRepo.Setup(r => r.ObterPorCpf("98765432100")).Returns(outro);

        // Act & Assert
        Assert.Throws<DomainException>(() => _service.Editar(1, "Maria Silva", "98765432100"));
        _mockRepo.Verify(r => r.Atualizar(It.IsAny<Funcionario>()), Times.Never);
    }

    [Fact]
    public void Editar_CpfPertenceAoProprioFuncionario_NaoLancaExcecao()
    {
        // Arrange: funcionário editando sem mudar o CPF — ObterPorCpf devolve ele mesmo
        var funcionario = Funcionario.Reconstituir(1, "Maria Silva", "12345678901", SituacaoCadastro.Ativo, DateTime.Now);
        _mockRepo.Setup(r => r.ObterPorId(1)).Returns(funcionario);
        _mockRepo.Setup(r => r.ObterPorCpf("12345678901")).Returns(funcionario);

        // Act & Assert: não deve lançar exceção, porque o CPF "duplicado" é do próprio
        var exception = Record.Exception(() => _service.Editar(1, "Maria Souza", "12345678901"));
        Assert.Null(exception);
    }

    // -------------------------
    // Ativar / Inativar
    // -------------------------

    [Fact]
    public void Inativar_FuncionarioAtivo_MudaSituacaoEAtualiza()
    {
        // Arrange
        var funcionario = Funcionario.Reconstituir(1, "Maria", "12345678901", SituacaoCadastro.Ativo, DateTime.Now);
        _mockRepo.Setup(r => r.ObterPorId(1)).Returns(funcionario);

        // Act
        _service.Inativar(1);

        // Assert
        Assert.Equal(SituacaoCadastro.Inativo, funcionario.Situacao);
        _mockRepo.Verify(r => r.Atualizar(funcionario), Times.Once);
    }

    [Fact]
    public void Ativar_FuncionarioInativo_MudaSituacaoEAtualiza()
    {
        // Arrange
        var funcionario = Funcionario.Reconstituir(1, "Maria", "12345678901", SituacaoCadastro.Inativo, DateTime.Now);
        _mockRepo.Setup(r => r.ObterPorId(1)).Returns(funcionario);

        // Act
        _service.Ativar(1);

        // Assert
        Assert.Equal(SituacaoCadastro.Ativo, funcionario.Situacao);
        _mockRepo.Verify(r => r.Atualizar(funcionario), Times.Once);
    }

    [Fact]
    public void Ativar_FuncionarioNaoEncontrado_LancaDomainException()
    {
        _mockRepo.Setup(r => r.ObterPorId(99)).Returns((Funcionario?)null);
        Assert.Throws<DomainException>(() => _service.Ativar(99));
    }
}