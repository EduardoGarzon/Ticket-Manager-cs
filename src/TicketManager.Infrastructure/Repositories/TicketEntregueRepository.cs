using Dapper;
using TicketManager.Domain.Entities;
using TicketManager.Domain.Enums;
using TicketManager.Domain.Repositories;

namespace TicketManager.Infrastructure.Repositories;

public class TicketEntregueRepository : ITicketEntregueRepository
{
    private readonly MySqlConnectionFactory _connectionFactory;

    public TicketEntregueRepository(MySqlConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public void Adicionar(TicketEntregue ticket)
    {
        const string sql = @"
            INSERT INTO tickets_entregues (funcionario_id, quantidade, situacao)
            VALUES (@FuncionarioId, @Quantidade, @Situacao);
            SELECT LAST_INSERT_ID();";

        using var conexao = _connectionFactory.CriarConexao();

        var novoId = conexao.QuerySingle<int>(sql, new
        {
            ticket.FuncionarioId,
            ticket.Quantidade,
            Situacao = MapearParaBanco(ticket.Situacao)
        });

        ticket.DefinirId(novoId);
    }

    public void Atualizar(TicketEntregue ticket)
    {
        const string sql = @"
            UPDATE tickets_entregues
            SET quantidade = @Quantidade, situacao = @Situacao
            WHERE id = @Id;";

        using var conexao = _connectionFactory.CriarConexao();

        conexao.Execute(sql, new
        {
            ticket.Id,
            ticket.Quantidade,
            Situacao = MapearParaBanco(ticket.Situacao)
        });
    }

    public TicketEntregue? ObterPorId(int id)
    {
        const string sql = @"
            SELECT id, funcionario_id AS FuncionarioId, quantidade, situacao, data_entrega AS DataEntrega
            FROM tickets_entregues
            WHERE id = @Id;";

        using var conexao = _connectionFactory.CriarConexao();
        var registro = conexao.QuerySingleOrDefault<TicketEntregueRegistro>(sql, new { Id = id });

        return registro is null ? null : ConverterParaDominio(registro);
    }

    public IEnumerable<TicketEntregue> ObterPorFuncionario(int funcionarioId)
    {
        const string sql = @"
            SELECT id, funcionario_id AS FuncionarioId, quantidade, situacao, data_entrega AS DataEntrega
            FROM tickets_entregues
            WHERE funcionario_id = @FuncionarioId;";

        using var conexao = _connectionFactory.CriarConexao();
        var registros = conexao.Query<TicketEntregueRegistro>(sql, new { FuncionarioId = funcionarioId });

        return registros.Select(ConverterParaDominio);
    }

    public IEnumerable<TicketEntregue> ObterPorPeriodo(DateTime inicio, DateTime fim)
    {
        const string sql = @"
            SELECT id, funcionario_id AS FuncionarioId, quantidade, situacao, data_entrega AS DataEntrega
            FROM tickets_entregues
            WHERE data_entrega BETWEEN @Inicio AND @Fim;";

        using var conexao = _connectionFactory.CriarConexao();
        var registros = conexao.Query<TicketEntregueRegistro>(sql, new { Inicio = inicio, Fim = fim });

        return registros.Select(ConverterParaDominio);
    }

    private static TicketEntregue ConverterParaDominio(TicketEntregueRegistro registro)
    {
        var situacao = registro.Situacao == "A" ? SituacaoCadastro.Ativo : SituacaoCadastro.Inativo;
        return TicketEntregue.Reconstituir(registro.Id, registro.FuncionarioId, registro.Quantidade, situacao, registro.DataEntrega);
    }

    private static string MapearParaBanco(SituacaoCadastro situacao)
    {
        return situacao == SituacaoCadastro.Ativo ? "A" : "I";
    }
}