using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WindowsFormsApp1.Properties;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using WindowsFormsApp1.Properties;
namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Dictionary<int, string> filters1;
        Dictionary<int, string> filters2;
        public Form1()
        {
            InitializeComponent();
            label1.Paint += Label1_Paint;
            label4.Paint += Label4_Paint;
            filters1 = new Dictionary<int, string>();
            filters1.Add(0, "-1");
            filters1.Add(1, "-1");
            filters1.Add(2, "-1");
            filters1.Add(3, "-1");
            filters1.Add(4, "-1");
            filters1.Add(5, "-1");
            filters1.Add(6, "-1");
            filters1.Add(7, "-1");

            filters2 = new Dictionary<int, string>();
            filters2.Add(0, "-1");
            filters2.Add(1, "-1");
            filters2.Add(2, "-1");
            filters2.Add(3, "-1");
            filters2.Add(4, "-1");
            filters2.Add(5, "-1");
            filters2.Add(6, "-1");
            filters2.Add(7, "-1");
            var conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
            conn.Open();
            var sql = "select distinct idfk from [MFU].[dbo].[Table_Form_Mould] order by idfk";
            var command = new SqlCommand(sql, conn);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                comboBox3.Items.Add(reader["idfk"]);
            }
            reader.Close();
            conn.Close();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            Fill_On_Line();
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
        string d1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0).ToString("yyyy-MM-ddTHH:mm:ss") + ".000",
            d2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59).ToString("yyyy-MM-ddTHH:mm:ss") + ".000";

        void Fill_On_Line()
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            //if (comboBox2.Text == "" || comboBox1.Text == "")
            //    return;
            var conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
            conn.Open();
            Dictionary<int, int> blackposition = new Dictionary<int, int>();
            Dictionary<int, int> whitekposition = new Dictionary<int, int>();
            List<int> moulds = new List<int>();
            var sql = "select Number_Mould from [MFU].[dbo].Table_Form_Mould where Mould_Type=1 and idfk = " + comboBox2.Text.Split(' ')[1];
            var command = new SqlCommand(sql, conn);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                moulds.Add(Convert.ToInt32(reader["Number_Mould"]));
            }
            reader.Close();
            int countchist = 0;
            foreach (var m in moulds)
            {
                sql =
                   // "drop table if exists #temp5; " +
                   "if object_id ('tempdb..#temp5') is not null begin drop table #temp5 end;" +
                    " select id_mould, time_action, code_defect, code_repair ,place_defect, section, place ," +
                    " ROW_NUMBER() over(order by[time_action]) as num into #temp5 from" +
                    " [MFU].[dbo].Table_MouldAction where idFK = " + comboBox2.Text.Split(' ')[1] + " and mouldtype = 1 and Mould_Action = 1 and Id_Mould = " + m + ";" +
                    "if object_id ('tempdb..#temp6') is not null begin drop table #temp6 end;" +
                    //" drop table if exists #temp6;" +
                    " select id_mould, time_action," +
                    " ROW_NUMBER() over(order by[time_action]) as num into #temp6 from" +
                    " [MFU].[dbo].Table_MouldAction where idFK = " + comboBox2.Text.Split(' ')[1] + " and mouldtype = 1 and Mould_Action = 2 and Id_Mould = " + m + ";" +
                    " " +
                    " SELECT tt.Id_Mould, tt.Time_Action as start,#temp5.Time_Action as stop, #temp5.Code_defect," +
                    " #temp5.section, #temp5.place,  DATEDIFF(hh,tt.Time_Action,#temp5.Time_Action) as narabotka," +
                    "  [mfu].[dbo].group_concat(distinct t.Code_repair ) as repair" +
                    " FROM #temp6 tt join #temp5 on tt.num = #temp5.num" +
                    " left join [mfu].[dbo].Table_MouldAction t on t.Id_Mould = #temp5.Id_Mould where  t.Time_Action >= #temp5.Time_Action" +
                    " and t.mouldtype = 1 and t.idFK = " + comboBox2.Text.Split(' ')[1] +
                    " and (t.time_action < (select time_action from #temp6 where #temp6.num =tt.num+1)" +
                    " or(select time_action from #temp6 where #temp6.num =tt.num+1) is null )" +
                    " and #temp5.Time_Action between '" + d1 + "' and '" + d2 + "' " +
                    "group by tt.Id_Mould, tt.Time_Action ," +
                    " #temp5.Time_Action,#temp5.Code_defect,  #temp5.section, #temp5.place, DATEDIFF(hh,tt.Time_Action,#temp5.Time_Action)";

                int idfk = Convert.ToInt32(comboBox2.Text.Split(' ')[1]);
                command = new SqlCommand(sql, conn);
                reader = command.ExecuteReader();


                while (reader.Read())
                {
                    dataGridView1.Rows.Add(Convert.ToDateTime(reader["stop"])/*.ToString("dd.MM.yyyy HH.mm")*/, m,
                        reader["Code_defect"], reader["narabotka"], "",
                        reader["section"],
                        reader["place"].ToString() == "2" ? "Ближняя" : "Дальняя", "",
                        reader["repair"] == DBNull.Value ? "" :
                        reader["repair"].ToString().Substring(reader["repair"].ToString().LastIndexOf(',') + 1));
                    if (dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[8].Value.ToString() != "")
                    {
                        var conn1 = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
                        conn1.Open();

                        var sql1 = "select name from [MFU].[dbo].[Table_Employees] where id=  " +
                            "(select top 1 [employee_id] from [MFU].[dbo].[Table_MouldAction] where time_action <'" +
                            Convert.ToDateTime(reader["stop"]).ToString("yyyy-MM-ddTHH:mm:ss.000") + "' and Id_Mould= "
                            + m + " and mouldtype = 1  and mould_action in(7,10) and not code_repair is null and" +
                            " idfk = " + comboBox2.Text.Split(' ')[1] + " order by Time_Action desc )";
                        var command1 = new SqlCommand(sql1, conn1);
                        var worker = command1.ExecuteScalar();

                        sql1 = "select name from [MFU].[dbo].[Table_Employees] where id=" +
                            "(select top 1 [employee_id] from [MFU].[dbo].[Table_MouldAction] where [Code_repair] = "
                            + dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[8].Value + " and time_action >'" +
                            Convert.ToDateTime(reader["stop"]).ToString("yyyy-MM-ddTHH:mm:ss.000") + "' and Id_Mould= "
                            + m + " and mouldtype = 1  and mould_action in(7,10) and not code_repair is null and " +
                            " idfk = " + comboBox2.Text.Split(' ')[1] + "  order by Time_Action )";
                        command1 = new SqlCommand(sql1, conn1);
                        worker += "\\" + command1.ExecuteScalar();
                        conn1.Close();
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[7].Value = worker;
                    }
                    if (!blackposition.Keys.Contains(m))
                        blackposition.Add(m, countchist);
                    else
                        blackposition[m] = countchist;
                    countchist++;
                }
                reader.Close();
            }

            label2.Text = countchist.ToString();
            countchist = 0;
            foreach (var m in moulds)
            {
                sql =
                //"drop table if exists #temp5; " +
                       "if object_id ('tempdb..#temp5') is not null begin drop table #temp5 end;" +
                       " select id_mould, time_action, code_defect, code_repair ,place_defect, section, place ," +
                       " ROW_NUMBER() over(order by[time_action]) as num into #temp5 from" +
                       " [MFU].[dbo].Table_MouldAction where idFK = " + comboBox2.Text.Split(' ')[1] + " and mouldtype = 2 and Mould_Action = 1 and Id_Mould = " + m + ";" +

                         //" drop table if exists  # #temp6;" +
                         "if object_id ('tempdb..#temp6') is not null begin drop table #temp6 end;" +
                       " select id_mould, time_action," +
                       " ROW_NUMBER() over(order by[time_action]) as num into  #temp6 from" +
                       " [MFU].[dbo].Table_MouldAction where idFK = " + comboBox2.Text.Split(' ')[1] + " and mouldtype = 2 and Mould_Action = 2 and Id_Mould = " + m + ";" +
                       " " +
                       " SELECT tt.Id_Mould, tt.Time_Action as start,#temp5.Time_Action as stop, #temp5.Code_defect," +
                       " #temp5.section, #temp5.place,  DATEDIFF(hh,tt.Time_Action,#temp5.Time_Action) as narabotka," +
                       "  [mfu].[dbo].group_concat(distinct t.Code_repair ) as repair" +
                       " FROM  #temp6 tt join #temp5 on tt.num = #temp5.num" +
                       " left join [mfu].[dbo].Table_MouldAction t on t.Id_Mould = #temp5.Id_Mould where  t.Time_Action >= #temp5.Time_Action" +
                       " and t.mouldtype = 2 and t.idFK = " + comboBox2.Text.Split(' ')[1] +
                       " and (t.time_action < (select time_action from  #temp6 where  #temp6.num =tt.num+1)" +
                       " or(select time_action from  #temp6 where  #temp6.num =tt.num+1) is null )" +
                       " and #temp5.Time_Action between '" + d1 + "' and '" + d2 + "' " +
                       "group by tt.Id_Mould, tt.Time_Action ," +
                       " #temp5.Time_Action,#temp5.Code_defect,  #temp5.section, #temp5.place, DATEDIFF(hh,tt.Time_Action,#temp5.Time_Action)";

                command = new SqlCommand(sql, conn);
                reader = command.ExecuteReader();

                while (reader.Read())
                {

                    dataGridView2.Rows.Add(Convert.ToDateTime(reader["stop"]), m,
                       reader["Code_defect"], reader["narabotka"], "",
                       reader["section"],
                       reader["place"].ToString() == "2" ? "Ближняя" : "Дальняя", "",
                       reader["repair"] == DBNull.Value ? "" :
                       reader["repair"].ToString().Substring(reader["repair"].ToString().LastIndexOf(',') + 1));
                    if (dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[8].Value.ToString() != "")
                    {
                        var conn1 = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
                        conn1.Open();
                        var sql1 = "select name from [MFU].[dbo].[Table_Employees] where id=  " +
                           "(select top 1 [employee_id] from [MFU].[dbo].[Table_MouldAction] where time_action <'" +
                           Convert.ToDateTime(reader["stop"]).ToString("yyyy-MM-ddTHH:mm:ss.000") + "' and Id_Mould= "
                           + m + " and mouldtype = 2 and mould_action in(8,9) and not code_repair is null and " +
                           "idfk = " + comboBox2.Text.Split(' ')[1] + " order by Time_Action desc )";
                        var command1 = new SqlCommand(sql1, conn1);
                        var worker = command1.ExecuteScalar();

                        sql1 = "select name from [MFU].[dbo].[Table_Employees] where id=" +
                            "(select top 1 [employee_id] from [MFU].[dbo].[Table_MouldAction] where [Code_repair] = "
                            + dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[8].Value + " and time_action >'" +
                            Convert.ToDateTime(reader["stop"]).ToString("yyyy-MM-ddTHH:mm:ss.000") + "' and Id_Mould= "
                            + m + " and mouldtype = 2  and mould_action in(8,9) and not code_repair is null" +
                            " and idfk = " + comboBox2.Text.Split(' ')[1] + " order by Time_Action )";
                        command1 = new SqlCommand(sql1, conn1);
                        worker += "\\" + command1.ExecuteScalar();
                        conn1.Close();
                        dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[7].Value = worker;
                    }
                    if (!whitekposition.Keys.Contains(m))
                    {
                        whitekposition.Add(m, countchist);
                    }
                    else
                    {
                        whitekposition[m] = countchist;
                    }
                    countchist++;
                }
                reader.Close();
            }
            //label5.Text = "автомат №" + comboBox1.Text[comboBox1.Text.Length - 1] + " ФК №" + comboBox2.Text.Split(' ')[1] + "\n";
            //label5.Paint += Label5_Paint;
            label3.Text = countchist.ToString();


            List<pair> speedchange = new List<pair>();

            sql = "select dt,speed from [CPS2].[dbo].[Line_1_speed] " +
                              "where idFK = " + comboBox2.Text.Split(' ')[1];
            speedchange = new List<pair>();
            command = new SqlCommand(sql, conn);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                speedchange.Add(new pair(Convert.ToDateTime(reader["dt"]), Convert.ToDouble(reader["speed"])));
            }
            reader.Close();


            sql = "select dt,speed from [CPS2].[dbo].[Line_2_speed] " +
                      "where idFK = " + comboBox2.Text.Split(' ')[1];
            command = new SqlCommand(sql, conn);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                speedchange.Add(new pair(Convert.ToDateTime(reader["dt"]), Convert.ToDouble(reader["speed"])));
            }
            reader.Close();


            sql = "select dt,speed from [CPS2].[dbo].[Line_3_speed] " +
                  "where idFK = " + comboBox2.Text.Split(' ')[1];
            command = new SqlCommand(sql, conn);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                speedchange.Add(new pair(Convert.ToDateTime(reader["dt"]), Convert.ToDouble(reader["speed"])));
            }
            reader.Close();

            sql = "select dt,speed from [CPS3].[dbo].[Line_1_speed] " +
                 "where idFK = " + comboBox2.Text.Split(' ')[1];
            command = new SqlCommand(sql, conn);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                speedchange.Add(new pair(Convert.ToDateTime(reader["dt"]), Convert.ToDouble(reader["speed"])));
            }
            reader.Close();

            sql = "select dt,speed from [CPS3].[dbo].[Line_2_speed] " +
                 "where idFK = " + comboBox2.Text.Split(' ')[1];
            command = new SqlCommand(sql, conn);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                speedchange.Add(new pair(Convert.ToDateTime(reader["dt"]), Convert.ToDouble(reader["speed"])));
            }
            reader.Close();

            sql = "select dt,speed from [CPS3].[dbo].[Line_3_speed] " +
                 "where idFK = " + comboBox2.Text.Split(' ')[1];
            command = new SqlCommand(sql, conn);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                speedchange.Add(new pair(Convert.ToDateTime(reader["dt"]), Convert.ToDouble(reader["speed"])));
            }
            reader.Close();

            foreach (var mold in whitekposition.Keys)
            {
                command = new SqlCommand(
                // $" DROP TABLE #temp5;" +
                // $" DROP TABLE #temp6;"+
                $" SELECT" +
                $"[Id_Mould]," +
                $" [Mould_Action]," +
                $" [Time_Action]," +
                $" ROW_NUMBER() over(order by[time_action]) as num" +
                $" into #temp5	  " +
                $" FROM[MFU].[dbo].[Table_MouldAction]" +
                $" Where idFK = @idFK and Mould_Action = 1 and mouldtype = @moldType and Id_Mould = " + mold + " order by time_action" +


                $" SELECT" +
                $" [Id_Mould], " +
                $" [Mould_Action], " +
                $" [Time_Action], " +
                $" ROW_NUMBER() over(order by[time_action]) as num, " +
                $" [idFK]" +
                $" into #temp6" +
                $" FROM[MFU].[dbo].[Table_MouldAction]" +
                $" Where idFK = @idFK and Mould_Action = 2 and mouldtype =  @moldType and Id_Mould = " + mold + " order by time_action" +

                $" SELECT" +
                $" #temp6.Id_Mould," +
                $" #temp6.Time_Action as start, " +
                $" #temp5.Time_Action as stop" +
                $" FROM #temp6" +
                $" join #temp5 on #temp6.num = #temp5.num", conn);

                command.Parameters.AddWithValue("@idFK", comboBox2.Text.Split(' ')[1]);
                command.Parameters.AddWithValue("@moldType", 2);
                reader = command.ExecuteReader();

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
                        res += (z.Days * 24 * 60 + z.Hours * 60 + z.Minutes + Convert.ToDouble(z.Seconds) / 60.0) * (speedchange[j].speed / 10.0);
                        curr = speedchange[j].dt;
                        j++;
                    }
                    var t = (Convert.ToDateTime(reader["stop"]) - curr).Duration();
                    res += (t.Days * 24 * 60 + t.Hours * 60 + t.Minutes + Convert.ToDouble(t.Seconds) / 60.0) *
                        (j > 0 ? (speedchange[j - 1].speed / 10.0) : 0);

                }
                if (whitekposition.Keys.Contains(mold))
                    dataGridView2.Rows[whitekposition[mold]].Cells[4].Value = Convert.ToInt32(res);
                //chart2.Series[0].Points.AddXY(mold, Convert.ToInt32(res));
                reader.Close();
            }

            foreach (var mold in blackposition.Keys)
            {
                command = new SqlCommand(
                // $" DROP TABLE #temp5;" +
                // $" DROP TABLE #temp6;"+
                $" SELECT" +
                $"[Id_Mould]," +
                $" [Mould_Action]," +
                $" [Time_Action]," +
                $" ROW_NUMBER() over(order by[time_action]) as num" +
                $" into #temp5	  " +
                $" FROM[MFU].[dbo].[Table_MouldAction]" +
                $" Where idFK = @idFK and Mould_Action = 1 and mouldtype = @moldType and Id_Mould = " + mold + " order by time_action" +

                $" SELECT" +
                $" [Id_Mould], " +
                $" [Mould_Action], " +
                $" [Time_Action], " +
                $" ROW_NUMBER() over(order by[time_action]) as num, " +
                $" [idFK]" +
                $" into #temp6" +
                $" FROM[MFU].[dbo].[Table_MouldAction]" +
                $" Where idFK = @idFK and Mould_Action = 2 and mouldtype =  @moldType and Id_Mould = " + mold + " order by time_action" +

                $" SELECT" +
                $" #temp6.Id_Mould," +
                $" #temp6.Time_Action as start, " +
                $" #temp5.Time_Action as stop" +
                $" FROM #temp6" +
                $" join #temp5 on #temp6.num = #temp5.num", conn);

                command.Parameters.AddWithValue("@idFK", comboBox2.Text.Split(' ')[1]);
                command.Parameters.AddWithValue("@moldType", 1);
                reader = command.ExecuteReader();

                double res = 0;

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
                        res += (z.Days * 24 * 60 + z.Hours * 60 + z.Minutes + Convert.ToDouble(z.Seconds) / 60.0) * (speedchange[j].speed / 10.0);
                        curr = speedchange[j].dt;
                        j++;
                    }
                    var t = (Convert.ToDateTime(reader["stop"]) - curr).Duration();
                    res += (t.Days * 24 * 60 + t.Hours * 60 + t.Minutes + Convert.ToDouble(t.Seconds) / 60.0) *
                        (j > 0 ? (speedchange[j - 1].speed / 10.0) : 0);

                }
                if (blackposition.Keys.Contains(mold))
                    dataGridView1.Rows[blackposition[mold]].Cells[4].Value = Convert.ToInt32(res);
                //chart2.Series[0].Points.AddXY(mold, Convert.ToInt32(res));
                reader.Close();
            }

            conn.Close();
            //Сортировка по дате в таблице
            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
            dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Ascending);
        }

        int comparator(pair p1, pair p2)
        {
            if (p1.dt < p2.dt)
                return -1;
            return 1;
        }
       // " and  #temp5.Time_Action between '2022-09-07T00:00:00.000' and'2022-09-07T23:59:59.000' " +
        void Fill_Off_Line()
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            //if (comboBox2.Text == "" || comboBox1.Text == "")
            //    return;
            var conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
            conn.Open();
            Dictionary<int, int> blackposition = new Dictionary<int, int>();
            Dictionary<int, int> whitekposition = new Dictionary<int, int>();
            List<int> moulds = new List<int>();
            var sql = "select Number_Mould from [MFU].[dbo].Table_Form_Mould where Mould_Type=1 and idfk = " + comboBox3.Text;
            var command = new SqlCommand(sql, conn);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                moulds.Add(Convert.ToInt32(reader["Number_Mould"]));
            }
            reader.Close();
            int countchist = 0;
            foreach (var m in moulds)
            {
                sql =
                   // "drop table if exists #temp5; " +
                   "if object_id ('tempdb..#temp5') is not null begin drop table #temp5 end;" +
                    " select id_mould, time_action, code_defect, code_repair ,place_defect, section, place ," +
                    " ROW_NUMBER() over(order by[time_action]) as num into #temp5 from" +
                    " [MFU].[dbo].Table_MouldAction where idFK = " + comboBox3.Text + " and mouldtype = 1 and Mould_Action = 1 and Id_Mould = " + m + ";" +
                    "if object_id ('tempdb..#temp6') is not null begin drop table #temp6 end;" +
                    //" drop table if exists #temp6;" +
                    " select id_mould, time_action," +
                    " ROW_NUMBER() over(order by[time_action]) as num into #temp6 from" +
                    " [MFU].[dbo].Table_MouldAction where idFK = " + comboBox3.Text + " and mouldtype = 1 and Mould_Action = 2 and Id_Mould = " + m + ";" +
                    " " +
                    " SELECT tt.Id_Mould, tt.Time_Action as start,#temp5.Time_Action as stop, #temp5.Code_defect," +
                    " #temp5.section, #temp5.place,  DATEDIFF(hh,tt.Time_Action,#temp5.Time_Action) as narabotka," +
                    "  [mfu].[dbo].group_concat(distinct t.Code_repair ) as repair" +
                    " FROM #temp6 tt join #temp5 on tt.num = #temp5.num" +
                    " left join [mfu].[dbo].Table_MouldAction t on t.Id_Mould = #temp5.Id_Mould where  t.Time_Action >= #temp5.Time_Action" +
                    " and t.mouldtype = 1 and t.idFK = " + comboBox3.Text +
                    " and (t.time_action < (select time_action from #temp6 where #temp6.num =tt.num+1)" +
                    " or(select time_action from #temp6 where #temp6.num =tt.num+1) is null )" +
                    "group by tt.Id_Mould, tt.Time_Action ," +
                    " #temp5.Time_Action,#temp5.Code_defect,  #temp5.section, #temp5.place, DATEDIFF(hh,tt.Time_Action,#temp5.Time_Action)";

                int idfk = Convert.ToInt32(comboBox3.Text);
                command = new SqlCommand(sql, conn);
                reader = command.ExecuteReader();


                while (reader.Read())
                {

                    dataGridView1.Rows.Add(Convert.ToDateTime(reader["stop"])/*.ToString("dd.MM.yyyy HH.mm")*/, m,
                        reader["Code_defect"], reader["narabotka"], "",
                        reader["section"],
                        reader["place"].ToString() == "2" ? "Ближняя" : "Дальняя", "",
                        reader["repair"] == DBNull.Value ? "" :
                        reader["repair"].ToString().Substring(reader["repair"].ToString().LastIndexOf(',') + 1));
                    if (dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[8].Value != "")
                    {
                        var conn1 = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
                        conn1.Open();
                        var sql1 = "select name from [MFU].[dbo].[Table_Employees] where id=  " +
                          "(select top 1 [employee_id] from [MFU].[dbo].[Table_MouldAction] where time_action <'" +
                          Convert.ToDateTime(reader["stop"]).ToString("yyyy-MM-ddTHH:mm:ss.000") + "' and Id_Mould= "
                          + m + " and mouldtype = 1  and mould_action in(7,10) and not code_repair is null  and idfk = " + comboBox3.Text + "" +
                          " order by Time_Action desc )";
                        var command1 = new SqlCommand(sql1, conn1);
                        var worker = command1.ExecuteScalar();

                        sql1 = "select name from [MFU].[dbo].[Table_Employees] where id=" +
                            "(select top 1 [employee_id] from [MFU].[dbo].[Table_MouldAction] where [Code_repair] = "
                            + dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[8].Value + " and time_action >'" +
                            Convert.ToDateTime(reader["stop"]).ToString("yyyy-MM-ddTHH:mm:ss.000") + "' and Id_Mould= "
                            + m + " and mouldtype = 1  and mould_action in(7,10) and not code_repair is null " +
                            " and  idfk = " + comboBox3.Text + " order by Time_Action )";
                        command1 = new SqlCommand(sql1, conn1);
                        worker += "\\" + command1.ExecuteScalar();
                        conn1.Close();
                        dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[7].Value = worker;
                    }
                    if (!blackposition.Keys.Contains(m))
                        blackposition.Add(m, countchist);
                    else
                        blackposition[m] = countchist;
                    countchist++;
                }
                reader.Close();
            }

            label2.Text = countchist.ToString();
            countchist = 0;
            foreach (var m in moulds)
            {
                sql =
                //"drop table if exists #temp5; " +
                       "if object_id ('tempdb..#temp5') is not null begin drop table #temp5 end;" +
                       " select id_mould, time_action, code_defect, code_repair ,place_defect, section, place ," +
                       " ROW_NUMBER() over(order by[time_action]) as num into #temp5 from" +
                       " [MFU].[dbo].Table_MouldAction where idFK = " + comboBox3.Text + " and mouldtype = 2 and Mould_Action = 1 and Id_Mould = " + m + ";" +

                         //" drop table if exists  # #temp6;" +
                         "if object_id ('tempdb..#temp6') is not null begin drop table #temp6 end;" +
                       " select id_mould, time_action," +
                       " ROW_NUMBER() over(order by[time_action]) as num into  #temp6 from" +
                       " [MFU].[dbo].Table_MouldAction where idFK = " + comboBox3.Text + " and mouldtype = 2 and Mould_Action = 2 and Id_Mould = " + m + ";" +
                       " " +
                       " SELECT tt.Id_Mould, tt.Time_Action as start,#temp5.Time_Action as stop, #temp5.Code_defect," +
                       " #temp5.section, #temp5.place,  DATEDIFF(hh,tt.Time_Action,#temp5.Time_Action) as narabotka," +
                       "  [mfu].[dbo].group_concat(distinct t.Code_repair ) as repair" +
                       " FROM  #temp6 tt join #temp5 on tt.num = #temp5.num" +
                       " left join [mfu].[dbo].Table_MouldAction t on t.Id_Mould = #temp5.Id_Mould where  t.Time_Action >= #temp5.Time_Action" +
                       " and t.mouldtype = 2 and t.idFK = " + comboBox3.Text +
                       " and (t.time_action < (select time_action from  #temp6 where  #temp6.num =tt.num+1)" +
                       " or(select time_action from  #temp6 where  #temp6.num =tt.num+1) is null )" +
                       "group by tt.Id_Mould, tt.Time_Action ," +
                       " #temp5.Time_Action,#temp5.Code_defect,  #temp5.section, #temp5.place, DATEDIFF(hh,tt.Time_Action,#temp5.Time_Action)";

                command = new SqlCommand(sql, conn);
                reader = command.ExecuteReader();

                while (reader.Read())
                {

                    dataGridView2.Rows.Add(Convert.ToDateTime(reader["stop"]), m,
                       reader["Code_defect"], reader["narabotka"], "",
                       reader["section"],
                       reader["place"].ToString() == "2" ? "Ближняя" : "Дальняя", "",
                       reader["repair"] == DBNull.Value ? "" :
                       reader["repair"].ToString().Substring(reader["repair"].ToString().LastIndexOf(',') + 1));
                    if (dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[8].Value != "")
                    {
                        var conn1 = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
                        conn1.Open();
                        var sql1 = "select name from [MFU].[dbo].[Table_Employees] where id=  " +
                          "(select top 1 [employee_id] from [MFU].[dbo].[Table_MouldAction] where time_action <'" +
                          Convert.ToDateTime(reader["stop"]).ToString("yyyy-MM-ddTHH:mm:ss.000") + "' and Id_Mould= "
                          + m + " and mouldtype = 2  and mould_action in(8,9) and not code_repair is null " +
                          " and idfk = " + comboBox3.Text + " order by Time_Action desc )";
                        var command1 = new SqlCommand(sql1, conn1);
                        var worker = command1.ExecuteScalar();

                        sql1 = "select name from [MFU].[dbo].[Table_Employees] where id=" +
                            "(select top 1 [employee_id] from [MFU].[dbo].[Table_MouldAction] where [Code_repair] = "
                            + dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[8].Value + " and time_action >'" +
                            Convert.ToDateTime(reader["stop"]).ToString("yyyy-MM-ddTHH:mm:ss.000") + "' and Id_Mould= "
                            + m + " and mouldtype = 2  and mould_action in(8,9) and not code_repair is null" +
                            " and idfk = " + comboBox3.Text + " order by Time_Action )";
                        command1 = new SqlCommand(sql1, conn1);
                        worker += "\\" + command1.ExecuteScalar();
                        conn1.Close();
                        dataGridView2.Rows[dataGridView2.Rows.Count - 1].Cells[7].Value = worker;
                    }
                    if (!whitekposition.Keys.Contains(m))
                    {
                        whitekposition.Add(m, countchist);
                    }
                    else
                    {
                        whitekposition[m] = countchist;
                    }
                    countchist++;
                }
                reader.Close();
            }
            //label5.Text = "автомат №" + comboBox1.Text[comboBox1.Text.Length - 1] + " ФК №" + comboBox2.Text.Split(' ')[1] + "\n" + name_a;
            //label5.Paint += Label5_Paint;
            label3.Text = countchist.ToString();


            List<pair> speedchange = new List<pair>();

            sql = "select dt,speed from [CPS2].[dbo].[Line_1_speed] " +
                              "where idFK = " + comboBox3.Text;
            speedchange = new List<pair>();
            command = new SqlCommand(sql, conn);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                speedchange.Add(new pair(Convert.ToDateTime(reader["dt"]), Convert.ToDouble(reader["speed"])));
            }
            reader.Close();


            sql = "select dt,speed from [CPS2].[dbo].[Line_2_speed] " +
                      "where idFK = " + comboBox3.Text;
            command = new SqlCommand(sql, conn);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                speedchange.Add(new pair(Convert.ToDateTime(reader["dt"]), Convert.ToDouble(reader["speed"])));
            }
            reader.Close();


            sql = "select dt,speed from [CPS2].[dbo].[Line_3_speed] " +
                  "where idFK = " + comboBox3.Text;
            command = new SqlCommand(sql, conn);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                speedchange.Add(new pair(Convert.ToDateTime(reader["dt"]), Convert.ToDouble(reader["speed"])));
            }
            reader.Close();

            sql = "select dt,speed from [CPS3].[dbo].[Line_1_speed] " +
                 "where idFK = " + comboBox3.Text;
            command = new SqlCommand(sql, conn);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                speedchange.Add(new pair(Convert.ToDateTime(reader["dt"]), Convert.ToDouble(reader["speed"])));
            }
            reader.Close();

            sql = "select dt,speed from [CPS3].[dbo].[Line_2_speed] " +
                 "where idFK = " + comboBox3.Text;
            command = new SqlCommand(sql, conn);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                speedchange.Add(new pair(Convert.ToDateTime(reader["dt"]), Convert.ToDouble(reader["speed"])));
            }
            reader.Close();

            sql = "select dt,speed from [CPS3].[dbo].[Line_3_speed] " +
                 "where idFK = " + comboBox3.Text;
            command = new SqlCommand(sql, conn);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                speedchange.Add(new pair(Convert.ToDateTime(reader["dt"]), Convert.ToDouble(reader["speed"])));
            }
            reader.Close();

            foreach (var mold in whitekposition.Keys)
            {
                command = new SqlCommand(
                // $" DROP TABLE #temp5;" +
                // $" DROP TABLE #temp6;"+
                $" SELECT" +
                $"[Id_Mould]," +
                $" [Mould_Action]," +
                $" [Time_Action]," +
                $" ROW_NUMBER() over(order by[time_action]) as num" +
                $" into #temp5	  " +
                $" FROM[MFU].[dbo].[Table_MouldAction]" +
                $" Where idFK = @idFK and Mould_Action = 1 and mouldtype = @moldType and Id_Mould = " + mold + " order by time_action" +


                $" SELECT" +
                $" [Id_Mould], " +
                $" [Mould_Action], " +
                $" [Time_Action], " +
                $" ROW_NUMBER() over(order by[time_action]) as num, " +
                $" [idFK]" +
                $" into #temp6" +
                $" FROM[MFU].[dbo].[Table_MouldAction]" +
                $" Where idFK = @idFK and Mould_Action = 2 and mouldtype =  @moldType and Id_Mould = " + mold + " order by time_action" +

                $" SELECT" +
                $" #temp6.Id_Mould," +
                $" #temp6.Time_Action as start, " +
                $" #temp5.Time_Action as stop" +
                $" FROM #temp6" +
                $" join #temp5 on #temp6.num = #temp5.num", conn);

                command.Parameters.AddWithValue("@idFK", comboBox3.Text);
                command.Parameters.AddWithValue("@moldType", 2);
                reader = command.ExecuteReader();

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
                        res += (z.Days * 24 * 60 + z.Hours * 60 + z.Minutes + Convert.ToDouble(z.Seconds) / 60.0) * (speedchange[j].speed / 10.0);
                        curr = speedchange[j].dt;
                        j++;
                    }
                    var t = (Convert.ToDateTime(reader["stop"]) - curr).Duration();
                    res += (t.Days * 24 * 60 + t.Hours * 60 + t.Minutes + Convert.ToDouble(t.Seconds) / 60.0) *
                        (j > 0 ? (speedchange[j - 1].speed / 10.0) : 0);

                }
                if (whitekposition.Keys.Contains(mold))
                    dataGridView2.Rows[whitekposition[mold]].Cells[4].Value = Convert.ToInt32(res);
                //chart2.Series[0].Points.AddXY(mold, Convert.ToInt32(res));
                reader.Close();
            }

            foreach (var mold in blackposition.Keys)
            {
                command = new SqlCommand(
                // $" DROP TABLE #temp5;" +
                // $" DROP TABLE #temp6;"+
                $" SELECT" +
                $"[Id_Mould]," +
                $" [Mould_Action]," +
                $" [Time_Action]," +
                $" ROW_NUMBER() over(order by[time_action]) as num" +
                $" into #temp5	  " +
                $" FROM[MFU].[dbo].[Table_MouldAction]" +
                $" Where idFK = @idFK and Mould_Action = 1 and mouldtype = @moldType and Id_Mould = " + mold + " order by time_action" +

                $" SELECT" +
                $" [Id_Mould], " +
                $" [Mould_Action], " +
                $" [Time_Action], " +
                $" ROW_NUMBER() over(order by[time_action]) as num, " +
                $" [idFK]" +
                $" into #temp6" +
                $" FROM[MFU].[dbo].[Table_MouldAction]" +
                $" Where idFK = @idFK and Mould_Action = 2 and mouldtype =  @moldType and Id_Mould = " + mold + " order by time_action" +

                $" SELECT" +
                $" #temp6.Id_Mould," +
                $" #temp6.Time_Action as start, " +
                $" #temp5.Time_Action as stop" +
                $" FROM #temp6" +
                $" join #temp5 on #temp6.num = #temp5.num", conn);

                command.Parameters.AddWithValue("@idFK", comboBox3.Text);
                command.Parameters.AddWithValue("@moldType", 1);
                reader = command.ExecuteReader();

                double res = 0;

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
                        res += (z.Days * 24 * 60 + z.Hours * 60 + z.Minutes + Convert.ToDouble(z.Seconds) / 60.0) * (speedchange[j].speed / 10.0);
                        curr = speedchange[j].dt;
                        j++;
                    }
                    var t = (Convert.ToDateTime(reader["stop"]) - curr).Duration();
                    res += (t.Days * 24 * 60 + t.Hours * 60 + t.Minutes + Convert.ToDouble(t.Seconds) / 60.0) *
                        (j > 0 ? (speedchange[j - 1].speed / 10.0) : 0);

                }
                if (blackposition.Keys.Contains(mold))
                    dataGridView1.Rows[blackposition[mold]].Cells[4].Value = Convert.ToInt32(res);
                //chart2.Series[0].Points.AddXY(mold, Convert.ToInt32(res));
                reader.Close();
            }

            conn.Close();
            //Сортировка по дате в таблице
            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
            dataGridView2.Sort(dataGridView2.Columns[0], ListSortDirection.Ascending);
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox3.Text == "")
            {
                Fill_On_Line();
            }
            else
            {
                Fill_Off_Line();
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //comboBox3.Text = "";
            comboBox2.Items.Clear();
            comboBox2.Text = "";
            var conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
            conn.Open();
            var sql = "select n_line, idfk from [" + comboBox1.Text + "].[dbo].[Table_Assortiment] where not idfk is null";
            var command = new SqlCommand(sql, conn);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                comboBox2.Items.Add(reader["n_line"] + " " + reader["idfk"]);
            }
            reader.Close();
            conn.Close();
        }

        private void Label1_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.Clear(this.BackColor);
            //e.Graphics.RotateTransform(-90);
            //SizeF textSize = e.Graphics.MeasureString(label1.Text, label1.Font);
            ////label1.Width = (int)textSize.Height + 2;
            ////label1.Height = (int)textSize.Width + 2;
            //e.Graphics.TranslateTransform(-label1.Height / 2, label1.Width / 2);
            //e.Graphics.DrawString(label1.Text, label1.Font, Brushes.Black, -(textSize.Width / 2), -(textSize.Height / 2));
        }
        private void Label4_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.Clear(this.BackColor);
            //e.Graphics.RotateTransform(-90);
            //SizeF textSize = e.Graphics.MeasureString(label1.Text, label1.Font);
            ////label1.Width = (int)textSize.Height + 2;
            ////label1.Height = (int)textSize.Width + 2;
            //e.Graphics.TranslateTransform(-label4.Height / 2, label4.Width / 2);
            //e.Graphics.DrawString(label4.Text, label4.Font, Brushes.Black, -(textSize.Width / 2), -(textSize.Height / 2));
        }

        private void Label5_Paint(object sender, PaintEventArgs e)
        {
            //e.Graphics.Clear(this.BackColor);
            //e.Graphics.RotateTransform(-90);
            //SizeF textSize = e.Graphics.MeasureString(label5.Text, label5.Font);
            ////label1.Width = (int)textSize.Height + 2;
            ////label1.Height = (int)textSize.Width + 2;
            //e.Graphics.TranslateTransform(-label5.Height / 2, label5.Width / 2);
            //e.Graphics.DrawString(label5.Text, label5.Font, Brushes.Black, -(textSize.Width / 2), -(textSize.Height / 2));
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                dateTimePicker2.Enabled = true;
                d2 = dateTimePicker2.Value.ToString("yyyy-MM-dd") + "T23:59:59.000";
            }
            else
            {
                dateTimePicker2.Enabled = false;
                d2 = dateTimePicker1.Value.ToString("yyyy-MM-dd") + "T23:59:59.000";
            }

        }

        //Дата начала диапазона
        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker1.Value > Convert.ToDateTime(d2) && checkBox1.Checked)
            {
                dateTimePicker1.Value = Convert.ToDateTime(d1);
                return;
            }
            d1 = dateTimePicker1.Value.ToString("yyyy-MM-dd") + "T00:00:00.000";
            if(!checkBox1.Checked)
            {
                d2 = dateTimePicker1.Value.ToString("yyyy-MM-dd") + "T23:59:59.000";
            }
            //button1_Click(sender, e);
        }

        //Дата конца диапазона
        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            if (dateTimePicker2.Value < Convert.ToDateTime(d1))
            {
                dateTimePicker2.Value = Convert.ToDateTime(d2);
                return;
            }
            d2 = dateTimePicker2.Value.ToString("yyyy-MM-dd") + "T23:59:59.000";
           // button1_Click(sender, e);

        }
        int lastcol = -1;

        //Вкл\Выкл фильтр
        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && (e.ColumnIndex == 2 || e.ColumnIndex == 8))
            {
                lastcol = e.ColumnIndex;
                checkedListBox1.Items.Clear();
                checkedListBox1.Items.Add("показать все");
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    if (!checkedListBox1.Items.Contains(dataGridView1.Rows[i].Cells[e.ColumnIndex].Value))
                    {
                        if (dataGridView1.Rows[i].Visible)
                            checkedListBox1.Items.Add(dataGridView1.Rows[i].Cells[e.ColumnIndex].Value, true);
                        else
                        {
                                checkedListBox1.Items.Add(dataGridView1.Rows[i].Cells[e.ColumnIndex].Value, false);
                        }
                    }
                }
                if (e.ColumnIndex == 2)
                    checkedListBox1.Location = new System.Drawing.Point(200, 248);
                else
                    checkedListBox1.Location = new System.Drawing.Point(800, 248);
                
                checkedListBox1.Visible = true;
            }
            if (e.Button==MouseButtons.Left)
            { 
                lastcol = e.ColumnIndex;

                if (dataGridView1.Columns[lastcol].HeaderCell.Style.ForeColor != Color.Orange)
                {
                    dataGridView1.EnableHeadersVisualStyles = false;
                    filters1[e.ColumnIndex] = dataGridView1.SelectedCells[0].Value.ToString();
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        bool f = false;
                        foreach (var x in dataGridView1.Rows[i].Cells[lastcol].Value.ToString().Split(','))
                        {
                            if ((","+dataGridView1.SelectedCells[0].Value.ToString()+",").Contains("," + x + ",") && x != "" ||
                                x == "" && dataGridView1.SelectedCells[0].Value.ToString() == "")
                                f = true;
                        }
                        if (!f)
                        {
                            dataGridView1.Rows[i].Visible = false;
                        }
                        else if (dataGridView1.Rows[i].Visible)
                        {
                            dataGridView1.Rows[i].Visible = true;
                        }
                    }
                    dataGridView1.Columns[lastcol].HeaderCell.Style.ForeColor = Color.Orange;
                }
                else
                {
                    //dataGridView1.Columns[lastcol].HeaderCell.Style.BackColor = Color.FromArgb(System.Drawing.SystemColors.Window.ToArgb());
                    dataGridView1.Columns[lastcol].HeaderCell.Style.ForeColor = Color.White;
                    filters1[e.ColumnIndex] = "-1";
                    for (int i = 0; i < dataGridView1.RowCount; i++)
                    {
                        var f = false;
                        int colb = 0;
                        f = false;
                        for (int j = 0; j < dataGridView1.Rows[i].Cells.Count; j++)
                        {
                            if (dataGridView1.Rows[i].Cells[j].Value == null)
                                continue;
                            foreach (var x in dataGridView1.Rows[i].Cells[j].Value.ToString().Split(','))
                            {
                                
                                if (dataGridView1.Columns[j].HeaderCell.Style.ForeColor == Color.Orange)
                                {
                                    if (filters1[j].Contains(x) && x != "" ||  x == "" && filters1[j] == "")
                                        f = true;
                                    colb++;
                                }
                            }
                            //if (dataGridView1.Columns[j].HeaderCell.Style.ForeColor == Color.Blue &&
                            //    dataGridView1.Rows[i].Cells[j].Value.ToString() != filters1[j])
                            //{
                            //    f = false;
                            //}
                        }
                        dataGridView1.Rows[i].Visible = (f || colb == 0);
                    }
                }
            }
        }

       

        private void checkedListBox1_MouseLeave(object sender, EventArgs e)
        {
            checkedListBox1.Visible = false;
            if(checkedListBox1.CheckedItems.Contains("показать все"))
            {
                for (int i = 0; i < dataGridView1.RowCount; i++)
                {
                    dataGridView1.Rows[i].Visible = true;
                }
                return;
            }
            for(int i = 0; i<dataGridView1.RowCount; i++)
            {
                
                if (!checkedListBox1.CheckedItems.Contains(dataGridView1.Rows[i].Cells[lastcol].Value))
                {
                    dataGridView1.Rows[i].Visible = false;
                }
                else
                {
                    dataGridView1.Rows[i].Visible = true;
                }
            }
        }

        private void dataGridView2_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && (e.ColumnIndex == 2 || e.ColumnIndex == 8))
            {
                lastcol = e.ColumnIndex;
                checkedListBox2.Items.Clear();
                checkedListBox2.Items.Add("показать все");
                for (int i = 0; i < dataGridView2.RowCount; i++)
                {
                    if (!checkedListBox2.Items.Contains(dataGridView2.Rows[i].Cells[e.ColumnIndex].Value))
                    {
                        if (dataGridView2.Rows[i].Visible)
                            checkedListBox2.Items.Add(dataGridView2.Rows[i].Cells[e.ColumnIndex].Value, true);
                        else
                        {
                            checkedListBox2.Items.Add(dataGridView2.Rows[i].Cells[e.ColumnIndex].Value, false);
                        }
                    }
                }

                if (e.ColumnIndex == 2)
                    checkedListBox2.Location = new System.Drawing.Point(200, 650);
                else
                    checkedListBox2.Location = new System.Drawing.Point(800, 650);
                checkedListBox2.Visible=true;
                
            }
            else if (e.Button == MouseButtons.Left)
            {
                lastcol = e.ColumnIndex;

                if (dataGridView2.Columns[lastcol].HeaderCell.Style.ForeColor != Color.Orange)
                {
                    dataGridView2.EnableHeadersVisualStyles = false;
                    filters1[e.ColumnIndex] = dataGridView2.SelectedCells[0].Value.ToString();
                    for (int i = 0; i < dataGridView2.RowCount; i++)
                    {
                        bool f = false;
                        foreach (var x in dataGridView2.Rows[i].Cells[lastcol].Value.ToString().Split(','))
                        {
                            if (("," + dataGridView2.SelectedCells[0].Value.ToString() + ",").Contains("," + x + ",") && x != "" ||
                                x == "" && dataGridView2.SelectedCells[0].Value.ToString() == "")
                                f = true;
                        }
                        if (!f)
                        {
                            dataGridView2.Rows[i].Visible = false;
                        }
                        else if (dataGridView2.Rows[i].Visible)
                        {
                            dataGridView2.Rows[i].Visible = true;
                        }
                    }
                    dataGridView2.Columns[lastcol].HeaderCell.Style.ForeColor = Color.Orange;
                }
                else
                {
                    //dataGridView2.Columns[lastcol].HeaderCell.Style.BackColor = Color.FromArgb(System.Drawing.SystemColors.Window.ToArgb());
                    dataGridView2.Columns[lastcol].HeaderCell.Style.ForeColor = Color.White;
                    filters1[e.ColumnIndex] = "-1";
                    for (int i = 0; i < dataGridView2.RowCount; i++)
                    {
                        var f = false;
                        int colb = 0;
                        f = false;
                        for (int j = 0; j < dataGridView2.Rows[i].Cells.Count; j++)
                        {
                            if (dataGridView2.Rows[i].Cells[j].Value == null)
                                continue;

                            foreach (var x in dataGridView2.Rows[i].Cells[j].Value.ToString().Split(','))
                            {
                                if (dataGridView2.Columns[j].HeaderCell.Style.ForeColor == Color.Orange)
                                {
                                    if (filters1[j].Contains(x) && x != "" || x == "" && filters1[j] == "")
                                        f = true;
                                    colb++;
                                }
                            }
                            //if (dataGridView2.Columns[j].HeaderCell.Style.ForeColor == Color.Blue &&
                            //    dataGridView2.Rows[i].Cells[j].Value.ToString() != filters1[j])
                            //{
                            //    f = false;
                            //}
                        }
                        dataGridView2.Rows[i].Visible = (f || colb == 0);

                    }
                }

            }

        }

        private void checkedListBox2_MouseLeave(object sender, EventArgs e)
        {
            checkedListBox2.Visible = false;
            if (checkedListBox2.CheckedItems.Contains("показать все"))
            {
                for (int i = 0; i < dataGridView2.RowCount; i++)
                {
                    dataGridView2.Rows[i].Visible = true;
                }
                return;
            }
            for (int i = 0; i < dataGridView2.RowCount; i++)
            {

                if (!checkedListBox2.CheckedItems.Contains(dataGridView2.Rows[i].Cells[lastcol].Value))
                {
                    dataGridView2.Rows[i].Visible = false;
                }
                else
                {
                    dataGridView2.Rows[i].Visible = true;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            var excelapp = new Excel.Application();
            {
                if (excelapp == null)
                {
                    Console.WriteLine("Excel is not installed!!");
                    return;
                }
            }
            Workbook workbook = excelapp.Workbooks.Open(Directory.GetCurrentDirectory() + "\\Шаблон.xlsx");
            Worksheet w = workbook.Worksheets[1];
            int startpos = 7;
            //w.Range["A7"].Value = label5.Text;
            w.Range["A7"].Orientation = 90;
            w.Range["B7"].Value = "черновые формы";
            w.Range["B7"].Orientation = 90;
            w.Range["C7"].Value = label2.Text;
            w.Range["A2"].Value = "Отчет с " + dateTimePicker1.Value.ToString("dd.MM.yyyy") + " по " + dateTimePicker2.Value.ToString("dd.MM.yyyy");
            w.Range["A2"].HorizontalAlignment = Constants.xlCenter;
            w.Range["A2:K2"].MergeCells = true;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                if (!dataGridView1.Rows[i].Visible)
                    continue;
                w.Range["D" + startpos].Value = dataGridView1.Rows[i].Cells[1].Value;
                w.Range["E" + startpos].Value = dataGridView1.Rows[i].Cells[2].Value;
                w.Range["F" + startpos].Value = dataGridView1.Rows[i].Cells[3].Value;
                w.Range["G" + startpos].Value = dataGridView1.Rows[i].Cells[4].Value;
                w.Range["H" + startpos].Value = dataGridView1.Rows[i].Cells[5].Value;
                w.Range["I" + startpos].Value = dataGridView1.Rows[i].Cells[6].Value;
                w.Range["J" + startpos].Value = dataGridView1.Rows[i].Cells[7].Value;
                w.Range["K" + startpos].Value = dataGridView1.Rows[i].Cells[8].Value;
                startpos++;
            }
            w.Range["B7:B" + (startpos - 1)].MergeCells = true;
            w.Range["C7:C" + (startpos - 1)].MergeCells = true;
            w.Range["B" + startpos].Value = "чистовые\nформы";
            w.Range["C" + startpos].Value = label3.Text;
            int startpos2 = startpos;
            for (int i = 0; i < dataGridView2.Rows.Count; i++)
            {
                if (!dataGridView2.Rows[i].Visible)
                    continue;
                w.Range["D" + startpos].Value = dataGridView2.Rows[i].Cells[1].Value;
                w.Range["E" + startpos].Value = dataGridView2.Rows[i].Cells[2].Value;
                w.Range["F" + startpos].Value = dataGridView2.Rows[i].Cells[3].Value;
                w.Range["G" + startpos].Value = dataGridView2.Rows[i].Cells[4].Value;
                w.Range["H" + startpos].Value = dataGridView2.Rows[i].Cells[5].Value;
                w.Range["I" + startpos].Value = dataGridView2.Rows[i].Cells[6].Value;
                w.Range["J" + startpos].Value = dataGridView2.Rows[i].Cells[7].Value;
                w.Range["K" + startpos].Value = dataGridView2.Rows[i].Cells[7].Value;
                startpos++;
            }
            w.Range["B" + startpos2 + ":B" + (startpos - 1)].MergeCells = true;
            w.Range["C" + startpos2 + ":C" + (startpos - 1)].MergeCells = true;
            w.Range["A7:A" + (startpos - 1)].MergeCells = true;
            w.Range["A7"].Font.Size = 18;
            w.Range["A7"].Font.Bold = true;
            w.Range["B7"].Font.Size = 18;
            w.Range["B7"].Font.Bold = true;
            w.Range["C7"].Font.Size = 18;
            w.Range["C7"].Font.Bold = true;
            w.Range["B" + startpos2].Font.Size = 18;
            w.Range["B" + startpos2].Orientation = 90;
            w.Range["B" + startpos2].Font.Bold = true;
            w.Range["C" + startpos2].Font.Size = 18;
            w.Range["C" + startpos2].Font.Bold = true;
            w.get_Range("A6", "K" + (startpos - 1)).Cells.Borders.Weight = Excel.XlBorderWeight.xlThin;
            SaveFileDialog ofd = new SaveFileDialog();
            w.Range["A6", "K" + (startpos - 1)].VerticalAlignment = Constants.xlCenter;
            w.Range["A6", "K" + (startpos - 1)].HorizontalAlignment = Constants.xlCenter;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                workbook.SaveAs(ofd.FileName);
            }

            workbook.Close();
            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form2 newForm;
            if (comboBox2.Text!="")
                newForm = new Form2(comboBox2.Text.Split(' ')[1]);
            else
                newForm = new Form2(comboBox3.Text);

            newForm.ShowDialog();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //comboBox1.Text = "";
            comboBox2.Text = "";
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox3.Text = "";
        }

        //Пролистывание назад
        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0)
            {
                comboBox1.SelectedIndex = (comboBox1.SelectedIndex + 1) % 2;
                comboBox2.SelectedIndex = comboBox2.Items.Count-1;
            }
            else
            {
                comboBox2.SelectedIndex--;
            }
            button1_Click(sender, e);
        }

        //Пролистывание вперед
        private void button5_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == comboBox2.Items.Count - 1)
            {
                comboBox1.SelectedIndex = (comboBox1.SelectedIndex + 1) % 2;
                comboBox2.SelectedIndex = 0;
            }
            else
            {
                comboBox2.SelectedIndex++;
            }
            button1_Click(sender, e);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            var Rep = new DayReport();
            Rep.Show();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Form3 newForm;
            if (comboBox2.Text != "")
                newForm = new Form3(comboBox2.Text.Split(' ')[1]);
            else
                newForm = new Form3(comboBox3.Text);

            newForm.ShowDialog();
            //var f3 = new Form3();
            //f3.Show();
        }

        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == 2 && e.RowIndex >= 0)
            {
                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "")
                    return;

                Dictionary<int, string> defects = new Dictionary<int, string>();
                var conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
                conn.Open();
                var sql = "SELECT [Name_defect], code  FROM[OTK].[dbo].[Table_Defect] where code in ("
                    + dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value + ")";
                var command = new SqlCommand(sql, conn);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    defects.Add(Convert.ToInt32(reader["code"]), reader["Name_defect"].ToString());
                }
                reader.Close();
                conn.Close();
                string s = "";
                foreach (var x in dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Split(','))
                {
                    s += defects[Convert.ToInt32(x)] + "\n";
                }
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = s;

            }
            else if (e.ColumnIndex == 8 && e.RowIndex >= 0)
            {
                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "")
                    return;
                Dictionary<int, string> repair = new Dictionary<int, string>();
                var conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
                conn.Open();
                var sql = "SELECT  [IdRepair]      ,[Repair_Description]  FROM[MFU].[dbo].[Table_MouldRepair]  where IdRepair in ("
                    + dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value + ")";
                var command = new SqlCommand(sql, conn);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    repair.Add(Convert.ToInt32(reader["IdRepair"]), reader["Repair_Description"].ToString());
                }
                reader.Close();
                conn.Close();
                string s = "";
                if (repair.Count > 0)
                {
                    foreach (var x in dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Split(','))
                    {
                        s += repair[Convert.ToInt32(x)] + "\n";
                    }
                }

                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = s;
            }
        }

        private void dataGridView2_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2 && e.RowIndex >= 0)
            {
                if (dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "" || e.RowIndex>dataGridView1.Rows.Count-1)
                    return;
                Dictionary<int, string> defects = new Dictionary<int, string>();
                var conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
                conn.Open();
                var sql = "SELECT [Name_defect], code  FROM[OTK].[dbo].[Table_Defect] where code in ("
                    + dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value + ")";
                var command = new SqlCommand(sql, conn);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    if(!defects.Keys.Contains(Convert.ToInt32(reader["code"])))
                        defects.Add(Convert.ToInt32(reader["code"]), reader["Name_defect"].ToString());
                }
                reader.Close();
                conn.Close();
                string s = "";
                foreach (var x in dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Split(','))
                {
                    s += defects[Convert.ToInt32(x)] + "\n";
                }
                dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = s;

            }
            else if (e.ColumnIndex == 8 && e.RowIndex >= 0)
            {
                if (dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "")
                    return;
                Dictionary<int, string> repair = new Dictionary<int, string>();
                var conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
                conn.Open();
                var sql = "SELECT  [IdRepair]      ,[Repair_Description]  FROM[MFU].[dbo].[Table_MouldRepair]  where IdRepair in ("
                    + dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value + ")";
                var command = new SqlCommand(sql, conn);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    repair.Add(Convert.ToInt32(reader["IdRepair"]), reader["Repair_Description"].ToString());
                }
                reader.Close();
                conn.Close();
                string s = "";
                foreach (var x in dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().Split(','))
                {
                    s += repair[Convert.ToInt32(x)] + "\n";
                }
                dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = s;
            }
        }
    }
}
