using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace afung.MangaWeb3.Server
{
    public class Database
    {
        public static MySqlConnection GetConnection(string server, int port, string username, string password, string database)
        {
            string connectionString = String.Format(
                "Server={0};User={1};Database={2};Port={3};Password={4};CharSet=utf8;",
                server,
                username,
                database,
                port,
                password);

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}