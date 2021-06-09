// public static class DatabaseProcessor
// {
//     public static void CreateDB()
//     {
//         connection.Open();
 
//         SqliteCommand command = connection.CreateCommand();
//         command.CommandText = 
//         @"
//             CREATE TABLE persons
//             (
//             personID    INTEGER,
//             fullName    TEXT,
//             address     TEXT
//             );
//         ";
//         command.Parameters.AddWithValue("$text", note.text);
//         command.Parameters.AddWithValue("$createdAt", note.createdAt.ToString("o"));
//         command.Parameters.AddWithValue("$userId", note.userId);
        
//         long newId = (long)command.ExecuteScalar();
        
//         connection.Close();
//         return newId;

//     }
// }