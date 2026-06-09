static class UserRegister
{
    private static bool WantsBack(string? input)
    {
        string text = input?.Trim().ToLower() ?? "";
        return text == "back" || text == "terug";
    }

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrEmpty(email)) return false;

        int at = email.IndexOf('@');
        if (at <= 0) return false;

        int dot = email.IndexOf('.', at);
        return dot > at + 1 && dot < email.Length - 1;
    }

    private static bool IsValidPassword(string password)
    {
        if (string.IsNullOrEmpty(password) || password.Length < 6) return false;
        return password.Any(char.IsDigit);
    }

    public static bool IsValidPhoneNumber(string phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return false;

        return phone.Any(char.IsDigit)
            && phone.All(c => char.IsDigit(c) || c == '+' || c == '-' || c == ' ');
    }

    public static bool IsValidNote(string note)
    {
        if (string.IsNullOrWhiteSpace(note)) return true;

        return !note.All(char.IsDigit);
    }

    private static bool AskEmail(ref string email)
    {
        while (true)
        {
            Console.Write("Enter your Email or type 'back': ");
            string? input = Console.ReadLine();

            if (WantsBack(input)) return false;

            if (IsValidEmail(input ?? ""))
            {
                email = input!;
                return true;
            }

            ShowError("Invalid email. A valid address contains '@' and '.' (e.g. name@mail.com).");
        }
    }

    private static bool AskPassword(ref string password)
    {
        while (true)
        {
            Console.Write("Enter your Password or type 'back' (min 6 chars, at least 1 digit): ");
            string input = ReadMaskedPassword();

            if (WantsBack(input)) return false;

            if (IsValidPassword(input))
            {
                password = input;
                return true;
            }

            ShowError("Password must be at least 6 characters and contain at least 1 digit.");
        }
    }

    private static bool AskFullName(ref string fullname)
    {
        while (true)
        {
            Console.Write("Enter your Full Name or type 'back': ");
            string? input = Console.ReadLine();

            if (WantsBack(input)) return false;

            if (!string.IsNullOrWhiteSpace(input))
            {
                fullname = input;
                return true;
            }

            ShowError("Name cannot be empty.");
        }
    }

    private static bool AskDate(string question, string errorMessage, ref DateTime value)
    {
        while (true)
        {
            Console.Write(question);
            string? input = Console.ReadLine();

            if (WantsBack(input)) return false;

            if (DateTime.TryParse(input, out DateTime date))
            {
                value = date;
                return true;
            }

            ShowError(errorMessage);
        }
    }

    private static bool AskPhoneNumber(ref string phonenumber)
    {
        while (true)
        {
            Console.Write("Enter your Phone number or type 'back': ");
            string? input = Console.ReadLine();

            if (WantsBack(input)) return false;

            if (IsValidPhoneNumber(input ?? ""))
            {
                phonenumber = input!;
                return true;
            }

            ShowError("Invalid phone number. Use digits, spaces, '+' or '-' only. Example: 0612345678");
        }
    }

    private static bool AskNotes(ref string notes)
    {
        while (true)
        {
            Console.Write("Enter your Notes or type 'back': ");
            string? input = Console.ReadLine();

            if (WantsBack(input)) return false;

            if (IsValidNote(input ?? ""))
            {
                notes = input ?? "";
                return true;
            }

            ShowError("Notes cannot be only numbers. Please describe in words.");
        }
    }

    private static string ReadMaskedPassword()
    {
        string password = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);

            if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[..^1];
                Console.Write("\b \b");
            }
            else if (key.Key != ConsoleKey.Enter && !char.IsControl(key.KeyChar))
            {
                password += key.KeyChar;
                Console.Write("*");
            }

        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return password;
    }

    private static void ShowHeader(int step)
    {
        Console.Clear();
        Console.WriteLine("=== Patient registration ===");
        Console.WriteLine();
        Console.WriteLine("Welcome to the hospital registration.");
        Console.WriteLine("Fill in your details so the hospital can prepare your care and appointments.");
        Console.WriteLine();
        Console.WriteLine($"Step {step + 1} of 7");
        Console.WriteLine("Type 'back' or 'terug' to go one step back.");
        Console.WriteLine();
    }

    private static void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void Start()
    {
        UserLogic userLogic = new();
        bool running = true;

        while (running)
        {
            int step = 0;

            string email = "";
            string password = "";
            string fullname = "";
            string phonenumber = "";
            string notes = "";

            DateTime birthdate = DateTime.MinValue;
            DateTime startdate = DateTime.MinValue;

            bool registrationFinished = false;

            while (!registrationFinished)
            {
                if (step == 0)
                {
                    ShowHeader(step);

                    bool next = AskEmail(ref email);

                    if (next)
                    {
                        step++;
                    }
                    else
                    {
                        registrationFinished = true;
                        running = false;
                    }
                }
                else if (step == 1)
                {
                    ShowHeader(step);

                    bool next = AskPassword(ref password);

                    if (next)
                    {
                        step++;
                    }
                    else
                    {
                        step--;
                    }
                }
                else if (step == 2)
                {
                    ShowHeader(step);

                    bool next = AskFullName(ref fullname);

                    if (next)
                    {
                        step++;
                    }
                    else
                    {
                        step--;
                    }
                }
                else if (step == 3)
                {
                    ShowHeader(step);

                    bool next = AskDate(
                        "Enter your BirthDate (yyyy-mm-dd) or type 'back': ",
                        "Invalid birthdate format.",
                        ref birthdate
                    );

                    if (next)
                    {
                        step++;
                    }
                    else
                    {
                        step--;
                    }
                }
                else if (step == 4)
                {
                    ShowHeader(step);

                    bool next = AskPhoneNumber(ref phonenumber);

                    if (next)
                    {
                        step++;
                    }
                    else
                    {
                        step--;
                    }
                }
                else if (step == 5)
                {
                    ShowHeader(step);

                    bool next = AskDate(
                        "Enter your Pregnancy start date (yyyy-mm-dd) or type 'back': ",
                        "Invalid start date.",
                        ref startdate
                    );

                    if (next)
                    {
                        step++;
                    }
                    else
                    {
                        step--;
                    }
                }
                else if (step == 6)
                {
                    ShowHeader(step);

                    bool next = AskNotes(ref notes);

                    if (next)
                    {
                        step++;
                    }
                    else
                    {
                        step--;
                    }
                }
                else if (step == 7)
                {
                    Console.Clear();

                    bool ok = userLogic.Register(
                        email,
                        password,
                        fullname,
                        birthdate.ToString("yyyy-MM-dd"),
                        phonenumber,
                        startdate.ToString("yyyy-MM-dd"),
                        notes
                    );

                    if (ok)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Account created! You can now log in.");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("An account with this email already exists.");
                    }

                    Console.ResetColor();

                    Console.WriteLine();
                    Console.WriteLine("Do you want to register another patient? (y/n)");
                    string? choice = Console.ReadLine();

                    if (choice?.ToLower() != "y")
                    {
                        running = false;
                    }

                    registrationFinished = true;
                }
            }
        }
    }
}