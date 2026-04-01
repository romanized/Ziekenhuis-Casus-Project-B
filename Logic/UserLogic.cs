public class UserLogic
{
    public static UserModel? CurrentAccount { get; private set; }
    private UserAccess _access = new();

    public UserLogic(){}

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
    // rigster a user
    public bool Register( string email, string password,string fullname,string date, string phoneNumber)
    {
        // Check of email exist
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
            Role = "ouder" // standaard rol
        };

        _access.Write(newUser);
        return true; // created
    }
}




