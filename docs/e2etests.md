# End-to-End Tests 

This end 2 end test cases are having a rough guess on how the UI would look and feel, might be imperfect. 

### List of tests for regression

Knowingly that Im already covering lot of use cases in unit and component test cases I would only use following test cases for regression:

- Create Study Group for Valid Subject
- Find Study Group and Join It
- List All Study Groups
- Leave Study Group

## Full list of test cases

### 1. Create Study Group for Valid Subject
**Test Case**: Verify that a user can create a study group for a valid subject (Math, Chemistry, or Physics).

**Steps**:
1. Log in to the web application as a user.
2. Navigate to the "Study Groups" section of the UI.
3. Click on the "Create Study Group" button.
4. In the form, enter the following details:
   - **Name**: "Math Study Group"
   - **Subject**: Select "Math" from the dropdown list.
   - **Users**: Select the user from the available list (e.g., User ID 1).
5. Click "Submit" to create the study group.
6. Verify that the UI displays a success message indicating the study group was created.
7. Navigate to the list of existing study groups and verify that the "Math Study Group" appears.

**Expected Outcome**:
- Study Group is created successfully for the "Math" subject.
- The new group should appear in the study group list.

---

### 2. Create Study Group for Invalid Subject
**Test Case**: Verify that the system does not allow users to create study groups for subjects other than Math, Chemistry, or Physics.

**Steps**:
1. Log in to the web application as a user.
2. Navigate to the "Study Groups" section of the UI.
3. Click on the "Create Study Group" button.
4. In the form, enter the following details:
   - **Name**: "History Study Group"
   - **Subject**: Select "History" from the dropdown list (invalid subject).
   - **Users**: Select the user from the available list (e.g., User ID 1).
5. Click "Submit" to attempt to create the study group.
6. Verify that the UI displays an error message indicating that the subject is invalid.

**Expected Outcome**:
- The system prevents creation of the "History" study group and displays an error message.

---

### 3. Validate Study Group Name Length
**Test Case**: Verify that the system enforces the rule for study group name length (5-30 characters).

**Steps**:
1. Log in to the web application as a user.
2. Navigate to the "Study Groups" section of the UI.
3. Click on the "Create Study Group" button.
4. In the form, enter the following details:
   - **Name**: "Math" (too short).
   - **Subject**: Select "Math" from the dropdown list.
   - **Users**: Select the user from the available list (e.g., User ID 1).
5. Click "Submit" to attempt to create the study group.
6. Verify that the UI displays an error message indicating that the name is too short.
   
7. Now, try creating a study group with a name that's too long:
   - **Name**: "A very long name that exceeds the maximum length for a study group name."
   - **Subject**: Select "Math".
   - **Users**: Select the user.
8. Verify that the UI displays an error message indicating that the name is too long.

**Expected Outcome**:
- The system prevents creation if the name is less than 5 characters or more than 30 characters.

---

### 4. Create Study Group and Join It
**Test Case**: Verify that a user can join a study group after it is created.

**Steps**:
1. Log in to the web application as a user.
2. Navigate to the "Study Groups" section.
3. Click on the "Create Study Group" button and enter the following details:
   - **Name**: "Physics Study Group"
   - **Subject**: "Physics"
   - **Users**: Select the user from the available list (e.g., User ID 1).
4. Click "Submit" to create the study group.
5. Once the study group is created, navigate to the list of study groups and click the "Join" button next to the "Physics Study Group."
6. Verify that the UI displays a success message indicating that the user joined the study group.
7. Verify that the user is now listed as a member of the "Physics Study Group."

**Expected Outcome**:
- The user successfully joins the study group.
- The group’s user list should now include the user.

---

### 5. Create Study Group and Attempt to Join Another Study Group with the Same Subject
**Test Case**: Verify that a user can only join one study group per subject.

**Steps**:
1. Log in to the web application as a user.
2. Navigate to the "Study Groups" section.
3. Create a "Math" study group by following the steps from **Test Case 1**.
4. After successfully creating the "Math Study Group," attempt to create a second "Math Study Group" with the same user.
5. Verify that the system displays an error message preventing the creation of a second study group for the same subject.
6. Check that only one study group for "Math" exists in the system.

**Expected Outcome**:
- The system prevents the creation of multiple study groups for the same subject by the same user.
- An error message should inform the user that they are already part of a study group for that subject.

---

### 6. List All Study Groups
**Test Case**: Verify that users can retrieve a list of all study groups.

**Steps**:
1. Log in to the web application as a user.
2. Navigate to the "Study Groups" section.
3. Verify that the UI lists all the available study groups.
4. Ensure that the list is up-to-date and includes any recently created study groups.

**Expected Outcome**:
- The list of all study groups should be displayed correctly on the UI.

---

### 7. Filter Study Groups by Subject
**Test Case**: Verify that users can filter study groups by a specific subject.

**Steps**:
1. Log in to the web application as a user.
2. Navigate to the "Study Groups" section.
3. In the filter section, select the "Math" subject from the dropdown.
4. Verify that the UI only displays study groups related to the "Math" subject.

**Expected Outcome**:
- The system should only show study groups for the selected subject.

---

### 8. Sort Study Groups by Creation Date (Most Recent)
**Test Case**: Verify that users can sort study groups by the most recent creation date.

**Steps**:
1. Log in to the web application as a user.
2. Navigate to the "Study Groups" section.
3. Select the "Sort by Most Recent" option from the sorting dropdown.
4. Verify that the most recently created study groups appear first in the list.

**Expected Outcome**:
- Study groups should be ordered by their creation date, with the most recent one listed first.

---

### 9. Sort Study Groups by Creation Date (Oldest)
**Test Case**: Verify that users can sort study groups by the oldest creation date.

**Steps**:
1. Log in to the web application as a user.
2. Navigate to the "Study Groups" section.
3. Select the "Sort by Oldest" option from the sorting dropdown.
4. Verify that the oldest study groups appear first in the list.

**Expected Outcome**:
- Study groups should be ordered by their creation date, with the oldest one listed first.

---

### 10. Leave Study Group
**Test Case**: Verify that a user can leave a study group they have joined.

**Steps**:
1. Log in to the web application as a user.
2. Navigate to the "Study Groups" section.
3. Join an existing study group by clicking on the "Join" button.
4. After joining, click the "Leave" button next to the group.
5. Verify that the UI displays a success message confirming the user has left the group.
6. Check that the user is no longer listed as a member of the group.

**Expected Outcome**:
- The user successfully leaves the study group.
- The user is no longer listed in the study group.

---

### 11. Validate Study Group Creation Time
**Test Case**: Verify that the creation time of study groups is recorded correctly.

**Steps**:
1. Log in to the web application as a user.
2. Navigate to the "Study Groups" section.
3. Create a new study group and note the time of creation.
4. After creation, navigate to the study group’s detail page and verify that the `createDate` matches the timestamp of when the group was created.

**Expected Outcome**:
- The `createDate` field should reflect the actual creation time.

---

### 12. Multiple Users Joining the Same Study Group
**Test Case**: Verify that multiple users can join the same study group.

**Steps**:
1. Log in to the web application as User A and create a study group (e.g., "Math Study Group").
2. Log in as User B and navigate to the "Study Groups" section.
3. Click on the "Join" button next to the "Math Study Group."
4. Log in as User C and repeat the process to join the same study group.
5. Verify that all users are listed as members of the study group.

**Expected Outcome**:
- Multiple users can successfully join the same study group.

---

### 13. Attempt to Join Non-Existent Study Group
**Test Case**: Verify that users cannot join a study group that doesn’t exist.

**Steps**:
1. Log in to the web application as a user.
2. Attempt to join a study group by clicking on a non-existent group.
3. Verify that the UI displays a "Group Not Found" message or similar indicating the group does not exist.

**Expected Outcome**:
- The system prevents joining a non-existent study group.
