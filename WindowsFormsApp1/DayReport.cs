using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WindowsFormsApp1.Properties;

namespace WindowsFormsApp1
{
    public partial class DayReport : Form
    {
        public DayReport()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        void fill_datagrid(DataGridView dgv, string Line, string Fk, Label label)
        {
            //черновые
            var conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
            conn.Open();
            DateTime d1 = DateTime.Now;
            //DateTime d1 = new DateTime(2022, 08, 15, 21, 32, 22);
            d1=d1.AddHours(-24);
            d1=d1.AddMinutes(-d1.Minute);
            d1=d1.AddSeconds(-d1.Second);
            d1=d1.AddMilliseconds(-d1.Millisecond);
            if(d1.Hour>=8 && d1.Hour < 20)
            {
               d1= d1.AddHours(8 - d1.Hour);
            }
            else
            {
                d1= d1.AddHours(20 - d1.Hour);
            }

            DateTime d2 = d1;
            d2 = d1.AddHours(12);

            var sql =
                "select count(*) from [mfu].[dbo].Table_MouldAction where Mould_Action = 1 and Time_Action" +
                " between '" + d1.ToString("yyyy-MM-ddTHH:mm:ss.000") + "' and '" + d2.ToString("yyyy-MM-ddTHH:mm:ss.000")
                + "' and (idFK in(select top 1  num_m from  [otk].[dbo].Link_" + comboBox1.Text + " where line = " + Line+
                " and DT_out< '"+d2.ToString("yyyy-MM-ddTHH:mm:ss.000")+"' order by DT_out desc) or idfk = "+ Fk + " ) " +
                " and mouldtype = 1";
            var command = new SqlCommand(sql, conn);
            var t = command.ExecuteScalar();
            dgv.Rows.Add("Черновая форма",t,"","","");
            d1 = d2;
            d2 = d1.AddHours(12);
            sql =
                "select count(*) from [mfu].[dbo].Table_MouldAction where Mould_Action = 1 and Time_Action" +
                " between '" + d1.ToString("yyyy-MM-ddTHH:mm:ss.000") + "' and '" + d2.ToString("yyyy-MM-ddTHH:mm:ss.000")
                + "' and (idFK in(select top 1  num_m from  [otk].[dbo].Link_" + comboBox1.Text + " where line = " + Line +
                " and DT_out< '" + d2.ToString("yyyy-MM-ddTHH:mm:ss.000") + "' order by DT_out desc) or idfk = " + Fk + " ) " +
                " and mouldtype = 1";
            command = new SqlCommand(sql, conn);
            t = command.ExecuteScalar();
            dgv.Rows[dgv.Rows.Count - 1].Cells[2].Value = t;
            d1 = d2;
            d2 = d1.AddHours(12);
            sql =
                "select count(*) from [mfu].[dbo].Table_MouldAction where Mould_Action = 1 and Time_Action" +
                " between '" + d1.ToString("yyyy-MM-ddTHH:mm:ss.000") + "' and '" + d2.ToString("yyyy-MM-ddTHH:mm:ss.000")
                + "' and (idFK in(select top 1  num_m from  [otk].[dbo].Link_"+comboBox1.Text+" where line = " + Line +
                " and DT_out< '" + d2.ToString("yyyy-MM-ddTHH:mm:ss.000") + "' order by DT_out desc) or idfk = " + Fk + " ) " +
                " and mouldtype = 1";
            command = new SqlCommand(sql, conn);
            t = command.ExecuteScalar();
            dgv.Rows[dgv.Rows.Count - 1].Cells[3].Value = t;

           // d1 = d2;
            //d2 = d1.AddHours(12);
            sql =
                "select count(*) as kol, code_defect from [mfu].[dbo].Table_MouldAction where Time_Action" +
                " between '" + d1.AddHours(-24).ToString ("yyyy-MM-ddTHH:mm:ss.000") + "' and '" + d2.ToString("yyyy-MM-ddTHH:mm:ss.000") + "'" +
                " and Mould_Action = 1 and mouldtype = 1" +
                " and (idFK in (select top 1  num_m from[otk].[dbo].Link_" + comboBox1.Text + " where line = " + Line + "" +
                " and DT_out < '" + d2.ToString("yyyy-MM-ddTHH:mm:ss.000") + "'" +
                " order by DT_out desc) or idfk = " + Fk + " ) group by Code_defect";
            command = new SqlCommand(sql, conn);
            var reader = command.ExecuteReader();
            var s = "";
            while (reader.Read())
            {
                s += reader["code_defect"] + "(" + reader["kol"] + "); ";
            }
            reader.Close();
            dgv.Rows[dgv.Rows.Count - 1].Cells[4].Value = s;

            //чистовые
            //d1 = new DateTime(2022, 08, 15, 21, 32, 22);
            d1 = DateTime.Now;
            d1 = d1.AddHours(-24);
            d1 = d1.AddMinutes(-d1.Minute);
            d1 = d1.AddSeconds(-d1.Second);
            d1 = d1.AddMilliseconds(-d1.Millisecond);
            if (d1.Hour >= 8 && d1.Hour < 20)
            {
                d1 = d1.AddHours(8 - d1.Hour);
            }
            else
            {
                d1 = d1.AddHours(20 - d1.Hour);
            }

            d2 = d1;
            d2 = d1.AddHours(12);

            sql =
                "select count(*) from [mfu].[dbo].Table_MouldAction where Mould_Action = 1 and Time_Action" +
                " between '" + d1.ToString("yyyy-MM-ddTHH:mm:ss.000") + "' and '" + d2.ToString("yyyy-MM-ddTHH:mm:ss.000")
                + "' and (idFK in(select top 1  num_m from  [otk].[dbo].Link_" + comboBox1.Text + " where line = " + Line +
                " and DT_out< '" + d2.ToString("yyyy-MM-ddTHH:mm:ss.000") + "' order by DT_out desc) or idfk = " + Fk + " ) " +
                " and mouldtype = 2";
             command = new SqlCommand(sql, conn);
             t = command.ExecuteScalar();
            dgv.Rows.Add("Чистовая форма", t, "", "", "");
            d1 = d2;
            d2 = d1.AddHours(12);
            sql =
                "select count(*) from [mfu].[dbo].Table_MouldAction where Mould_Action = 1 and Time_Action" +
                " between '" + d1.ToString("yyyy-MM-ddTHH:mm:ss.000") + "' and '" + d2.ToString("yyyy-MM-ddTHH:mm:ss.000")
                + "' and (idFK in(select top 1  num_m from  [otk].[dbo].Link_" + comboBox1.Text + " where line = " + Line +
                " and DT_out< '" + d2.ToString("yyyy-MM-ddTHH:mm:ss.000") + "' order by DT_out desc) or idfk = " + Fk + " ) " +
                " and mouldtype = 2";
            command = new SqlCommand(sql, conn);
            t = command.ExecuteScalar();
            dgv.Rows[dgv.Rows.Count - 1].Cells[2].Value = t;
            d1 = d2;
            d2 = d1.AddHours(12);
            sql =
                "select count(*) from [mfu].[dbo].Table_MouldAction where Mould_Action = 1 and Time_Action" +
                " between '" + d1.ToString("yyyy-MM-ddTHH:mm:ss.000") + "' and '" + d2.ToString("yyyy-MM-ddTHH:mm:ss.000")
                + "' and (idFK in(select top 1  num_m from  [otk].[dbo].Link_" + comboBox1.Text + " where line = " + Line +
                " and DT_out< '" + d2.ToString("yyyy-MM-ddTHH:mm:ss.000") + "' order by DT_out desc) or idfk = " + Fk + " ) " +
                " and mouldtype = 2";
            command = new SqlCommand(sql, conn);
            t = command.ExecuteScalar();
            dgv.Rows[dgv.Rows.Count - 1].Cells[3].Value = t;

            sql =
                "select count(*) as kol, code_defect from [mfu].[dbo].Table_MouldAction where Time_Action" +
                " between '" + d1.AddHours(-24).ToString("yyyy-MM-ddTHH:mm:ss.000") + "' and '" + d2.ToString("yyyy-MM-ddTHH:mm:ss.000") + "'" +
                " and Mould_Action = 1 and mouldtype = 2" +
                " and (idFK in (select top 1  num_m from[otk].[dbo].Link_" + comboBox1.Text + " where line = " + Line + "" +
                " and DT_out < '" + d2.ToString("yyyy-MM-ddTHH:mm:ss.000") + "'" +
                " order by DT_out desc) or idfk = " +Fk + " ) group by Code_defect";
            command = new SqlCommand(sql, conn);
            reader = command.ExecuteReader();
            s = "";
            while (reader.Read())
            {
                s += reader["code_defect"] + "(" + reader["kol"] + "); ";
            }
            reader.Close();
            dgv.Rows[dgv.Rows.Count - 1].Cells[4].Value = s;

            sql = "select top 1 dt_out from [otk].[dbo].Link_" + comboBox1.Text + " where num_m = " + Fk +" order by dt_out desc";
            command = new SqlCommand(sql, conn);
            DateTime lastdate;
            reader = command.ExecuteReader();
            if(reader.HasRows)
            {
                reader.Read();
                lastdate = (DateTime)reader["dt_out"];
            }
            else
            {
                lastdate = DateTime.Now;
            }
            reader.Close();
            if (DateTime.Now.Subtract(lastdate).Days * 24 + DateTime.Now.Subtract(lastdate).Hours > 12)
            {
                sql = "select name_a from [OTK].[dbo].[Table_assortment] where id_a = (select assortiment from [" + comboBox1.Text + "].[dbo].[Table_Assortiment]" +
                    " where n_line = " + Line + ")";
                command = new SqlCommand(sql, conn);
                label.Text= (string)command.ExecuteScalar();
                //this.Text = (string)command.ExecuteScalar();
            }
            else
            {
                sql = "select top 2 dt_out, num_m from [otk].[dbo].Link_" + comboBox1.Text + " where line="+Line+" order by dt_out desc";
                command = new SqlCommand(sql, conn);
                reader = command.ExecuteReader();
                reader.Read();
                reader.Read();
                var idfk = reader["num_m"];
                reader.Close();
                sql = "select name_a from[OTK].[dbo].[Table_assortment] where id_a = " +
                    "(select top 1 assortment from[OTK].[dbo].[Weight_Control_"+comboBox1.Text[comboBox1.Text.Length-1]+"]" +
                    " where line = " + Line + " and num_f = "+idfk+")";
            command = new SqlCommand(sql, conn);
            reader = command.ExecuteReader();
            if(reader.HasRows)
            {
                reader.Read();
                    //this.Text = reader["name_a"].ToString();
                    label.Text= reader["name_a"].ToString();
                }
            else
            {
                    //this.Text = "";
                    label.Text = "";
            }
              reader.Close();   
            }
            conn.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            var conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
            conn.Open();
            var sql = "select n_line,idfk from [" + comboBox1.Text + "].[dbo].[Table_Assortiment]";
            var command = new SqlCommand(sql, conn);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (reader["idfk"] == DBNull.Value)
                    continue;
                comboBox2.Items.Add(reader["n_line"] + " " + reader["idfk"]);
            }
            reader.Close();
            conn.Close();
            comboBox2.SelectedIndex = 0;
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();
            fill_datagrid(dataGridView1,comboBox2.Items[0].ToString().Split(' ')[0], comboBox2.Items[0].ToString().Split(' ')[1],label1);
            fill_datagrid(dataGridView2, comboBox2.Items[1].ToString().Split(' ')[0], comboBox2.Items[1].ToString().Split(' ')[1], label2);
            fill_datagrid(dataGridView3, comboBox2.Items[2].ToString().Split(' ')[0], comboBox2.Items[2].ToString().Split(' ')[1], label3);

        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView2.Rows.Clear();
            dataGridView3.Rows.Clear();
            fill_datagrid(dataGridView1, comboBox2.Items[0].ToString().Split(' ')[0], comboBox2.Items[0].ToString().Split(' ')[1], label1);
            fill_datagrid(dataGridView2, comboBox2.Items[1].ToString().Split(' ')[0], comboBox2.Items[1].ToString().Split(' ')[1], label2);
            fill_datagrid(dataGridView3, comboBox2.Items[2].ToString().Split(' ')[0], comboBox2.Items[2].ToString().Split(' ')[1], label3);
        }

        //Пролистывание назад
        private void button1_Click(object sender, EventArgs e)
        {
            //if (comboBox2.SelectedIndex == 0)
            //{
            //    comboBox1.SelectedIndex = (comboBox1.SelectedIndex + 1) % 2;
            //    comboBox2.SelectedIndex = comboBox2.Items.Count - 1;
            //}
            //else
            //{
            //    comboBox2.SelectedIndex--;
            //}
            comboBox1.SelectedIndex = (comboBox1.SelectedIndex + 1) % 2;
        }

        //Пролистывание вперед
        private void button2_Click(object sender, EventArgs e)
        {
            //if (comboBox2.SelectedIndex == comboBox2.Items.Count - 1)
            //{
                comboBox1.SelectedIndex = (comboBox1.SelectedIndex + 1) % 2;
            //    comboBox2.SelectedIndex = 0;
            //}
            //else
            //{
            //    comboBox2.SelectedIndex++;
            //}
        }

        private void dataGridView1_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4 && e.RowIndex >= 0)
            {
                if (dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "")
                    return;

                Dictionary<int, string> defects = new Dictionary<int, string>();
                var conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
                conn.Open();
                string d = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                d= Regex.Replace(d, @"[(][0-9]*[)]", "");
                d = d.Replace(";", ",");
                var sql = "SELECT [Name_defect], code  FROM[OTK].[dbo].[Table_Defect] where code in ("
                    + d.Substring(0,d.Length-2) + ")";
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
                foreach (var x in d.Substring(0, d.Length - 2).ToString().Split(','))
                {
                    s += defects[Convert.ToInt32(x)] + "\n";
                }
                dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = s;

            }
        }

        private void dataGridView2_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4 && e.RowIndex >= 0)
            {
                if (dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "")
                    return;

                Dictionary<int, string> defects = new Dictionary<int, string>();
                var conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
                conn.Open();
                string d = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                d = Regex.Replace(d, @"[(][0-9]*[)]", "");
                d = d.Replace(";", ",");
                var sql = "SELECT [Name_defect], code  FROM[OTK].[dbo].[Table_Defect] where code in ("
                    + d.Substring(0, d.Length - 2) + ")";
                var command = new SqlCommand(sql, conn);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    defects.Add(Convert.ToInt32(reader["code"]), reader["Name_defect"].ToString());
                }
                reader.Close();
                conn.Close();
                string s = "";
                foreach (var x in d.Substring(0, d.Length - 2).ToString().Split(','))
                {
                    s += defects[Convert.ToInt32(x)] + "\n";
                }
                dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = s;

            }
        }

        private void dataGridView3_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4 && e.RowIndex >= 0)
            {
                if (dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() == "")
                    return;

                Dictionary<int, string> defects = new Dictionary<int, string>();
                var conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
                conn.Open();
                string d = dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
                d = Regex.Replace(d, @"[(][0-9]*[)]", "");
                d = d.Replace(";", ",");
                var sql = "SELECT [Name_defect], code  FROM[OTK].[dbo].[Table_Defect] where code in ("
                    + d.Substring(0, d.Length - 2) + ")";
                var command = new SqlCommand(sql, conn);
                var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    defects.Add(Convert.ToInt32(reader["code"]), reader["Name_defect"].ToString());
                }
                reader.Close();
                conn.Close();
                string s = "";
                foreach (var x in d.Substring(0, d.Length - 2).ToString().Split(','))
                {
                    s += defects[Convert.ToInt32(x)] + "\n";
                }
                dataGridView3.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = s;

            }
        }

        private void DayReport_Load(object sender, EventArgs e)
        {

        }
    }
}
