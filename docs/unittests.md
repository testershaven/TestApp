# Unit Tests

My main objective with the unit tests is to assert that all functions do what they are meant to be, This functions are isolated with mocking techniques using the Moq library.

For the `StudyGroupController` class in the TestApp.Api project, I created the `StudyGroupControllerTests` with the following tests:
 - CreateStudyGroup_ShouldReturnOkResult
 - GetStudyGroups_ShouldReturnOkObjectResultWithStudyGroups
 - SearchStudyGroups_ShouldReturnOkObjectResultWithFilteredStudyGroups
 - JoinStudyGroup_ShouldReturnOkResult
 - LeaveStudyGroup_ShouldReturnOkResult
 - CreateStudyGroup_UserAlreadyInStudyGroupWithSubject_ShouldThrowBadHttpRequestException
 - JoinStudyGroup_UserAlreadyInStudyGroupWithSubject_ShouldThrowBadHttpRequestException
 - JoinStudyGroup_StudyGroupNotFound_ShouldHandleException

For the `StudyGroup` class in the TestApp.Core project, I created the `StudyGroupTests` with the following tests:



