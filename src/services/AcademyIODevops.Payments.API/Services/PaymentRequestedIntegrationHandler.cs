using AcademyIODevops.Core.Messages.Integration;
using AcademyIODevops.MessageBus;
using FluentValidation.Results;
using MediatR;

namespace AcademyIODevops.Payments.API.Services
{
    public class PaymentRequestedIntegrationHandler: BackgroundService
    {
        private readonly IMessageBus _bus;
        private readonly IServiceProvider _serviceProvider;
        private bool _responderRegistered = false;

        public PaymentRequestedIntegrationHandler(
                            IServiceProvider serviceProvider,
                            IMessageBus bus)
        {
            Console.WriteLine($"[Consumer] Tipo do evento: {typeof(PaymentRegisteredIntegrationEvent).AssemblyQualifiedName}");
            _serviceProvider = serviceProvider;
            _bus = bus;
        }

        private void RegisterResponder()
        {
          
            // Garante que só registra uma vez (evita duplicação)
            if (_responderRegistered)
                return;

            Console.WriteLine("[RabbitMQ] Registrando responder de PaymentRegisteredIntegrationEvent...");
            _bus.RespondAsync<PaymentRegisteredIntegrationEvent, ResponseMessage>(async request =>
            {
                Console.WriteLine($"[RabbitMQ] Mensagem recebida: CourseId={request.CourseId}, StudentId={request.StudentId}");
                return await RegisterPayment(request);
            });

            _responderRegistered = true;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _bus.AdvancedBus.Connected += OnConnect;

            // Se o bus já estiver conectado, registramos imediatamente
            if (_bus.AdvancedBus.IsConnected)
            {
                RegisterResponder();
            }

            return Task.CompletedTask;
        }

        private void OnConnect(object s, EventArgs e)
        {
            Console.WriteLine("[RabbitMQ] Conectado — registrando responder...");
            RegisterResponder();
        }

        private async Task<ResponseMessage> RegisterPayment(PaymentRegisteredIntegrationEvent message)
        {
            var command = new ValidatePaymentCourseCommand(message.CourseId, message.StudentId, message.CardName,
                                                        message.CardNumber, message.CardExpirationDate,
                                                        message.CardCVV);
            bool sucesso;
            ValidationResult validationResult = new();

            using (var scope = _serviceProvider.CreateScope())
            {
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                sucesso = await mediator.Send(command);
            }
            if (!sucesso)
                validationResult.Errors.Add(new ValidationFailure() { ErrorMessage = "Falha ao realizar pagamento." });
            return new ResponseMessage(validationResult);
        }
    }
}
