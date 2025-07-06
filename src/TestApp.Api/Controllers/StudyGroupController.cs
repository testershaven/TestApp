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
    public async Task<IActionResult> CreateStudyGroup([FromBody] StudyGroup studyGroup)
    {
        foreach (var user in studyGroup.Users)
        {
            if (await _studyGroupRepository.IsUserInStudyGroupWithSubject(user.ID, studyGroup.Subject))
            {
                throw new BadHttpRequestException($"User {user.ID} is already in a study group with subject {studyGroup.Subject}");
            }
        }
        await _studyGroupRepository.CreateStudyGroup(studyGroup);

        return new OkResult();
    }

    [HttpGet]
    public async Task<IActionResult> SearchStudyGroups(Subject? subject = null, string sortOrder = "asc")
    {
        IEnumerable<StudyGroup> studyGroups;

        if (subject.HasValue)
        {
            studyGroups = await _studyGroupRepository.SearchStudyGroups(subject.Value);
        }
        else
        {
            studyGroups = await _studyGroupRepository.GetStudyGroups();
        }

        // Sort by creation date
        studyGroups = sortOrder?.ToLower() == "desc"
            ? studyGroups.OrderByDescending(sg => sg.CreateDate)
            : studyGroups.OrderBy(sg => sg.CreateDate);

        return new OkObjectResult(studyGroups);
    }

    [HttpPatch("join")]
    public async Task<IActionResult> JoinStudyGroup(int studyGroupId, int userId)
    {
        var studyGroups = await _studyGroupRepository.GetStudyGroups();
        var studyGroup = studyGroups.FirstOrDefault(sg => sg.StudyGroupId == studyGroupId);

        if (studyGroup == null)
        {
            return BadRequest($"Study group with ID {studyGroupId} not found");
        }

        if (await _studyGroupRepository.IsUserInStudyGroupWithSubject(userId, studyGroup.Subject))
        {
            return BadRequest($"User {userId} is already in a study group with subject {studyGroup.Subject}");
        }

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