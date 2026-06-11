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
            Console.Write("Voer uw e-mailadres in of typ 'back': ");
            string? input = Console.ReadLine();

            if (WantsBack(input)) return false;

            if (IsValidEmail(input ?? ""))
            {
                email = input!;
                return true;
            }

            ShowError("Ongeldig e-mailadres. Een geldig adres bevat '@' en '.' (bijv. naam@mail.com).");
        }
    }

    private static bool AskPassword(ref string password)
    {
        while (true)
        {
            Console.Write("Voer uw wachtwoord in of typ 'back' (minimaal 6 tekens, minstens 1 cijfer): ");
            string input = ReadMaskedPassword();

            if (WantsBack(input)) return false;

            if (IsValidPassword(input))
            {
                password = input;
                return true;
            }

            ShowError("Wachtwoord moet minimaal 6 tekens bevatten en minstens 1 cijfer.");
        }
    }

    private static bool AskFullName(ref string fullname)
    {
        while (true)
        {
            Console.Write("Voer uw volledige naam in of typ 'back': ");
            string? input = Console.ReadLine();

            if (WantsBack(input)) return false;

            if (!string.IsNullOrWhiteSpace(input))
            {
                fullname = input;
                return true;
            }

            ShowError("Naam mag niet leeg zijn.");
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
            Console.Write("Voer uw telefoonnummer in of typ 'back': ");
            string? input = Console.ReadLine();

            if (WantsBack(input)) return false;

            if (IsValidPhoneNumber(input ?? ""))
            {
                phonenumber = input!;
                return true;
            }

            ShowError("Ongeldig telefoonnummer. Gebruik alleen cijfers, spaties, '+' of '-'. Voorbeeld: 0612345678");
        }
    }

    private static bool AskNotes(ref string notes)
    {
        while (true)
        {
            Console.Write("Voer uw notities in of typ 'back': ");
            string? input = Console.ReadLine();

            if (WantsBack(input)) return false;

            if (IsValidNote(input ?? ""))
            {
                notes = input ?? "";
                return true;
            }

            ShowError("Notities mogen niet alleen uit cijfers bestaan. Beschrijf het in woorden.");
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
        Console.WriteLine("=== Patiëntenregistratie ===");
        Console.WriteLine();
        Console.WriteLine("Welkom bij de ziekenhuisregistratie.");
        Console.WriteLine("Vul uw gegevens in zodat het ziekenhuis uw zorg en afspraken kan voorbereiden.");
        Console.WriteLine();
        Console.WriteLine($"Stap {step + 1} van 7");
        Console.WriteLine("Typ 'back' of 'terug' om een stap terug te gaan.");
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
                        "Voer uw geboortedatum in (yyyy-mm-dd) of typ 'back': ",
                        "Ongeldig geboorteformat.",
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
                        "Voer uw zwangerschapsstartdatum in (yyyy-mm-dd) of typ 'back': ",
                        "Ongeldige startdatum.",
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
                        Console.WriteLine("Account aangemaakt! U kunt nu inloggen.");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Er bestaat al een account met dit e-mailadres.");
                    }

                    Console.ResetColor();

                    Console.WriteLine();
                    Console.WriteLine("Wilt u nog een patiënt registreren? (y/n)");
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