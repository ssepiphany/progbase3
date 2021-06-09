using Microsoft.Data.Sqlite;
using System.Collections.Generic;

public class AwardRepository
{
    private SqliteConnection connection;
    public AwardRepository(SqliteConnection connection)
    {
        this.connection = connection;
    }

    public string GetWinnerByYearAndNomination(int year, string nomination)
    {
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM awards WHERE Year = $year AND Award = $nomination";
        command.Parameters.AddWithValue("$year", year);
        command.Parameters.AddWithValue("$nomination", nomination);
        
        SqliteDataReader reader = command.ExecuteReader();
        string winner = "";
        
        if (reader.Read())
        {
            string winner = reader.GetString(4);
        }
        
        reader.Close();
        
        connection.Close();
        return winner;
    }

    public List<string> GetAllNominationsByYear(int year)
    {
        SqliteCommand command = connection.CreateCommand();
        command.CommandText = @"SELECT * FROM awards WHERE Year = $year";
        command.Parameters.AddWithValue("$year", year);
        
        SqliteDataReader reader = command.ExecuteReader();
        List<string> nominations = new List<string>();
        
        if (reader.Read())
        {
            string nomination = reader.GetString(2);
            nomination.Add(nomination);
        }
        
        reader.Close();
        
        connection.Close();
        return nominations;
    }
}