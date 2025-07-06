# Component Tests

My main objective with the component tests is to assert that all the components inside the project, work cohesively between each other. This means that Im running the WebApi InMemory, through WebApplicationFactory and injecting the Repository, in this case its a class, so was a easy injection, but in case is a real database I would probably mock that part and inject the mock . For this tests I added Allure Steps that can be shown in the reporter to a more granular detail.

As its by nature all Component tests are mandatorily run as an automated regression, on each build in the pipeline, please see [dotnet.yml](../.github/workflows/dotnet.yml)

This are the following tests added: 
---

### 1. `CreateStudyGroup_ThenGetStudyGroups_ShouldReturnCreatedGroup`

**Steps:**
1. Create a new `StudyGroup` object with subject `Math` and one user.
2. Send POST request to `/api/studygroup`.
3. Assert response status is `200 OK`.
4. Send GET request to `/api/studygroup`.
5. Assert:
   - Response status is `200 OK`.
   - Response contains 1 group.
   - Group name is "Math Study Group".
   - Subject is `Math`.

---

### 2. `CreateStudyGroup_ThenSearchBySubject_ShouldReturnMatchingGroup`

**Steps:**
1. Create two groups: one `Math`, one `Physics`.
2. POST both groups.
3. GET `/api/studygroup?subject=Math`.
4. Assert:
   - Response status is `200 OK`.
   - Only one group is returned.
   - Group's subject is `Math`.

---

### 3. `JoinStudyGroup_ShouldAddUserToGroup`

**Steps:**
1. Create a `Chemistry` group with no users.
2. POST the group.
3. PATCH to `/api/studygroup/join?studyGroupId=1&userId=5`.
4. Assert:
   - Join response is `200 OK`.
5. GET `/api/studygroup`.
6. Assert:
   - One user is added to the group.
   - User ID is `5`.

---

### 4. `JoinStudyGroup_ThenLeaveStudyGroup_ShouldRemoveUserFromGroup`

**Steps:**
1. Create a `Chemistry` group.
2. POST it.
3. Join group with user ID `5`.
4. Leave the group using `/api/studygroup/leave?studyGroupId=1&userId=5`.
5. Assert:
   - Leave response is `200 OK`.
6. GET all groups.
7. Assert:
   - Group has zero users.

---

### 5. `CreateStudyGroup_WithUserAlreadyInGroupWithSameSubject_ShouldFail`

**Steps:**
1. Create a `Math` group with a user.
2. POST it.
3. Create another `Math` group with the same user.
4. POST it.
5. Assert:
   - Response is `400 Bad Request`.
6. GET all groups.
7. Assert only one group exists.

---

### 6. `JoinStudyGroup_UserAlreadyInGroupWithSameSubject_ShouldFail`

**Steps:**
1. Create two `Math` groups.
2. POST both.
3. Join the first group with user ID `10`.
4. Attempt to join the second.
5. Assert:
   - Join response is `400 Bad Request`.

---

### 7. `SearchStudyGroups_WithoutSubject_ShouldReturnAllGroups`

**Steps:**
1. Create 3 groups: `Math`, `Physics`, `Chemistry`.
2. POST all.
3. GET `/api/studygroup` (no filter).
4. Assert:
   - Response is `200 OK`.
   - All 3 groups are returned.
   - Subjects include `Math`, `Physics`, and `Chemistry`.

---

### 8. `SearchStudyGroups_WithAscendingSort_ShouldReturnOrderedGroups`

**Steps:**
1. Create groups with dates:
   - 10 days ago → "Oldest Group"
   - 5 days ago → "Middle Group"
   - Today → "Newest Group"
2. POST all (in any order).
3. GET `/api/studygroup?sortOrder=asc`.
4. Assert:
   - Groups are sorted by creation date ascending:
     1. Oldest
     2. Middle
     3. Newest

---

### 9. `SearchStudyGroups_WithDescendingSort_ShouldReturnOrderedGroups`

**Steps:**
1. Create same groups as above (oldest, middle, newest).
2. POST all.
3. GET `/api/studygroup?sortOrder=desc`.
4. Assert:
   - Groups are sorted by creation date descending:
     1. Newest
     2. Middle
     3. Oldest

---

### 10. `SearchStudyGroups_WithSubjectAndSort_ShouldReturnFilteredOrderedGroups`

**Steps:**
1. Create:
   - Old Math Group (10 days ago)
   - New Math Group (today)
   - Physics Group (5 days ago)
2. POST all.
3. GET `/api/studygroup?subject=Math&sortOrder=desc`.
4. Assert:
   - Only Math groups are returned.
   - Sorted descending by date:
     1. New Math Group
     2. Old Math Group
   - Physics group is not included.

---
