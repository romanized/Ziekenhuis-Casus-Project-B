public class UserLogic
{
    public static UserModel? CurrentAccount { get; private set; }
    private UserAccess _access;

    public UserLogic() { _access = new UserAccess(); }

    // overload voor tests: geef je eigen UserAccess mee met in-memory db
    public UserLogic(UserAccess access) { _access = access; }

    public UserModel CheckLogin(string email, string password)
    {
        UserModel acc = _access.GetByEmail(email);
        if (acc != null && acc.Password == password)
        {
            CurrentAccount = acc;
            return acc;
        }
        return null!;
    }

    public static void Logout()
    {
        CurrentAccount = null;
    }

    public bool Register(string email, string password, string fullname, string date, string phoneNumber, string startdate, string notes)
    {
        UserModel? existingUser = _access.GetByEmail(email);
        if (existingUser != null)
        {
            return false;
        }

        UserModel newUser = new UserModel
        {
            Email = email,
            Password = password,
            FullName = fullname,
            BirthDate = date,
            PhoneNumber = phoneNumber,
            Specialty = null!,
            Role = "ouder",
            StartDate = startdate,
            Notes = notes
        };

        _access.Write(newUser);
        return true;
    }

    public bool CreateEmployee(string email, string password, string fullname, string role, string specialty)
    {
        UserModel? existing = _access.GetByEmail(email);
        if (existing != null)
        {
            return false;
        }

        UserModel newEmployee = new UserModel
        {
            Email = email,
            Password = password,
            FullName = fullname,
            BirthDate = "",
            PhoneNumber = "",
            StartDate = "",
            Role = role,
            Specialty = specialty
        };

        _access.Write(newEmployee);
        return true;
    }
}




