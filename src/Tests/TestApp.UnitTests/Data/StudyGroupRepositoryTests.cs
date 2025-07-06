using Allure.NUnit;
using Allure.NUnit.Attributes;
using TestApp.Data.Repositories;
using TestApp.Enums;
using TestApp.Models;

namespace TestApp.UnitTests
{
    [AllureNUnit]
    [AllureFeature("StudyGroup features")]
    [AllureParentSuite("Unit Tests")]
    public class StudyGroupRepositoryTests
    {
        private StudyGroupRepository _repository;
        private User _user1;
        private User _user2;
        private StudyGroup _mathGroup;
        private StudyGroup _physicsGroup;

        [SetUp]
        public void Setup()
        {
            _repository = new StudyGroupRepository();
            _user1 = new User(1, "User 1");
            _user2 = new User(2, "User 2");

            _mathGroup = new StudyGroup(
                studyGroupId: 1,
                name: "Math Study Group",
                subject: Subject.Math,
                createDate: DateTime.Now,
                users: [_user1]
            );

            _physicsGroup = new StudyGroup(
                studyGroupId: 2,
                name: "Physics Study Group",
                subject: Subject.Physics,
                createDate: DateTime.Now,
                users: [_user2]
            );
        }

        [Test]
        public async Task CreateStudyGroup_AddsGroupToRepository()
        {
            var chemistryGroup = new StudyGroup(
                studyGroupId: 3,
                name: "Chemistry Study Group",
                subject: Subject.Chemistry,
                createDate: DateTime.Now,
                users: [new User(3, "User 3")]
            );

            await _repository.CreateStudyGroup(chemistryGroup);
            var groups = await _repository.GetStudyGroups();

            Assert.That(groups.Count, Is.EqualTo(1));
            Assert.That(groups[0].StudyGroupId, Is.EqualTo(chemistryGroup.StudyGroupId));
        }

        [Test]
        public async Task GetStudyGroups_ReturnsAllGroups()
        {
            await _repository.CreateStudyGroup(_mathGroup);
            await _repository.CreateStudyGroup(_physicsGroup);

            var groups = await _repository.GetStudyGroups();

            Assert.That(groups.Count, Is.EqualTo(2));
            Assert.That(groups.Any(g => g.StudyGroupId == _mathGroup.StudyGroupId));
            Assert.That(groups.Any(g => g.StudyGroupId == _physicsGroup.StudyGroupId));
        }

        [Test]
        public async Task JoinStudyGroup_AddsUserToGroup()
        {
            // Arrange
            await _repository.CreateStudyGroup(_mathGroup);
            int newUserId = 3;

            // Act
            await _repository.JoinStudyGroup(_mathGroup.StudyGroupId, newUserId);

            // Assert
            var groups = await _repository.GetStudyGroups();
            var group = groups.First(g => g.StudyGroupId == _mathGroup.StudyGroupId);

            Assert.That(group.Users.Count, Is.EqualTo(2));
            Assert.That(group.Users.Any(u => u.ID == newUserId), Is.True);
        }

        [Test]
        public async Task JoinStudyGroup_GroupNotFound_ThrowsException()
        {
            int nonExistentGroupId = 999;

            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _repository.JoinStudyGroup(nonExistentGroupId, 1));

            Assert.That(ex.Message, Does.Contain($"Study group with ID {nonExistentGroupId} not found"));
        }

        [Test]
        public async Task LeaveStudyGroup_RemovesUserFromGroup()
        {
            await _repository.CreateStudyGroup(_mathGroup);

            await _repository.LeaveStudyGroup(_mathGroup.StudyGroupId, _user1.ID);

            var groups = await _repository.GetStudyGroups();
            var group = groups.First(g => g.StudyGroupId == _mathGroup.StudyGroupId);

            Assert.That(group.Users.Count, Is.EqualTo(0));
            Assert.That(group.Users.Any(u => u.ID == _user1.ID), Is.False);
        }

        [Test]
        public async Task LeaveStudyGroup_GroupNotFound_ThrowsException()
        {
            int nonExistentGroupId = 999;

            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _repository.LeaveStudyGroup(nonExistentGroupId, 1));

            Assert.That(ex.Message, Does.Contain($"Study group with ID {nonExistentGroupId} not found"));
        }

        [Test]
        public async Task LeaveStudyGroup_UserNotInGroup_ThrowsException()
        {
            await _repository.CreateStudyGroup(_mathGroup);
            int nonMemberUserId = 999;

            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _repository.LeaveStudyGroup(_mathGroup.StudyGroupId, nonMemberUserId));

            Assert.That(ex.Message, Does.Contain($"User {nonMemberUserId} is not a member"));
        }

        [Test]
        public async Task SearchStudyGroupsBySubject_ReturnsMatchingGroups()
        {
            await _repository.CreateStudyGroup(_mathGroup);
            await _repository.CreateStudyGroup(_physicsGroup);

            var mathGroups = await _repository.SearchStudyGroups(Subject.Math);
            var physicsGroups = await _repository.SearchStudyGroups(Subject.Physics);
            var chemistryGroups = await _repository.SearchStudyGroups(Subject.Chemistry);

            // Assert
            Assert.That(mathGroups.Count, Is.EqualTo(1));
            Assert.That(mathGroups[0].StudyGroupId, Is.EqualTo(_mathGroup.StudyGroupId));

            Assert.That(physicsGroups.Count, Is.EqualTo(1));
            Assert.That(physicsGroups[0].StudyGroupId, Is.EqualTo(_physicsGroup.StudyGroupId));

            Assert.That(chemistryGroups.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task SearchStudyGroupsByString_ReturnsMatchingGroups()
        {
            await _repository.CreateStudyGroup(_mathGroup);
            await _repository.CreateStudyGroup(_physicsGroup);

            var mathGroups = await _repository.SearchStudyGroups(Subject.Math);
            var physicsGroups = await _repository.SearchStudyGroups(Subject.Physics);
            var chemistryGroups = await _repository.SearchStudyGroups(Subject.Chemistry);

            Assert.That(mathGroups.Count, Is.EqualTo(1));
            Assert.That(mathGroups[0].StudyGroupId, Is.EqualTo(_mathGroup.StudyGroupId));

            Assert.That(physicsGroups.Count, Is.EqualTo(1));
            Assert.That(physicsGroups[0].StudyGroupId, Is.EqualTo(_physicsGroup.StudyGroupId));

            Assert.That(chemistryGroups.Count, Is.EqualTo(0));
        }

        [Test]
        public async Task IsUserInStudyGroupWithSubject_ReturnsTrueForExistingUser()
        {
            await _repository.CreateStudyGroup(_mathGroup);

            var result = await _repository.IsUserInStudyGroupWithSubject(_user1.ID, Subject.Math);

            Assert.That(result);
        }

        [Test]
        public async Task IsUserInStudyGroupWithSubject_ReturnsFalseForNonExistingUser()
        {
            await _repository.CreateStudyGroup(_mathGroup);

            var result = await _repository.IsUserInStudyGroupWithSubject(_user1.ID, Subject.Physics);
            var result2 = await _repository.IsUserInStudyGroupWithSubject(999, Subject.Math);

            Assert.That(result, Is.False);
            Assert.That(result2, Is.False);
        }

        [Test]
        [AllureDescription("Verify searching an empty repository returns empty list")]
        public async Task SearchStudyGroups_EmptyRepository_ReturnsEmptyList()
        {
            // Act
            var mathGroups = await _repository.SearchStudyGroups(Subject.Math);

            // Assert
            Assert.That(mathGroups, Is.Empty);
        }
    }
}