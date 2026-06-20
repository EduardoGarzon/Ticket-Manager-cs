using Dapper;
using TicketManager.Domain.Entities;
using TicketManager.Domain.Enums;
using TicketManager.Domain.Repositories;

namespace TicketManager.Infrastructure.Repositories;

public class FuncionarioRepository : IFuncionarioRepository
{
    private readonly MySqlConnectionFactory _connectionFactory;

    public FuncionarioRepository(MySqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void Adicionar(Funcionario funcionario)
    {
        const string sql = @"
            INSERT INTO funcionarios (nome, cpf, situacao)
            VALUES (@Nome, @Cpf, @Situacao);
            SELECT LAST_INSERT_ID();";

        using var conexao = _connectionFactory.CriarConexao();

        var novoId = conexao.QuerySingle<int>(sql, new
        {
            funcionario.Nome,
            funcionario.Cpf,
            Situacao = MapearParaBanco(funcionario.Situacao)
        });

        funcionario.DefinirId(novoId);
    }

    public void Atualizar(Funcionario funcionario)
    {
        const string sql = @"
            UPDATE funcionarios
            SET nome = @Nome, cpf = @Cpf, situacao = @Situacao
            WHERE id = @Id;";

        using var conexao = _connectionFactory.CriarConexao();

        conexao.Execute(sql, new
        {
            funcionario.Id,
            funcionario.Nome,
            funcionario.Cpf,
            Situacao = MapearParaBanco(funcionario.Situacao)
        });
    }

    public Funcionario? ObterPorId(int id)
    {
        const string sql = @"
            SELECT id, nome, cpf, situacao, data_alteracao AS DataAlteracao
            FROM funcionarios
            WHERE id = @Id;";

        using var conexao = _connectionFactory.CriarConexao();
        var registro = conexao.QuerySingleOrDefault<FuncionarioRegistro>(sql, new { Id = id });

        return registro is null ? null : ConverterParaDominio(registro);
    }

    public Funcionario? ObterPorCpf(string cpf)
    {
        const string sql = @"
            SELECT id, nome, cpf, situacao, data_alteracao AS DataAlteracao
            FROM funcionarios
            WHERE cpf = @Cpf;";

        using var conexao = _connectionFactory.CriarConexao();
        var registro = conexao.QuerySingleOrDefault<FuncionarioRegistro>(sql, new { Cpf = cpf });

        return registro is null ? null : ConverterParaDominio(registro);
    }

    public IEnumerable<Funcionario> ObterTodos()
    {
        const string sql = @"
            SELECT id, nome, cpf, situacao, data_alteracao AS DataAlteracao
            FROM funcionarios;";

        using var conexao = _connectionFactory.CriarConexao();
        var registros = conexao.Query<FuncionarioRegistro>(sql);

        return registros.Select(ConverterParaDominio);
    }

    public IEnumerable<Funcionario> ObterTodosAtivos()
    {
        const string sql = @"
            SELECT id, nome, cpf, situacao, data_alteracao AS DataAlteracao
            FROM funcionarios
            WHERE situacao = 'A';";

        using var conexao = _connectionFactory.CriarConexao();
        var registros = conexao.Query<FuncionarioRegistro>(sql);

        return registros.Select(ConverterParaDominio);
    }

    public IEnumerable<Funcionario> ObterTodosInativos()
    {
        const string sql = @"
            SELECT id, nome, cpf, situacao, data_alteracao AS DataAlteracao
            FROM funcionarios
            WHERE situacao = 'I';";

        using var conexao = _connectionFactory.CriarConexao();
        var registros = conexao.Query<FuncionarioRegistro>(sql);

        return registros.Select(ConverterParaDominio);
    }

    private static Funcionario ConverterParaDominio(FuncionarioRegistro registro)
    {
        var situacao = registro.Situacao == "A" ? SituacaoCadastro.Ativo : SituacaoCadastro.Inativo;
        return Funcionario.Reconstituir(registro.Id, registro.Nome, registro.Cpf, situacao, registro.DataAlteracao);
    }

    private static string MapearParaBanco(SituacaoCadastro situacao)
    {
        return situacao == SituacaoCadastro.Ativo ? "A" : "I";
    }
}