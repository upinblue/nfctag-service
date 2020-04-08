using System;
using MySql.Data.MySqlClient;


namespace NFCTagService.Controllers
{
    public class DatabaseController
    {
        private MySqlConnection connection;
        public DatabaseController()
        {

            

            connection = new MySqlConnection(connectionString);

            connection.Open();
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
        public bool addFlashHistory(string version, string serial)
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


                string sql = "INSERT INTO `factory`.`FlashHistory` (`Date`, `ChipID`, `SWVersion`) VALUES (NOW(), '" + id + "', '" + version + "');";
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
