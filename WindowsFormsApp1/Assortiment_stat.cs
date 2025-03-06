using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Statistic.Properties;
using Microsoft.Office.Interop.Excel;
using Excel = Microsoft.Office.Interop.Excel;

namespace Statistic
{
    public partial class Assortiment_stat : Form
    {
        public Assortiment_stat()
        {
            InitializeComponent();
            combo_ceh.SelectedIndex = 0;
            combo_line.SelectedIndex = 0;
            Fill_assort();
            this.toolTip1.ShowAlways = true;
            Fill_Code_defect();

        }
        void Fill_assort()
        {
            combo_assort.Text = "";
            combo_idFk.Text = "";
            combo_assort.Items.Clear();
            combo_idFk.Items.Clear();
            var conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
            conn.Open();
            var sql = File.ReadAllText("assort_for_combo.sql");

            var d1 = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, timePicker1.Value.Hour, timePicker1.Value.Minute, 0);
            var d2 = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, dateTimePicker2.Value.Day, timePicker2.Value.Hour, timePicker2.Value.Minute+1, 0);

            sql = sql.Replace("@ceh", combo_ceh.Text);
            var command = new SqlCommand(sql, conn);
            command.Parameters.AddWithValue("@line", combo_line.Text);
            command.Parameters.AddWithValue("@Date_1", d1);
            command.Parameters.AddWithValue("@Date_2", d2);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                combo_assort.Items.Add(reader["Name_a"]);
            }
            reader.Close();

            combo_idFk.Items.Clear();
            conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
            conn.Open();
            sql = File.ReadAllText("idFk_for_combo.sql");          

            sql = sql.Replace("@ceh", combo_ceh.Text);
            command = new SqlCommand(sql, conn);
            command.Parameters.AddWithValue("@line", combo_line.Text);
            command.Parameters.AddWithValue("@Date_1", d1);
            command.Parameters.AddWithValue("@Date_2", d2);
            reader = command.ExecuteReader();
            while (reader.Read())
            {
                combo_idFk.Items.Add(reader["Num_m"]);
            }
            reader.Close();

            conn.Close();
        }

        void Fill_Code_defect()
        {
            text_Code_defect.Text="";
           var code = File.ReadAllText("Code_defect_out.txt");
            code = code.Replace("(", "");
            code = code.Replace(")", "");
            foreach(var x in code.Split(','))
            {
                text_Code_defect.Text += x+"\r\n";
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            Fill_assort();
        }

        private void timePicker1_ValueChanged(object sender, EventArgs e)
        {
            Fill_assort();
        }

        private void dateTimePicker2_ValueChanged(object sender, EventArgs e)
        {
            Fill_assort();
        }

        private void timePicker2_ValueChanged(object sender, EventArgs e)
        {
            Fill_assort();
        }

        struct pair
        {
            public int section;
            public string code_column;
            public string count_column;
            public int current_string;
            public pair(int sec, string code, string col, int cur)
            {
                section = sec;
                code_column = code;
                count_column = col;
                current_string = cur;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Workbook workbook;
            Worksheet w;           
            var excelapp = new Excel.Application();
            {
                if (excelapp == null)
                {
                    Console.WriteLine("Excel is not installed!!");
                    return;
                }
            }

            workbook = excelapp.Workbooks.Open(Directory.GetCurrentDirectory() + "\\" + "Шаблон_ассортимент.xlsx");
            w = workbook.Worksheets[1];
            List<pair> in_strings = new List<pair>() {new pair(1,"C","D",6), new pair(2, "E", "F", 6), new pair(3, "G", "H", 6),new pair(4,"I","J",6),
                new pair(5,"K","L",6),new pair(6,"M","N",6),new pair(7,"O","P",6),new pair(8,"Q","R",6),new pair(9,"S","T",6),new pair(10,"U","V",6)};
            List<pair> out_strings = new List<pair>() {new pair(1,"C","D",18), new pair(2, "E", "F", 18), new pair(3, "G", "H", 18),new pair(4,"I","J",18),
                new pair(5,"K","L",18),new pair(6,"M","N",18),new pair(7,"O","P",18),new pair(8,"Q","R",18),new pair(9,"S","T",18),new pair(10,"U","V",18)};

            var conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
            conn.Open();
            var sql = "";
            if (combo_assort.Text != null && combo_assort.Text != "")
            {
                sql = File.ReadAllText("assort_excel.sql");
            }
            else
            {
                sql = File.ReadAllText("idFk_excel.sql");
            }
            sql = sql.Replace("@ceh", combo_ceh.Text);
            var codes = File.ReadAllText("Code_defect_out.txt");          
            if (codes != "()")
                sql = sql.Replace("@Code_defect", codes);
            else
                sql = sql.Replace("AND Code_defect NOT IN @Code_defect", "");
            var d1 = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, timePicker1.Value.Hour, timePicker1.Value.Minute, 0);
            var d2 = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, dateTimePicker2.Value.Day, timePicker2.Value.Hour, timePicker2.Value.Minute + 1, 0);

            var command = new SqlCommand(sql, conn);
            command.Parameters.AddWithValue("@Name_assort", combo_assort.Text);
            command.Parameters.AddWithValue("@DATE1", d1);
            command.Parameters.AddWithValue("@DATE2", d2);
            command.Parameters.AddWithValue("@Num_m", combo_idFk.Text);

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (reader["place"].ToString() == "2")
                {
                    w.Range[in_strings[Convert.ToInt32(reader["section"]) - 1].code_column + in_strings[Convert.ToInt32(reader["section"]) - 1].current_string.ToString()].Value =
                         reader["code_defect"].ToString();
                    w.Range[in_strings[Convert.ToInt32(reader["section"]) - 1].count_column + in_strings[Convert.ToInt32(reader["section"]) - 1].current_string.ToString()].Value =
                        reader["count_defect"].ToString();
                    int sectionIndex = Convert.ToInt32(reader["section"]) - 1;

                    // Получаем элемент из списка
                    var currentPair = in_strings[sectionIndex];

                    // Изменяем свойство элемента
                    currentPair.current_string += 1;

                    // Присваиваем измененный элемент обратно в список
                    in_strings[sectionIndex] = currentPair;

                }
                else
                {
                    w.Range[out_strings[Convert.ToInt32(reader["section"]) - 1].code_column + out_strings[Convert.ToInt32(reader["section"]) - 1].current_string.ToString()].Value =
                       reader["code_defect"].ToString();
                    w.Range[out_strings[Convert.ToInt32(reader["section"]) - 1].count_column + out_strings[Convert.ToInt32(reader["section"]) - 1].current_string.ToString()].Value =
                        reader["count_defect"].ToString();
                    int sectionIndex = Convert.ToInt32(reader["section"]) - 1;

                    // Получаем элемент из списка
                    var currentPair = out_strings[sectionIndex];

                    // Изменяем свойство элемента
                    currentPair.current_string += 1;

                    // Присваиваем измененный элемент обратно в список
                    out_strings[sectionIndex] = currentPair;
                }
            }
            reader.Close();
            conn.Close();
            SaveFileDialog ofd = new SaveFileDialog();
            ofd.Filter = "Excel Files (*.xlsx)|*.xlsx";
            ofd.FileName = "Статистика по ассортименту " + combo_assort.Text + ".xlsx";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                workbook.SaveAs(ofd.FileName);
            }
            workbook.Close();


        }

        private void combo_idFk_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(combo_idFk.Text!=null && combo_idFk.Text != "")
                combo_assort.SelectedIndex = -1;
        }

        private void combo_assort_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (combo_assort.Text != null && combo_assort.Text != "")
                combo_idFk.SelectedIndex = -1;
        }

        private void dataGridView_b_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
        {
            AdjustLabelPositions();
        }
        private void AdjustLabelPositions()
        {
            int dgvLeft = dataGridView_b.Left;
            int dgvTop = dataGridView_b.Top;

            label1.Left = dataGridView_b.GetCellDisplayRectangle(0, -1, true).Left + dgvLeft;
            label1.Width = dataGridView_b.GetCellDisplayRectangle(1, -1, true).Right - label1.Left + dgvLeft;
            label1.Height = dataGridView_b.GetCellDisplayRectangle(1, -1, true).Height ;
            label1.Top = dataGridView_b.GetCellDisplayRectangle(0, -1, true).Top+ dgvTop;


            label2.Left = dataGridView_b.GetCellDisplayRectangle(2, -1, true).Left + dgvLeft;
            label2.Width = dataGridView_b.GetCellDisplayRectangle(3, -1, true).Right - label2.Left + dgvLeft;
            label2.Height = dataGridView_b.GetCellDisplayRectangle(1, -1, true).Height;
            label2.Top = dataGridView_b.GetCellDisplayRectangle(0, -1, true).Top + dgvTop;

            label3.Left = dataGridView_b.GetCellDisplayRectangle(4, -1, true).Left + dgvLeft;
            label3.Width = dataGridView_b.GetCellDisplayRectangle(5, -1, true).Right - label3.Left + dgvLeft;
            label3.Height = dataGridView_b.GetCellDisplayRectangle(1, -1, true).Height;
            label3.Top = dataGridView_b.GetCellDisplayRectangle(0, -1, true).Top + dgvTop;

            label4.Left = dataGridView_b.GetCellDisplayRectangle(6, -1, true).Left + dgvLeft;
            label4.Width = dataGridView_b.GetCellDisplayRectangle(7, -1, true).Right - label4.Left + dgvLeft;
            label4.Height = dataGridView_b.GetCellDisplayRectangle(1, -1, true).Height;
            label4.Top = dataGridView_b.GetCellDisplayRectangle(0, -1, true).Top + dgvTop;

            label5.Left = dataGridView_b.GetCellDisplayRectangle(8, -1, true).Left + dgvLeft;
            label5.Width = dataGridView_b.GetCellDisplayRectangle(9, -1, true).Right - label5.Left + dgvLeft;
            label5.Height = dataGridView_b.GetCellDisplayRectangle(1, -1, true).Height;
            label5.Top = dataGridView_b.GetCellDisplayRectangle(0, -1, true).Top + dgvTop;

            label6.Left = dataGridView_b.GetCellDisplayRectangle(10, -1, true).Left + dgvLeft;
            label6.Width = dataGridView_b.GetCellDisplayRectangle(11, -1, true).Right - label6.Left + dgvLeft;
            label6.Height = dataGridView_b.GetCellDisplayRectangle(1, -1, true).Height;
            label6.Top = dataGridView_b.GetCellDisplayRectangle(0, -1, true).Top + dgvTop;

            label7.Left = dataGridView_b.GetCellDisplayRectangle(12, -1, true).Left + dgvLeft;
            label7.Width = dataGridView_b.GetCellDisplayRectangle(13, -1, true).Right - label7.Left + dgvLeft;
            label7.Height = dataGridView_b.GetCellDisplayRectangle(1, -1, true).Height;
            label7.Top = dataGridView_b.GetCellDisplayRectangle(0, -1, true).Top + dgvTop;

            label8.Left = dataGridView_b.GetCellDisplayRectangle(14, -1, true).Left + dgvLeft;
            label8.Width = dataGridView_b.GetCellDisplayRectangle(15, -1, true).Right - label8.Left + dgvLeft;
            label8.Height = dataGridView_b.GetCellDisplayRectangle(1, -1, true).Height;
            label8.Top = dataGridView_b.GetCellDisplayRectangle(0, -1, true).Top + dgvTop;

            label9.Left = dataGridView_b.GetCellDisplayRectangle(16, -1, true).Left + dgvLeft;
            label9.Width = dataGridView_b.GetCellDisplayRectangle(17, -1, true).Right - label9.Left + dgvLeft;
            label9.Height = dataGridView_b.GetCellDisplayRectangle(1, -1, true).Height;
            label9.Top = dataGridView_b.GetCellDisplayRectangle(0, -1, true).Top + dgvTop;

            label10.Left = dataGridView_b.GetCellDisplayRectangle(18, -1, true).Left + dgvLeft;
            label10.Width = dataGridView_b.GetCellDisplayRectangle(19, -1, true).Right - label10.Left + dgvLeft;
            label10.Height = dataGridView_b.GetCellDisplayRectangle(1, -1, true).Height;
            label10.Top = dataGridView_b.GetCellDisplayRectangle(0, -1, true).Top + dgvTop;
        }

        private void Assortiment_stat_Load(object sender, EventArgs e)
        {
            AdjustLabelPositions();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            dataGridView_b.Rows.Clear();
            dataGridView_d.Rows.Clear();
            List<pair> in_strings = new List<pair>() {new pair(1,"code_1","count_1",0), new pair(2, "code_2","count_2", 0), new pair(3, "code_3","count_3", 0),
                new pair(4,"code_4","count_4",0), new pair(5,"code_5","count_5",0),new pair(6,"code_6","count_6",0),new pair(7,"code_7","count_7",0),
                new pair(8,"code_8","count_8",0),new pair(9,"code_9","count_9",0),new pair(10,"code_10","count_10",0)};
            List<pair> out_strings = new List<pair>() {new pair(1,"code_1_d","count_1_d",0), new pair(2, "code_2_d","count_2_d", 0), 
                new pair(3, "code_3_d","count_3_d", 0), new pair(4,"code_4_d","count_4_d",0), new pair(5,"code_5_d","count_5_d",0),
                new pair(6,"code_6_d","count_6_d",0),new pair(7,"code_7_d","count_7_d",0),
                new pair(8,"code_8_d","count_8_d",0),new pair(9,"code_9_d","count_9_d",0),new pair(10,"code_10_d","count_10_d",0)};

            var conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
            conn.Open();
            var sql = "";
            if (combo_assort.Text != null && combo_assort.Text != "")
            {
                sql = File.ReadAllText("assort_excel.sql");
            }
            else
            {
                sql = File.ReadAllText("idFk_excel.sql");
            }
            var codes = File.ReadAllText("Code_defect_out.txt");
            sql = sql.Replace("@ceh", combo_ceh.Text);
            if (codes != "()")
                sql = sql.Replace("@Code_defect", codes);
            else
                sql = sql.Replace("AND Code_defect NOT IN @Code_defect", "");
            var d1 = new DateTime(dateTimePicker1.Value.Year, dateTimePicker1.Value.Month, dateTimePicker1.Value.Day, timePicker1.Value.Hour, timePicker1.Value.Minute, 0);
            var d2 = new DateTime(dateTimePicker2.Value.Year, dateTimePicker2.Value.Month, dateTimePicker2.Value.Day, timePicker2.Value.Hour, timePicker2.Value.Minute + 1, 0);

            var command = new SqlCommand(sql, conn);
            command.Parameters.AddWithValue("@Name_assort", combo_assort.Text);
            command.Parameters.AddWithValue("@DATE1", d1);
            command.Parameters.AddWithValue("@DATE2", d2);
            command.Parameters.AddWithValue("@Num_m", combo_idFk.Text);

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                if (reader["section"] == DBNull.Value)
                    continue;
                if (reader["place"].ToString() == "2")
                {
                    if(dataGridView_b.Rows.Count <= in_strings[Convert.ToInt32(reader["section"]) - 1].current_string)
                    {
                        dataGridView_b.Rows.Add();
                    }
                    dataGridView_b.Rows[in_strings[Convert.ToInt32(reader["section"]) - 1].current_string].Cells[in_strings[Convert.ToInt32(reader["section"]) - 1].code_column].Value =
                         reader["code_defect"].ToString();
                    dataGridView_b.Rows[in_strings[Convert.ToInt32(reader["section"]) - 1].current_string].Cells[in_strings[Convert.ToInt32(reader["section"]) - 1].count_column].Value =
                         reader["count_defect"].ToString();

                    int sectionIndex = Convert.ToInt32(reader["section"]) - 1;
                    // Получаем элемент из списка
                    var currentPair = in_strings[sectionIndex];

                    // Изменяем свойство элемента
                    currentPair.current_string += 1;

                    // Присваиваем измененный элемент обратно в список
                    in_strings[sectionIndex] = currentPair;

                }
                else
                {
                    
                    if (dataGridView_d.Rows.Count <= out_strings[Convert.ToInt32(reader["section"]) - 1].current_string)
                    {
                        dataGridView_d.Rows.Add();
                    }
                    dataGridView_d.Rows[out_strings[Convert.ToInt32(reader["section"]) - 1].current_string].Cells[out_strings[Convert.ToInt32(reader["section"]) - 1].code_column].Value =
                         reader["code_defect"].ToString();
                    dataGridView_d.Rows[out_strings[Convert.ToInt32(reader["section"]) - 1].current_string].Cells[out_strings[Convert.ToInt32(reader["section"]) - 1].count_column].Value =
                         reader["count_defect"].ToString();

                    int sectionIndex = Convert.ToInt32(reader["section"]) - 1;

                    // Получаем элемент из списка
                    var currentPair = out_strings[sectionIndex];

                    // Изменяем свойство элемента
                    currentPair.current_string += 1;

                    // Присваиваем измененный элемент обратно в список
                    out_strings[sectionIndex] = currentPair;
                }
            }
            reader.Close();
            conn.Close();
        }

        private void dataGridView_b_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if(e.RowIndex>=0 && e.ColumnIndex % 2 == 0)
            {
                if (dataGridView_b.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                    return;
                var conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
                conn.Open();
                var sql = "SELECT Name_defect FROM [OTK].[dbo].[Table_Defect] WHERE Code = " + dataGridView_b.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                var command = new SqlCommand(sql, conn);
                var reader = command.ExecuteReader();
                reader.Read();
                dataGridView_b.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = reader["Name_defect"].ToString();
                reader.Close();
                conn.Close();
            }

        }

        private void dataGridView_b_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            //toolTip1.SetToolTip(dataGridView_b, string.Empty);
        }

        private void dataGridView_d_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView_d.Rows[e.RowIndex].Cells[e.ColumnIndex].Value == null)
                return;
            if (e.RowIndex >= 0 && e.ColumnIndex % 2 == 0)
            {
                var conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
                conn.Open();
                var sql = "SELECT Name_defect FROM [OTK].[dbo].[Table_Defect] WHERE Code = " + dataGridView_d.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                var command = new SqlCommand(sql, conn);
                var reader = command.ExecuteReader();
                reader.Read();
                dataGridView_d.Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = reader["Name_defect"].ToString();
                reader.Close();
                conn.Close();
            }
        }

        private void dataGridView_d_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
           // toolTip1.SetToolTip(dataGridView_d, string.Empty);
        }

        private void text_Code_defect_Leave(object sender, EventArgs e)
        {
            File.WriteAllText("Code_defect_out.txt","("+ text_Code_defect.Text.Trim().Replace('\n', ',')+")");
        }

        private void combo_line_SelectedIndexChanged(object sender, EventArgs e)
        {
            Fill_assort();
        }

        private void combo_ceh_SelectedIndexChanged(object sender, EventArgs e)
        {
            Fill_assort();
        }

        private void text_Code_defect_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsDigit(e.KeyChar) && e.KeyChar!= (char)Keys.Enter && e.KeyChar!= (char)Keys.Back && e.KeyChar != (char)Keys.Delete)
            {
                e.Handled = true;
            }
        }
    }
}
