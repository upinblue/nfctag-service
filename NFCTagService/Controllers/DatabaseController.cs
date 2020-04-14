using System;
using MySql.Data.MySqlClient;


namespace NFCTagService.Controllers
{
    public class DatabaseController
    {
        private MySqlConnection connection;
        private MySqlConnection connectionSK;
        public DatabaseController()
        {

            string connectionString = "Server=sql307.your-server.de;Database=factory;Uid=upinblb_8;Pwd=AYQG4UrVYQuSn98P";
            string connectionStringSK = "Server=sql435.your-server.de;Database=directory;Uid=usr_dir_uib;Pwd=0I3gc4MR9wj7DjAQ";

            connection = new MySqlConnection(connectionString);
            connection.Open();

            connectionSK = new MySqlConnection(connectionStringSK);
            connectionSK.Open();

        }

        public MySqlConnection getConnection()
        {
            return connection;
        }

        public bool saveTag(string type, string serial)
        {

            // check if tag is already in db
            // if not, save in chips
            // anyway save in history
            bool success = false;
            if (connection.State == System.Data.ConnectionState.Open)
            {
                string sql = "SELECT count(*) FROM `factory`.`Chips` WHERE  `SerialNumber` = '" + serial + "';";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader rdr = cmd.ExecuteReader();
                rdr.Read();
                string count = "";
                if (rdr.HasRows)
                    count = rdr[0].ToString();

                if (Int32.Parse(rdr[0].ToString()) > 0)
                {
                    rdr.Close();
                    cmd.Dispose();
                    return true;
                }

                rdr.Close();
                cmd.Dispose();

                string sql2 = "INSERT INTO `factory`.`Chips` (`Type`, `SerialNumber`) VALUES ('"+ type + "', '"+ serial +"');";
                MySqlCommand cmd2 = new MySqlCommand(sql2, connection);
                MySqlDataReader rdr2 = cmd2.ExecuteReader();

                rdr2.Close();
                cmd2.Dispose();

                success = true;
            }
            return success;
        }
        public bool addFlashHistory(string version, string serial, int userID)
        {
            bool success = false;
            if (connection.State == System.Data.ConnectionState.Open)
            {



                string sql1 = "SELECT  `ChipID` FROM `factory`.`Chips` WHERE  `SerialNumber` = '" + serial + "';";
                MySqlCommand cmd1 = new MySqlCommand(sql1, connection);
                MySqlDataReader rdr1 = cmd1.ExecuteReader();
                rdr1.Read();
                string id = "";
                if (rdr1.HasRows)
                    id = rdr1[0].ToString();

                rdr1.Close();
                cmd1.Dispose();


                string sql = "INSERT INTO `factory`.`FlashHistory` (`Date`, `ChipID`, `Value`, `UserID`) VALUES (NOW(), '" + id + "', '" + version + "', '" + userID + "');";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                MySqlDataReader rdr = cmd.ExecuteReader();

                rdr.Close();
                cmd.Dispose();
                success = true;
            }
            return success;
        }

        public int getUserIDBySessionToken(string token)
        {
            int id = -1;

            string sql = "SELECT user_id FROM directory.sessions WHERE token = '" + token + "';";
            MySqlCommand cmd = new MySqlCommand(sql, connectionSK);
            MySqlDataReader rdr = cmd.ExecuteReader();

            rdr.Read();
            string userID = "";
            if (rdr.HasRows)
                userID = rdr[0].ToString();

            rdr.Close();
            cmd.Dispose();

            try
            {
                id = Int32.Parse(userID);
            } catch
            {
                id = -1;
            }

            return id;
        }
    }
}
