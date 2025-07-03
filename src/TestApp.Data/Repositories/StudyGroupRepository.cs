using TestApp.Models;

namespace TestApp.Data.Repositories
{
    public class StudyGroupRepository : IStudyGroupRepository
    {
        public Task CreateStudyGroup(StudyGroup studyGroup)
        {
            throw new NotImplementedException();
        }

        public Task<List<StudyGroup>> GetStudyGroups()
        {
            throw new NotImplementedException();
        }

        public Task JoinStudyGroup(int studyGroupId, int userId)
        {
            throw new NotImplementedException();
        }

        public Task LeaveStudyGroup(int studyGroupId, int userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<StudyGroup>> SearchStudyGroups(string subject)
        {
            throw new NotImplementedException();
        }
    }
}
