using System;
using MySql.Data.MySqlClient;

namespace CyberBotSA_part2
{
    public class DatabaseManager
    {
        private string connectionString = "Server=localhost;Database=cyberbotsa;Uid=root;Pwd=root1234;";

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }

        public bool TestConnection()
        {
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void ExecuteNonQuery(string query, Action<MySqlCommand> parameterAction = null)
        {
            using (MySqlConnection conn = GetConnection())
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                parameterAction?.Invoke(cmd);
                cmd.ExecuteNonQuery();
            }
        }

        public MySqlDataReader ExecuteReader(string query, MySqlConnection conn)
        {
            MySqlCommand cmd = new MySqlCommand(query, conn);
            return cmd.ExecuteReader();
        }
    }
}