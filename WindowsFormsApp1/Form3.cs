using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using WindowsFormsApp1.Properties;

namespace WindowsFormsApp1
{
    public partial class Form3 : Form
    {
        List<double> _moulds = new List<double>();
        List<DotVolume> _dots = new List<DotVolume>();
        private double _deltaWeight = 1;
        private double _deltaVolume = 1;

        public Form3(string idfk)
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

        public class DotVolume
        {
            public double X;
            public double Y1;
            public double Y2;
        }

        private void VolumeChart()
        {
            _dots.Clear();
            var sqlConnection = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
            try
            {
                sqlConnection.Open();

                SqlDataAdapter dataAdapter = new SqlDataAdapter(
                    $" SELECT " +
                    //$" REPLACE(Volume, ',','.') as Volume, " +
                    $" cast(REPLACE(Volume, ',','.') as float) as Volume, " +
                    $" Date, " +
                    $" Number_mould " +
                    $" FROM [MFU].[dbo].[Volume_mess]" +
                    $" WHERE Number_fk = @idFk AND Side = N'Черновая'", sqlConnection);

                dataAdapter.SelectCommand.Parameters.AddWithValue("@idFK", comboBox1.SelectedItem);

                var db = new DataSet();

                chart1.Series[0].Points.Clear();
                chart1.Series[1].Points.Clear();

                if (dataAdapter.Fill(db) != 0)
                {
                    chart1.ChartAreas[0].AxisY.StripLines.Clear();

                    SqlCommand command = new SqlCommand(
                        "Select Set_min, Set_max FROM [MFU].[dbo].[Volume_ex] where N_fk = @idFk AND Actual = 1 ORDER BY Data_set DESC",
                        sqlConnection);
                    command.Parameters.AddWithValue("@idFK", comboBox1.SelectedItem);

                    var linesTrend = command.ExecuteReader();
                    if (linesTrend.HasRows)
                    {
                        linesTrend.Read();
                        StripLine line1 = new StripLine();
                        StripLine line2 = new StripLine();
                        StripLine line3 = new StripLine();
                        line1.Interval = 0;
                        line2.Interval = 0;
                        line3.Interval = 0;
                        line2.BackColor = line1.BackColor = Color.FromArgb(50,111,98,112);
                        line3.BackColor = Color.FromArgb(150,87,162,161);
                        line1.StripWidth = Convert.ToDouble(linesTrend["Set_min"]);
                        line2.StripWidth = 40;
                        line3.StripWidth = Convert.ToDouble(linesTrend["Set_max"]) - Convert.ToDouble(linesTrend["Set_min"]);
                        line1.IntervalOffset = 0;
                        line2.IntervalOffset = Convert.ToDouble(linesTrend["Set_max"]);
                        line3.IntervalOffset = Convert.ToDouble(linesTrend["Set_min"]);
                        chart1.ChartAreas[0].AxisY.StripLines.Add(line1);
                        chart1.ChartAreas[0].AxisY.StripLines.Add(line2);
                        chart1.ChartAreas[0].AxisY.StripLines.Add(line3);

                        // ОБЪЁМ ---->
                        var avg = ((Convert.ToDouble(linesTrend["Set_max"]) + Convert.ToDouble(linesTrend["Set_min"])) / 2);
                        _deltaVolume = Convert.ToDouble(linesTrend["Set_max"]) - avg;

                        vol.Text = avg + @"  ( +/-" + _deltaVolume + @" )";
                        // -----<

                        chart1.ChartAreas[0].AxisY.Minimum = avg - 0.5;
                        chart1.ChartAreas[0].AxisY.Maximum = avg + 0.5; 

                        linesTrend.Close();
                    }
                    else
                    {
                        MessageBox.Show(@"Для выбранного формокомплекта нет средних значений", @"Отсутствуют данные",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }

                    chart1.Series[2].Points.Clear();

                    if (chart1Type1.Checked)
                    {
                        foreach (var mold in _moulds)
                        {
                            var result = db.Tables[0].AsEnumerable()
                                .OrderByDescending(r => r.Field<DateTime>("Date"))
                                .Where(r => r.Field<string>("Number_mould") == mold.ToString())
                                .Select(r => r.Field<Double>("Volume")).ToArray();

                            if (result.Length > 0)
                            {
                                chart1.Series[1].Points.AddXY(mold, result[0]);
                                chart1.Series[2].Points.AddXY(mold, Convert.ToDouble(vol.Text.Split('(')[0]));
                            }
                                
                            else
                            {
                                chart1.Series[1].Points.AddXY(mold, 0);
                                chart1.Series[2].Points.AddXY(mold, Convert.ToDouble(vol.Text.Split('(')[0]));
                            }
                                

                            if (result.Length > 1)
                            {
                                chart1.Series[0].Points.AddXY(mold, result[1]);
                            }

                            else
                            {
                                 chart1.Series[0].Points.AddXY(mold, 0);
                                //chart1.Series[2].Points.AddXY(mold, Convert.ToDouble(vol.Text.Split('(')[0]));
                            }

                        }
                    }
                    else
                    {
                        foreach (var mold in _moulds)
                        {
                            var result = db.Tables[0].AsEnumerable()
                                .OrderByDescending(r => r.Field<DateTime>("Date"))
                                .Where(r => r.Field<string>("Number_mould") == mold.ToString())
                                .Select(r => r.Field<Double>("Volume")).ToArray();
                            
                            switch (result.Length)
                            {
                                case 0:
                                    _dots.Add(new DotVolume() {X = mold, Y1 = 0, Y2 = 0});
                                    break;
                                case 1:
                                    _dots.Add(new DotVolume() {X = mold, Y1 = result[0], Y2 = 0});
                                    break;
                                case 2:
                                    _dots.Add(new DotVolume()
                                        {X = mold, Y1 = result[1], Y2 = result[0]});
                                    break;
                                default:
                                    _dots.Add(new DotVolume()
                                        {X = mold, Y1 = result[1], Y2 = result[0]});
                                    break;
                            }
                        }
                        _dots.Sort((one, two) => one.Y2.CompareTo(two.Y2));
                        _dots.ForEach(RePaintWeight);
                    }


                    var minValue = chart1.Series[0].Points.AsEnumerable()
                        .Where(r => r.YValues[0] > 0)
                        .Min(r => r.YValues[0]) - _deltaVolume;

                   // chart1.ChartAreas[0].AxisY.Minimum = minValue;
                   // chart1.ChartAreas[0].AxisY.Maximum = chart1.Series[0].Points.FindMaxByValue("Y", 0).YValues[0] + _deltaVolume;

                    chart1.ChartAreas[0].AxisX.Interval = 1;
                    chart1.Series[0].IsXValueIndexed = true;
                    chart1.Series[1].IsXValueIndexed = true;
                    chart1.Series[2].IsXValueIndexed = true;

                }
                else
                {
                    MessageBox.Show(@"Отсутствуют данные для графика объема", @"Отсутствуют данные",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Ошибка построения графика объема. Описание ошибки: " + ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private void WeightChart()
        {
            var sqlConnection = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
            try
            {
                sqlConnection.Open();
                SqlDataAdapter dataAdapter;

                if (chart2Type1.Checked)
                {
                    dataAdapter = new SqlDataAdapter(
                        $" SELECT" +
                        $" CAST(REPLACE(Level_b, ',', '.') as float) AS Level_b," +
                        $" Num_form," +
                        $" Date," +
                        $" Assortment" +
                        $" FROM" +
                        $" ( SELECT" +
                        $" Date, Level_b, Num_form, Assortment" +
                        $" FROM" +
                        $" [OTK].[dbo].[Weight_Control_2]" +
                        $" WHERE" +
                        $" Num_f = @idFk and Level_b <> 0" +
                        $" UNION SELECT" +
                        $" Date, Level_b, Num_form, Assortment" +
                        $" FROM" +
                        $" [OTK].[dbo].[Weight_Control_3]" +
                        $" WHERE" +
                        $" Num_f = @idFk and Level_b <> 0) b", sqlConnection);

                    dataAdapter.SelectCommand.Parameters.AddWithValue("@idFK", comboBox1.SelectedItem);
                }
                else
                {
                     dataAdapter = new SqlDataAdapter(
                        $" SELECT " +
                        $" cast(REPLACE(Volume, ',','.') as float) as Volume, " +
                        $" Date, " +
                        $" Number_mould " +
                        $" FROM [MFU].[dbo].[Volume_mess]" +
                        $" WHERE Number_fk = @idFk AND Side = N'Черновая'", sqlConnection);

                    dataAdapter.SelectCommand.Parameters.AddWithValue("@idFK", comboBox1.SelectedItem);
                }

                var db = new DataSet();

                chart2.Series[0].Points.Clear();
                chart2.Series[1].Points.Clear();
                chart2.Series[2].Points.Clear();
                chart2.Series[3].Points.Clear();


                if (dataAdapter.Fill(db) != 0)
                {
                    double[] res = chart2Type1.Checked ? null : GetValForCalcVolume(comboBox1.SelectedItem.ToString(), sqlConnection);

                    if (chart1Type1.Checked)
                    {
                        double[] result;
                        foreach (var mold in _moulds)
                        {
                            if (chart2Type1.Checked)
                            {
                                result = db.Tables[0].AsEnumerable()
                                    .OrderByDescending(r => r.Field<DateTime>("Date"))
                                    .Where(r => r.Field<string>("Num_form") == mold.ToString())
                                    .Select(r => r.Field<Double>("Level_b")).ToArray();
                            }
                            else
                            {
                                result = db.Tables[0].AsEnumerable()
                                    .OrderByDescending(r => r.Field<DateTime>("Date"))
                                    .Where(r => r.Field<string>("Number_mould") == mold.ToString())
                                    .Select(r => r.Field<Double>("Volume")).ToArray();
                            }

                            if (result.Length > 2)
                                chart2.Series[0].Points.AddXY(mold, CalcVolume(result[2], res));
                            else
                                chart2.Series[0].Points.AddXY(mold, 0);

                            if (result.Length > 1)
                                chart2.Series[1].Points.AddXY(mold, CalcVolume(result[1], res));
                            else
                                chart2.Series[1].Points.AddXY(mold, 0);

                            if (result.Length > 0)
                                chart2.Series[2].Points.AddXY(mold, CalcVolume(result[0], res));
                            else
                                chart2.Series[2].Points.AddXY(mold, 0);

                            chart2.Series[3].Points.AddXY(mold, Convert.ToDouble(cap.Text.Split('(')[0]));

                        }
                    }
                    else
                    {
                        double[] result;
                        foreach (var dot in _dots)
                        {
                            if (chart2Type1.Checked)
                            {
                                result = db.Tables[0].AsEnumerable()
                                    .OrderByDescending(r => r.Field<DateTime>("Date"))
                                    .Where(r => r.Field<string>("Num_form") == dot.X.ToString())
                                    .Select(r => r.Field<Double>("Level_b")).ToArray();
                            }
                            else
                            {
                                result = db.Tables[0].AsEnumerable()
                                    .OrderByDescending(r => r.Field<DateTime>("Date"))
                                    .Where(r => r.Field<string>("Number_mould") == dot.X.ToString())
                                    .Select(r => r.Field<Double>("Volume")).ToArray();
                            }

                            if (result.Length > 2)
                                chart2.Series[0].Points.AddXY(dot.X, CalcVolume(result[2], res));
                            else
                                chart2.Series[0].Points.AddXY(dot.X, 0);

                            if (result.Length > 1)
                                chart2.Series[1].Points.AddXY(dot.X, CalcVolume(result[1], res));
                            else
                                chart2.Series[1].Points.AddXY(dot.X, 0);

                            if (result.Length > 0)
                                chart2.Series[2].Points.AddXY(dot.X, CalcVolume(result[0], res));
                            else
                                chart2.Series[2].Points.AddXY(dot.X, 0);

                            chart2.Series[3].Points.AddXY(dot.X, Convert.ToDouble(cap.Text.Split('(')[0]));

                        }
                    }

                    chart2.ChartAreas[0].AxisX.Interval = 1;
                    chart2.Series[0].IsXValueIndexed = true;
                    chart2.Series[1].IsXValueIndexed = true;
                    chart2.Series[2].IsXValueIndexed = true;
                    chart2.Series[3].IsXValueIndexed = true;


                    var delta = chart2Type2.Checked ? _deltaVolume : _deltaWeight;

                   // var minValue = chart2.Series[0].Points.AsEnumerable()
                   //     .Where(r => r.YValues[0] > 0)
                   //     .Min(r => r.YValues[0]) - _deltaWeight;

                    //chart2.ChartAreas[0].AxisY.Minimum = minValue;
                    //chart2.ChartAreas[0].AxisY.Maximum = chart2.Series[0].Points.FindMaxByValue("Y", 0).YValues[0] + _deltaWeight;

                }
                else
                {
                    MessageBox.Show(@"Отсутствуют данные для графика объема", @"Отсутствуют данные",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Ошибка построения графика объема. Описание ошибки: " + ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private double CalcVolume(double value, double[] res)
        {
            return chart2Type1.Checked ? value : value - (res[0] - res[1])/2.51;
        }

        private double[] GetValForCalcVolume(string idFK, SqlConnection sqlConnection)
        {
            var dataCommand = new SqlCommand(
                $" SELECT TOP(1) Weight_b FROM (" +
                $" SELECT" +
                $" [OTK].[dbo].[Weight_Control_2].Weight_b, [OTK].[dbo].[Weight_Control_2].Date" +
                $" FROM" +
                $" [OTK].[dbo].[Weight_Control_2]" +
                $" WHERE" +
                $" [OTK].[dbo].[Weight_Control_2].Num_f = @idFK" +
                $" UNION SELECT" +
                $" [OTK].[dbo].[Weight_Control_3].Weight_b, [OTK].[dbo].[Weight_Control_3].Date" +
                $" FROM" +
                $" [OTK].[dbo].[Weight_Control_3]" +
                $" WHERE" +
                $" [OTK].[dbo].[Weight_Control_3].Num_f = @idFK) b" +
                $" ORDER BY" +
                $" Date DESC", sqlConnection);
            dataCommand.Parameters.AddWithValue("@idFK", comboBox1.SelectedItem);

            var Weight_b = dataCommand.ExecuteScalar().ToString();

            if (Weight_b == "0")
            {
                MessageBox.Show(@"Нет данных измеренного веса для выбранного формокомплекта. В расчетах используется = 0", @"Отсутствуют данные",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            dataCommand = new SqlCommand(
                $" SELECT TOP(1) W_nom FROM (" +
                $" SELECT" +
                $" [OTK].[dbo].[Link_CPS2].W_nom, [OTK].[dbo].[Link_CPS2].DT_out" +
                $" FROM" +
                $" [OTK].[dbo].[Link_CPS2]" +
                $" WHERE" +
                $" [OTK].[dbo].[Link_CPS2].Num_m = @idFK" +
                $" UNION SELECT" +
                $" [OTK].[dbo].[Link_CPS3].W_nom, [OTK].[dbo].[Link_CPS3].DT_out" +
                $" FROM" +
                $" [OTK].[dbo].[Link_CPS3]" +
                $" WHERE" +
                $" [OTK].[dbo].[Link_CPS3].Num_m = @idFK) b" +
                $" ORDER BY" +
                $" DT_out DESC", sqlConnection);
            dataCommand.Parameters.AddWithValue("@idFK", comboBox1.SelectedItem);

            var W_nom = dataCommand.ExecuteScalar().ToString();

            if (W_nom == "0")
            {
                MessageBox.Show(@"Не данных номинального веса для выбранного формокомплекта. В расчетах используется = 0", @"Отсутствуют данные",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return new double[2] {Convert.ToDouble(W_nom.Replace(".", ",")), Convert.ToDouble(Weight_b.Replace(".", ",")) };
        }

        private void RePaintWeight(DotVolume dot)
        {
            chart1.Series[2].Points.AddXY(dot.X, Convert.ToDouble(vol.Text.Split('(')[0]));
            chart1.Series[0].Points.AddXY(dot.X, dot.Y1);
            chart1.Series[1].Points.AddXY(dot.X, dot.Y2);
           

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            _moulds.Clear();

            var sqlConnection = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
            try
            {
                sqlConnection.Open();

                SqlCommand dataCommand = new SqlCommand(
                    $" SELECT * FROM (" +
                    $" SELECT" +
                    $" [MFU].[dbo].[Volume_mess].Number_mould" +
                    $" FROM" +
                    $" [MFU].[dbo].[Volume_mess]" +
                    $" WHERE" +
                    $" [MFU].[dbo].[Volume_mess].Number_fk = @idFK" +
                    $" UNION SELECT" +
                    $" [OTK].[dbo].[Weight_Control_2].Num_form" +
                    $" FROM" +
                    $" [OTK].[dbo].[Weight_Control_2]" +
                    $" WHERE" +
                    $" [OTK].[dbo].[Weight_Control_2].Num_f = @idFK" +
                    $" UNION SELECT" +
                    $" [OTK].[dbo].[Weight_Control_3].Num_form" +
                    $" FROM" +
                    $" [OTK].[dbo].[Weight_Control_3]" +
                    $" WHERE" +
                    $" [OTK].[dbo].[Weight_Control_3].Num_f = @idFK ) b" +
                    $" ORDER BY" +
                    $" len(Number_mould), Number_mould", sqlConnection);
                dataCommand.Parameters.AddWithValue("@idFK", comboBox1.SelectedItem);

                SqlDataReader reader = dataCommand.ExecuteReader();


                if (reader.HasRows)
                    while (reader.Read())
                        _moulds.Add(Convert.ToDouble(reader.GetValue(0)));
                else
                {
                    MessageBox.Show(@"В таблицах [MFU].[dbo].[Volume_mess], [OTK].[dbo].[Weight_Control_2], [OTK].[dbo].[Weight_Control_3] отсутствуют данные. Не удалось выбать формокомплеты", @"Отсутствуют данные",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                reader.Close();

                // АССОРТИМЕНТ И ДАТА --->
                dataCommand = new SqlCommand(
                    $" SELECT TOP(1) Name_a, CONVERT(DATETIME, Date) as Date FROM (" +
                    $" SELECT" +
                    $" [OTK].[dbo].[Weight_Control_2].Assortment, [OTK].[dbo].[Weight_Control_2].Date" +
                    $" FROM" +
                    $" [OTK].[dbo].[Weight_Control_2]" +
                    $" WHERE" +
                    $" [OTK].[dbo].[Weight_Control_2].Num_f = @idFK" +
                    $" UNION SELECT" +
                    $" [OTK].[dbo].[Weight_Control_3].Assortment, [OTK].[dbo].[Weight_Control_3].Date" +
                    $" FROM" +
                    $" [OTK].[dbo].[Weight_Control_3]" +
                    $" WHERE" +
                    $" [OTK].[dbo].[Weight_Control_3].Num_f = @idFK) b" +
                    $" LEFT JOIN " +
                    $" [OTK].[dbo].[Table_assortment] " +
                    $" ON  b.Assortment = [OTK].[dbo].[Table_assortment].Id_a " +
                    $" ORDER BY" +
                    $" b.Date DESC", sqlConnection);

                dataCommand.Parameters.AddWithValue("@idFK", comboBox1.SelectedItem);
                reader = dataCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    nameFK.Text = reader[0].ToString();
                    var txt = reader[1].ToString();
                    date.Text = Convert.ToDateTime(txt).ToString("dd-MM-yyyy");
                }
                reader.Close();
                // ----<

                // ВЕС ВМЕСТИМОСТЬ ДАТА --->
                dataCommand = new SqlCommand(
                    $" SELECT TOP(1) W_nom, Cap_Nom, DT_out, Cap_Nmax, W_min, W_max, Cap_Nmin FROM (" +
                    $" SELECT" +
                    $" [OTK].[dbo].[Link_CPS2].W_nom, [OTK].[dbo].[Link_CPS2].Cap_Nom, [OTK].[dbo].[Link_CPS2].DT_out, [OTK].[dbo].[Link_CPS2].Cap_Nmax, [OTK].[dbo].[Link_CPS2].Cap_Nmin, W_min, W_max" +
                    $" FROM" +
                    $" [OTK].[dbo].[Link_CPS2]" +
                    $" WHERE" +
                    $" [OTK].[dbo].[Link_CPS2].Num_m = @idFK" +
                    $" UNION SELECT" +
                    $" [OTK].[dbo].[Link_CPS3].W_nom, [OTK].[dbo].[Link_CPS3].Cap_Nom, [OTK].[dbo].[Link_CPS3].DT_out, [OTK].[dbo].[Link_CPS3].Cap_Nmax, [OTK].[dbo].[Link_CPS3].Cap_Nmin, W_min, W_max" +
                    $" FROM" +
                    $" [OTK].[dbo].[Link_CPS3]" +
                    $" WHERE" +
                    $" [OTK].[dbo].[Link_CPS3].Num_m = @idFK) b" +
                    $" ORDER BY" +
                    $" b.DT_out DESC", sqlConnection);

                dataCommand.Parameters.AddWithValue("@idFK", comboBox1.SelectedItem);

                reader = dataCommand.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    _deltaWeight = Convert.ToDouble(reader[5]) - Convert.ToDouble(reader[0]);
                    weight.Text = reader[0] + @"  ( +/-" + _deltaWeight + @" )";

                    var delta1 = Convert.ToDouble(reader[3]) - Convert.ToDouble(reader[1]);
                    cap.Text = reader[1] + @"  ( +/-" + delta1 + @" )";

                    chart2.ChartAreas[0].AxisY.Minimum = Convert.ToDouble(reader[6]) - delta1;
                    chart2.ChartAreas[0].AxisY.Maximum = Convert.ToDouble(reader[3]) + delta1;


                    // ЛИНИИ НА ГРАФИКЕ 2 ----->

                    chart2.ChartAreas[0].AxisY.StripLines.Clear();

                    if (chart2Type1.Checked)
                    {
                        StripLine line1 = new StripLine();
                        StripLine line2 = new StripLine();
                        StripLine line3 = new StripLine();
                        line1.Interval = 0;
                        line2.Interval = 0;
                        line3.Interval = 0;
                        line2.BackColor = line1.BackColor = Color.FromArgb(50, 111, 98, 112);
                        line3.BackColor = Color.FromArgb(150, 87, 162, 161);
                        line1.StripWidth = Convert.ToDouble(reader[6]);
                        line2.StripWidth = 40;
                        line3.StripWidth = Convert.ToDouble(reader[3]) - Convert.ToDouble(reader[6]);
                        line1.IntervalOffset = 0;
                        line2.IntervalOffset = Convert.ToDouble(reader[3]);
                        line3.IntervalOffset = Convert.ToDouble(reader[6]);
                        chart2.ChartAreas[0].AxisY.StripLines.Add(line1);
                        chart2.ChartAreas[0].AxisY.StripLines.Add(line2);
                        chart2.ChartAreas[0].AxisY.StripLines.Add(line3);
                    }
                    // ----->

                    reader.Close();
                }
                else
                {
                    MessageBox.Show(@"Для выбранного формокомплекта не заданы ВМЕСТИМОСТЬ и/или ВЕС", @"Отсутствуют данные",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                // PAINT CHARTS --->
                VolumeChart();
                WeightChart();
                panel1.Visible = true;
                // ---<

            }
            catch (Exception ex)
            {
                MessageBox.Show(@"Ошибка выбора форм для формокомплекта. Описание ошибки: " + ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }

           
        }

        private void chart1Type2_CheckedChanged(object sender, EventArgs e)
        {
            VolumeChart();
            //WeightChart();
        }

        private void chart2Type1_CheckedChanged(object sender, EventArgs e)
        {
            //if(chart2Type2.Checked)
            //   chart2.ChartAreas[0].AxisY.StripLines.Clear();
            WeightChart();

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            _ = checkBox1.Checked ? chart1.Series[0].Enabled = false : chart1.Series[0].Enabled = true;
            
        }
    }
}
