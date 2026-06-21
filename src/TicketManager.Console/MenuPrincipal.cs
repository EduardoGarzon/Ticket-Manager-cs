using TicketManager.Application.Services;
using TicketManager.Domain.Exceptions;
using TicketManager.Domain.Entities;

namespace TicketManager.Console;

public class MenuPrincipal
{
    private readonly FuncionarioService _funcionarioService;
    private readonly TicketEntregueService _ticketService;
    private readonly RelatorioService _relatorioService;

    public MenuPrincipal(
        FuncionarioService funcionarioService,
        TicketEntregueService ticketService,
        RelatorioService relatorioService)
    {
        _funcionarioService = funcionarioService;
        _ticketService = ticketService;
        _relatorioService = relatorioService;
    }

    public void Executar()
    {
        var continuar = true;

        while (continuar)
        {
            System.Console.Clear();

            ExibirMenu();
            var opcao = System.Console.ReadLine();

            try
            {
                switch (opcao)
                {
                    case "1": CadastrarFuncionario(); break;
                    case "2": EditarFuncionario(); break;
                    case "3": ListarFuncionarios(); break;
                    case "4": RegistrarTicket(); break;
                    case "5": EditarTicket(); break;
                    case "6": EmitirRelatorios(); break;
                    case "0": continuar = false; break;
                    default:
                        System.Console.Write(Environment.NewLine);
                        System.Console.WriteLine("Opção inválida.");
                        break;
                }
            }
            catch (DomainException ex)
            {
                System.Console.Write(Environment.NewLine);
                System.Console.WriteLine($"Erro: {ex.Message}");
            }
            finally
            {
                System.Console.Write(Environment.NewLine);
                System.Console.Write("Pressione ENTER para continuar...");
                System.Console.ReadLine();
            }
        }
    }

    private static void ExibirMenu()
    {
        System.Console.WriteLine("===== Ticket Manager =====");
        System.Console.WriteLine("[1] -> Cadastrar funcionário");
        System.Console.WriteLine("[2] -> Editar funcionário");
        System.Console.WriteLine("[3] -> Listar funcionários");
        System.Console.WriteLine("[4] -> Registrar ticket entregue");
        System.Console.WriteLine("[5] -> Editar ticket entregue");
        System.Console.WriteLine("[6] -> Emitir Relatórios");
        System.Console.WriteLine("[0] -> Sair");
        System.Console.WriteLine("==========================");
        System.Console.Write(Environment.NewLine);
        System.Console.Write("Escolha uma opção: ");
    }

    private void CadastrarFuncionario()
    {
        System.Console.Clear();

        System.Console.Write("Nome: ");
        var nome = System.Console.ReadLine() ?? string.Empty;

        System.Console.Write(Environment.NewLine);
        System.Console.Write("CPF (somente números): ");
        var cpf = System.Console.ReadLine() ?? string.Empty;

        var funcionario = _funcionarioService.Cadastrar(nome, cpf);

        System.Console.Write(Environment.NewLine);
        System.Console.WriteLine($"Funcionário cadastrado com Id {funcionario.Id}.");
    }

    private void EditarFuncionario()
    {
        System.Console.Clear();
        System.Console.Write(Environment.NewLine);

        System.Console.WriteLine("[1] -> Buscar pelo ID do Funcionário");
        System.Console.WriteLine("[2] -> Buscar pelo CPF do Funcionário");
        System.Console.WriteLine("======================================");

        System.Console.Write(Environment.NewLine);
        System.Console.Write("Escolha uma opção: ");
        var opcao = System.Console.ReadLine();

        Funcionario? funcionario;

        if (opcao == "1")
        {
            System.Console.Write(Environment.NewLine);
            System.Console.Write("Id do funcionário: ");
            if (!int.TryParse(System.Console.ReadLine(), out var funcionarioId) || funcionarioId <= 0)
            {
                System.Console.WriteLine("Id inválido.");
                return;
            }

            funcionario = _funcionarioService.ObterFuncionarioPorId(funcionarioId);
        }
        else if (opcao == "2")
        {
            System.Console.Write(Environment.NewLine);
            System.Console.Write("CPF do funcionário: ");
            var funcionarioCpf = System.Console.ReadLine() ?? string.Empty;

            funcionario = _funcionarioService.ObterFuncionarioPorCpf(funcionarioCpf);
        }
        else
        {
            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine("Opção inválida.");
            return;
        }

        if (funcionario is null)
        {
            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine("Funcionário não encontrado.");
            return;
        }

        System.Console.Write(Environment.NewLine);
        System.Console.Write("Novo nome: ");
        var nome = System.Console.ReadLine() ?? string.Empty;

        System.Console.Write(Environment.NewLine);
        System.Console.Write("Novo CPF: ");
        var cpf = System.Console.ReadLine() ?? string.Empty;

        _funcionarioService.Editar(funcionario.Id, nome, cpf);

        System.Console.Write(Environment.NewLine);
        System.Console.Write("Funcionário ativo? (S/N): ");
        var resposta = System.Console.ReadLine();

        if (string.Equals(resposta, "S", StringComparison.OrdinalIgnoreCase))
            _funcionarioService.Ativar(funcionario.Id);
        else if (string.Equals(resposta, "N", StringComparison.OrdinalIgnoreCase))
            _funcionarioService.Inativar(funcionario.Id);

        System.Console.Write(Environment.NewLine);
        System.Console.WriteLine("Funcionário atualizado.");
    }

    private void ListarFuncionarios()
    {
        System.Console.Clear();

        System.Console.WriteLine("[1] -> Listar Funcinários Ativos");
        System.Console.WriteLine("[2] -> Editar Funcionários Inativos");
        System.Console.WriteLine("[3] -> Listar Todos os funcionários");

        System.Console.Write(Environment.NewLine);
        System.Console.Write("Escolha uma opção: ");

        var opcao = System.Console.ReadLine();
        switch (opcao)
        {
            case "1": ListarFuncionariosAtivos(); break;
            case "2": ListarFuncionariosInativos(); break;
            case "3": ListarTodosFuncionarios(); break;
            default:
                System.Console.Write(Environment.NewLine);
                System.Console.WriteLine("Opção inválida.");
                break;
        }
    }

    private void ListarTodosFuncionarios()
    {
        System.Console.Write(Environment.NewLine);
        System.Console.WriteLine($"{"ID",-5} {"NOME",-30} {"CPF",-15} {"SITUAÇÃO",-10}");
        System.Console.WriteLine(new string('-', 65));

        foreach (var funcionario in _funcionarioService.ListarTodos())
        {
            System.Console.WriteLine($"{funcionario.Id,-5} {funcionario.Nome,-30} {funcionario.Cpf,-15} {funcionario.Situacao,-10}");
        }
    }

    private void ListarFuncionariosAtivos()
    {
        System.Console.Write(Environment.NewLine);
        System.Console.WriteLine($"{"ID",-5} {"NOME",-30} {"CPF",-15} {"SITUAÇÃO",-10}");
        System.Console.WriteLine(new string('-', 65));

        foreach (var funcionario in _funcionarioService.ListarTodosAtivos())
        {
            System.Console.WriteLine($"{funcionario.Id,-5} {funcionario.Nome,-30} {funcionario.Cpf,-15} {funcionario.Situacao,-10}");
        }
    }

    private void ListarFuncionariosInativos()
    {
        System.Console.Write(Environment.NewLine);
        System.Console.WriteLine($"{"ID",-5} {"NOME",-30} {"CPF",-15} {"SITUAÇÃO",-10}");
        System.Console.WriteLine(new string('-', 65));

        foreach (var funcionario in _funcionarioService.ListarTodosInativos())
        {
            System.Console.WriteLine($"{funcionario.Id,-5} {funcionario.Nome,-30} {funcionario.Cpf,-15} {funcionario.Situacao,-10}");
        }
    }

    private void RegistrarTicket()
    {
        System.Console.Clear();
        System.Console.Write(Environment.NewLine);

        System.Console.WriteLine("[1] -> Buscar pelo ID do Funcionário");
        System.Console.WriteLine("[2] -> Buscar pelo CPF do Funcionário");
        System.Console.WriteLine("======================================");

        System.Console.Write(Environment.NewLine);
        System.Console.Write("Escolha uma opção: ");
        var opcao = System.Console.ReadLine();

        Funcionario? funcionario;

        if (opcao == "1")
        {
            System.Console.Write(Environment.NewLine);
            System.Console.Write("Id do funcionário: ");
            if (!int.TryParse(System.Console.ReadLine(), out var funcionarioId) || funcionarioId <= 0)
            {
                System.Console.WriteLine("Id inválido.");
                return;
            }

            funcionario = _funcionarioService.ObterFuncionarioPorId(funcionarioId);
        }
        else if (opcao == "2")
        {
            System.Console.Write(Environment.NewLine);
            System.Console.Write("CPF do funcionário: ");
            var funcionarioCpf = System.Console.ReadLine() ?? string.Empty;

            funcionario = _funcionarioService.ObterFuncionarioPorCpf(funcionarioCpf);
        }
        else
        {
            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine("Opção inválida.");
            return;
        }

        if (funcionario is null)
        {
            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine("Funcionário não encontrado.");
            return;
        }

        System.Console.Write(Environment.NewLine);
        System.Console.Write("Quantidade de tickets: ");
        if (!int.TryParse(System.Console.ReadLine(), out var quantidade) || quantidade <= 0)
        {
            System.Console.WriteLine("Quantidade inválida.");
            return;
        }

        var ticket = _ticketService.RegistrarEntrega(funcionario.Id, quantidade);
        System.Console.Write(Environment.NewLine);
        System.Console.WriteLine($"Ticket registrado com Id {ticket.Id}.");
    }

    private void EditarTicket()
    {
        System.Console.Clear();

        System.Console.Write("Id do ticket: ");
        if (!int.TryParse(System.Console.ReadLine(), out var id))
        {
            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine("Id inválido.");
            return;
        }

        if (id <= 0)
        {
            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine("O Id deve ser maior que zero.");
            return;
        }

        System.Console.Write(Environment.NewLine);
        System.Console.Write("Nova quantidade: ");
        if (!int.TryParse(System.Console.ReadLine(), out var quantidade))
        {
            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine("Quantidade inválida.");
            return;
        }

        System.Console.Write(Environment.NewLine);
        System.Console.Write("Ticket ativo? (S/N): ");
        var situacao = System.Console.ReadLine();

        if (string.Equals(situacao, "S", StringComparison.OrdinalIgnoreCase))
            _ticketService.Ativar(id);
        else if (string.Equals(situacao, "N", StringComparison.OrdinalIgnoreCase))
            _ticketService.Inativar(id);

        _ticketService.EditarQuantidade(id, quantidade);

        System.Console.Write(Environment.NewLine);
        System.Console.WriteLine("Ticket atualizado.");
    }

    private void EmitirRelatorios()
    {
        System.Console.Clear();
        System.Console.WriteLine("[1] -> Emitir tickets de um funcionário");
        System.Console.WriteLine("[2] -> Emitir relatório Completo por período (Funcionarios + Total de Tickets + Total Geral)");
        System.Console.WriteLine("[3] -> Emitir relatório Completo (Funcionarios + Total de Tickets + Total Geral)");
        System.Console.WriteLine("=============================================================================================");

        System.Console.Write(Environment.NewLine);
        System.Console.Write("Escolha uma opção: ");

        var opcao = System.Console.ReadLine();
        switch (opcao)
        {
            case "1": EmitirRelatorioTicketsDoFuncionario(); break;
            case "2": EmitirRelatorioFuncionariosTicketsPorPeriodo(); break;
            case "3": EmitirRelatorioFuncionariosTicketsCompleto(); break;
            default:
                System.Console.Write(Environment.NewLine);
                System.Console.WriteLine("Opção inválida.");
                break;
        }
    }

    // Emite todos os tickets de um funcionario.
    private void EmitirRelatorioTicketsDoFuncionario()
    {
        System.Console.Clear();
        System.Console.Write(Environment.NewLine);

        System.Console.WriteLine("[1] -> Buscar pelo ID do Funcionário");
        System.Console.WriteLine("[2] -> Buscar pelo CPF do Funcionário");
        System.Console.WriteLine("==========================");

        System.Console.Write(Environment.NewLine);
        System.Console.Write("Escolha uma opção: ");
        var opcao = System.Console.ReadLine();

        Funcionario? funcionario;

        if (opcao == "1")
        {
            System.Console.Write(Environment.NewLine);
            System.Console.Write("Id do funcionário: ");
            if (!int.TryParse(System.Console.ReadLine(), out var funcionarioId) || funcionarioId <= 0)
            {
                System.Console.WriteLine("Id inválido.");
                return;
            }

            funcionario = _funcionarioService.ObterFuncionarioPorId(funcionarioId);
        }
        else if (opcao == "2")
        {
            System.Console.Write(Environment.NewLine);
            System.Console.Write("CPF do funcionário: ");
            var cpf = System.Console.ReadLine() ?? string.Empty;

            funcionario = _funcionarioService.ObterFuncionarioPorCpf(cpf);
        }
        else
        {
            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine("Opção inválida.");
            return;
        }

        if (funcionario is null)
        {
            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine("Funcionário não encontrado.");
            return;
        }

        var tickets = _ticketService.ListarPorFuncionario(funcionario.Id).ToList();

        System.Console.Write(Environment.NewLine);
        System.Console.WriteLine($"Exibindo Tickets Entregues do Funcionário: {funcionario.Nome}");

        System.Console.Write(Environment.NewLine);
        System.Console.WriteLine($"{"ID",-5} {"QUANTIDADE",-12} {"SITUAÇÃO",-12} {"DATA ENTREGA",-20}");
        System.Console.WriteLine(new string('-', 60));
        foreach (var ticket in tickets)
        {
            System.Console.WriteLine($"{ticket.Id,-5} {ticket.Quantidade,-12} {ticket.Situacao,-12} {ticket.DataEntrega,-20}");
        }
    }

    // Exibe ID, Funcionario, Situacao do Cadastro, Total de Tickets do Funcionario (Ativos e Inativos) e Total Geral de Tickets no intervalo de tempo indicado.
    private void EmitirRelatorioFuncionariosTicketsPorPeriodo()
    {
        System.Console.Clear();
        System.Console.Write(Environment.NewLine);

        System.Console.Write("Data inicial (dd/MM/yyyy): ");
        if (!DateTime.TryParse(System.Console.ReadLine(), out var inicio))
        {
            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine("Data inválida.");
            return;
        }

        System.Console.Write(Environment.NewLine);
        System.Console.Write("Data final (dd/MM/yyyy): ");
        if (!DateTime.TryParse(System.Console.ReadLine(), out var fim))
        {
            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine("Data inválida.");
            return;
        }

        var relatorio = _relatorioService.GerarRelatorioPorPeriodo(inicio, fim);

        System.Console.Write(Environment.NewLine);
        System.Console.WriteLine($"Relatório de {relatorio.Inicio:dd/MM/yyyy} a {relatorio.Fim:dd/MM/yyyy}");

        System.Console.Write(Environment.NewLine);
        System.Console.WriteLine($"{"ID",-10} | {"NOME",-30} | {"SITUACAO",-15} | {"TOTAL DE TICKETS",-15}");
        System.Console.WriteLine(new string('-', 90));
        foreach (var item in relatorio.Itens)
        {
            System.Console.WriteLine($"{item.FuncionarioId,-10} | {item.NomeFuncionario,-30} | {item.SituacaoFuncionario,-15} | {item.TotalTickets,-15}");
        }
        System.Console.WriteLine(new string('-', 90));
        System.Console.WriteLine($"TOTAL DE TICKETS: {relatorio.TotalGeral}");
    }

    // Exibe ID, Funcionario, Situacao do Cadastro, Total de Tickets do Funcionario (Ativos e Inativos) e o Total Geral de Tickets.
    private void EmitirRelatorioFuncionariosTicketsCompleto()
    {
        System.Console.Clear();
        System.Console.Write(Environment.NewLine);

        var resultado = _relatorioService.GerarRelatorioCompleto();

        System.Console.WriteLine($"Relatório completo do sistema.");

        System.Console.Write(Environment.NewLine);
        System.Console.WriteLine($"{"ID",-10} | {"NOME",-30} | {"CADASTRO",-15} | {"TOTAL DE TICKETS",-15}");
        System.Console.WriteLine(new string('-', 90));

        foreach (var item in resultado.Itens)
        {
            System.Console.WriteLine(
                $"{item.Funcionario.Id,-10} | {item.Funcionario.Nome,-30} | {item.Funcionario.Situacao,-15} | {item.TotalTicketsFuncionario,-15}");
        }

        System.Console.WriteLine(new string('-', 90));
        System.Console.WriteLine($"TOTAL DE TICKETS: {resultado.TotalGeral}");
    }
}