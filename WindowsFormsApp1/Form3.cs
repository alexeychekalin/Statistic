using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using WindowsFormsApp1.Properties;
using Excel = Microsoft.Office.Interop.Excel;

namespace WindowsFormsApp1
{
    public partial class Form3 : Form
    {
        List<double> _moulds = new List<double>();
        List<DotVolume> _dots = new List<DotVolume>();
        List<DotWeight> _dotsWeight = new List<DotWeight>();
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

        public class DotWeight
        {
            public double X;
            public double Y1;
            public double Y2;
            public double Y3;
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
            _dotsWeight.Clear();
            var sqlConnection = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
            try
            {
                sqlConnection.Open();
                SqlDataAdapter dataAdapter;

               // if (chart2Type1.Checked)
               // {
                    dataAdapter = new SqlDataAdapter(
                        $" SELECT" +
                        $" CAST(REPLACE(Level_b, ',', '.') as float) AS Level_b," +
                        $" Num_form," +
                        $" Date," +
                        $" Assortment," +
                        $" Weight_b" +
                        $" FROM" +
                        $" ( SELECT" +
                        $" Date, Level_b, Num_form, Assortment, Weight_b" +
                        $" FROM" +
                        $" [OTK].[dbo].[Weight_Control_2]" +
                        $" WHERE" +
                        $" Num_f = @idFk and Level_b <> 0" +
                        $" UNION SELECT" +
                        $" Date, Level_b, Num_form, Assortment, Weight_b" +
                        $" FROM" +
                        $" [OTK].[dbo].[Weight_Control_3]" +
                        $" WHERE" +
                        $" Num_f = @idFk and Level_b <> 0) b", sqlConnection);

                    dataAdapter.SelectCommand.Parameters.AddWithValue("@idFK", comboBox1.SelectedItem);
                //}
                /*
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
                */

                var db = new DataSet();

                chart2.Series[0].Points.Clear();
                chart2.Series[1].Points.Clear();
                chart2.Series[2].Points.Clear();
                chart2.Series[3].Points.Clear();
    

                if (dataAdapter.Fill(db) != 0)
                {
                    double res = chart2Type1.Checked ? 0 : GetValForCalcVolume(comboBox1.SelectedItem.ToString(), sqlConnection);

                    if (chart1Type1.Checked)
                    {
                        double[] result;
                        foreach (var mold in _moulds)
                        {
                            result = db.Tables[0].AsEnumerable()
                                .OrderByDescending(r => r.Field<DateTime>("Date"))
                                .Where(r => r.Field<string>("Num_form") == mold.ToString())
                                .Select(r => r.Field<Double>("Level_b")).ToArray();

                            var mf = db.Tables[0].AsEnumerable()
                                .OrderByDescending(r => r.Field<DateTime>("Date"))
                                .Where(r => r.Field<string>("Num_form") == mold.ToString())
                                .Select(r => r.Field<Double>("Weight_b")).ToArray();

                            /*
                            if (result.Length > 2)
                                chart2.Series[0].Points.AddXY(mold, CalcVolume(result[2], new double[] {res, mf[0] }));
                            else
                                chart2.Series[0].Points.AddXY(mold, 0);

                            if (result.Length > 1)
                                chart2.Series[1].Points.AddXY(mold, CalcVolume(result[1], new double[] { res, mf[0] }));
                            else
                                chart2.Series[1].Points.AddXY(mold, 0);

                            if (result.Length > 0)
                                chart2.Series[2].Points.AddXY(mold, CalcVolume(result[0], new double[] { res, mf[0] }));
                            else
                                chart2.Series[2].Points.AddXY(mold, 0);

                            chart2.Series[3].Points.AddXY(mold, Convert.ToDouble(cap.Text.Split('(')[0]));
                            */
                            var newDot = new DotWeight();
                            newDot.X = mold;

                            if (result.Length > 2)
                                newDot.Y3 = CalcVolume(result[2], new double[] { res, mf[0] }); //chart2.Series[0].Points.AddXY(dot.X, CalcVolume(result[2], new double[] { res, mf[0] }));
                            else
                                newDot.Y3 = 0; //chart2.Series[0].Points.AddXY(dot.X, 0);

                            if (result.Length > 1)
                                newDot.Y2 = CalcVolume(result[1], new double[] { res, mf[0] }); //chart2.Series[1].Points.AddXY(dot.X, CalcVolume(result[1], new double[] { res, mf[0] }));
                            else
                                newDot.Y2 = 0; //chart2.Series[1].Points.AddXY(dot.X, 0);

                            if (result.Length > 0)
                                newDot.Y1 = CalcVolume(result[0], new double[] { res, mf[0] }); //chart2.Series[2].Points.AddXY(dot.X, CalcVolume(result[0], new double[] { res, mf[0] }));
                            else
                                newDot.Y1 = 0; //chart2.Series[2].Points.AddXY(dot.X, 0);

                            // chart2.Series[3].Points.AddXY(dot.X, Convert.ToDouble(cap.Text.Split('(')[0]));
                            _dotsWeight.Add(newDot);
                        }
                    }
                    else
                    {
                        double[] result;
                        foreach (var dot in _dots)
                        {
                            result = db.Tables[0].AsEnumerable()
                                .OrderByDescending(r => r.Field<DateTime>("Date"))
                                .Where(r => r.Field<string>("Num_form") == dot.X.ToString())
                                .Select(r => r.Field<Double>("Level_b")).ToArray();

                            var mf = db.Tables[0].AsEnumerable()
                                .OrderByDescending(r => r.Field<DateTime>("Date"))
                                .Where(r => r.Field<string>("Num_form") == dot.X.ToString())
                                .Select(r => r.Field<Double>("Weight_b")).ToArray();

                            var newDot = new DotWeight();
                            newDot.X = dot.X;

                            if (result.Length > 2)
                                newDot.Y3 = CalcVolume(result[2], new double[] { res, mf[0] }); //chart2.Series[0].Points.AddXY(dot.X, CalcVolume(result[2], new double[] { res, mf[0] }));
                            else
                               newDot.Y3 = 0; //chart2.Series[0].Points.AddXY(dot.X, 0);

                            if (result.Length > 1)
                               newDot.Y2 = CalcVolume(result[1], new double[] { res, mf[0] }); //chart2.Series[1].Points.AddXY(dot.X, CalcVolume(result[1], new double[] { res, mf[0] }));
                            else
                               newDot.Y2 = 0; //chart2.Series[1].Points.AddXY(dot.X, 0);

                            if (result.Length > 0)
                                newDot.Y1 = CalcVolume(result[0], new double[] { res, mf[0] }); //chart2.Series[2].Points.AddXY(dot.X, CalcVolume(result[0], new double[] { res, mf[0] }));
                            else
                                newDot.Y1 = 0; //chart2.Series[2].Points.AddXY(dot.X, 0);

                           // chart2.Series[3].Points.AddXY(dot.X, Convert.ToDouble(cap.Text.Split('(')[0]));
                            _dotsWeight.Add(newDot);
                        }
                    }

                    if(ascOrder.Checked) _dotsWeight.Sort((one, two) => one.Y3.CompareTo(two.Y3));

                    _dotsWeight.ForEach(RePaintSecond);

                    chart2.ChartAreas[0].AxisX.Interval = 1;
                    chart2.Series[0].IsXValueIndexed = true;
                    chart2.Series[1].IsXValueIndexed = true;
                    chart2.Series[2].IsXValueIndexed = true;
                    chart2.Series[3].IsXValueIndexed = true;


                    var delta = chart2Type2.Checked ? _deltaVolume : _deltaWeight;

                    var minValue = chart2.Series[0].Points.AsEnumerable()
                        .Where(r => r.YValues[0] > 0)
                        .Min(r => r.YValues[0]) - _deltaWeight;

                    chart2.ChartAreas[0].AxisY.Minimum = minValue;
                    chart2.ChartAreas[0].AxisY.Maximum = chart2.Series[0].Points.FindMaxByValue("Y", 0).YValues[0] + _deltaWeight;

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
            return chart2Type1.Checked ? value : Math.Round(value - (res[0] - res[1])/2.51, 1);
        }

        private double GetValForCalcVolume(string idFK, SqlConnection sqlConnection)
        {
            /*
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
            */
            var dataCommand = new SqlCommand(
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

            return Convert.ToDouble(W_nom.Replace(".", ","));
        }

        private void RePaintWeight(DotVolume dot)
        {
            chart1.Series[2].Points.AddXY(dot.X, Convert.ToDouble(vol.Text.Split('(')[0]));
            chart1.Series[0].Points.AddXY(dot.X, dot.Y1);
            chart1.Series[1].Points.AddXY(dot.X, dot.Y2);
        }

        private void RePaintSecond(DotWeight dot)
        {
            chart2.Series[3].Points.AddXY(dot.X, Convert.ToDouble(vol.Text.Split('(')[0]));
            chart2.Series[0].Points.AddXY(dot.X, dot.Y1);
            chart2.Series[1].Points.AddXY(dot.X, dot.Y2);
            chart2.Series[2].Points.AddXY(dot.X, dot.Y3);
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

        private void button6_Click(object sender, EventArgs e)
        {
            //new Karta0209(comboBox1.Text);
            DrawExcel(comboBox1.Text);


        }

        private void fillColor(int r, int c, string val, string eq, Excel.Worksheet wh)
        {
            var color = Color.White;
            if (Convert.ToDouble(val) > Convert.ToDouble(eq) + 0.25)
                color = Color.Red;
            else if (Convert.ToDouble(val) < Convert.ToDouble(eq) - 0.25)
                color = Color.Chocolate;
            Excel.Range range = wh.Cells[r, c];
            range.Interior.Color = color;
        }
        Color[] myColor = new Color[] { Color.LightBlue, Color.LightCoral, Color.LightGreen, Color.LightCyan, Color.LightSeaGreen, Color.LightSalmon, Color.LightSlateGray, Color.LightYellow, Color.LightSteelBlue };
        private void DrawExcel(string idfk)
        {
            var conn = DBWalker.GetConnection(Resources.Server, Resources.User, Resources.Password, Resources.secure);
            conn.Open();
            var sql = @"select 
                            (cast(REPLACE(Set_min, ',','.') as float) + cast(REPLACE(Set_max, ',','.') as float)) / 2 as avgval,
		                    Data_set,
                            (
		                        select MIN(CONVERT(datetime, [Data_set], 105))
                                from [MFU].[dbo].[Volume_ex] t2
                                where t2.N_fk = @idfk and  
                                      CONVERT(datetime, t2.[Data_set], 105) > CONVERT(datetime, t.[Data_set], 105)
                             ) as endDate
                        from [MFU].[dbo].[Volume_ex] t
                        where t.N_fk = @idfk order by CONVERT(smalldatetime, t.[Data_set], 103)";
            var command = new SqlCommand(sql, conn);
            command.Parameters.AddWithValue("@idfk", idfk);
            var reader = command.ExecuteReader();

            var rows = new DataTable();
            rows.Load(reader);

            reader.Close();

            int colorCount = 0;
            int cnt = 2;

            Excel.Application excelApp = new Excel.Application();
            //  Excel.Workbook workbook = excelApp.Workbooks.Open(@"D:\ISKRA\Statistic_Git\отчет 31_07_23\WindowsFormsApp1\bin\Release\0209.xlsx");
            Excel.Workbook workbook = excelApp.Workbooks.Open(Environment.CurrentDirectory + "\\0209.xlsx");

            Excel.Worksheet worksheet = workbook.ActiveSheet;

            int newRows = 0;

            if (rows.Rows.Count >= 2)
            {
                for (int n = 0; n < rows.Rows.Count - 1; n++)
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
                worksheet.Cells[newRows + 8 + r, 1] = r + 1;
            }

            worksheet.Cells[1, 2] = idfk;
            worksheet.Cells[2, 2] = "-";
            worksheet.Cells[4, 2] = DateTime.Now.ToString("dd-MM-yyyy");

            // rowMergeView2.Rows[1].Cells[0].Value = "Номер формы";
            // rowMergeView2.Rows[1].Cells[1].Value = "Последний замер";

            foreach (DataRow row in rows.Rows)
            {
                worksheet.Cells[3 + colorCount, 4] = row["avgval"];
                Excel.Range range = worksheet.Cells[3 + colorCount, 4];
                range.Interior.Color = myColor[colorCount];
                worksheet.Cells[3 + colorCount, 6] = row["Data_Set"];
                worksheet.Cells[3 + colorCount, 8] = row["endDate"].ToString();


                //if (row.Cells.Count == 0) return;
                sql = @"set language us_english; 
                    SELECT 
		            MAX(CONVERT(VARCHAR(10), Date, 104) ) as date
                    FROM [MFU].[dbo].[Volume_mess]
                    where Number_fk = @idfk and [Date] between @start and @end group by CAST(Date AS DATE) order by CAST(Date AS DATE)";

                command = new SqlCommand(sql, conn);
                command.Parameters.AddWithValue("@idfk", idfk);

                string start = DateTime.Parse(row["Data_Set"].ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                string end = row["endDate"].ToString() == "" ? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") : DateTime.Parse(row["endDate"].ToString()).AddMinutes(1).ToString("yyyy-MM-dd HH:mm:ss");
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

                worksheet.Range[worksheet.Cells[newRows + 5, cnt + 1], worksheet.Cells[newRows + 5, cnt + dt.Rows.Count]].Merge();
                worksheet.Range[worksheet.Cells[newRows + 5, cnt + 1], worksheet.Cells[newRows + 5, cnt + dt.Rows.Count]].Interior.Color = myColor[colorCount];
                worksheet.Range[worksheet.Cells[newRows + 5, cnt + 1], worksheet.Cells[newRows + 5, cnt + dt.Rows.Count]] = "скорректированный и/или установленный обьем " + row["avgval"] + " +-0,25";


                foreach (DataRow tabRow in dt.Rows)
                {
                    // rowMergeView2.Columns.Add(new DataGridViewColumn() { CellTemplate = new DataGridViewTextBoxCell() });
                    // rowMergeView2.Rows[0].Cells[cnt].Style.BackColor = myColor[colorCount];
                    // rowMergeView2.Rows[1].Cells[cnt].Value = tabRow["date"];

                    worksheet.Cells[newRows + 7, cnt + 1] = tabRow["date"];
                    range = worksheet.Cells[newRows + 7, cnt + 1];
                    range.Interior.Color = myColor[colorCount];

                    //rowMergeView2.Rows[Convert.ToInt32(el.Field<string>("Number_mould")) + 1].Cells[cnt].Value = el.Field<string>("Volume");
                    dt2.AsEnumerable().Where(x => x.Field<string>("date") == tabRow["date"].ToString()).ToList().ForEach(el => { worksheet.Cells[Convert.ToInt32(el.Field<string>("Number_mould")) + newRows + 7, cnt + 1] = el.Field<string>("Volume"); fillColor(Convert.ToInt32(el.Field<string>("Number_mould")) + newRows + 7, cnt + 1, el.Field<string>("Volume"), row["avgval"].ToString(), worksheet); });

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




        private void ascOrder_CheckedChanged(object sender, EventArgs e)
        {
            WeightChart();
        }
    }
}
