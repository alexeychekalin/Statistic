using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    class DBWalker
    {
        public static SqlConnection GetConnection(string server,
            string user,
            string password,
            string security
            //string database
            )
        {

            SqlConnection conn;
            try
            {
                conn = new SqlConnection(@"Data Source = " + server + @";"+ //Initial Catalog =" + database + @";" +
                                         @"Integrated Security = " + security + @"; User ID =" + user + @"; Password = " + password);
            }
            catch (Exception e)
            {
                MessageBox.Show(@"Не удалось подключиться к БД." + Environment.NewLine + e.Message);
                return null;
            }

            return conn;
        }
    }
}