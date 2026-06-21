using TicketManager.Application.Services;
using TicketManager.Console;
using TicketManager.Infrastructure;
using TicketManager.Infrastructure.Repositories;

var connectionString = "Server=localhost;Port=3306;Database=ticket_manager_db;User=root;Password=SUA_SENHA_AQUI";

var connectionFactory = new MySqlConnectionFactory(connectionString);
var funcionarioRepository = new FuncionarioRepository(connectionFactory);
var ticketRepository = new TicketEntregueRepository(connectionFactory);

var funcionarioService = new FuncionarioService(funcionarioRepository);
var ticketService = new TicketEntregueService(ticketRepository, funcionarioRepository);
var relatorioService = new RelatorioService(ticketRepository, funcionarioRepository);

var menu = new MenuPrincipal(funcionarioService, ticketService, relatorioService);
menu.Executar();