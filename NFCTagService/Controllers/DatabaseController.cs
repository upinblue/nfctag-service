using System;
using MySql.Data.MySqlClient;


namespace NFCTagService.Controllers
{
    public class DatabaseController
    {
        private MySqlConnection connection;
        public DatabaseController()
        {

            string connectionString = "";

            connection = new MySqlConnection(connectionString);

            connection.Open();
        }

        public MySqlConnection getConnection()
        {
            return connection;
        }

        public bool saveTag(string type, string serial)
        {
            bool success = false;
            if (connection.State == System.Data.ConnectionState.Open)
            {
                string sql = "INSERT INTO `factory`.`Chips` (`Type`, `SerialNumber`) VALUES ('"+ type + "', '"+ serial +"');";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader rdr = cmd.ExecuteReader();

                rdr.Close();
                cmd.Dispose();

                success = true;
            }
            return success;
        }
        public bool addFlashHistory(string version)
        {
            bool success = false;
            if (connection.State == System.Data.ConnectionState.Open)
            {
                string sql = "INSERT INTO `factory`.`FlashHistory` (`Date`, `SWVersion`) VALUES (NOW(), '"+ version + "');";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader rdr = cmd.ExecuteReader();

                rdr.Close();
                cmd.Dispose();
                success = true;
            }
            return success;
        }
    }
}
