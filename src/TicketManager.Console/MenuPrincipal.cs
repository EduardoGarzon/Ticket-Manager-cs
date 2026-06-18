using TicketManager.Application.Services;
using TicketManager.Domain.Exceptions;

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
            System.Console.Clear();

            try
            {
                switch (opcao)
                {
                    case "1": CadastrarFuncionario(); break;
                    case "2": EditarFuncionario(); break;
                    case "3": ListarFuncionarios(); break;
                    case "4": RegistrarTicket(); break;
                    case "5": EditarTicket(); break;
                    case "6": ListarTicketsPorFuncionario(); break;
                    case "7": EmitirRelatorio(); break;
                    case "0": continuar = false; break;
                    default: System.Console.WriteLine("Opção inválida."); break;
                }
            }
            catch (DomainException ex)
            {
                System.Console.Write(Environment.NewLine);
                System.Console.WriteLine($"Erro: {ex.Message}");
            }

            System.Console.Write(Environment.NewLine);
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
        System.Console.WriteLine("[6] -> Listar tickets de um funcionário");
        System.Console.WriteLine("[7] -> Emitir relatório por período");
        System.Console.WriteLine("[0] -> Sair");
        System.Console.WriteLine("==========================");
        System.Console.Write(Environment.NewLine);
        System.Console.Write("Escolha uma opção: ");
    }

    private void CadastrarFuncionario()
    {
        try
        {
            System.Console.Write(Environment.NewLine);
            System.Console.Write("Nome: ");
            var nome = System.Console.ReadLine() ?? string.Empty;

            System.Console.Write(Environment.NewLine);
            System.Console.Write("CPF (somente números): ");
            var cpf = System.Console.ReadLine() ?? string.Empty;
            var funcionario = _funcionarioService.Cadastrar(nome, cpf);
            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine($"Funcionário cadastrado com Id {funcionario.Id}.");
        }
        catch (DomainException ex)
        {
            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine(ex.Message);
        }
        finally
        {
            System.Console.Write(Environment.NewLine);
            System.Console.Write("Pressione ENTER para continuar...");
            System.Console.ReadLine();
        }
    }

    private void EditarFuncionario()
    {
        try
        {
            System.Console.WriteLine(Environment.NewLine);
            System.Console.Write("Id do funcionário: ");
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
            System.Console.Write("Novo nome: ");
            var nome = System.Console.ReadLine() ?? string.Empty;

            System.Console.Write(Environment.NewLine);
            System.Console.Write("Novo CPF: ");
            var cpf = System.Console.ReadLine() ?? string.Empty;

            _funcionarioService.Editar(id, nome, cpf);

            System.Console.Write(Environment.NewLine);
            System.Console.Write("Funcionário ativo? (S/N): ");
            var resposta = System.Console.ReadLine();

            if (string.Equals(resposta, "S", StringComparison.OrdinalIgnoreCase))
                _funcionarioService.Ativar(id);
            else if (string.Equals(resposta, "N", StringComparison.OrdinalIgnoreCase))
                _funcionarioService.Inativar(id);

            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine("Funcionário atualizado.");

        }
        catch (DomainException ex)
        {
            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine(ex.Message);
        }
        finally
        {
            System.Console.Write(Environment.NewLine);
            System.Console.Write("Pressione ENTER para continuar...");
            System.Console.ReadLine();
        }
    }

    private void ListarFuncionarios()
    {
        try
        {
            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine($"{"ID",-5} {"NOME",-30} {"CPF",-15} {"SITUAÇÃO",-10}");
            System.Console.WriteLine(new string('-', 65));

            foreach (var funcionario in _funcionarioService.ListarTodos())
            {
                System.Console.WriteLine($"{funcionario.Id,-5} {funcionario.Nome,-30} {funcionario.Cpf,-15} {funcionario.Situacao,-10}");
            }
        }
        catch (DomainException ex)
        {
            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine(ex.Message);
        }
        finally
        {
            System.Console.Write(Environment.NewLine);
            System.Console.Write("Pressione ENTER para continuar...");
            System.Console.ReadLine();
        }
    }

    private void RegistrarTicket()
    {
        try
        {
            System.Console.Write(Environment.NewLine);
            System.Console.Write("Id do funcionário: ");
            if (!int.TryParse(System.Console.ReadLine(), out var funcionarioId))
            {
                System.Console.Write(Environment.NewLine);
                System.Console.WriteLine("Id inválido.");
                return;
            }

            if (funcionarioId <= 0)
            {
                System.Console.Write(Environment.NewLine);
                System.Console.WriteLine("O Id deve ser maior que zero.");
                return;
            }

            System.Console.Write(Environment.NewLine);
            System.Console.Write("Quantidade de tickets: ");
            if (!int.TryParse(System.Console.ReadLine(), out var quantidade))
            {
                System.Console.WriteLine("Quantidade inválida.");
                return;
            }

            if (quantidade <= 0)
            {
                System.Console.Write(Environment.NewLine);
                System.Console.WriteLine("A Quantidade deve ser maior que zero.");
                return;
            }

            var ticket = _ticketService.RegistrarEntrega(funcionarioId, quantidade);
            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine($"Ticket registrado com Id {ticket.Id}.");
        }
        catch (DomainException ex)
        {
            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine(ex.Message);
        }
        finally
        {
            System.Console.Write(Environment.NewLine);
            System.Console.Write("Pressione ENTER para continuar...");
            System.Console.ReadLine();
        }
    }

    private void EditarTicket()
    {
        try
        {
            System.Console.Write(Environment.NewLine);
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

            _ticketService.EditarQuantidade(id, quantidade);
            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine("Ticket atualizado.");
        }
        catch (DomainException ex)
        {
            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine(ex.Message);
        }
        finally
        {
            System.Console.Write(Environment.NewLine);
            System.Console.Write("Pressione ENTER para continuar...");
            System.Console.ReadLine();
        }
    }

    private void ListarTicketsPorFuncionario()
    {
        try
        {
            System.Console.Write(Environment.NewLine);
            System.Console.Write("Id do funcionário: ");
            if (!int.TryParse(System.Console.ReadLine(), out var funcionarioId))
            {
                System.Console.Write(Environment.NewLine);
                System.Console.WriteLine("Id inválido.");
                return;
            }

            if (funcionarioId <= 0)
            {
                System.Console.Write(Environment.NewLine);
                System.Console.WriteLine("O Id deve ser maior que zero.");
                return;
            }

            if (_ticketService.ListarPorFuncionario(funcionarioId) is not null)
            {
                System.Console.Write(Environment.NewLine);
                System.Console.WriteLine($"{"ID",-5} {"QUANTIDADE",-12} {"SITUAÇÃO",-12} {"DATA ENTREGA",-20}");

                foreach (var ticket in _ticketService.ListarPorFuncionario(funcionarioId))
                {
                    System.Console.WriteLine(
                        $"{ticket.Id,-5} {ticket.Quantidade,-12} {ticket.Situacao,-12} {ticket.DataEntrega,-20}");
                }
            }

        }
        catch (DomainException ex)
        {
            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine(ex.Message);
        }
        finally
        {
            System.Console.Write(Environment.NewLine);
            System.Console.Write("Pressione ENTER para continuar...");
            System.Console.ReadLine();
        }
    }

    private void EmitirRelatorio()
    {
        try
        {
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

            var relatorio = _relatorioService.GerarPorPeriodo(inicio, fim);

            System.Console.WriteLine($"Relatório de {relatorio.Inicio:dd/MM/yyyy} a {relatorio.Fim:dd/MM/yyyy}");
            System.Console.Write(Environment.NewLine);

            System.Console.WriteLine($"{"FUNCIONARIO",-30} | {"TOTAL DE TICKETS",-15}");
            System.Console.WriteLine(new string('-', 50));
            foreach (var item in relatorio.Itens)
            {
                System.Console.WriteLine($"{item.NomeFuncionario,-30} | {item.TotalTickets,-15}");
            }
            System.Console.WriteLine(new string('-', 50));
            System.Console.WriteLine($"TOTAL DE TICKETS: {relatorio.TotalGeral}");
        }
        catch (DomainException ex)
        {
            System.Console.Write(Environment.NewLine);
            System.Console.WriteLine(ex.Message);
        }
        finally
        {
            System.Console.Write(Environment.NewLine);
            System.Console.Write("Pressione ENTER para continuar...");
            System.Console.ReadLine();
        }
    }
}