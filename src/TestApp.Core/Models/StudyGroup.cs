using TestApp.Enums;

namespace TestApp.Models;

public class StudyGroup
{
    public StudyGroup(int studyGroupId, string name, Subject subject, DateTime createDate, List<User> users)
    {
        // Name validation
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name), "Name cannot be null or empty");
        }

        if (name.Length < 5 || name.Length > 30)
        {
            throw new ArgumentException($"Name must be between 5 and 30 characters. Current length: {name.Length}", nameof(name));
        }

        StudyGroupId = studyGroupId;

        Name = name;

        Subject = subject;

        CreateDate = createDate;

        Users = users;
    }


    //Some logic will be missing to validate values according to acceptance criteria, but imagine it is existing or do it yourself

    public int StudyGroupId { get; }


    public string Name { get; }

    public Subject Subject { get; }

    public DateTime CreateDate { get; }

    public List<User> Users { get; }

    public void AddUser(User user)
    {
        Users.Add(user);
    }

    public void RemoveUser(User user)
    {
        Users.Remove(user);
    }
}