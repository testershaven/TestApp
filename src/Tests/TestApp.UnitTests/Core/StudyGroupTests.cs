using Allure.NUnit;
using Allure.NUnit.Attributes;
using TestApp.Enums;
using TestApp.Models;

namespace TestApp.UnitTests.Core
{
    [AllureNUnit]
    [AllureFeature("StudyGroup features")]
    [AllureParentSuite("Unit Tests")]

    public class StudyGroupTests
    {
        [Test]
        public void Constructor_ValidParameters_CreatesInstance()
        {
            int id = 1;
            string name = "Test Group";
            Subject subject = Subject.Chemistry;
            DateTime createDate = DateTime.Now;
            List<User> users = [];

            var studyGroup = new StudyGroup(id, name, subject, createDate, users);

            Assert.Multiple(() =>
            {
                Assert.That(studyGroup.StudyGroupId, Is.EqualTo(id));
                Assert.That(studyGroup.Name, Is.EqualTo(name));
                Assert.That(studyGroup.Subject, Is.EqualTo(subject));
                Assert.That(studyGroup.CreateDate, Is.EqualTo(createDate));
                Assert.That(studyGroup.Users, Is.SameAs(users));
            });
        }

        [Test]
        public void Constructor_NullName_ThrowsArgumentNullException()
        {
            string? name = null;

            var exception = Assert.Throws<ArgumentNullException>(() =>
                new StudyGroup(1, name, Subject.Chemistry, DateTime.Now, new List<User>()));

            Assert.That(exception.ParamName, Is.EqualTo("name"));
        }

        [Test]
        public void Constructor_EmptyName_ThrowsArgumentNullException()
        {
            string name = string.Empty;

            var exception = Assert.Throws<ArgumentNullException>(() =>
                new StudyGroup(1, name, Subject.Chemistry, DateTime.Now, new List<User>()));

            Assert.That(exception.ParamName, Is.EqualTo("name"));
        }

        [Test]
        public void Constructor_NameTooShort_ThrowsArgumentException()
        {
            string name = "Test"; // 4 characters, minimum is 5

            var exception = Assert.Throws<ArgumentException>(() =>
                new StudyGroup(1, name, Subject.Chemistry, DateTime.Now, new List<User>()));

            Assert.That(exception.Message, Does.Contain("Name must be between 5 and 30 characters"));
        }

        [Test]
        public void Constructor_NameTooLong_ThrowsArgumentException()
        {
            string name = "ThisNameIsWayTooLongForAStudyGroupName"; // 37 characters, maximum is 30

            var exception = Assert.Throws<ArgumentException>(() =>
                new StudyGroup(1, name, Subject.Chemistry, DateTime.Now, new List<User>()));

            Assert.That(exception.Message, Does.Contain("Name must be between 5 and 30 characters"));
        }

        [Test]
        public void AddUser_AddsUserToList()
        {
            var users = new List<User>();
            var studyGroup = new StudyGroup(1, "Test Group", Subject.Chemistry, DateTime.Now, users);
            var user = new User(1, "Test User");

            studyGroup.AddUser(user);

            Assert.That(studyGroup.Users, Does.Contain(user));
        }

        [Test]
        public void RemoveUser_RemovesUserFromList()
        {
            var user = new User(1, "Test User");
            var user2 = new User(2, "Test User 2");
            var users = new List<User> { user, user2 };
            var studyGroup = new StudyGroup(1, "Test Group", Subject.Chemistry, DateTime.Now, users);

            studyGroup.RemoveUser(user);

            Assert.That(studyGroup.Users, Does.Not.Contain(user));
            Assert.That(studyGroup.Users, Is.Not.Empty);
        }

        [Test]
        public void Constructor_SpecificDateTime_AssignsCorrectly()
        {
            var specificDate = new DateTime(2023, 1, 1, 12, 0, 0);

            var studyGroup = new StudyGroup(1, "Valid Group Name", Subject.Math, specificDate, new List<User>());

            Assert.That(studyGroup.CreateDate, Is.EqualTo(specificDate));
        }


        [Test]
        public void RemoveUser_UserNotInList_ReturnsFalse()
        {
            var existingUser = new User(1, "Existing User");
            var nonExistingUser = new User(2, "Non-Existing User");
            var users = new List<User> { existingUser };
            var studyGroup = new StudyGroup(1, "Valid Group Name", Subject.Chemistry, DateTime.Now, users);

            studyGroup.RemoveUser(nonExistingUser);

            Assert.That(studyGroup.Users.Count, Is.EqualTo(1));
        }
    }
}