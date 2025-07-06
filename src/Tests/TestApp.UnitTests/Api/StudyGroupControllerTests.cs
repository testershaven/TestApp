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
        [AllureDescription("Test creating a valid study group")]
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
        [AllureDescription("Test searching for study groups without specifying a subject")]
        public async Task SearchStudyGroups_WithNoParameter_ShouldReturnAllStudyGroups()
        {
            var studyGroups = new List<StudyGroup>
            {
                new(1, "Math Study", Subject.Math, DateTime.Now, new List<User>()),
                new(2, "Physics Group", Subject.Physics, DateTime.Now, new List<User>())
            };

            _mockRepository.Setup(repo => repo.GetStudyGroups())
                .ReturnsAsync(studyGroups);

            var result = await _controller.SearchStudyGroups();

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            IEnumerable<StudyGroup>? returnedStudyGroups = okResult?.Value as IEnumerable<StudyGroup>;
            Assert.That(returnedStudyGroups, Is.Not.Null);
            Assert.That(returnedStudyGroups?.Count(), Is.EqualTo(2));
            _mockRepository.Verify(repo => repo.GetStudyGroups(), Times.Once);
        }

        [Test]
        [AllureDescription("Test searching for study groups with a specific subject")]
        public async Task SearchStudyGroups_WithSubject_ShouldReturnOkObjectResultWithFilteredStudyGroups()
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
            var returnedStudyGroups = okResult?.Value as IEnumerable<StudyGroup>;
            Assert.That(returnedStudyGroups, Is.Not.Null);
            Assert.That(returnedStudyGroups?.Count(), Is.EqualTo(1));
            _mockRepository.Verify(repo => repo.SearchStudyGroups(subjectToSearch), Times.Once);
            _mockRepository.Verify(repo => repo.GetStudyGroups(), Times.Never);
        }

        [Test]
        [AllureDescription("Test searching for study groups with null subject parameter")]
        public async Task SearchStudyGroups_WithNullSubject_ShouldReturnAllStudyGroups()
        {
            var allStudyGroups = new List<StudyGroup>
            {
                new(1, "Math Study", Subject.Math, DateTime.Now, new List<User>()),
                new(2, "Physics Group", Subject.Physics, DateTime.Now, new List<User>())
            };

            _mockRepository.Setup(repo => repo.GetStudyGroups())
                .ReturnsAsync(allStudyGroups);

            var result = await _controller.SearchStudyGroups(null);

            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var returnedStudyGroups = okResult?.Value as IEnumerable<StudyGroup>;
            Assert.That(returnedStudyGroups, Is.Not.Null);
            Assert.That(returnedStudyGroups?.Count(), Is.EqualTo(2));
            _mockRepository.Verify(repo => repo.GetStudyGroups(), Times.Once);
            _mockRepository.Verify(repo => repo.SearchStudyGroups(It.IsAny<Subject>()), Times.Never);
        }

        [Test]
        [AllureDescription("Test joining a study group successfully")]
        public async Task JoinStudyGroup_ShouldReturnOkResult()
        {
            int studyGroupId = 1;
            int userId = 5;
            var subject = Subject.Physics;

            var studyGroup = new StudyGroup(studyGroupId, "Physics Group", subject, DateTime.Now, new List<User>());
            var studyGroups = new List<StudyGroup> { studyGroup };

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
        [AllureDescription("Test leaving a study group successfully")]
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

            // Use await to ensure the async method completes
            await Task.CompletedTask;

            Assert.That(exception.Message, Does.Contain($"User {user.ID} is already in a study group with subject {studyGroup.Subject}"));
            _mockRepository.Verify(repo => repo.CreateStudyGroup(It.IsAny<StudyGroup>()), Times.Never);
        }

        [Test]
        [AllureDescription("Test that joining a study group when user is already in another group with same subject returns BadRequest")]
        public async Task JoinStudyGroup_UserAlreadyInStudyGroupWithSubject_ShouldReturnBadRequest()
        {
            // Arrange
            int studyGroupId = 1;
            int userId = 5;
            var subject = Subject.Physics;

            var studyGroup = new StudyGroup(studyGroupId, "Physics Group", subject, DateTime.Now, new List<User>());
            var studyGroups = new List<StudyGroup> { studyGroup };

            _mockRepository.Setup(repo => repo.GetStudyGroups())
                .ReturnsAsync(studyGroups);

            _mockRepository.Setup(repo => repo.IsUserInStudyGroupWithSubject(userId, subject))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.JoinStudyGroup(studyGroupId, userId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value?.ToString(), Does.Contain("already in a study group with subject").IgnoreCase);

            _mockRepository.Verify(repo => repo.JoinStudyGroup(studyGroupId, userId), Times.Never);
        }

        [Test]
        [AllureDescription("Test handling when trying to join a non-existent study group")]
        public async Task JoinStudyGroup_StudyGroupNotFound_ShouldReturnBadRequest()
        {
            // Arrange
            int nonExistentStudyGroupId = 999;
            int userId = 5;

            _mockRepository.Setup(repo => repo.GetStudyGroups())
                .ReturnsAsync(new List<StudyGroup>());  // Empty list, no study groups

            // Act
            var result = await _controller.JoinStudyGroup(nonExistentStudyGroupId, userId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
            var badRequestResult = result as BadRequestObjectResult;
            Assert.That(badRequestResult?.Value?.ToString(), Does.Contain("not found").IgnoreCase);

            _mockRepository.Verify(repo => repo.IsUserInStudyGroupWithSubject(It.IsAny<int>(), It.IsAny<Subject>()), Times.Never);
            _mockRepository.Verify(repo => repo.JoinStudyGroup(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Test]
        [AllureDescription("Test searching for study groups with ascending sort order")]
        public async Task SearchStudyGroups_WithAscendingSortOrder_ShouldReturnSortedGroups()
        {
            // Arrange
            var olderDate = new DateTime(2023, 1, 1);
            var newerDate = new DateTime(2023, 2, 1);

            var studyGroups = new List<StudyGroup>
            {
                new(1, "Math Study", Subject.Math, newerDate, new List<User>()),
                new(2, "Physics Group", Subject.Physics, olderDate, new List<User>())
            };

            _mockRepository.Setup(repo => repo.GetStudyGroups())
                .ReturnsAsync(studyGroups);

            // Act
            var result = await _controller.SearchStudyGroups(null, "asc");

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var returnedGroups = okResult?.Value as IEnumerable<StudyGroup>;
            Assert.That(returnedGroups, Is.Not.Null);

            var groupsList = returnedGroups?.ToList();
            Assert.That(groupsList?.Count, Is.EqualTo(2));

            // First item should be the oldest (with olderDate)
            Assert.That(groupsList?[0].CreateDate, Is.EqualTo(olderDate));
            Assert.That(groupsList?[0].StudyGroupId, Is.EqualTo(2));

            // Second item should be the newest (with newerDate)
            Assert.That(groupsList?[1].CreateDate, Is.EqualTo(newerDate));
            Assert.That(groupsList?[1].StudyGroupId, Is.EqualTo(1));

            _mockRepository.Verify(repo => repo.GetStudyGroups(), Times.Once);
        }

        [Test]
        [AllureDescription("Test searching for study groups with descending sort order")]
        public async Task SearchStudyGroups_WithDescendingSortOrder_ShouldReturnSortedGroups()
        {
            // Arrange
            var olderDate = new DateTime(2023, 1, 1);
            var newerDate = new DateTime(2023, 2, 1);

            var studyGroups = new List<StudyGroup>
            {
                new(1, "Math Study", Subject.Math, newerDate, new List<User>()),
                new(2, "Physics Group", Subject.Physics, olderDate, new List<User>())
            };

            _mockRepository.Setup(repo => repo.GetStudyGroups())
                .ReturnsAsync(studyGroups);

            // Act
            var result = await _controller.SearchStudyGroups(null, "desc");

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var returnedGroups = okResult?.Value as IEnumerable<StudyGroup>;
            Assert.That(returnedGroups, Is.Not.Null);

            var groupsList = returnedGroups?.ToList();
            Assert.That(groupsList?.Count, Is.EqualTo(2));

            // First item should be the newest (with newerDate)
            Assert.That(groupsList?[0].CreateDate, Is.EqualTo(newerDate));
            Assert.That(groupsList?[0].StudyGroupId, Is.EqualTo(1));

            // Second item should be the oldest (with olderDate)
            Assert.That(groupsList?[1].CreateDate, Is.EqualTo(olderDate));
            Assert.That(groupsList?[1].StudyGroupId, Is.EqualTo(2));

            _mockRepository.Verify(repo => repo.GetStudyGroups(), Times.Once);
        }

        [Test]
        [AllureDescription("Test searching for study groups with subject and sort order")]
        public async Task SearchStudyGroups_WithSubjectAndSortOrder_ShouldReturnSortedFilteredGroups()
        {
            // Arrange
            var olderDate = new DateTime(2023, 1, 1);
            var newerDate = new DateTime(2023, 2, 1);
            var subjectToSearch = Subject.Math;

            var studyGroups = new List<StudyGroup>
            {
                new(1, "Advanced Math", Subject.Math, newerDate, new List<User>()),
                new(2, "Basic Math", Subject.Math, olderDate, new List<User>())
            };

            _mockRepository.Setup(repo => repo.SearchStudyGroups(subjectToSearch))
                .ReturnsAsync(studyGroups);

            // Act - using descending order
            var result = await _controller.SearchStudyGroups(subjectToSearch, "desc");

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            var returnedGroups = okResult?.Value as IEnumerable<StudyGroup>;
            Assert.That(returnedGroups, Is.Not.Null);

            var groupsList = returnedGroups?.ToList();
            Assert.That(groupsList?.Count, Is.EqualTo(2));

            // First item should be the newest (with newerDate)
            Assert.That(groupsList?[0].CreateDate, Is.EqualTo(newerDate));
            Assert.That(groupsList?[0].Name, Is.EqualTo("Advanced Math"));

            // Second item should be the oldest (with olderDate)
            Assert.That(groupsList?[1].CreateDate, Is.EqualTo(olderDate));
            Assert.That(groupsList?[1].Name, Is.EqualTo("Basic Math"));

            _mockRepository.Verify(repo => repo.SearchStudyGroups(subjectToSearch), Times.Once);
            _mockRepository.Verify(repo => repo.GetStudyGroups(), Times.Never);
        }
    }
}