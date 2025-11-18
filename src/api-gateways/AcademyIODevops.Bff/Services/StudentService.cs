using AcademyIODevops.Bff.Extensions;
using AcademyIODevops.Bff.Models;
using AcademyIODevops.Core.Communication;
using AcademyIODevops.WebAPI.Core.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AcademyIODevops.Bff.Services
{
    public interface IStudentService
    {
        Task<ResponseResult> RegisterToCourse(Guid courseId);
        Task<List<RegistrationViewModel>> GetRegistration();
        Task<List<RegistrationViewModel>> GetAllRegistrations();
    }

    public class StudentService : Service, IStudentService
    {
        private readonly HttpClient _httpClient;
        private readonly IAspNetUser _aspNetUser;

        public StudentService(HttpClient httpClient, IOptions<AppServicesSettings> settings, IAspNetUser aspNetUser)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(settings.Value.StudentUrl);
            _aspNetUser = aspNetUser;
        }

        public async Task<List<RegistrationViewModel>> GetAllRegistrations()
        {
            var response = await _httpClient.GetAsync($"/api/student/get-all-registrations");

            ManageHttpResponse(response);

            return await DeserializeResponse<List<RegistrationViewModel>>(response);
        }

        public async Task<List<RegistrationViewModel>> GetRegistration()
        {
            var studentId = _aspNetUser.GetUserId();
            var response = await _httpClient.GetAsync($"/api/student/get-registration/{studentId}");

            ManageHttpResponse(response);

            return await DeserializeResponse<List<RegistrationViewModel>>(response);
        }

        public async Task<ResponseResult> RegisterToCourse(Guid courseId)
        {
            var response = await _httpClient.PostAsync($"/api/student/register-to-course/{courseId}", null);

            if (!ManageHttpResponse(response)) return await DeserializeResponse<ResponseResult>(response);

            return Ok();
        }
    }
}
