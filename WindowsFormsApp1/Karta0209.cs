using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using WindowsFormsApp1.Properties;
using Excel = Microsoft.Office.Interop.Excel;

namespace WindowsFormsApp1
{
    public partial class Karta0209 : Form
    {
        Color[] myColor = new Color[] { Color.LightBlue, Color.LightCoral, Color.LightGreen, Color.LightCyan, Color.LightSeaGreen , Color.LightSeaGreen};

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
            /*
            rowMergeView1.Rows.Clear();
            rowMergeView2.Rows.Clear();
            rowMergeView2.ColumnHeadersVisible = false;
            //this.rowMergeView1.DataSource = dt;
            this.rowMergeView1.ColumnHeadersHeight = 40;
            this.rowMergeView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.rowMergeView2.ColumnHeadersHeight = 40;
            this.rowMergeView2.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.rowMergeView1.AddSpanHeader(2, 2, "период действия объема");
            */
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
                        where t.N_fk = @idfk order by CONVERT(smalldatetime, t.[Data_set], 103)";
            var command = new SqlCommand(sql, conn);
            command.Parameters.AddWithValue("@idfk", idfk);
            var reader = command.ExecuteReader();
            int i = 1;

            // таблицы
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

            Excel.Application excelApp = new Excel.Application();
            Excel.Workbook workbook = excelApp.Workbooks.Open(@"D:\ISKRA\Statistic_Git\отчет 31_07_23\WindowsFormsApp1\bin\Release\0209.xlsx");
            Excel.Worksheet worksheet = workbook.ActiveSheet;

           // rowMergeView2.Rows.Add(101);

            int newRows = 0;

            if(rowMergeView1.Rows.Count > 2)
            {
                for(int  n = 0; n < rowMergeView1.Rows.Count - 1; n++)
                {
                    Excel.Range range2 = ((Excel.Range)worksheet.Cells[3 + n, 1]).EntireRow;
                    range2.Insert(Excel.XlInsertShiftDirection.xlShiftDown, false);
                    range2 = ((Excel.Range)worksheet.Cells[3 + 1 + n, 1]).EntireRow;
                    range2.Copy(((Excel.Range)worksheet.Cells[3 + n, 1]).EntireRow);
                    worksheet.Cells[3 + 1 + n, 3] = n + 2;
                    newRows++;
                }
            }

            for (int r = 0; r < 99; r++)
            {
               // rowMergeView2.Rows[r + 2].Cells[0].Value = r + 1;
                worksheet.Cells[newRows + 8 + r, 1] = r + 1;
            }

            worksheet.Cells[1, 2] =  idfk;
            worksheet.Cells[2, 2] = "-";
            worksheet.Cells[4, 2] = DateTime.Now.ToString("dd-MM-yyyy");

           // rowMergeView2.Rows[1].Cells[0].Value = "Номер формы";
           // rowMergeView2.Rows[1].Cells[1].Value = "Последний замер";

            foreach (DataGridViewRow row in rowMergeView1.Rows)
            {
                worksheet.Cells[3 + colorCount, 4] = row.Cells[1].Value;
                Excel.Range range = worksheet.Cells[3 + colorCount, 4];
                range.Interior.Color = myColor[colorCount];
                worksheet.Cells[3 + colorCount, 6] = row.Cells[2].Value;
                worksheet.Cells[3 + colorCount, 8] = row.Cells[3].Value;


                //if (row.Cells.Count == 0) return;
                sql = @"set language us_english; 
                    SELECT 
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

                sql = @" set language us_english;
                        SELECT 
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

                worksheet.Range[worksheet.Cells[newRows + 5, cnt + 1], worksheet.Cells[newRows + 5, cnt +dt.Rows.Count]].Merge();
                worksheet.Range[worksheet.Cells[newRows + 5, cnt + 1], worksheet.Cells[newRows + 5, cnt + dt.Rows.Count]].Interior.Color = myColor[colorCount];
                worksheet.Range[worksheet.Cells[newRows + 5, cnt + 1], worksheet.Cells[newRows + 5, cnt + dt.Rows.Count]] = "скорректированный и/или установленный обьем " + row.Cells[1].Value + " +-0,25";
                

                foreach (DataRow tabRow in dt.Rows)
                {
                   // rowMergeView2.Columns.Add(new DataGridViewColumn() { CellTemplate = new DataGridViewTextBoxCell() });
                   // rowMergeView2.Rows[0].Cells[cnt].Style.BackColor = myColor[colorCount];
                   // rowMergeView2.Rows[1].Cells[cnt].Value = tabRow["date"];

                    worksheet.Cells[newRows + 7, cnt+1] = tabRow["date"];
                    range = worksheet.Cells[newRows + 7, cnt+1];
                    range.Interior.Color = myColor[colorCount];

                    //rowMergeView2.Rows[Convert.ToInt32(el.Field<string>("Number_mould")) + 1].Cells[cnt].Value = el.Field<string>("Volume");
                    dt2.AsEnumerable().Where(x => x.Field<string>("date") == tabRow["date"].ToString()).ToList().ForEach(el => { worksheet.Cells[Convert.ToInt32(el.Field<string>("Number_mould")) + newRows + 7, cnt+1] = el.Field<string>("Volume"); fillColor(Convert.ToInt32(el.Field<string>("Number_mould")) + newRows + 7, cnt + 1, el.Field<string>("Volume"), row.Cells[1].Value.ToString(), worksheet); }) ;

                    cnt++;
                }

                colorCount++;
            }

            sql = @"SELECT 
		            Volume as volume, Number_mould
                    FROM [MFU].[dbo].[Volume_mess] as a
                    where Date = (SELECT MAX(DATE) FROM [MFU].[dbo].[Volume_mess] WHERE Number_mould = a.Number_mould AND Number_fk = @idfk)";

            command = new SqlCommand(sql, conn);
            command.Parameters.AddWithValue("@idfk", idfk);

            DataTable dt3 = new DataTable();
            SqlDataAdapter adapter3 = new SqlDataAdapter(command);

            adapter3.Fill(dt3);

            foreach (DataRow item in dt3.Rows)
            {
               // rowMergeView2.Rows[Convert.ToInt32(item["Number_mould"]) + 1].Cells[1].Value = item["Volume"];
                worksheet.Cells[Convert.ToInt32(item["Number_mould"]) + newRows + 7, 2] = item["Volume"];
            }

            conn.Close();

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excels files (*.xlsx)|*.xlsx";

            saveFileDialog.FilterIndex = 0;
            saveFileDialog.RestoreDirectory = true;
            saveFileDialog.CreatePrompt = true;
            saveFileDialog.FileName = null;
            saveFileDialog.Title = "Где сохранить?";
            saveFileDialog.ShowDialog();

            workbook.SaveAs(saveFileDialog.FileName);
            excelApp.Quit();
            Marshal.ReleaseComObject(workbook);
            Marshal.ReleaseComObject(excelApp);

            System.Diagnostics.Process.Start(saveFileDialog.FileName);

        }

        private void fillColor(int r, int c, string val, string eq, Excel.Worksheet wh)
        {
            var color = Color.White;
            if (Convert.ToDouble(val) > Convert.ToDouble(eq) + 0.25)
                color = Color.Red; 
            else if(Convert.ToDouble(val) < Convert.ToDouble(eq) - 0.25)
                color = Color.Chocolate;
            Excel.Range range = wh.Cells[r, c];
            range.Interior.Color = color;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            DrawForm(comboBox1.Text);
            label1.Text = "Номер формокомплекта: " + comboBox1.Text;
            // label2.Text = "Наименование ФК: " + comboBox1.Text;
            label4.Text = "Дата отчета: " + DateTime.Now.ToString("dd-MM-yyyy");

        }

        private void button1_Click(object sender, EventArgs e)
        {

        }
    }
}
