# Ziekenhuis Casus - Project B

Dit is onze Sprint 2 opdracht voor school. Het is een console applicatie voor een ziekenhuis waar zwangere vrouwen hun afspraken kunnen inzien en waar dokters, planners en een admin mee kunnen werken. De hele opdracht staat in [Ziekenhuis opdracht.md](Ziekenhuis%20opdracht.md).

## Wat kan het programma?

Er zijn 4 rollen die inloggen via hetzelfde login scherm. Per rol zie je een ander menu.

**Ouder (patient)**
- Inloggen of registreren als nieuwe gebruiker
- Afsprakenoverzicht bekijken, opgedeeld in komende afspraken en afspraken die al geweest zijn
- Details per afspraak bekijken (datum, tijd, type, status, kamer, locatie en welke specialist)

**Specialist (dokter)**
- Volgende afspraak bekijken
- Alle afspraken bekijken
- Dagagenda bekijken

**Planner**
- Kalender / agenda bekijken
- Nieuwe afspraken inplannen: patient kiezen, datum en tijd, kamer en specialist
- Kamerstatus bekijken (vrij of bezet op een bepaalde datum)

**Admin**
- Nieuwe dokters toevoegen (met specialisme)
- Planners toevoegen
- Kamers toevoegen

## Structuur van het project

We hebben de code opgedeeld in mapjes zodat het overzichtelijk blijft.

```
Program.cs          -> start het programma, roept Menu.Start() aan
Presentation/       -> alle console menus
Logic/              -> business logic
DataAccess/         -> SQL queries naar de database
DataModels/         -> de C# classes die een rij uit de database voorstellen
DataSources/        -> hier staat project.db (de SQLite database)
```

In [Presentation/](Presentation) staan onder andere [Menu.cs](Presentation/Menu.cs), `UserLogin.cs`, `UserRegister.cs`, `ParentsMenu.cs`, `DoctorMenu.cs`, `PlannerMenu.cs` en `AdminMenu.cs`. Elk menu is een aparte class zodat we het makkelijk uit elkaar kunnen houden.

In [DataAccess/](DataAccess) staan `UserAccess.cs`, `AppointmentAccess.cs`, `DoctorAccess.cs` en `RoomAccess.cs`. Deze classes praten met de database via Dapper. De models in [DataModels/](DataModels) zijn `UserModel.cs`, `AppointmentModel.cs` (soms ook ReservationModel genoemd) en `RoomModel.cs`.

## Technieken

- .NET 8.0 console applicatie (C# 12)
- SQLite als database, via `Microsoft.Data.Sqlite` versie 7.0.20
- `Dapper` 2.1.66 als lightweight ORM zodat we niet alles handmatig hoeven te mappen

## Database

De database is een SQLite bestand (`DataSources/project.db`). We hebben 5 tabellen:

- `User` - alle gebruikers in een tabel: patienten, dokters, planners en admins. Met o.a. email, password, naam, rol, specialty en telefoon
- `Reservation` - de afspraken. Gekoppeld aan een user, een room en een specialist. Met datum/tijd, type en status
- `Room` - ziekenhuiskamers met naam, type en locatie
- `InfoFolder` - informatiefolders die bij een afspraaktype horen
- `Notitie` - notities bij een afspraak

## Stukje code

Hieronder twee kleine stukjes uit de code om te laten zien hoe het ongeveer werkt.

**Login check** (uit [Logic/UserLogic.cs](Logic/UserLogic.cs)):

```csharp
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
```

Hier checken we of de ingevoerde email in de database staat en of het wachtwoord klopt. Als dat zo is slaan we het account op in `CurrentAccount` zodat we overal in het programma kunnen zien wie er ingelogd is.

**Nieuwe afspraak aanmaken** (uit [DataAccess/AppointmentAccess.cs](DataAccess/AppointmentAccess.cs)):

```csharp
public void CreateReservation(long userId, long roomId, long? specialistId, string dateTime, string type)
{
    string sql = @"INSERT INTO Reservation (User_ID, Room_ID, Specialist_ID, Date, Type, Status)
                   VALUES (@UserId, @RoomId, @SpecialistId, @Date, @Type, 'geplan')";
    _connection.Execute(sql, new { UserId = userId, RoomId = roomId, SpecialistId = specialistId, Date = dateTime, Type = type });
}
```

Dit wordt door de planner aangeroepen als die een nieuwe afspraak inplant. We geven de user, kamer, specialist, datum en type mee en de status staat standaard op `geplan`. Dapper vult de `@` parameters dan automatisch in.

## Hoe start je het programma?

Vanuit de root folder van het project:

```
dotnet run
```

Je kan het project ook gewoon openen in Visual Studio of VS Code en daar op run klikken. De database staat al in `DataSources/project.db` dus je hoeft niks te seeden.

## Wat is nog niet af

Een paar dingen staan wel in het schema maar zijn nog niet helemaal klaar:

- `InfoFolder` tabel bestaat, maar er is nog geen UI voor. Je kan als ouder nog geen folders openen bij een afspraak.
- `Notitie` tabel staat in de database maar wordt nog nergens gebruikt. Dat willen we later toevoegen zodat dokters een notitie bij een afspraak kunnen zetten.
- Standaard afspraken zoals de 20-wekenecho hebben nog geen aparte flow. Nu moet de planner het gewoon handmatig inplannen als normale afspraak.

Verder werkt de basis wel: inloggen, afspraken bekijken, afspraken inplannen, kamers toevoegen en dokters toevoegen zijn allemaal getest.
