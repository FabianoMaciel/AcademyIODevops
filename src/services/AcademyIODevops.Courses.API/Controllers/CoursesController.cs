using AcademyIODevops.Core.Messages.Integration;
using AcademyIODevops.Courses.API.Application.Commands;
using AcademyIODevops.Courses.API.Application.Queries;
using AcademyIODevops.Courses.API.Application.Queries.ViewModels;
using AcademyIODevops.Courses.API.Models.ViewModels;
using AcademyIODevops.MessageBus;
using AcademyIODevops.WebAPI.Core.Controllers;
using AcademyIODevops.WebAPI.Core.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace AcademyIODevops.Courses.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : MainController
    {
        private readonly IMediator _mediator;
        private readonly ICourseQuery _courseQuery;
        private readonly IAspNetUser _aspNetUser;
        private readonly IMessageBus _bus;

        public CoursesController(IMediator mediator, ICourseQuery courseQuery, IAspNetUser aspNetUser, IMessageBus bus)
        {
            _mediator = mediator;
            _courseQuery = courseQuery;
            _aspNetUser = aspNetUser;
            _bus = bus;

            Console.WriteLine($"[Publisher] Tipo do evento: {typeof(PaymentRegisteredIntegrationEvent).AssemblyQualifiedName}");
        }

        /// <summary>
        /// Retorna todos os cursos registrados
        /// </summary>
        /// <returns><see cref="IEnumerable{CourseViewModel}"/> Retorna uma lista de CourseViewModel</returns>
        [AllowAnonymous]
        [HttpGet()]
        [ProducesResponseType(typeof(IEnumerable<CourseViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CourseViewModel>>> GetAll()
        {
            var courses = await _courseQuery.GetAll();
            return CustomResponse(courses);
        }

        /// <summary>
        /// Retorna curso referente ao Id do parametro
        /// </summary>
        /// <param name="id"></param>
        /// <returns><see cref="CourseViewModel"/>Retorna os dados do curso</returns>
        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CourseViewModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<CourseViewModel>> GetById(Guid id)
        {
            var course = await _courseQuery.GetById(id);
            return CustomResponse(course);
        }

        /// <summary>
        /// Cria um novo curso
        /// </summary>
        /// <param name="course"></param>
        /// <returns>Retorna que o curso foi criado, status 201</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPost("create")]
        [ProducesResponseType(typeof(CourseViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create(CourseViewModel course)
        {
            var command = new AddCourseCommand(course.Name, course.Description, _aspNetUser.GetUserId(), course.Price);
            await _mediator.Send(command);

            return CustomResponse(HttpStatusCode.Created);
        }

        /// <summary>
        /// Atualizar curso
        /// </summary>
        /// <param name="course"></param>
        /// <returns>Retorna que o curso foi atualizado, status 204</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPut("update")]
        [ProducesResponseType(typeof(CourseViewModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Update(CourseViewModel course)
        {
            var command = new UpdateCourseCommand(course.Name, course.Description, _aspNetUser.GetUserId(), course.Price, course.Id);
            await _mediator.Send(command);

            return CustomResponse(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Remover curso
        /// </summary>
        /// <param name="course"></param>
        /// <returns>Retorna que o curso foi removido, status 204</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("remove/{id:guid}")]
        [ProducesResponseType(typeof(CourseViewModel), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Remove(Guid id)
        {
            var command = new RemoveCourseCommand(id);
            await _mediator.Send(command);

            return CustomResponse(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Faz o pagamento do curso referenciado nos parametro 
        /// </summary>
        /// <param name="courseId"></param>
        /// <param name="paymentViewModel"></param>
        /// <returns>Retorna que o pagamento foi feito, status 201</returns>
        [Authorize(Roles = "STUDENT")]
        [HttpPost("{courseId:guid}/make-payment")]
        public async Task<ResponseMessage> MakePayment(Guid courseId, [FromBody] PaymentViewModel paymentViewModel)
        {
            var paymentRegistered = new PaymentRegisteredIntegrationEvent(courseId, _aspNetUser.GetUserId(), paymentViewModel.CardName,
                                                        paymentViewModel.CardNumber, paymentViewModel.CardExpirationDate,
                                                        paymentViewModel.CardCVV);
            try
            {
                
                return await _bus.RequestAsync<PaymentRegisteredIntegrationEvent, ResponseMessage>(paymentRegistered);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
