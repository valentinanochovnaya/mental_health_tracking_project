using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.CompilerServices;
using Npgsql;
// string queryString = "INSERT INTO Note VALUES(1, NULL, 'title', 'description', '28.9.2015 05:50:00')";

string PATH_TO_FOLDER = "C:\\Users\\Ihor\\source\\repos\\data\\";

executeQueryForSelect(getAllDataQuery());
/*fillInDefaultUsersFromFile(PATH_TO_FOLDER);
fillInDefaultSpecialistsFromFile(PATH_TO_FOLDER);
fillInuserToSpecialistInterface(PATH_TO_FOLDER);
fillInNotifications(PATH_TO_FOLDER);
fillInFeedbacks(PATH_TO_FOLDER);
fillInNotes(PATH_TO_FOLDER);
fillInCertificates(PATH_TO_FOLDER);
fillInPhoto(PATH_TO_FOLDER);*/

static void fillInPhoto(string path_to_folder)
{
    string[] lines = System.IO.File.ReadAllLines(path_to_folder + "photos.txt");

    foreach (var line in lines)
    {
        string[] words = line.Split("\\(*)/");

        string id = words[0];
        string uri = words[1];
        string public_key = words[2];
        string note_id = words[3];

        string queryStr =
            $"INSERT INTO Photo VALUES({id}, {note_id}, '{uri}', '{public_key}');";

        executeQuery(queryStr);
    }
}

static void fillInCertificates(string path_to_folder)
{
    string[] lines = System.IO.File.ReadAllLines(path_to_folder + "certificates.txt");

    foreach (var line in lines)
    {
        string[] words = line.Split("\\(*)/");

        string id = words[1];
        string specialist_id = words[2];
        string uri = words[3];
        string public_key = words[4];

        string queryStr =
            $"INSERT INTO Certificate VALUES({id}, {specialist_id}, '{uri}', '{public_key}');";

        executeQuery(queryStr);
    }
}

static void fillInNotes(string path_to_folder)
{
    string[] lines = System.IO.File.ReadAllLines(path_to_folder + "notes.txt");

    Random rnd = new Random();
    foreach (var line in lines)
    {
        string[] words = line.Split("\\(*)/");

        string id = words[0];
        string user_id = words[1];
        string feedback_id = words[2];
        string title = words[3];
        string text = words[4];
        string datetime = words[5];
        string short_mood = rnd.Next(0, 5).ToString();

        string queryStr =
            $"INSERT INTO Note VALUES({id}, {user_id}, {feedback_id}, '{title}', '{text}', '{datetime}', {short_mood});";

        executeQuery(queryStr);

    }
}

static void fillInFeedbacks(string path_to_folder)
{
    string[] lines = System.IO.File.ReadAllLines(path_to_folder + "feedbacks.txt");

    foreach (var line in lines)
    {
        string[] words = line.Split("\\(*)/");

        string id = words[0];
        string text = words[1];

        string queryStr =
            $"INSERT INTO Feedback VALUES({id}, '{text}');";

        executeQuery(queryStr);
    }
}
static void fillInNotifications(string path_to_folder)
{
    string[] lines = System.IO.File.ReadAllLines(path_to_folder + "notifications.txt");

    foreach (var line in lines)
    {
        string[] words = line.Split("\\(*)/");

        string id = words[0];
        string title = words[1];
        string description = words[2];
        string default_user_id = words[3];
        string specialist_id = words[4];
        string queryStr =
            $"INSERT INTO Notification VALUES({id}, '{title}', '{description}', {default_user_id}, {specialist_id});";

        executeQuery(queryStr);
    }
}

static void fillInuserToSpecialistInterface(string path_to_folder)
{
    string[] lines = System.IO.File.ReadAllLines(path_to_folder + "userToSpecialistInterface.txt");
    foreach (var line in lines)
    {
        string[] words = Regex.Split(line, pattern: "\\\\\\(\\*\\)/");

        string id = words[0];
        string default_user_id = words[1];
        string specialist_id = words[2];
        string rating = words[3];
        string is_current_default_user_specialist = words[4];

        string queryStr = $"INSERT INTO DefaultUserToSpecialistInterface VALUES({id}, {default_user_id}, {specialist_id}," +
        $"{rating}, {is_current_default_user_specialist});";
        executeQuery(queryStr);
    }
}

static void fillInDefaultSpecialistsFromFile(string path_to_folder)
{
    string[] lines = System.IO.File.ReadAllLines(path_to_folder + "specialists.txt");

    foreach (var line in lines)
    {
        string[] words = line.Split("\\(*)/");

        string id = words[0];
        string first_name = words[1];
        string last_name = words[2];
        string email = words[3];
        string phone_number = words[4];

        string passw = words[5];

        string img_public_key = words[6];
        string img_uri = words[7];

        using var hmac = new HMACSHA512();
        var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(passw));
        var passwordSalt = hmac.Key;

        Console.WriteLine(img_uri);
        Console.WriteLine(Convert.ToBase64String(passwordSalt));

        string queryStr = $"INSERT INTO Specialist VALUES({id}, '{first_name}', '{last_name}', '{email}', '{phone_number}'," +
                          $"decode('{Convert.ToBase64String(passwordSalt)}', 'base64'), decode('{Convert.ToBase64String(passwordHash)}', 'base64'), '{img_public_key}', '{img_uri}')";
        executeQuery(queryStr);
    }
}
static void fillInDefaultUsersFromFile(string path_to_folder)
{
    string[] lines = System.IO.File.ReadAllLines(path_to_folder + "users.txt");

    foreach (var line in lines)
    {
        string[] words = Regex.Split(line, pattern: "\\\\\\(\\*\\)/");
        Console.WriteLine(words[0]);

        string id = words[0];
        string first_name = words[1];
        string last_name = words[2];
        string email = words[3];
        string phone_number = words[4];

        string passw = words[5];

        string img_public_key = words[6];
        string img_uri = words[7];

        using var hmac = new HMACSHA512();
        var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(passw));
        var passwordSalt = hmac.Key;

        Console.WriteLine(img_uri);
        Console.WriteLine(Convert.ToBase64String(passwordSalt));

        string queryStr = $"INSERT INTO DefaultUser VALUES({id}, '{first_name}', '{last_name}', '{email}', '{phone_number}'," +
        $"decode('{Convert.ToBase64String(passwordSalt)}', 'base64'), decode('{Convert.ToBase64String(passwordHash)}', 'base64'), '{img_public_key}', '{img_uri}')";
        executeQuery(queryStr);
    }
}

static string getCreationalTablesQueryString()
{
    string createTablesQueryString = "";
    createTablesQueryString += @"
        CREATE TABLE DefaultUser(
             default_user_id SERIAL NOT NULL PRIMARY KEY,
             first_name varchar(60) NOT NULL,
             last_name varchar(60) NOT NULL,
             email varchar(255) UNIQUE NOT NULL,
             phone_number varchar(15) UNIQUE NOT NULL,
             salt bytea UNIQUE NOT NULL,
             password_hash bytea UNIQUE NOT NULL,
             avatar_public_id varchar(255) UNIQUE,
             avatar_uri varchar(255) UNIQUE
        );";

    createTablesQueryString += @"
        CREATE TABLE Specialist(
         specialist_id SERIAL NOT NULL PRIMARY KEY,
         first_name varchar(60) NOT NULL,
         last_name varchar(60) NOT NULL,
         email varchar(255) UNIQUE NOT NULL,
         phone_number varchar(15) UNIQUE NOT NULL,
         salt bytea UNIQUE NOT NULL,
         password_hash bytea UNIQUE NOT NULL,
         avatar_public_id varchar(255) UNIQUE,
         avatar_uri varchar(255) UNIQUE
        );";

    createTablesQueryString += @"
        CREATE TABLE DefaultUserToSpecialistInterface(
         default_user_to_specialist_interface_id SERIAL NOT NULL PRIMARY KEY,
         default_user_id int NOT NULL REFERENCES DefaultUser(default_user_id),
         specialist_id int NOT NULL REFERENCES Specialist(specialist_id),
         rating int,
         is_current_default_user_specialist boolean NOT NULL DEFAULT false
        );";

    createTablesQueryString += @"
        CREATE TABLE Notification(
         notification_id SERIAL NOT NULL PRIMARY KEY,
         title text,
         description text, 
         sent_by_default_user_id int NOT NULL REFERENCES DefaultUser(default_user_id),
         sent_to_specialist_id int NOT NULL REFERENCES Specialist(specialist_id)
        );";

    createTablesQueryString += @"
        CREATE TABLE Certificate(
         certificate_id SERIAL NOT NULL PRIMARY KEY,
         specialist_id int NOT NULL REFERENCES Specialist(specialist_id),
         certificate_uri varchar(255) NOT NULL, 
         certificate_public_id varchar(255) NOT NULL
        );";

    createTablesQueryString += @"
    CREATE TABLE Feedback (
       feedback_id SERIAL NOT NULL PRIMARY KEY,
       description text NOT NULL
    );";

    createTablesQueryString += @"
        CREATE TABLE Note(
         note_id SERIAL NOT NULL PRIMARY KEY,
         default_user_id int NOT NULL REFERENCES DefaultUser(default_user_id),
         feedback_id int NOT NULL UNIQUE REFERENCES Feedback(feedback_id),
         title text NOT NULL,
         description text NOT NULL,
         note_datetime timestamp NOT NULL,
         short_mood int
        );";

    createTablesQueryString += @"
        CREATE TABLE Photo(
         photo_id SERIAL NOT NULL PRIMARY KEY,
         note_id int NOT NULL REFERENCES Note(note_id),
         photo_uri varchar(255) NOT NULL UNIQUE,
         photo_public_id varchar(255) NOT NULL UNIQUE
        );";

    return createTablesQueryString;
}

static List<string> getAllDataQuery()
{
    var queryList = new List<string>()
    {
        @"SELECT * FROM public.certificate 
            ORDER BY certificate_id ASC; ",
        @"SELECT * FROM public.certificate 
            ORDER BY certificate_id ASC; ",
        @"SELECT * FROM public.defaultuser 
            ORDER BY default_user_id ASC; ",
        @"SELECT * FROM public.defaultusertospecialistinterface
            ORDER BY default_user_to_specialist_interface_id ASC;",
        @"SELECT * FROM public.feedback
            ORDER BY feedback_id ASC; ",
        @"SELECT * FROM public.note
            ORDER BY note_id ASC; ",
        @"SELECT * FROM public.notification
            ORDER BY notification_id ASC; ",
        @"SELECT * FROM public.photo
            ORDER BY photo_id ASC; ",
        @"SELECT * FROM public.specialist
            ORDER BY specialist_id ASC; "
    };
    return queryList;
}

static Tuple<string, string> ReadUserIdAndPassword()
{
    var path = "C:\\Users\\Ihor\\source\\repos\\data\\creds.txt";
    string[] lines = System.IO.File.ReadAllLines(path);
    string id = "";
    string pass = "";
    foreach (var line in lines)
    {
        string[] words = line.Split(' ');
        id = words[0];
        pass = words[1];
    }


    return new Tuple<string, string>(id, pass);
}

static void executeQueryForSelect(List<string> queryList)
{
    var creds = ReadUserIdAndPassword();
    var connectionString = $"Server=redservo.duckdns.org;Port=5432;Database=postgres;User Id={creds.Item1};Password={creds.Item2};";
    using var conn = new NpgsqlConnection(connectionString);

    conn.Open();
    if (conn != null)
    {
        foreach (var query in queryList)
        {
            using (NpgsqlConnection connection =
               new NpgsqlConnection(connectionString))
            {
                // Create the Command and Parameter objects.
                NpgsqlCommand command = new NpgsqlCommand(query, connection);
                // command.Parameters.AddWithValue("@pricePoint", paramValue);

                // Open the connection in a try/catch block.
                // Create and execute the DataReader, writing the result
                // set to the console window.
                try
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        Console.WriteLine("-------------------------------");
                        while (reader.Read())
                        {
                            Object[] values = new Object[reader.FieldCount];
                            int fieldCount = reader.GetValues(values);
                           
                            for (int i = 0; i < fieldCount; i++)
                            {
                                Console.Write(values[i] + " ");
                            }
                            Console.WriteLine(" ");
                        }
                        reader.Close();
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(query);
                }

                //Console.ReadLine();
            }
        }

        // conn.Open();
        Console.WriteLine(conn.FullState);
        Console.WriteLine("Success");
        conn.Close();
    }
}

static void executeQuery(string queryString)
{
    var creds = ReadUserIdAndPassword();
    var connectionString = $"Server=redservo.duckdns.org;Port=5432;Database=postgres;User Id={creds.Item1};Password={creds.Item2};";
    using var conn = new NpgsqlConnection(connectionString);

    conn.Open();
    if (conn != null)
    {
        using (NpgsqlConnection connection =
               new NpgsqlConnection(connectionString))
        {
            // Create the Command and Parameter objects.
            NpgsqlCommand command = new NpgsqlCommand(queryString, connection);
            // command.Parameters.AddWithValue("@pricePoint", paramValue);

            // Open the connection in a try/catch block.
            // Create and execute the DataReader, writing the result
            // set to the console window.
            try
            {
                connection.Open();
                NpgsqlDataReader reader = command.ExecuteReader();

                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(queryString);
            }

            //Console.ReadLine();
        }

        // conn.Open();
        Console.WriteLine(conn.FullState);
        Console.WriteLine("Success");
        conn.Close();
    }
}