# Component Tests

My main objective with the component tests is to assert that all the components inside the project, work cohesively between each other. This means that Im running the WebApi InMemory, through WebApplicationFactory and injecting the Repository, in this case its a class, so was a easy injection, but in case is a real database I would probably mock that part and inject the mock instead

As its by nature all Component tests are mandatorily run as an automated regression, on each build in the pipeline, please see [dotnet.yml](../.github/workflows/dotnet.yml)

This are the following tests added: 
---

### 1. **`CreateStudyGroup_ThenGetStudyGroups_ShouldReturnCreatedGroup`**

**Purpose**: Verify that creating a study group and then fetching all groups returns the created one.

**Steps**:
- **Arrange**: Create a new `StudyGroup` object with a user.
- **Act**: Send a `POST` request to `/api/studygroup` to create it.
- **Assert**:
  - Ensure response is `200 OK`.
  - Send a `GET` request to `/api/studygroup`.
  - Ensure response is `200 OK` and contains exactly one group with correct `Name` and `Subject`.

---

### 2. **`CreateStudyGroup_ThenSearchBySubject_ShouldReturnMatchingGroup`**

**Purpose**: Ensure subject-based search returns only the matching group(s).

**Steps**:
- **Arrange**: Create a Math group and a Physics group.
- **Act**:
  - Send two `POST` requests to `/api/studygroup` for both groups.
  - Send a `GET` request to `/api/studygroup/search?subject=Math`.
- **Assert**:
  - Ensure response is `200 OK`.
  - Validate only the Math group is returned.

---

### 3. **`JoinStudyGroup_ShouldAddUserToGroup`**

**Purpose**: Validate that a user can successfully join a study group.

**Steps**:
- **Arrange**: Create an empty Chemistry group via `POST`.
- **Act**:
  - Send a `PATCH` request to `/api/studygroup/join?studyGroupId=1&userId=5`.
- **Assert**:
  - Ensure join response is `200 OK`.
  - Fetch study groups via `GET`, and validate the group contains the user with ID 5.

---

### 4. **`JoinStudyGroup_ThenLeaveStudyGroup_ShouldRemoveUserFromGroup`**

**Purpose**: Ensure a user can join then leave a group successfully.

**Steps**:
- **Arrange**: Create an empty Chemistry group via `POST`.
- **Act**:
  - Join the group (`PATCH /join`).
  - Leave the group (`PATCH /leave`).
- **Assert**:
  - Both responses are `200 OK`.
  - After leaving, fetch groups and ensure the group's user list is empty.

---

### 5. **`CreateStudyGroup_WithUserAlreadyInGroupWithSameSubject_ShouldFail`**

**Purpose**: Prevent users from being in multiple groups of the same subject.

**Steps**:
- **Arrange**:
  - Create a Math group with a user via `POST`.
  - Create another Math group with the same user.
- **Act**:
  - Attempt to create the second group via `POST`.
- **Assert**:
  - Expect a `400 Bad Request`.
  - Ensure only one group exists via `GET`.

---

### 6. **`JoinStudyGroup_UserAlreadyInGroupWithSameSubject_ShouldFail`**

**Purpose**: Ensure users can't join multiple groups for the same subject.

**Steps**:
- **Arrange**:
  - Create two empty Math groups via `POST`.
- **Act**:
  - Join the first group (`PATCH /join`).
  - Try joining the second group (`PATCH /join`).
- **Assert**:
  - Second join request should fail with `400 Bad Request`.

---