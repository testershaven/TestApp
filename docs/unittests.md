# Unit Tests

My main objective with the unit tests is to assert that all functions do what they are meant to be, This functions are isolated with mocking techniques using the Moq library.

As its by nature all Unit tests are mandatorily run as an automated regression, on each build in the pipeline, please see [dotnet.yml](../.github/workflows/dotnet.yml)

This unit tests are self explanatory by name and steps can be seen in their respective test classes, detailing here would be an unnecesary duplication of information 

For the [StudyGroupController](../src/TestApp.Api/Controllers/StudyGroupController.cs) class in the TestApp.Api project, I created the [StudyGroupControllerTests](../src/Tests/TestApp.UnitTests/Api/StudyGroupControllerTests.cs) with the following tests:
 - CreateStudyGroup_ShouldReturnOkResult
 - GetStudyGroups_ShouldReturnOkObjectResultWithStudyGroups
 - SearchStudyGroups_ShouldReturnOkObjectResultWithFilteredStudyGroups
 - JoinStudyGroup_ShouldReturnOkResult
 - LeaveStudyGroup_ShouldReturnOkResult
 - CreateStudyGroup_UserAlreadyInStudyGroupWithSubject_ShouldThrowBadHttpRequestException
 - JoinStudyGroup_UserAlreadyInStudyGroupWithSubject_ShouldThrowBadHttpRequestException
 - JoinStudyGroup_StudyGroupNotFound_ShouldHandleException

For the [StudyGroup](../src/TestApp.Core/Models/StudyGroup.cs) class in the TestApp.Core project, I created the [StudyGroupTests](../src/Tests/TestApp.UnitTests/Core/StudyGroupTests.cs) with the following tests:

- Constructor_ValidParameters_CreatesInstance
- Constructor_NullName_ThrowsArgumentNullException
- Constructor_EmptyName_ThrowsArgumentNullException
- Constructor_NameTooShort_ThrowsArgumentException
- Constructor_NameTooLong_ThrowsArgumentException
- AddUser_AddsUserToList
- RemoveUser_RemovesUserFromList
- Constructor_SpecificDateTime_AssignsCorrectly
- RemoveUser_UserNotInList_ReturnsFalse

For the [StudyGroupRepository](../src/TestApp.Data/Repositories/StudyGroupRepository.cs) class in the TestApp.Core project, I created the [StudyGroupRepositoryTests](../src/Tests/TestApp.UnitTests/Data/StudyGroupRepositoryTests.cs) class with the following tests:

- CreateStudyGroup_AddsGroupToRepository
- GetStudyGroups_ReturnsAllGroups
- JoinStudyGroup_AddsUserToGroup
- JoinStudyGroup_GroupNotFound_ThrowsException
- LeaveStudyGroup_RemovesUserFromGroup
- LeaveStudyGroup_GroupNotFound_ThrowsException
- LeaveStudyGroup_UserNotInGroup_ThrowsException
- SearchStudyGroupsBySubject_ReturnsMatchingGroups
- SearchStudyGroupsByString_ReturnsMatchingGroups
- IsUserInStudyGroupWithSubject_ReturnsTrueForExistingUser
- IsUserInStudyGroupWithSubject_ReturnsFalseForNonExistingUser
- SearchStudyGroups_EmptyRepository_ReturnsEmptyList