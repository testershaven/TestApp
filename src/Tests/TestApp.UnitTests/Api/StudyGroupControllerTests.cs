using Microsoft.AspNetCore.Mvc;
using Moq;
using TestApp.Api.Controllers;
using TestApp.Data.Repositories;
using TestApp.Enums;
using TestApp.Models;

namespace TestApp.UnitTests.Api
{
    [TestFixture]
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
                users: new List<User>()
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
                new StudyGroup(1, "Math Study", Subject.Chemistry, DateTime.Now, new List<User>()),
                new StudyGroup(2, "Physics Group", Subject.Physics, DateTime.Now, new List<User>())
            };

            _mockRepository.Setup(repo => repo.GetStudyGroups())
                .ReturnsAsync(studyGroups);

            var result = await _controller.GetStudyGroups();

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var returnedStudyGroups = okResult.Value as IEnumerable<StudyGroup>;
            Assert.That(returnedStudyGroups.Count(), Is.EqualTo(2));
            _mockRepository.Verify(repo => repo.GetStudyGroups(), Times.Once);
        }

        [Test]
        public async Task SearchStudyGroups_ShouldReturnOkObjectResultWithFilteredStudyGroups()
        {
            var subjectToSearch = Subject.Math;
            var studyGroups = new List<StudyGroup>
            {
                new StudyGroup(1, "Math Study", Subject.Math, DateTime.Now, new List<User>())
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
    // Arrange
    int studyGroupId = 1;
    int userId = 5;
    var subject = Subject.Physics;
    
    // Setup the study group to be found
    var studyGroups = new List<StudyGroup>
    {
        new StudyGroup(studyGroupId, "Physics Group", subject, DateTime.Now, new List<User>())
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
    }
}