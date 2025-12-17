using AcademyIODevops.Core.Messages.Integration;
using AcademyIODevops.MessageBus;
using AcademyIODevops.Students.API.Application.Commands;
using FluentValidation.Results;
using MediatR;

namespace AcademyIODevops.Students.API.Services
{
    public class UserRegisteredIntegrationHandler : BackgroundService
    {
        private readonly IMessageBus _bus;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<UserRegisteredIntegrationHandler> _logger;
        private IDisposable _responder;

        public UserRegisteredIntegrationHandler(
                            IServiceProvider serviceProvider,
                            IMessageBus bus,
                            ILogger<UserRegisteredIntegrationHandler> logger)
        {
            _serviceProvider = serviceProvider;
            _bus = bus;
            _logger = logger;
        }

        private void SetResponder()
        {
            // Dispose do responder anterior se existir
            _responder?.Dispose();

            // Registra o responder
            _responder = _bus.RespondAsync<UserRegisteredIntegrationEvent, ResponseMessage>(
                async request =>
                {
                    _logger.LogInformation($"Recebendo requisição para usuário: {request.UserName}");
                    return await RegisterStudent(request);
                });

            _bus.AdvancedBus.Connected += OnConnect;
            _bus.AdvancedBus.Disconnected += OnDisconnect;
        }

        private void OnConnect(object s, EventArgs e)
        {
            _logger.LogInformation("RabbitMQ reconectado. Re-registrando responder...");
            SetResponder();
        }

        private void OnDisconnect(object s, EventArgs e)
        {
            _logger.LogWarning("RabbitMQ desconectado.");
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Iniciando UserRegisteredIntegrationHandler...");

            // Aguarda conexão com RabbitMQ
            var retries = 0;
            while (!_bus.IsConnected && retries < 10)
            {
                _logger.LogWarning($"Aguardando conexão com RabbitMQ... Tentativa {retries + 1}");
                await Task.Delay(3000, stoppingToken);
                retries++;
            }

            if (!_bus.IsConnected)
            {
                _logger.LogError("Falha ao conectar com RabbitMQ");
                return;
            }

            _logger.LogInformation("RabbitMQ conectado. Registrando responder...");

            try
            {
                SetResponder();
                _logger.LogInformation("Responder registrado com sucesso!");

                // Aguarda até ser cancelado
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Background service sendo encerrado...");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro no background service");
            }
        }

        private async Task<ResponseMessage> RegisterStudent(UserRegisteredIntegrationEvent message)
        {
            var command = new AddUserCommand(message.Id, message.UserName, message.IsAdmin,
                                             message.FirstName, message.LastName,
                                             message.DateOfBirth, message.UserName);
            bool sucesso;
            ValidationResult validationResult = new();

            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                    sucesso = await mediator.Send(command);
                }
            }
            catch (OperationCanceledException)
            {
                validationResult.Errors.Add(new ValidationFailure()
                {
                    ErrorMessage = "Operação cancelada"
                });
                return new ResponseMessage(validationResult);
            }

            if (!sucesso)
                validationResult.Errors.Add(new ValidationFailure()
                {
                    ErrorMessage = "Falha ao cadastrar estudante"
                });

            return new ResponseMessage(validationResult);
        }

        public override void Dispose()
        {
            _logger.LogInformation("Disposing UserRegisteredIntegrationHandler...");

            _responder?.Dispose();

            if (_bus?.AdvancedBus != null)
            {
                _bus.AdvancedBus.Connected -= OnConnect;
                _bus.AdvancedBus.Disconnected -= OnDisconnect;
            }

            base.Dispose();
        }
    }
}