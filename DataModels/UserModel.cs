public class UserModel
{

    public Int64 Id { get; set; }
    public string Email { get; set; }

    public string Password { get; set; }

    public string FullName { get; set; }
    public string Specialty {get;set;}
    public string Role {get; set;}
    public string PhoneNumber {get;set;}

    public UserModel() { }

    public UserModel(Int64 id, string email, string password, string fullName, string specialty, string role,string phoneNumber)
    {
        Id = id;
        Email = email;
        Password = password;
        FullName = fullName;
        Specialty = specialty;
        Role = role;
        PhoneNumber = phoneNumber;
    }


}



