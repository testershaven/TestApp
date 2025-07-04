using TestApp.Enums;
using TestApp.Models;

namespace TestApp.Data.Repositories
{
    public class StudyGroupRepository : IStudyGroupRepository
    {
        private List<StudyGroup> StudyGroups = new List<StudyGroup>();

        public async Task CreateStudyGroup(StudyGroup studyGroup)
        {
            StudyGroups.Add(studyGroup);
        }

        public Task<List<StudyGroup>> GetStudyGroups()
        {
            return Task.FromResult(StudyGroups);
        }

        public async Task JoinStudyGroup(int studyGroupId, int userId)
        {
            var studyGroup = StudyGroups.FirstOrDefault(sg => sg.StudyGroupId == studyGroupId) ?? throw new ArgumentException($"Study group with ID {studyGroupId} not found");

            var user = new User(userId, $"User {userId}");

            studyGroup.AddUser(user);
        }

        public Task LeaveStudyGroup(int studyGroupId, int userId)
        {
            var studyGroup = StudyGroups.FirstOrDefault(sg => sg.StudyGroupId == studyGroupId) ?? throw new ArgumentException($"Study group with ID {studyGroupId} not found");

            var user = studyGroup.Users.FirstOrDefault(u => u.ID == userId) ?? throw new ArgumentException($"User {userId} is not a member of study group {studyGroupId}");

            studyGroup.RemoveUser(user);

            return Task.CompletedTask;
        }

        public Task<List<StudyGroup>> SearchStudyGroups(Subject subject)
        {
            var filteredGroups = StudyGroups
                .Where(sg => sg.Subject == subject)
                .ToList();

            return Task.FromResult(filteredGroups);
        }

        public Task<bool> IsUserInStudyGroupWithSubject(int userId, Subject subject)
        {
            return Task.FromResult(StudyGroups.Any(sg =>
                sg.Subject == subject &&
                sg.Users.Any(u => u.ID == userId)));
        }
    }
}
