using Microsoft.AspNetCore.Mvc;
using TestApp.Data.Repositories;
using TestApp.Enums;
using TestApp.Models;

namespace TestApp.Api.Controllers;

[Produces("application/json")]
[Route("api/studygroup")]
[ApiController]
public class StudyGroupController : ControllerBase
{
    private readonly IStudyGroupRepository _studyGroupRepository;

    public StudyGroupController(IStudyGroupRepository studyGroupRepository)
    {
        _studyGroupRepository = studyGroupRepository;
    }

    [HttpPost]
    public async Task<IActionResult> CreateStudyGroup(StudyGroup studyGroup)
    {
        await _studyGroupRepository.CreateStudyGroup(studyGroup);

        return new OkResult();
    }

    [HttpGet]
    public async Task<IActionResult> GetStudyGroups()
    {
        var studyGroups = await _studyGroupRepository.GetStudyGroups();

        return new OkObjectResult(studyGroups);
    }

    [HttpGet("search")]
    public async Task<IActionResult> SearchStudyGroups(Subject subject)
    {
        var studyGroups = await _studyGroupRepository.SearchStudyGroups(subject);

        return new OkObjectResult(studyGroups);
    }

    [HttpPatch("join")]
    public async Task<IActionResult> JoinStudyGroup(int studyGroupId, int userId)
    {
        await _studyGroupRepository.JoinStudyGroup(studyGroupId, userId);

        return new OkResult();
    }

    [HttpPatch("leave")]
    public async Task<IActionResult> LeaveStudyGroup(int studyGroupId, int userId)
    {
        await _studyGroupRepository.LeaveStudyGroup(studyGroupId, userId);

        return new OkResult();
    }
}