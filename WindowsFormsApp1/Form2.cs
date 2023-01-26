using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsApp1.Properties;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        public Form2(string idfk)
        {
            InitializeComponent();
            
            var conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
            conn.Open();
            var sql = "select idfk from [MFU].[dbo].[Table_Form_Mould] group by [IdFK] order BY [IdFk] ";
            var command = new SqlCommand(sql, conn);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                comboBox1.Items.Add(reader["IdFk"].ToString());
            }

            reader.Close();
            conn.Close();
            comboBox1.SelectedIndex = comboBox1.Items.IndexOf(idfk);
        }

        struct pair
        {
            public DateTime dt;
            public double speed;

            public pair(DateTime dateTime, double v) : this()
            {
                this.dt = dateTime;
                this.speed = v;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
                tabControl1_SelectedIndexChanged(this, EventArgs.Empty);
            else
            {
                var sqlConnection = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
                try
                {
                    chart1.Series[0].Points.Clear();

                    sqlConnection.Open();

                    var sql = "select distinct number_Mould as Id_Mould  from [MFU].[dbo].[Table_Form_Mould] " +
                          "where idFK = @idFk and mould_type = @moldType  order by number_Mould";
                    var command = new SqlCommand(sql, sqlConnection);
                    command.Parameters.AddWithValue("@idFk", comboBox1.SelectedItem);
                    command.Parameters.AddWithValue("moldType", radioButton2.Checked ? 2 : 1);
                    var reader = command.ExecuteReader();
                    List<int> IdMoulds = new List<int>();
                    while (reader.Read())
                    {
                        IdMoulds.Add(Convert.ToInt32(reader["Id_Mould"]));
                    }
                    reader.Close();

                    foreach (var mold in IdMoulds)
                    {
                        SqlCommand dataAdapter = new SqlCommand(
                            // $" DROP TABLE #temp5;" +
                            // $" DROP TABLE #temp6;"+
                            $" SELECT" +
                            $"[Id_Mould]," +
                            $" [Mould_Action]," +
                            $" [Time_Action]," +
                            $" ROW_NUMBER() over(order by[time_action]) as num" +
                            $" into #temp5	  " +
                            $" FROM[MFU].[dbo].[Table_MouldAction]" +
                            $" Where idFK = @idFK and Mould_Action = 2 and mouldtype = @moldType and Id_Mould = @mold" +
                            $" order by Time_Action" +

                            $" SELECT" +
                            $" [Id_Mould], " +
                            $" [Mould_Action], " +
                            $" [Time_Action], " +
                            $" ROW_NUMBER() over(order by[time_action]) as num, " +
                            $" [idFK]" +
                            $" into #temp6" +
                            $" FROM[MFU].[dbo].[Table_MouldAction]" +
                            $" Where idFK = @idFK and Mould_Action = 1 and mouldtype = @moldType and Id_Mould = @mold" +
                            $" order by Time_Action" +

                            $" SELECT DATEDIFF(hh,#temp5.Time_Action,#temp6.Time_Action) as narabotka," +
                            $" #temp5.Time_Action as start," +
                            $" #temp6.Time_Action as stop" +
                            $"  FROM #temp6" +
                            $"  inner join #temp5 on #temp5.num= #temp6.num", sqlConnection);


                        dataAdapter.Parameters.AddWithValue("@idFK", comboBox1.SelectedItem);
                        dataAdapter.Parameters.AddWithValue("@moldType", radioButton2.Checked ? 2 : 1);
                        dataAdapter.Parameters.AddWithValue("@mold", mold);


                        reader = dataAdapter.ExecuteReader();


                        DateTime firstDate, secondDate;
                        double hours = 0;

                        while (reader.Read())
                        {
                            //TimeSpan diff = DBNull.Value.Equals(reader["stop"]) ? DateTime.Now - Convert.ToDateTime(reader["start"]) : Convert.ToDateTime(reader["stop"]) - Convert.ToDateTime(reader["start"]);
                            // hours += diff.TotalHours;
                            hours += Convert.ToDouble(reader["narabotka"]);
                        }
                        chart1.Series[0].Points.AddXY(mold, Convert.ToInt32(hours));
                        reader.Close();
                    }
                    // Настройка внешнего вида графика 
                    chart1.Series[0].IsValueShownAsLabel = true;
                    chart1.ChartAreas[0].AxisX.Interval = 1;
                }

                catch (Exception ex)
                {
                    MessageBox.Show(@"Ошибка построения графика. Описание ошибки: " + ex.Message);
                }
                finally
                {
                    sqlConnection.Close();
                }
            }

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
                if (tabControl1.SelectedIndex == 1)
                    comboBox1_SelectedIndexChanged(this, EventArgs.Empty);
                else
                    tabControl1_SelectedIndexChanged(this, EventArgs.Empty);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                if (comboBox1.SelectedItem != null)
                {
                    var sqlConnection = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password,
                        Resources.secure);

                    chart2.Series[0].Points.Clear();
                    try
                    {
                        sqlConnection.Open();

                        var sql = "select dt,speed from [CPS2].[dbo].[Line_1_speed] " +
                              "where idFK = " + comboBox1.SelectedItem;
                        List<pair> speedchange = new List<pair>();
                        var command = new SqlCommand(sql, sqlConnection);
                        var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            speedchange.Add(new pair(Convert.ToDateTime(reader["dt"]), Convert.ToDouble(reader["speed"])));
                        }
                        reader.Close();


                        sql = "select dt,speed from [CPS2].[dbo].[Line_2_speed] " +
                                  "where idFK = " + comboBox1.SelectedItem;
                        command = new SqlCommand(sql, sqlConnection);
                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            speedchange.Add(new pair(Convert.ToDateTime(reader["dt"]), Convert.ToDouble(reader["speed"])));
                        }
                        reader.Close();


                        sql = "select dt,speed from [CPS2].[dbo].[Line_3_speed] " +
                              "where idFK = " + comboBox1.SelectedItem;
                        command = new SqlCommand(sql, sqlConnection);
                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            speedchange.Add(new pair(Convert.ToDateTime(reader["dt"]), Convert.ToDouble(reader["speed"])));
                        }
                        reader.Close();

                        sql = "select dt,speed from [CPS3].[dbo].[Line_1_speed] " +
                             "where idFK = " + comboBox1.SelectedItem;
                        command = new SqlCommand(sql, sqlConnection);
                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            speedchange.Add(new pair(Convert.ToDateTime(reader["dt"]), Convert.ToDouble(reader["speed"])));
                        }
                        reader.Close();

                        sql = "select dt,speed from [CPS3].[dbo].[Line_2_speed] " +
                             "where idFK = " + comboBox1.SelectedItem;
                        command = new SqlCommand(sql, sqlConnection);
                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            speedchange.Add(new pair(Convert.ToDateTime(reader["dt"]), Convert.ToDouble(reader["speed"])));
                        }
                        reader.Close();

                        sql = "select dt,speed from [CPS3].[dbo].[Line_3_speed] " +
                             "where idFK = " + comboBox1.SelectedItem;
                        command = new SqlCommand(sql, sqlConnection);
                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            speedchange.Add(new pair(Convert.ToDateTime(reader["dt"]), Convert.ToDouble(reader["speed"])));
                        }
                        reader.Close();

                        sql = "select number_mould as Id_Mould from [MFU].[dbo].[Table_form_mould] " +
                              "where idFK = @idFk and mould_type = @mouldType order by number_mould";
                        command = new SqlCommand(sql, sqlConnection);
                        command.Parameters.AddWithValue("@idFk", comboBox1.Text);
                        command.Parameters.AddWithValue("mouldType", radioButton2.Checked ? 2 : 1);
                        reader = command.ExecuteReader();
                        List<int> IdMoulds = new List<int>();
                        while (reader.Read())
                        {
                            IdMoulds.Add(Convert.ToInt32(reader["Id_Mould"]));
                        }

                        // speedchange.Sort();

                        reader.Close();

                        foreach (var mold in IdMoulds)
                        {
                            SqlCommand dataAdapter = new SqlCommand(
                            // $" DROP TABLE #temp5;" +
                            // $" DROP TABLE #temp6;"+
                            $" SELECT" +
                            $"[Id_Mould]," +
                            $" [Mould_Action]," +
                            $" [Time_Action]," +
                            $" ROW_NUMBER() over(order by[Time_Action]) as num" +
                            $" into #temp5	  " +
                            $" FROM[MFU].[dbo].[Table_MouldAction]" +
                            $" Where idFK = @idFK and Mould_Action = 1 and mouldtype = @moldType and Id_Mould = " + mold + "ORDER BY Time_Action"+

                            $" SELECT" +
                            $" [Id_Mould], " +
                            $" [Mould_Action], " +
                            $" [Time_Action], " +
                            $" ROW_NUMBER() over(order by[Time_Action]) as num, " +
                            $" [idFK]" +
                            $" into #temp6" +
                            $" FROM[MFU].[dbo].[Table_MouldAction]" +
                            $" Where idFK = @idFK and Mould_Action = 2 and mouldtype =  @moldType and Id_Mould = " + mold + "ORDER BY Time_Action"+

                            $" SELECT" +
                            $" #temp6.Id_Mould," +
                            $" #temp6.Time_Action as start, " +
                            $" #temp5.Time_Action as stop" +
                            $" FROM #temp6" +
                            $" join #temp5 on #temp6.num = #temp5.num", sqlConnection);

                            dataAdapter.Parameters.AddWithValue("@idFK", comboBox1.SelectedItem);
                            dataAdapter.Parameters.AddWithValue("@moldType", radioButton2.Checked ? 2 : 1);
                            reader = dataAdapter.ExecuteReader();

                            double res = 0;
                            speedchange.Sort(comparator);
                            while (reader.Read())
                            {
                                if (speedchange.Count() == 0)
                                    speedchange.Add(new pair(Convert.ToDateTime(reader["start"]), 0.0));
                            
                                DateTime curr = Convert.ToDateTime(reader["start"]);
                                int j = 0;
                                while (speedchange[j].dt < curr)
                                {
                                    j++;
                                    if (j == speedchange.Count())
                                    {

                                        break;
                                    }
                                }

                                while (j < speedchange.Count() && speedchange[j].dt < Convert.ToDateTime(reader["stop"]))
                                {
                                    var z = (speedchange[j].dt - curr).Duration();
                                    res += (z.Days * 24 * 60 + z.Hours * 60 + z.Minutes + Convert.ToDouble(z.Seconds) / 60.0) * (speedchange[j].speed/10.0);
                                    curr = speedchange[j].dt;
                                    j++;
                                }
                                var t = (Convert.ToDateTime(reader["stop"]) - curr).Duration();
                                res += (t.Days * 24 * 60 + t.Hours * 60 + t.Minutes + Convert.ToDouble(t.Seconds) / 60.0) *
                                    (j > 0 ? (speedchange[j - 1].speed / 10.0) : 0);


                            }
                            chart2.Series[0].Points.AddXY(mold, Convert.ToInt32(res));
                            reader.Close();
                        }

                        // Настройка внешнего вида графика 
                        chart2.Series[0].IsValueShownAsLabel = true;
                        chart2.ChartAreas[0].AxisX.Interval = 1;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(@"Ошибка построения графика КАПЛЯ. Описание ошибки: " + ex.Message);
                    }
                    finally
                    {
                        sqlConnection.Close();
                    }
                }
            }
            else
            {
                if (comboBox1.SelectedItem != null)
                    comboBox1_SelectedIndexChanged(this, EventArgs.Empty);
            }
        }
        int comparator(pair p1, pair p2)
        {
            if (p1.dt < p2.dt)
                return -1;
            return 1;
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
