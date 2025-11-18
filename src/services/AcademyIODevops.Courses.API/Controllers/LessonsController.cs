using AcademyIODevops.Core.Enums;
using AcademyIODevops.Core.Interfaces.Services;
using AcademyIODevops.Courses.API.Application.Commands;
using AcademyIODevops.Courses.API.Application.Queries;
using AcademyIODevops.Courses.API.Application.Queries.ViewModels;
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
    public class LessonsController(IMediator _mediator,
                                ILessonQuery lessonQuery,
                                IAspNetUser aspNetUser) : MainController
    {
        /// <summary>
        /// Retorna todas as aulas registradas
        /// </summary>
        /// <returns><see cref="IEnumerable{LessonViewModel}"/>Retorna uma lista de aulas e suas informações</returns>
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LessonViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LessonViewModel>>> GetAll()
        {
            var lessons = await lessonQuery.GetAll();
            return CustomResponse(lessons);
        }

        /// <summary>
        /// Retorna aulas referentes ao CourseId do parametro
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns><see cref="IEnumerable{LessonViewModel}"/>Retorna uma lista de aulas e suas informações</returns>
        [AllowAnonymous]
        [HttpGet("get-by-courseId/{courseId:guid}")]
        [ProducesResponseType(typeof(IEnumerable<LessonViewModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LessonViewModel>>> GetByCourseId(Guid courseId)
        {
            var lessons = await lessonQuery.GetByCourseId(courseId);
            return CustomResponse(lessons);
        }

        /// <summary>
        /// Retorna o progresso do aluno dentro das aulas
        /// </summary>
        /// <returns><see cref="IEnumerable{LessonProgressViewModel}"/>Retorna uma lista de progresso de aulas</returns>
        [Authorize(Roles = "STUDENT")]
        [HttpGet("get-progress")]
        [ProducesResponseType(typeof(IEnumerable<LessonProgressViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProgress()
        {
            var progress = await lessonQuery.GetProgress(aspNetUser.GetUserId());

            return CustomResponse(progress);
        }

        /// <summary>
        /// Recebe um modelo de aula para ser adicionada
        /// </summary>
        /// <param name="lesson"></param>
        /// <returns>Retorna sucesso 201</returns>
        [Authorize(Roles = "ADMIN")]
        [HttpPost]
        [ProducesResponseType(typeof(LessonViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Add(LessonViewModel lesson)
        {
            var command = new AddLessonCommand(lesson.Name, lesson.Subject, lesson.CourseId, lesson.TotalHours);
            await _mediator.Send(command);

            return CustomResponse(HttpStatusCode.Created); ;
        }

        /// <summary>
        /// Registra que o aluno iniciou a aula
        /// </summary>
        /// <param name="lessonId"></param>
        /// <returns>Se aluno estiver matriculdo e nao concluiu, sucesso 204</returns>
        [Authorize(Roles = "STUDENT")]
        [HttpPost("{lessonId:guid}/start-class")]
        [ProducesResponseType(typeof(CourseViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> StartClass(Guid lessonId)
        {
            if (!lessonQuery.ExistsProgress(lessonId, aspNetUser.GetUserId()))
            {
                AddErrorToStack("Você ainda não está matriculado a essa aula.");
                return CustomResponse();
            }   

            var status = lessonQuery.GetProgressStatusLesson(lessonId, aspNetUser.GetUserId());

            if (status == EProgressLesson.Completed)
            {
                AddErrorToStack("Você já concluiu essa aula.");
                return CustomResponse();
            }   

            var command = new StartLessonCommand(lessonId, aspNetUser.GetUserId());
            await _mediator.Send(command);

            return CustomResponse(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// Finaliza aula
        /// </summary>
        /// <param name="lessonId"></param>
        /// <returns>Se o aluno já se matriculou e iniciou, retorna sucesso 204</returns>
        [Authorize(Roles = "STUDENT")]
        [HttpPost("{lessonId:guid}/finish-class")]
        [ProducesResponseType(typeof(CourseViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FinishClass(Guid lessonId)
        {
            if (!lessonQuery.ExistsProgress(lessonId, aspNetUser.GetUserId()))
            {
                AddErrorToStack("Você ainda não está matriculado a essa aula.");
                return CustomResponse();
            }  

            var status = lessonQuery.GetProgressStatusLesson(lessonId, aspNetUser.GetUserId());

            if (status == EProgressLesson.NotStarted)
            {
                AddErrorToStack("Você ainda não teve progresso nesta aula.");
                return CustomResponse();
            }    

            var command = new FinishLessonCommand(lessonId, aspNetUser.GetUserId());
            await _mediator.Send(command);

            return CustomResponse(HttpStatusCode.NoContent); ;
        }
    }
}
