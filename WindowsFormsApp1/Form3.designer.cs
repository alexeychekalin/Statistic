
namespace WindowsFormsApp1
{
    partial class Form3
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series8 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series9 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series10 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series11 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series12 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series13 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series14 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.nameFK = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.weight = new System.Windows.Forms.Label();
            this.date = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.whe = new System.Windows.Forms.Label();
            this.vol = new System.Windows.Forms.Label();
            this.cap = new System.Windows.Forms.Label();
            this.chart1Type2 = new System.Windows.Forms.RadioButton();
            this.chart1Type1 = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.button6 = new System.Windows.Forms.Button();
            this.panel7 = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.chart2Type1 = new System.Windows.Forms.RadioButton();
            this.chart2Type2 = new System.Windows.Forms.RadioButton();
            this.panel8 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.panel9 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.ascOrder = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel7.SuspendLayout();
            this.panel6.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(27, 27);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(98, 23);
            this.comboBox1.TabIndex = 1;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(45, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(68, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Номер ФК";
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chart1.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.chart1.BorderlineColor = System.Drawing.Color.Black;
            chartArea3.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(98)))), ((int)(((byte)(112)))));
            chartArea3.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(98)))), ((int)(((byte)(112)))));
            chartArea3.AxisX.Title = "Номера форм";
            chartArea3.AxisX.TitleFont = new System.Drawing.Font("Comic Sans MS", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            chartArea3.AxisY.LineColor = System.Drawing.Color.DimGray;
            chartArea3.AxisY.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chartArea3.AxisY.Title = "Объем";
            chartArea3.AxisY.TitleFont = new System.Drawing.Font("Comic Sans MS", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            chartArea3.BackColor = System.Drawing.SystemColors.ControlLight;
            chartArea3.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea3);
            legend3.Enabled = false;
            legend3.Name = "Legend1";
            this.chart1.Legends.Add(legend3);
            this.chart1.Location = new System.Drawing.Point(0, 113);
            this.chart1.Name = "chart1";
            this.chart1.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            series8.BorderColor = System.Drawing.SystemColors.Control;
            series8.ChartArea = "ChartArea1";
            series8.Color = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(82)))), ((int)(((byte)(64)))));
            series8.Legend = "Legend1";
            series8.Name = "Series1";
            series8.SmartLabelStyle.CalloutBackColor = System.Drawing.Color.Black;
            series8.SmartLabelStyle.CalloutLineAnchorCapStyle = System.Windows.Forms.DataVisualization.Charting.LineAnchorCapStyle.Diamond;
            series8.SmartLabelStyle.CalloutStyle = System.Windows.Forms.DataVisualization.Charting.LabelCalloutStyle.Box;
            series8.SmartLabelStyle.IsMarkerOverlappingAllowed = true;
            series9.BorderColor = System.Drawing.SystemColors.Control;
            series9.ChartArea = "ChartArea1";
            series9.Color = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(63)))), ((int)(((byte)(105)))));
            series9.Legend = "Legend1";
            series9.Name = "Series2";
            series10.ChartArea = "ChartArea1";
            series10.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series10.Color = System.Drawing.Color.OrangeRed;
            series10.Legend = "Legend1";
            series10.Name = "Series3";
            this.chart1.Series.Add(series8);
            this.chart1.Series.Add(series9);
            this.chart1.Series.Add(series10);
            this.chart1.Size = new System.Drawing.Size(1684, 328);
            this.chart1.TabIndex = 3;
            this.chart1.Text = "chart2";
            // 
            // chart2
            // 
            this.chart2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.chart2.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.chart2.BorderlineColor = System.Drawing.Color.Black;
            chartArea4.AxisX.LineColor = System.Drawing.Color.DimGray;
            chartArea4.AxisX.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chartArea4.AxisX.Title = "Номера форм";
            chartArea4.AxisX.TitleFont = new System.Drawing.Font("Comic Sans MS", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            chartArea4.AxisY.LineColor = System.Drawing.Color.DimGray;
            chartArea4.AxisY.MajorGrid.LineColor = System.Drawing.Color.Gray;
            chartArea4.AxisY.Title = "Номинальная вместимость";
            chartArea4.AxisY.TitleFont = new System.Drawing.Font("Comic Sans MS", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            chartArea4.BackColor = System.Drawing.SystemColors.ControlLight;
            chartArea4.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea4);
            legend4.Enabled = false;
            legend4.Name = "Legend1";
            this.chart2.Legends.Add(legend4);
            this.chart2.Location = new System.Drawing.Point(0, 496);
            this.chart2.Name = "chart2";
            this.chart2.Palette = System.Windows.Forms.DataVisualization.Charting.ChartColorPalette.Bright;
            series11.BorderColor = System.Drawing.SystemColors.Control;
            series11.ChartArea = "ChartArea1";
            series11.Color = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(82)))), ((int)(((byte)(64)))));
            series11.Legend = "Legend1";
            series11.Name = "Series1";
            series12.BorderColor = System.Drawing.SystemColors.Control;
            series12.ChartArea = "ChartArea1";
            series12.Color = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            series12.Legend = "Legend1";
            series12.Name = "Series2";
            series13.BorderColor = System.Drawing.SystemColors.Control;
            series13.ChartArea = "ChartArea1";
            series13.Color = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(63)))), ((int)(((byte)(105)))));
            series13.Legend = "Legend1";
            series13.Name = "Series3";
            series14.ChartArea = "ChartArea1";
            series14.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series14.Color = System.Drawing.Color.OrangeRed;
            series14.Legend = "Legend1";
            series14.Name = "Series4";
            this.chart2.Series.Add(series11);
            this.chart2.Series.Add(series12);
            this.chart2.Series.Add(series13);
            this.chart2.Series.Add(series14);
            this.chart2.Size = new System.Drawing.Size(1684, 303);
            this.chart2.TabIndex = 4;
            this.chart2.Text = "chart1";
            // 
            // nameFK
            // 
            this.nameFK.AutoSize = true;
            this.nameFK.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nameFK.ForeColor = System.Drawing.SystemColors.Control;
            this.nameFK.Location = new System.Drawing.Point(23, 5);
            this.nameFK.Name = "nameFK";
            this.nameFK.Size = new System.Drawing.Size(58, 16);
            this.nameFK.TabIndex = 5;
            this.nameFK.Text = "nameFK";
            // 
            // label33
            // 
            this.label33.AutoSize = true;
            this.label33.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label33.ForeColor = System.Drawing.SystemColors.Control;
            this.label33.Location = new System.Drawing.Point(606, 5);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(113, 15);
            this.label33.TabIndex = 2;
            this.label33.Text = "Номинальный вес";
            // 
            // weight
            // 
            this.weight.AutoSize = true;
            this.weight.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.weight.ForeColor = System.Drawing.SystemColors.Control;
            this.weight.Location = new System.Drawing.Point(42, 4);
            this.weight.Name = "weight";
            this.weight.Size = new System.Drawing.Size(46, 16);
            this.weight.TabIndex = 5;
            this.weight.Text = "weight";
            this.weight.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // date
            // 
            this.date.AutoSize = true;
            this.date.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.date.ForeColor = System.Drawing.SystemColors.Control;
            this.date.Location = new System.Drawing.Point(36, 4);
            this.date.Name = "date";
            this.date.Size = new System.Drawing.Size(35, 16);
            this.date.TabIndex = 5;
            this.date.Text = "date";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.ForeColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(779, 5);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(239, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Установленный обьем черновых форм ";
            // 
            // whe
            // 
            this.whe.AutoSize = true;
            this.whe.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.whe.ForeColor = System.Drawing.SystemColors.Control;
            this.whe.Location = new System.Drawing.Point(1040, 5);
            this.whe.Name = "whe";
            this.whe.Size = new System.Drawing.Size(170, 15);
            this.whe.TabIndex = 2;
            this.whe.Text = "Номинальная вместимость";
            // 
            // vol
            // 
            this.vol.AutoSize = true;
            this.vol.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.vol.ForeColor = System.Drawing.SystemColors.Control;
            this.vol.Location = new System.Drawing.Point(40, 3);
            this.vol.Name = "vol";
            this.vol.Size = new System.Drawing.Size(26, 16);
            this.vol.TabIndex = 5;
            this.vol.Text = "vol";
            this.vol.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cap
            // 
            this.cap.AutoSize = true;
            this.cap.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.cap.ForeColor = System.Drawing.SystemColors.Control;
            this.cap.Location = new System.Drawing.Point(33, 3);
            this.cap.Name = "cap";
            this.cap.Size = new System.Drawing.Size(31, 16);
            this.cap.TabIndex = 5;
            this.cap.Text = "cap";
            // 
            // chart1Type2
            // 
            this.chart1Type2.AutoSize = true;
            this.chart1Type2.Location = new System.Drawing.Point(468, 93);
            this.chart1Type2.Name = "chart1Type2";
            this.chart1Type2.Size = new System.Drawing.Size(253, 17);
            this.chart1Type2.TabIndex = 8;
            this.chart1Type2.Text = "Отображать в порядке возрастания обьема ";
            this.chart1Type2.UseVisualStyleBackColor = true;
            this.chart1Type2.CheckedChanged += new System.EventHandler(this.chart1Type2_CheckedChanged);
            // 
            // chart1Type1
            // 
            this.chart1Type1.AutoSize = true;
            this.chart1Type1.Checked = true;
            this.chart1Type1.Location = new System.Drawing.Point(152, 93);
            this.chart1Type1.Name = "chart1Type1";
            this.chart1Type1.Size = new System.Drawing.Size(287, 17);
            this.chart1Type1.TabIndex = 9;
            this.chart1Type1.TabStop = true;
            this.chart1Type1.Text = "Отображать в порядке возрастания номеров форм";
            this.chart1Type1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(63)))), ((int)(((byte)(75)))));
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.button6);
            this.panel1.Controls.Add(this.panel7);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.panel6);
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.panel5);
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.comboBox1);
            this.panel1.Controls.Add(this.label33);
            this.panel1.Controls.Add(this.whe);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1684, 78);
            this.panel1.TabIndex = 11;
            // 
            // button6
            // 
            this.button6.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.button6.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button6.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button6.ForeColor = System.Drawing.Color.Black;
            this.button6.Location = new System.Drawing.Point(1485, 20);
            this.button6.Margin = new System.Windows.Forms.Padding(2);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(138, 30);
            this.button6.TabIndex = 31;
            this.button6.Text = "Карта объемов";
            this.button6.UseVisualStyleBackColor = false;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // panel7
            // 
            this.panel7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel7.Controls.Add(this.date);
            this.panel7.Location = new System.Drawing.Point(1270, 25);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(136, 25);
            this.panel7.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.ForeColor = System.Drawing.SystemColors.Control;
            this.label5.Location = new System.Drawing.Point(1250, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(173, 15);
            this.label5.TabIndex = 14;
            this.label5.Text = "Дата последней постановки";
            // 
            // panel6
            // 
            this.panel6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel6.Controls.Add(this.cap);
            this.panel6.Location = new System.Drawing.Point(1059, 25);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(129, 25);
            this.panel6.TabIndex = 13;
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.nameFK);
            this.panel4.Location = new System.Drawing.Point(205, 25);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(338, 27);
            this.panel4.TabIndex = 12;
            // 
            // panel5
            // 
            this.panel5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel5.Controls.Add(this.vol);
            this.panel5.Location = new System.Drawing.Point(807, 25);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(168, 25);
            this.panel5.TabIndex = 12;
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel3.Controls.Add(this.weight);
            this.panel3.Location = new System.Drawing.Point(587, 25);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(144, 25);
            this.panel3.TabIndex = 12;
            // 
            // chart2Type1
            // 
            this.chart2Type1.AutoSize = true;
            this.chart2Type1.Checked = true;
            this.chart2Type1.Location = new System.Drawing.Point(3, 5);
            this.chart2Type1.Name = "chart2Type1";
            this.chart2Type1.Size = new System.Drawing.Size(225, 17);
            this.chart2Type1.TabIndex = 10;
            this.chart2Type1.TabStop = true;
            this.chart2Type1.Text = "Отобразить замеренную вместимость ";
            this.chart2Type1.UseVisualStyleBackColor = true;
            this.chart2Type1.CheckedChanged += new System.EventHandler(this.chart2Type1_CheckedChanged);
            // 
            // chart2Type2
            // 
            this.chart2Type2.AutoSize = true;
            this.chart2Type2.Location = new System.Drawing.Point(316, 7);
            this.chart2Type2.Name = "chart2Type2";
            this.chart2Type2.Size = new System.Drawing.Size(239, 17);
            this.chart2Type2.TabIndex = 9;
            this.chart2Type2.Text = "Отобразить пересчитанную вместимость ";
            this.chart2Type2.UseVisualStyleBackColor = true;
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(63)))), ((int)(((byte)(105)))));
            this.panel8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel8.Location = new System.Drawing.Point(1321, 440);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(25, 17);
            this.panel8.TabIndex = 12;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label2.Location = new System.Drawing.Point(1357, 439);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(162, 15);
            this.label2.TabIndex = 15;
            this.label2.Text = "- последний замер объема";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.label6.Location = new System.Drawing.Point(1128, 439);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(172, 15);
            this.label6.TabIndex = 17;
            this.label6.Text = "- предыдущий замер объема";
            // 
            // panel9
            // 
            this.panel9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(130)))), ((int)(((byte)(82)))), ((int)(((byte)(64)))));
            this.panel9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel9.Location = new System.Drawing.Point(1092, 440);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(25, 17);
            this.panel9.TabIndex = 16;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.chart2Type2);
            this.panel2.Controls.Add(this.chart2Type1);
            this.panel2.Location = new System.Drawing.Point(152, 469);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(583, 27);
            this.panel2.TabIndex = 18;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(767, 93);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(175, 17);
            this.checkBox1.TabIndex = 19;
            this.checkBox1.Text = "Спрятать предыдущий замер";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(0, 0);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(80, 17);
            this.checkBox2.TabIndex = 20;
            this.checkBox2.Text = "checkBox2";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // ascOrder
            // 
            this.ascOrder.AutoSize = true;
            this.ascOrder.Location = new System.Drawing.Point(791, 475);
            this.ascOrder.Name = "ascOrder";
            this.ascOrder.Size = new System.Drawing.Size(279, 17);
            this.ascOrder.TabIndex = 21;
            this.ascOrder.Text = "Отобразить в порядке возрастания вместимости";
            this.ascOrder.UseVisualStyleBackColor = true;
            this.ascOrder.CheckedChanged += new System.EventHandler(this.ascOrder_CheckedChanged);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.ClientSize = new System.Drawing.Size(1684, 811);
            this.Controls.Add(this.ascOrder);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.panel9);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel8);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.chart1Type1);
            this.Controls.Add(this.chart1Type2);
            this.Controls.Add(this.chart2);
            this.Controls.Add(this.chart1);
            this.Name = "Form3";
            this.ShowIcon = false;
            this.Text = "ISKRA МФУ графики";
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private System.Windows.Forms.Label nameFK;
        private System.Windows.Forms.Label label33;
        private System.Windows.Forms.Label weight;
        private System.Windows.Forms.Label date;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label whe;
        private System.Windows.Forms.Label vol;
        private System.Windows.Forms.Label cap;
        private System.Windows.Forms.RadioButton chart1Type2;
        private System.Windows.Forms.RadioButton chart1Type1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.RadioButton chart2Type1;
        private System.Windows.Forms.RadioButton chart2Type2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel panel8;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel9;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox ascOrder;
    }
}