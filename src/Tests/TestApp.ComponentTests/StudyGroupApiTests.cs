using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TestApp.Data.Repositories;
using TestApp.Enums;
using TestApp.Models;

namespace TestApp.ComponentTests
{
    [TestFixture]
    public class StudyGroupApiTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        services.AddSingleton<IStudyGroupRepository, StudyGroupRepository>();
                    });
                });

            _client = _factory.CreateClient();

        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [Test]
        public async Task CreateStudyGroup_ThenGetStudyGroups_ShouldReturnCreatedGroup()
        {
            // Arrange
            var newStudyGroup = new StudyGroup(
                studyGroupId: 1,
                name: "Math Study Group",
                subject: Subject.Math,
                createDate: DateTime.Now,
                users: new List<User> { new User(1, "Test User") }
            );

            // Create a new study group
            var createResponse = await _client.PostAsJsonAsync("/api/studygroup", newStudyGroup);

            // Response should be successful
            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            // Get all study groups
            var getResponse = await _client.GetAsync("/api/studygroup");
            var returnedGroups = await getResponse.Content.ReadFromJsonAsync<List<StudyGroup>>();

            // Response should be successful and contain the created group
            Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(returnedGroups, Is.Not.Null);
            Assert.That(returnedGroups, Has.Count.EqualTo(1));
            Assert.That(returnedGroups[0].Name, Is.EqualTo("Math Study Group"));
            Assert.That(returnedGroups[0].Subject, Is.EqualTo(Subject.Math));
        }

        [Test]
        public async Task CreateStudyGroup_ThenSearchBySubject_ShouldReturnMatchingGroup()
        {
            var mathGroup = new StudyGroup(
                studyGroupId: 1,
                name: "Math Study Group",
                subject: Subject.Math,
                createDate: DateTime.Now,
                users: new List<User>()
            );

            var physicsGroup = new StudyGroup(
                studyGroupId: 2,
                name: "Physics Study Group",
                subject: Subject.Physics,
                createDate: DateTime.Now,
                users: new List<User>()
            );

            // Create two study groups
            await _client.PostAsJsonAsync("/api/studygroup", mathGroup);
            await _client.PostAsJsonAsync("/api/studygroup", physicsGroup);

            // Search for math groups
            var searchResponse = await _client.GetAsync("/api/studygroup/search?subject=Math");
            var returnedGroups = await searchResponse.Content.ReadFromJsonAsync<List<StudyGroup>>();

            // Should return only math group
            Assert.That(searchResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
            Assert.That(returnedGroups, Is.Not.Null);
            Assert.That(returnedGroups, Has.Count.EqualTo(1));
            Assert.That(returnedGroups[0].Subject, Is.EqualTo(Subject.Math));
        }

        [Test]
        public async Task JoinStudyGroup_ShouldAddUserToGroup()
        {
            // Create a study group
            var studyGroup = new StudyGroup(
                studyGroupId: 1,
                name: "Chemistry Study Group",
                subject: Subject.Chemistry,
                createDate: DateTime.Now,
                users: []
            );
            await _client.PostAsJsonAsync("/api/studygroup", studyGroup);

            int userId = 5;

            // Join the study group
            var joinResponse = await _client.PatchAsync(
                $"/api/studygroup/join?studyGroupId=1&userId={userId}",
                null);

            // Response should be successful
            Assert.That(joinResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            // Get the updated study groups
            var getResponse = await _client.GetAsync("/api/studygroup");
            var returnedGroups = await getResponse.Content.ReadFromJsonAsync<List<StudyGroup>>();

            // User should be in the group
            Assert.That(returnedGroups[0].Users, Has.Count.EqualTo(1));
            Assert.That(returnedGroups[0].Users[0].ID, Is.EqualTo(userId));
        }

        [Test]
        public async Task JoinStudyGroup_ThenLeaveStudyGroup_ShouldRemoveUserFromGroup()
        {
            // Create a study group
            var studyGroup = new StudyGroup(
                studyGroupId: 1,
                name: "Chemistry Study Group",
                subject: Subject.Chemistry,
                createDate: DateTime.Now,
                users: []
            );
            await _client.PostAsJsonAsync("/api/studygroup", studyGroup);

            int userId = 5;

            // Join the study group
            await _client.PatchAsync(
                $"/api/studygroup/join?studyGroupId=1&userId={userId}",
                null);

            // Act - Leave the study group
            var leaveResponse = await _client.PatchAsync(
                $"/api/studygroup/leave?studyGroupId=1&userId={userId}",
                null);

            // Assert - Response should be successful
            Assert.That(leaveResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            // Act - Get the updated study groups
            var getResponse = await _client.GetAsync("/api/studygroup");
            var returnedGroups = await getResponse.Content.ReadFromJsonAsync<List<StudyGroup>>();

            // Assert - User should no longer be in the group
            Assert.That(returnedGroups[0].Users, Is.Empty);
        }

        [Test]
        public async Task CreateStudyGroup_WithUserAlreadyInGroupWithSameSubject_ShouldFail()
        {
            // Arrange - Create a study group with a user
            var user = new User(1, "Test User");
            var mathGroup1 = new StudyGroup(
                studyGroupId: 1,
                name: "First Math Group",
                subject: Subject.Math,
                createDate: DateTime.Now,
                users: [user]
            );
            await _client.PostAsJsonAsync("/api/studygroup", mathGroup1);

            // Create a second math group with the same user
            var mathGroup2 = new StudyGroup(
                studyGroupId: 2,
                name: "Second Math Group",
                subject: Subject.Math,
                createDate: DateTime.Now,
                users: [user]
            );

            // Act - Try to create another math group with the same user
            var createResponse = await _client.PostAsJsonAsync("/api/studygroup", mathGroup2);

            // Assert - Should fail because user is already in a math group
            Assert.That(createResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            // Verify only one group was created
            var getResponse = await _client.GetAsync("/api/studygroup");
            var returnedGroups = await getResponse.Content.ReadFromJsonAsync<List<StudyGroup>>();
            Assert.That(returnedGroups, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task JoinStudyGroup_UserAlreadyInGroupWithSameSubject_ShouldFail()
        {
            // Arrange - Create two math groups
            var mathGroup1 = new StudyGroup(
                studyGroupId: 1,
                name: "First Math Group",
                subject: Subject.Math,
                createDate: DateTime.Now,
                users: []
            );

            var mathGroup2 = new StudyGroup(
                studyGroupId: 2,
                name: "Second Math Group",
                subject: Subject.Math,
                createDate: DateTime.Now,
                users: []
            );

            await _client.PostAsJsonAsync("/api/studygroup", mathGroup1);
            await _client.PostAsJsonAsync("/api/studygroup", mathGroup2);

            int userId = 10;

            // Join the first math group
            await _client.PatchAsync(
                $"/api/studygroup/join?studyGroupId=1&userId={userId}",
                null);

            // Try to join the second math group
            var joinResponse = await _client.PatchAsync(
                $"/api/studygroup/join?studyGroupId=2&userId={userId}",
                null);

            // Should fail because user is already in a math group
            Assert.That(joinResponse.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }
    }
}