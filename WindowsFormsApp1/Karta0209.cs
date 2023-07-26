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

namespace WindowsFormsApp1
{
    public partial class Karta0209 : Form
    {
        Color[] myColor = new Color[] { Color.Blue, Color.Black, Color.Red, Color.Bisque };

        public Karta0209()
        {
            InitializeComponent();

            var conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
            conn.Open();
            var sql = "select [N_fk] from [MFU].[dbo].[Volume_ex] group by [N_fk] order BY [N_fk] ";
            var command = new SqlCommand(sql, conn);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                comboBox1.Items.Add(reader["N_fk"].ToString());
            }
            reader.Close();
            conn.Close();

            //DrawForm();
        }

        private void DrawForm(string idfk)
        {
            rowMergeView1.Rows.Clear();
            rowMergeView2.Rows.Clear();

            rowMergeView2.ColumnHeadersVisible = false;

            //this.rowMergeView1.DataSource = dt;
            this.rowMergeView1.ColumnHeadersHeight = 40;
            this.rowMergeView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.rowMergeView2.ColumnHeadersHeight = 40;
            this.rowMergeView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.rowMergeView1.AddSpanHeader(2, 2, "период действия объема");

            var conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
            conn.Open();
            var sql = @"select 
                            (cast(REPLACE(Set_min, ',','.') as float) + cast(REPLACE(Set_max, ',','.') as float)) / 2 as avgval,
		                    Data_set,
                            (select MAX([Data_set])
                                from [MFU].[dbo].[Volume_ex] t2
                                where t2.N_fk = @idfk and  
                                      CONVERT(datetime, t2.[Data_set], 105) > CONVERT(datetime, t.[Data_set], 105)
                             ) as endDate
                        from [MFU].[dbo].[Volume_ex] t
                        where t.N_fk = @idfk";
            var command = new SqlCommand(sql, conn);
            command.Parameters.AddWithValue("@idfk", idfk);
            var reader = command.ExecuteReader();
            int i = 1;

            while (reader.Read())
            {
                if (rowMergeView1.Rows.Count > 0)
                {
                    if (rowMergeView1.Rows[rowMergeView1.Rows.Count - 1].Cells[1].Value.ToString() == reader["avgval"].ToString())
                    {
                        rowMergeView1.Rows[rowMergeView1.Rows.Count - 1].Cells[3].Value = reader["endDate"];
                    }
                    else
                    {
                        rowMergeView1.Rows.Add(i++, reader["avgval"], reader["Data_set"], reader["endDate"]);
                    }
                }
                else
                    rowMergeView1.Rows.Add(i++, reader["avgval"], reader["Data_set"], reader["endDate"]);
            }
            reader.Close();

            int colorCount = 0;
            int cnt = 2;

            rowMergeView2.Rows.Add(101);

            for (int r = 0; r < rowMergeView2.Rows.Count - 2; r++)
            {
                rowMergeView2.Rows[r + 2].Cells[0].Value = r + 1;
            }

            rowMergeView2.Rows[1].Cells[0].Value = "Номер формы";
            rowMergeView2.Rows[1].Cells[1].Value = "Последний замер";


            foreach (DataGridViewRow row in rowMergeView1.Rows)
            {
                //if (row.Cells.Count == 0) return;
                sql = @"SELECT 
		            MAX(CONVERT(VARCHAR(10), Date, 104) ) as date
                    FROM [MFU].[dbo].[Volume_mess]
                    where Number_fk = @idfk and [Date] between @start and @end group by CAST(Date AS DATE) order by CAST(Date AS DATE)";

                command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@idfk", idfk);

                string start = DateTime.Parse(row.Cells[2].Value.ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                string end = row.Cells[3].Value.ToString() == "" ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Parse(row.Cells[3].Value.ToString()).AddMinutes(1).ToString("yyyy-MM-dd HH:mm:ss");
                // DateTime end = DateTime.ParseExact(val, "dd.M.yyyy hh:mm:ss", null);

                command.Parameters.AddWithValue("@start", start);
                command.Parameters.AddWithValue("@end", end);

                DataTable dt = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter(command);

                adapter.Fill(dt);

                sql = @"SELECT 
		                CONVERT(VARCHAR(10), Date, 104) as date
                      ,[Volume]
	                  ,[Number_mould]
                    FROM [MFU].[dbo].[Volume_mess]
                    where Number_fk = @idfk and [Date] between @start and @end order by CAST(Date AS DATE)";

                command = new SqlCommand(sql, conn);

                command.Parameters.AddWithValue("@start", start);
                command.Parameters.AddWithValue("@end", end);
                command.Parameters.AddWithValue("@idfk", idfk);

                DataTable dt2 = new DataTable();
                SqlDataAdapter adapter2 = new SqlDataAdapter(command);

                adapter2.Fill(dt2);

                foreach (DataRow tabRow in dt.Rows)
                {
                    rowMergeView2.Columns.Add(new DataGridViewColumn() { CellTemplate = new DataGridViewTextBoxCell() });
                    rowMergeView2.Rows[0].Cells[cnt].Style.BackColor = myColor[colorCount];
                    rowMergeView2.Rows[1].Cells[cnt].Value = tabRow["date"];

                    dt2.AsEnumerable().Where(x => x.Field<string>("date") == tabRow["date"].ToString()).ToList().ForEach(el => rowMergeView2.Rows[Convert.ToInt32(el.Field<string>("Number_mould")) + 1].Cells[cnt].Value = el.Field<string>("Volume"));

                    cnt++;
                }

                colorCount++;
            }

            sql = @"SELECT 
		            MAX(Volume) as volume, Number_mould
                    FROM [MFU].[dbo].[Volume_mess]
                    where Number_fk = @idfk group by Number_mould";

            command = new SqlCommand(sql, conn);
            command.Parameters.AddWithValue("@idfk", idfk);

            DataTable dt3 = new DataTable();
            SqlDataAdapter adapter3 = new SqlDataAdapter(command);

            adapter3.Fill(dt3);

            foreach (DataRow item in dt3.Rows)
            {
                rowMergeView2.Rows[Convert.ToInt32(item["Number_mould"]) + 1].Cells[1].Value = item["Volume"];
            }

            conn.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawForm(comboBox1.Text);
            label1.Text = "Номер формокомплекта: " + comboBox1.Text;
            // label2.Text = "Наименование ФК: " + comboBox1.Text;
            label4.Text = "Дата отчета: " + DateTime.Now.ToString("dd-MM-yyyy");

        }
    }
}
