
using System;
using System.Collections.Generic;
using MySqlConnector;

namespace WeatherAlertSystem
{
    public static class Database
    {
        private const string ConnectionString =
            "Server=localhost;Port=3306;Database=weatheralerts;User ID=root;Password=;";

        public static void Initialize()
        {
            using var connection =
                new MySqlConnection(ConnectionString);

            connection.Open();

            string sql = @"
                CREATE TABLE IF NOT EXISTS Subscribers (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    Name VARCHAR(100) NOT NULL,
                    Phone VARCHAR(30) NOT NULL,
                    Area VARCHAR(150) NOT NULL,
                    ConsentGiven TINYINT(1) NOT NULL DEFAULT 0,
                    CreatedAt DATETIME NOT NULL
                );

                CREATE TABLE IF NOT EXISTS SmsLogs (
                    Id INT AUTO_INCREMENT PRIMARY KEY,
                    Recipient VARCHAR(30) NOT NULL,
                    Message TEXT NOT NULL,
                    Status VARCHAR(50) NOT NULL,
                    CreatedAt DATETIME NOT NULL
                );";

            using var command =
                new MySqlCommand(sql, connection);

            command.ExecuteNonQuery();
        }

        public static void AddSubscriber(
            string name, string phone, string area)
        {
            using var connection =
                new MySqlConnection(ConnectionString);

            connection.Open();

            string sql = @"
                INSERT INTO Subscribers
                (Name, Phone, Area, ConsentGiven, CreatedAt)
                VALUES (@name, @phone, @area, 1, @createdAt);";

            using var command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@phone", phone);
            command.Parameters.AddWithValue("@area", area);
            command.Parameters.AddWithValue(
                "@createdAt", DateTime.Now);

            command.ExecuteNonQuery();
        }

        public static List<(string Name, string Phone, string Area)>
            GetSubscribers()
        {
            var subscribers =
                new List<(string Name, string Phone, string Area)>();

            using var connection =
                new MySqlConnection(ConnectionString);

            connection.Open();

            string sql =
                "SELECT Name, Phone, Area FROM Subscribers ORDER BY Id DESC;";

            using var command =
                new MySqlCommand(sql, connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                subscribers.Add((
                    reader.GetString("Name"),
                    reader.GetString("Phone"),
                    reader.GetString("Area")
                ));
            }

            return subscribers;
        }

        public static void AddSmsLog(
            string recipient, string message, string status)
        {
            using var connection =
                new MySqlConnection(ConnectionString);

            connection.Open();

            string sql = @"
                INSERT INTO SmsLogs
                (Recipient, Message, Status, CreatedAt)
                VALUES (@recipient, @message, @status, @createdAt);";

            using var command =
                new MySqlCommand(sql, connection);

            command.Parameters.AddWithValue("@recipient", recipient);
            command.Parameters.AddWithValue("@message", message);
            command.Parameters.AddWithValue("@status", status);
            command.Parameters.AddWithValue(
                "@createdAt", DateTime.Now);

            command.ExecuteNonQuery();
        }

        public static List<(
            string Recipient,
            string Message,
            string Status,
            string CreatedAt)> GetSmsLogs()
        {
            var logs = new List<(
                string Recipient,
                string Message,
                string Status,
                string CreatedAt)>();

            using var connection =
                new MySqlConnection(ConnectionString);

            connection.Open();

            string sql = @"
                SELECT Recipient, Message, Status, CreatedAt
                FROM SmsLogs ORDER BY Id DESC;";

            using var command =
                new MySqlCommand(sql, connection);

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                logs.Add((
                    reader.GetString("Recipient"),
                    reader.GetString("Message"),
                    reader.GetString("Status"),
                    reader.GetDateTime("CreatedAt")
                        .ToString("yyyy-MM-dd HH:mm:ss")
                ));
            }

            return logs;
        }
    }
}
