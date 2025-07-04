using Allure.NUnit;
using Allure.NUnit.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TestApp.Api.Controllers;
using TestApp.Data.Repositories;
using TestApp.Enums;
using TestApp.Models;

namespace TestApp.UnitTests.Api
{
    [AllureNUnit]
    [AllureFeature("StudyGroup features")]
    [AllureParentSuite("Unit Tests")]
    public class StudyGroupControllerTests
    {
        private Mock<IStudyGroupRepository> _mockRepository;
        private StudyGroupController _controller;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IStudyGroupRepository>();
            _controller = new StudyGroupController(_mockRepository.Object);
        }

        [Test]
        public async Task CreateStudyGroup_ShouldReturnOkResult()
        {
            var studyGroup = new StudyGroup(
                studyGroupId: 1,
                name: "Test Group",
                subject: Subject.Chemistry,
                createDate: DateTime.Now,
                users: []
            );

            _mockRepository.Setup(repo => repo.CreateStudyGroup(It.IsAny<StudyGroup>()))
                .Returns(Task.CompletedTask);

            var result = await _controller.CreateStudyGroup(studyGroup);

            Assert.That(result, Is.InstanceOf<OkResult>());
            _mockRepository.Verify(repo => repo.CreateStudyGroup(studyGroup), Times.Once);
        }

        [Test]
        public async Task GetStudyGroups_ShouldReturnOkObjectResultWithStudyGroups()
        {
            var studyGroups = new List<StudyGroup>
            {
                new(1, "Math Study", Subject.Chemistry, DateTime.Now, new List<User>()),
                new(2, "Physics Group", Subject.Physics, DateTime.Now, new List<User>())
            };

            _mockRepository.Setup(repo => repo.GetStudyGroups())
                .ReturnsAsync(studyGroups);

            var result = await _controller.GetStudyGroups();

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            IEnumerable<StudyGroup>? returnedStudyGroups = okResult.Value as IEnumerable<StudyGroup>;
            Assert.That(returnedStudyGroups.Count(), Is.EqualTo(2));
            _mockRepository.Verify(repo => repo.GetStudyGroups(), Times.Once);
        }

        [Test]
        public async Task SearchStudyGroups_ShouldReturnOkObjectResultWithFilteredStudyGroups()
        {
            var subjectToSearch = Subject.Math;
            var studyGroups = new List<StudyGroup>
            {
                new(1, "Math Study", Subject.Math, DateTime.Now, new List<User>())
            };

            _mockRepository.Setup(repo => repo.SearchStudyGroups(subjectToSearch))
                .ReturnsAsync(studyGroups);

            var result = await _controller.SearchStudyGroups(subjectToSearch);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var returnedStudyGroups = okResult.Value as IEnumerable<StudyGroup>;
            Assert.That(returnedStudyGroups.Count(), Is.EqualTo(1));
            _mockRepository.Verify(repo => repo.SearchStudyGroups(subjectToSearch), Times.Once);
        }

        [Test]
        public async Task JoinStudyGroup_ShouldReturnOkResult()
        {
            int studyGroupId = 1;
            int userId = 5;
            var subject = Subject.Physics;

            var studyGroups = new List<StudyGroup>
            {
                new(studyGroupId, "Physics Group", subject, DateTime.Now, new List<User>())
            };

            _mockRepository.Setup(repo => repo.GetStudyGroups())
                .ReturnsAsync(studyGroups);

            // Setup the user is not in any study group with this subject
            _mockRepository.Setup(repo => repo.IsUserInStudyGroupWithSubject(userId, subject))
                .ReturnsAsync(false);

            // Setup join operation
            _mockRepository.Setup(repo => repo.JoinStudyGroup(studyGroupId, userId))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.JoinStudyGroup(studyGroupId, userId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkResult>());
            _mockRepository.Verify(repo => repo.IsUserInStudyGroupWithSubject(userId, subject), Times.Once);
            _mockRepository.Verify(repo => repo.JoinStudyGroup(studyGroupId, userId), Times.Once);
        }

        [Test]
        public async Task LeaveStudyGroup_ShouldReturnOkResult()
        {
            int studyGroupId = 1;
            int userId = 5;

            _mockRepository.Setup(repo => repo.LeaveStudyGroup(studyGroupId, userId))
                .Returns(Task.CompletedTask);

            var result = await _controller.LeaveStudyGroup(studyGroupId, userId);

            Assert.That(result, Is.InstanceOf<OkResult>());
            _mockRepository.Verify(repo => repo.LeaveStudyGroup(studyGroupId, userId), Times.Once);
        }

        [Test]
[AllureDescription("Test that creating a study group with a user already in another group with same subject throws exception")]
public async Task CreateStudyGroup_UserAlreadyInStudyGroupWithSubject_ShouldThrowBadHttpRequestException()
{
    // Arrange
    var user = new User(1, "Test User");
    var studyGroup = new StudyGroup(
        studyGroupId: 1,
        name: "Test Group",
        subject: Subject.Chemistry,
        createDate: DateTime.Now,
        users: new List<User> { user }
    );

    _mockRepository.Setup(repo => repo.IsUserInStudyGroupWithSubject(user.ID, studyGroup.Subject))
        .ReturnsAsync(true);

    // Act & Assert
    var exception = Assert.ThrowsAsync<BadHttpRequestException>(async () => 
        await _controller.CreateStudyGroup(studyGroup));
    
    Assert.That(exception.Message, Does.Contain($"User {user.ID} is already in a study group with subject {studyGroup.Subject}"));
    _mockRepository.Verify(repo => repo.CreateStudyGroup(It.IsAny<StudyGroup>()), Times.Never);
}

[Test]
[AllureDescription("Test that joining a study group when user is already in another group with same subject throws exception")]
public async Task JoinStudyGroup_UserAlreadyInStudyGroupWithSubject_ShouldThrowBadHttpRequestException()
{
    // Arrange
    int studyGroupId = 1;
    int userId = 5;
    var subject = Subject.Physics;

    var studyGroups = new List<StudyGroup>
    {
        new(studyGroupId, "Physics Group", subject, DateTime.Now, new List<User>())
    };

    _mockRepository.Setup(repo => repo.GetStudyGroups())
        .ReturnsAsync(studyGroups);

    _mockRepository.Setup(repo => repo.IsUserInStudyGroupWithSubject(userId, subject))
        .ReturnsAsync(true);

    // Act & Assert
    var exception = Assert.ThrowsAsync<BadHttpRequestException>(async () => 
        await _controller.JoinStudyGroup(studyGroupId, userId));
    
    Assert.That(exception.Message, Does.Contain($"User {userId} is already in a study group with subject {subject}"));
    _mockRepository.Verify(repo => repo.JoinStudyGroup(studyGroupId, userId), Times.Never);
}

[Test]
[AllureDescription("Test handling when trying to join a non-existent study group")]
public void JoinStudyGroup_StudyGroupNotFound_ShouldHandleException()
{
    // Arrange
    int nonExistentStudyGroupId = 999;
    int userId = 5;

    _mockRepository.Setup(repo => repo.GetStudyGroups())
        .ReturnsAsync(new List<StudyGroup>());  // Empty list, no study groups

    // Act & Assert
    var exception = Assert.ThrowsAsync<InvalidOperationException>(async () => 
        await _controller.JoinStudyGroup(nonExistentStudyGroupId, userId));
    
    // The First() method on an empty sequence throws InvalidOperationException
    _mockRepository.Verify(repo => repo.IsUserInStudyGroupWithSubject(It.IsAny<int>(), It.IsAny<Subject>()), Times.Never);
    _mockRepository.Verify(repo => repo.JoinStudyGroup(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
}
    }
}