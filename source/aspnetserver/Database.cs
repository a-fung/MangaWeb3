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

        private static MySqlConnection _connection = null;

        private static MySqlConnection DefaultConnection()
        {
            if (_connection == null)
            {
                _connection = GetConnection(
                    Config.MySQLServer,
                    Config.MySQLPort,
                    Config.MySQLUser,
                    Config.MySQLPassword,
                    Config.MySQLDatabase);
            }

            return _connection;
        }

        public static string Quote(string str)
        {
            return "'" + MySqlHelper.EscapeString(str) + "'";
        }

        public static Dictionary<string, object>[] Select(string table)
        {
            return Select(table, String.Empty, String.Empty, String.Empty, "*");
        }

        public static Dictionary<string, object>[] Select(string table, string where)
        {
            return Select(table, where, String.Empty, String.Empty, "*");
        }

        public static Dictionary<string, object>[] Select(string table, string where, string order)
        {
            return Select(table, where, order, String.Empty, "*");
        }

        public static Dictionary<string, object>[] Select(string table, string where, string order, string limit)
        {
            return Select(table, where, order, limit, "*");
        }

        public static Dictionary<string, object>[] Select(string table, string where, string order, string limit, string fields)
        {
            if (!String.IsNullOrWhiteSpace(where))
            {
                where = " WHERE " + where;
            }

            if (!String.IsNullOrWhiteSpace(order))
            {
                order = " ORDER BY " + order;
            }

            if (!String.IsNullOrWhiteSpace(limit))
            {
                limit = " LIMIT " + limit;
            }

            string sql = "SELECT " + fields + " FROM `" + table + "`" + where + order + limit;
            MySqlDataReader resultSet = new MySqlCommand(sql, DefaultConnection()).ExecuteReader();
            List<Dictionary<string, object>> rtn = new List<Dictionary<string, object>>();

            while (resultSet.Read())
            {
                Dictionary<string, object> row = new Dictionary<string, object>();
                for (int j = 0; j < resultSet.FieldCount; j++)
                {
                    row[resultSet.GetName(j)] = resultSet.GetValue(j);
                }
                rtn.Add(row);
            }

            resultSet.Close();
            return rtn.ToArray();
        }

        private static void InsertOrReplace(string method, string table, Dictionary<string, object> data)
        {
            string field = String.Empty;
            string value = String.Empty;

            foreach (string key in data.Keys)
            {
                field += ",`" + key + "`";
                value += "," + Quote(data[key].ToString());
            }

            if (field == "" || value == "")
            {
                return;
            }

            field = field.Substring(1);
            value = value.Substring(1);

            string sql = method + " INTO `" + table + "` ( " + field + " ) VALUES ( " + value + " )";
            new MySqlCommand(sql, DefaultConnection()).ExecuteScalar();
        }

        public static void Insert(string table, Dictionary<string, object> data)
        {
            InsertOrReplace("INSERT", table, data);
        }

        public static void Replace(string table, Dictionary<string, object> data)
        {
            InsertOrReplace("REPLACE", table, data);
        }

        public static void Update(string table, Dictionary<string, object> data, string where)
        {
            Update(table, data, where, String.Empty);
        }

        public static void Update(string table, Dictionary<string, object> data, string where, string limit)
        {
            string clause = String.Empty;
            foreach (string key in data.Keys)
            {
                clause += ",`" + key + "`=" + Quote(data[key].ToString());
            }

            if (clause == "")
            {
                return;
            }

            clause = clause.Substring(1);

            if (!String.IsNullOrWhiteSpace(limit))
            {
                limit = " LIMIT " + limit;
            }

            string sql = "UPDATE `" + table + "` SET " + clause + " WHERE " + where + limit;
            new MySqlCommand(sql, DefaultConnection()).ExecuteScalar();
        }

        public static void Clear(string table)
        {
            string sql = "TRUNCATE TABLE `" + table + "`";
            new MySqlCommand(sql, DefaultConnection()).ExecuteScalar();
        }

        public static void Delete(string table, string where)
        {
            string sql = "DELETE FROM `" + table + "` WHERE " + where;
            new MySqlCommand(sql, DefaultConnection()).ExecuteScalar();
        }

        public static void ExecuteSql(string sql)
        {
            new MySqlCommand(sql, DefaultConnection()).ExecuteScalar();
        }

        public static int LastInsertId()
        {
            return Convert.ToInt32(new MySqlCommand("SELECT LAST_INSERT_ID()", DefaultConnection()).ExecuteScalar());
        }
    }
}