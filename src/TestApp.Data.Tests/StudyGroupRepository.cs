using TestApp.Data.Repositories;
using TestApp.Enums;
using TestApp.Models;

namespace TestApp.Data.Tests
{
    [TestFixture]
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
                users: new List<User> { _user1 }
            );

            _physicsGroup = new StudyGroup(
                studyGroupId: 2,
                name: "Physics Study Group",
                subject: Subject.Physics,
                createDate: DateTime.Now,
                users: new List<User> { _user2 }
            );
        }

        [Test]
        public async Task CreateStudyGroup_AddsGroupToRepository()
        {
            // Arrange
            var chemistryGroup = new StudyGroup(
                studyGroupId: 3,
                name: "Chemistry Study Group",
                subject: Subject.Chemistry,
                createDate: DateTime.Now,
                users: new List<User> { new User(3, "User 3") }
            );

            // Act
            await _repository.CreateStudyGroup(chemistryGroup);
            var groups = await _repository.GetStudyGroups();

            // Assert
            Assert.AreEqual(1, groups.Count);
            Assert.AreEqual(chemistryGroup.StudyGroupId, groups[0].StudyGroupId);
        }

        [Test]
        public async Task CreateStudyGroup_UserAlreadyInGroupWithSubject_ThrowsException()
        {
            // Arrange
            await _repository.CreateStudyGroup(_mathGroup);

            var newMathGroup = new StudyGroup(
                studyGroupId: 3,
                name: "Another Math Group",
                subject: Subject.Math,
                createDate: DateTime.Now,
                users: new List<User> { _user1 }  // Same user as in _mathGroup
            );

            // Act & Assert
            var ex = Assert.ThrowsAsync<Exception>(async () =>
                await _repository.CreateStudyGroup(newMathGroup));

            Assert.That(ex.Message, Does.Contain($"User {_user1.ID} is already in a study group with subject {Subject.Math}"));
        }

        [Test]
        public async Task GetStudyGroups_ReturnsAllGroups()
        {
            // Arrange
            await _repository.CreateStudyGroup(_mathGroup);
            await _repository.CreateStudyGroup(_physicsGroup);

            // Act
            var groups = await _repository.GetStudyGroups();

            // Assert
            Assert.AreEqual(2, groups.Count);
            Assert.IsTrue(groups.Any(g => g.StudyGroupId == _mathGroup.StudyGroupId));
            Assert.IsTrue(groups.Any(g => g.StudyGroupId == _physicsGroup.StudyGroupId));
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

            Assert.AreEqual(2, group.Users.Count);
            Assert.IsTrue(group.Users.Any(u => u.ID == newUserId));
        }

        [Test]
        public async Task JoinStudyGroup_UserAlreadyInGroupWithSubject_ThrowsException()
        {
            // Arrange
            await _repository.CreateStudyGroup(_mathGroup);
            await _repository.CreateStudyGroup(_physicsGroup);

            // First join should succeed
            await _repository.JoinStudyGroup(_mathGroup.StudyGroupId, 3);

            // Try to join another math group with same user
            var anotherMathGroup = new StudyGroup(
                studyGroupId: 4,
                name: "Another Math Group",
                subject: Subject.Math,
                createDate: DateTime.Now,
                users: new List<User>()
            );

            await _repository.CreateStudyGroup(anotherMathGroup);

            var ex2 = Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await _repository.JoinStudyGroup(anotherMathGroup.StudyGroupId, 3));

            Assert.That(ex2.Message, Does.Contain("already in a study group with subject"));
        }

        [Test]
        public async Task JoinStudyGroup_GroupNotFound_ThrowsException()
        {
            // Arrange
            int nonExistentGroupId = 999;

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _repository.JoinStudyGroup(nonExistentGroupId, 1));

            Assert.That(ex.Message, Does.Contain($"Study group with ID {nonExistentGroupId} not found"));
        }

        [Test]
        public async Task LeaveStudyGroup_RemovesUserFromGroup()
        {
            // Arrange
            await _repository.CreateStudyGroup(_mathGroup);

            // Act
            await _repository.LeaveStudyGroup(_mathGroup.StudyGroupId, _user1.ID);

            // Assert
            var groups = await _repository.GetStudyGroups();
            var group = groups.First(g => g.StudyGroupId == _mathGroup.StudyGroupId);

            Assert.AreEqual(0, group.Users.Count);
            Assert.IsFalse(group.Users.Any(u => u.ID == _user1.ID));
        }

        [Test]
        public async Task LeaveStudyGroup_GroupNotFound_ThrowsException()
        {
            // Arrange
            int nonExistentGroupId = 999;

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _repository.LeaveStudyGroup(nonExistentGroupId, 1));

            Assert.That(ex.Message, Does.Contain($"Study group with ID {nonExistentGroupId} not found"));
        }

        [Test]
        public async Task LeaveStudyGroup_UserNotInGroup_ThrowsException()
        {
            // Arrange
            await _repository.CreateStudyGroup(_mathGroup);
            int nonMemberUserId = 999;

            // Act & Assert
            var ex = Assert.ThrowsAsync<ArgumentException>(async () =>
                await _repository.LeaveStudyGroup(_mathGroup.StudyGroupId, nonMemberUserId));

            Assert.That(ex.Message, Does.Contain($"User {nonMemberUserId} is not a member"));
        }

        [Test]
        public async Task SearchStudyGroupsBySubject_ReturnsMatchingGroups()
        {
            // Arrange
            await _repository.CreateStudyGroup(_mathGroup);
            await _repository.CreateStudyGroup(_physicsGroup);

            // Act
            var mathGroups = await _repository.SearchStudyGroups(Subject.Math);
            var physicsGroups = await _repository.SearchStudyGroups(Subject.Physics);
            var chemistryGroups = await _repository.SearchStudyGroups(Subject.Chemistry);

            // Assert
            Assert.AreEqual(1, mathGroups.Count);
            Assert.AreEqual(_mathGroup.StudyGroupId, mathGroups[0].StudyGroupId);

            Assert.AreEqual(1, physicsGroups.Count);
            Assert.AreEqual(_physicsGroup.StudyGroupId, physicsGroups[0].StudyGroupId);

            Assert.AreEqual(0, chemistryGroups.Count);
        }

        [Test]
        public async Task SearchStudyGroupsByString_ReturnsMatchingGroups()
        {
            // Arrange
            await _repository.CreateStudyGroup(_mathGroup);
            await _repository.CreateStudyGroup(_physicsGroup);

            // Act
            var mathGroups = await _repository.SearchStudyGroups(Subject.Math);
            var physicsGroups = await _repository.SearchStudyGroups(Subject.Physics);
            var chemistryGroups = await _repository.SearchStudyGroups(Subject.Chemistry);

            // Assert
            Assert.AreEqual(1, mathGroups.Count);
            Assert.AreEqual(_mathGroup.StudyGroupId, mathGroups[0].StudyGroupId);

            Assert.AreEqual(1, physicsGroups.Count);
            Assert.AreEqual(_physicsGroup.StudyGroupId, physicsGroups[0].StudyGroupId);

            Assert.AreEqual(0, chemistryGroups.Count);
        }

        [Test]
        public async Task IsUserInStudyGroupWithSubject_ReturnsTrueForExistingUser()
        {
            // Arrange
            await _repository.CreateStudyGroup(_mathGroup);

            // Act
            var result = await _repository.IsUserInStudyGroupWithSubject(_user1.ID, Subject.Math);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public async Task IsUserInStudyGroupWithSubject_ReturnsFalseForNonExistingUser()
        {
            // Arrange
            await _repository.CreateStudyGroup(_mathGroup);

            // Act
            var result = await _repository.IsUserInStudyGroupWithSubject(_user1.ID, Subject.Physics);
            var result2 = await _repository.IsUserInStudyGroupWithSubject(999, Subject.Math);

            // Assert
            Assert.IsFalse(result);
            Assert.IsFalse(result2);
        }
    }
}