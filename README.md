# TestApp Home Assignment

This is the repository that holds the solution for the home assignment provided, the assignment can be seen [here](docs/homeassignment.md).

## Thoght proccess

When I read the home assignment and understanding Testing as a practice that is determinatory, that means that fail/pass are the only outputs possible, I decided to be more opinionated on how the App should work, so I decided that I would create a .net WebApi containing the classes provided in the home assignmet with the logic necessary, the only thing that I left outside for the sake of the Assignment is to create the SQL database with its corresponding migrations, and followed an InMemory approach

## General structure of repository

This repository contains all the information needed to satisfy the home assignment in the following order

 -  .github [Contains the yaml to run the tests in a pipeline and uploads results to an allure server]
 -  src [Contains the Application and Tests code]
    - TestApp.Api [Contains code relevant to the WebApi]
    - TestApp.Core [Contains code relevant to different parts of the app]
    - TestApp.Data [Contains code relevant to the repositories and data management]
    - Tests Folder
        - TestApp.ComponentTests [Contains the Component Tests]
        - TestApp.UnitTests [Contains the unit tests]
- docs [Contains the documentation with all the explanation required by the assignment]
- utils
    - uploadFiles.sh [bash script that uploads the allure results into a server]


## Assigment Requirements

```
Write a list of different test cases to check this feature and the integrity of new entity StudyGroups according to the acceptance criteria:
 - Describe some high level steps and expectations (you can make assumptions of how the app works - just explain it)
 - Highlight what are the inputs you will be using on each test case
 - Define testing level of this test case: unit testing, component testing or e2e testing (manual) - considering that:
    - We have a unit test framework in TestApp using Nunit framework
    - We have a component test framework in our TestAppAPI using Nunit framework
    - We don't have any automation to test the UI, so manual testing will be required on e2e level  
 - Consider if you want to add all test cases to regression or not
``` 

 For the test cases I have divided in 3 different docs:
- [UnitTests](docs/unittests.md)
- [ComponentTests](docs/componenttests.md)
- [E2ETests](docs/e2etests.md)

---

```
Write the code for all automated tests you described on the different frameworks
```

To see the code of the automated test cases please refer to the files

 - ComponentTests
    - [StudyGroupApiTests](src/Tests/TestApp.ComponentTests/StudyGroupApiTests.cs)

  - Unit Tests
    - [StudyGroupRepositoryTests](src/Tests/TestApp.UnitTests/Data/StudyGroupRepositoryTests.cs)
    - [StudyGroupControllerTests](src/Tests/TestApp.UnitTests/Api/StudyGroupControllerTests.cs)
    - [StudyGroupTests](src/Tests/TestApp.UnitTests/Core/StudyGroupTests.cs)

---

```
Write a SQL query that will return "all the StudyGroups which have at least an user with 'name' starting on 'M' sorted by 'creation date'" like "Miguel" or "Manuel".
```

The SQL query should be 

``` sql
SELECT sg.*
FROM StudyGroup sg
WHERE EXISTS (
    SELECT 1
    FROM Users u
    INNER JOIN StudyGroup_User sgu ON u.UserId = sgu.UserId
    WHERE sgu.StudyGroupId = sg.StudyGroupId
    AND u.Name LIKE 'M%'
)
ORDER BY sg.CreateDate ASC;
-- It was not defined if the order should be ASC or DESC
```

## Extras for home assignment

I did 4 extra main points for this homeassignment
 - Created a running WebApi with logic according to acceptance criteria
 - Run all the tests through a pipeline in github actions, those can be found in [HERE](https://github.com/testershaven/TestApp/actions)
 - Mounted an allure server where all results are being stored, I added some documentation [HERE](./docs/allureserver.md)
 - Created a bash script to upload results into the allure server [HERE](./utils/uploadFiles.sh)

## Out of scope

I left 3 things out of scope but I would gladly discusss them in a 1on1 meeting 

 - Setup a msql server and a Migrations project
 - Was thinking on creating an ui application to connect to the api and do some e2e automated tests but seemed a bit out of scope
 - Dockerize the application, altought all tests and apps should be run inside a dockerized architecture to reduce the deltas with a production build, seemed a bit out of scope