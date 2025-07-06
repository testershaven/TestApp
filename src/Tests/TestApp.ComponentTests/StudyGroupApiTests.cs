using Allure.Net.Commons;
using Allure.NUnit;
using Allure.NUnit.Attributes;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using TestApp.Data.Repositories;
using TestApp.Enums;
using TestApp.Models;

namespace TestApp.ComponentTests
{
    [AllureNUnit]
    [AllureFeature("StudyGroup features")]
    [AllureParentSuite("Component Tests")]
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
            var newStudyGroup = new StudyGroup(1, "Math Study Group", Subject.Math, DateTime.Now, new List<User> { new User(1, "Test User") });

            AllureApi.Step("Creating a new study group");
            var response = await _client.PostAsJsonAsync("/api/studygroup", newStudyGroup);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            AllureApi.Step("Retrieving all study groups and verifying the created group");
            var getResponse = await _client.GetAsync("/api/studygroup");
            var returnedGroups = await getResponse.Content.ReadFromJsonAsync<List<StudyGroup>>();

            Assert.Multiple(() =>
            {
                Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(returnedGroups, Is.Not.Null);
                Assert.That(returnedGroups, Has.Count.EqualTo(1));
                Assert.That(returnedGroups[0].Name, Is.EqualTo("Math Study Group"));
                Assert.That(returnedGroups[0].Subject, Is.EqualTo(Subject.Math));
            });
        }

        [Test]
        public async Task CreateStudyGroup_ThenSearchBySubject_ShouldReturnMatchingGroup()
        {
            var mathGroup = new StudyGroup(1, "Math Study Group", Subject.Math, DateTime.Now, []);
            var physicsGroup = new StudyGroup(2, "Physics Study Group", Subject.Physics, DateTime.Now, []);

            AllureApi.Step("Creating two study groups");
            await _client.PostAsJsonAsync("/api/studygroup", mathGroup);
            await _client.PostAsJsonAsync("/api/studygroup", physicsGroup);

            AllureApi.Step("Searching for study groups with subject 'Math'");
            var response = await _client.GetAsync("/api/studygroup?subject=Math");
            var groups = await response.Content.ReadFromJsonAsync<List<StudyGroup>>();

            Assert.Multiple(() =>
            {
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(groups, Is.Not.Null);
                Assert.That(groups, Has.Count.EqualTo(1));
                Assert.That(groups[0].Subject, Is.EqualTo(Subject.Math));
            });
        }

        [Test]
        public async Task JoinStudyGroup_ShouldAddUserToGroup()
        {
            var studyGroup = new StudyGroup(1, "Chemistry Study Group", Subject.Chemistry, DateTime.Now, []);
            int userId = 5;

            AllureApi.Step("Creating a chemistry study group");
            await _client.PostAsJsonAsync("/api/studygroup", studyGroup);

            AllureApi.Step($"User {userId} joins the study group");
            var response = await _client.PatchAsync($"/api/studygroup/join?studyGroupId=1&userId={userId}", null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            AllureApi.Step("Verifying the user was added to the group");
            var getResponse = await _client.GetAsync("/api/studygroup");
            var groups = await getResponse.Content.ReadFromJsonAsync<List<StudyGroup>>();

            Assert.Multiple(() =>
            {
                Assert.That(groups, Is.Not.Null);
                Assert.That(groups?[0].Users, Has.Count.EqualTo(1));
                Assert.That(groups?[0].Users?[0].ID, Is.EqualTo(userId));
            });
        }

        [Test]
        public async Task JoinStudyGroup_ThenLeaveStudyGroup_ShouldRemoveUserFromGroup()
        {
            var studyGroup = new StudyGroup(1, "Chemistry Study Group", Subject.Chemistry, DateTime.Now, []);
            int userId = 5;

            await _client.PostAsJsonAsync("/api/studygroup", studyGroup);
            await _client.PatchAsync($"/api/studygroup/join?studyGroupId=1&userId={userId}", null);

            AllureApi.Step($"User {userId} leaves the study group");
            var response = await _client.PatchAsync($"/api/studygroup/leave?studyGroupId=1&userId={userId}", null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

            AllureApi.Step("Verifying the user was removed from the group");
            var getResponse = await _client.GetAsync("/api/studygroup");
            var groups = await getResponse.Content.ReadFromJsonAsync<List<StudyGroup>>();

            Assert.Multiple(() =>
            {
                Assert.That(groups, Is.Not.Null);
                Assert.That(groups?[0].Users, Is.Empty);
            });
        }

        [Test]
        public async Task CreateStudyGroup_WithUserAlreadyInGroupWithSameSubject_ShouldFail()
        {
            var user = new User(1, "Test User");

            var group1 = new StudyGroup(1, "First Math Group", Subject.Math, DateTime.Now, [user]);
            var group2 = new StudyGroup(2, "Second Math Group", Subject.Math, DateTime.Now, [user]);

            await _client.PostAsJsonAsync("/api/studygroup", group1);

            AllureApi.Step("Attempting to create a second group with the same subject and user");
            var response = await _client.PostAsJsonAsync("/api/studygroup", group2);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));

            AllureApi.Step("Verifying that only one group was created");
            var getResponse = await _client.GetAsync("/api/studygroup");
            var groups = await getResponse.Content.ReadFromJsonAsync<List<StudyGroup>>();
            Assert.That(groups, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task JoinStudyGroup_UserAlreadyInGroupWithSameSubject_ShouldFail()
        {
            var group1 = new StudyGroup(1, "First Math Group", Subject.Math, DateTime.Now, []);
            var group2 = new StudyGroup(2, "Second Math Group", Subject.Math, DateTime.Now, []);
            int userId = 10;

            await _client.PostAsJsonAsync("/api/studygroup", group1);
            await _client.PostAsJsonAsync("/api/studygroup", group2);
            await _client.PatchAsync($"/api/studygroup/join?studyGroupId=1&userId={userId}", null);

            AllureApi.Step("Attempting to join a second group with the same subject");
            var response = await _client.PatchAsync($"/api/studygroup/join?studyGroupId=2&userId={userId}", null);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
        }

        [Test]
        [AllureDescription("Test searching study groups without specifying a subject")]
        public async Task SearchStudyGroups_WithoutSubject_ShouldReturnAllGroups()
        {
            await _client.PostAsJsonAsync("/api/studygroup", new StudyGroup(1, "Mathematics", Subject.Math, DateTime.Now, []));
            await _client.PostAsJsonAsync("/api/studygroup", new StudyGroup(2, "Physics", Subject.Physics, DateTime.Now, []));
            await _client.PostAsJsonAsync("/api/studygroup", new StudyGroup(3, "Chemistry", Subject.Chemistry, DateTime.Now, []));

            AllureApi.Step("Fetching all study groups without filtering");
            var response = await _client.GetAsync("/api/studygroup");
            var groups = await response.Content.ReadFromJsonAsync<List<StudyGroup>>();

            Assert.Multiple(() =>
            {
                Assert.That(groups, Has.Count.EqualTo(3));
                CollectionAssert.Contains(groups.Select(g => g.Subject), Subject.Math);
                CollectionAssert.Contains(groups.Select(g => g.Subject), Subject.Physics);
                CollectionAssert.Contains(groups.Select(g => g.Subject), Subject.Chemistry);
            });
        }

        [Test]
        [AllureDescription("Test sorting study groups by creation date in ascending order")]
        public async Task SearchStudyGroups_WithAscendingSort_ShouldReturnOrderedGroups()
        {
            await _client.PostAsJsonAsync("/api/studygroup", new StudyGroup(1, "Oldest", Subject.Math, DateTime.Now.AddDays(-10), []));
            await _client.PostAsJsonAsync("/api/studygroup", new StudyGroup(3, "Newest", Subject.Chemistry, DateTime.Now, []));
            await _client.PostAsJsonAsync("/api/studygroup", new StudyGroup(2, "Middle", Subject.Physics, DateTime.Now.AddDays(-5), []));

            AllureApi.Step("Verifying ascending order of groups by creation date");
            var response = await _client.GetAsync("/api/studygroup?sortOrder=asc");
            var groups = await response.Content.ReadFromJsonAsync<List<StudyGroup>>();

            Assert.That(groups[0].Name, Is.EqualTo("Oldest"));
            Assert.That(groups[1].Name, Is.EqualTo("Middle"));
            Assert.That(groups[2].Name, Is.EqualTo("Newest"));
        }

        [Test]
        [AllureDescription("Test sorting study groups by creation date in descending order")]
        public async Task SearchStudyGroups_WithDescendingSort_ShouldReturnOrderedGroups()
        {
            await _client.PostAsJsonAsync("/api/studygroup", new StudyGroup(1, "Oldest", Subject.Math, DateTime.Now.AddDays(-10), []));
            await _client.PostAsJsonAsync("/api/studygroup", new StudyGroup(3, "Newest", Subject.Chemistry, DateTime.Now, []));
            await _client.PostAsJsonAsync("/api/studygroup", new StudyGroup(2, "Middle", Subject.Physics, DateTime.Now.AddDays(-5), []));

            AllureApi.Step("Verifying descending order of groups by creation date");
            var response = await _client.GetAsync("/api/studygroup?sortOrder=desc");
            var groups = await response.Content.ReadFromJsonAsync<List<StudyGroup>>();

            Assert.That(groups[0].Name, Is.EqualTo("Newest"));
            Assert.That(groups[1].Name, Is.EqualTo("Middle"));
            Assert.That(groups[2].Name, Is.EqualTo("Oldest"));
        }

        [Test]
        [AllureDescription("Test sorting filtered study groups by creation date")]
        public async Task SearchStudyGroups_WithSubjectAndSort_ShouldReturnFilteredOrderedGroups()
        {
            await _client.PostAsJsonAsync("/api/studygroup", new StudyGroup(1, "Old Math", Subject.Math, DateTime.Now.AddDays(-10), []));
            await _client.PostAsJsonAsync("/api/studygroup", new StudyGroup(3, "Physics", Subject.Physics, DateTime.Now.AddDays(-5), []));
            await _client.PostAsJsonAsync("/api/studygroup", new StudyGroup(2, "New Math", Subject.Math, DateTime.Now, []));

            AllureApi.Step("Filtering by subject 'Math' and sorting descending");
            var response = await _client.GetAsync("/api/studygroup?subject=Math&sortOrder=desc");
            var groups = await response.Content.ReadFromJsonAsync<List<StudyGroup>>();

            Assert.Multiple(() =>
            {
                Assert.That(groups, Has.Count.EqualTo(2));
                Assert.That(groups[0].Name, Is.EqualTo("New Math"));
                Assert.That(groups[1].Name, Is.EqualTo("Old Math"));
                Assert.That(groups.All(g => g.Subject == Subject.Math), Is.True);
            });
        }
    }
}
