namespace VKR_1
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            groupBox13 = new GroupBox();
            buttonForMoreInfo1 = new Button();
            textBoxLogInput = new TextBox();
            buttonEmbed = new Button();
            groupBox6 = new GroupBox();
            progressBar1 = new ProgressBar();
            label2 = new Label();
            label1 = new Label();
            groupBox5 = new GroupBox();
            pictureBox2 = new PictureBox();
            groupBox4 = new GroupBox();
            pictureBox1 = new PictureBox();
            groupBox3 = new GroupBox();
            checkedRGBInput = new CheckedListBox();
            comboBoxStegInput = new ComboBox();
            groupBox2 = new GroupBox();
            tabControl2 = new TabControl();
            tabPage4 = new TabPage();
            pathSecret = new TextBox();
            buttonPathSecret = new Button();
            tabPage5 = new TabPage();
            textBoxForSecMess = new TextBox();
            groupBox1 = new GroupBox();
            pathConteiner = new TextBox();
            buttonPathInputConteiner = new Button();
            tabPage2 = new TabPage();
            groupBox14 = new GroupBox();
            button3 = new Button();
            textBoxLogDecode = new TextBox();
            buttonDecode = new Button();
            groupBox10 = new GroupBox();
            textBoxDecode = new TextBox();
            buttonSaveDecode = new Button();
            groupBox9 = new GroupBox();
            pictureBoxConteinerDecode = new PictureBox();
            groupBox8 = new GroupBox();
            textBoxConteinerDecode = new TextBox();
            buttonConteinerDecode = new Button();
            groupBox7 = new GroupBox();
            checkedListBoxDecode = new CheckedListBox();
            comboBoxDecode = new ComboBox();
            tabPage3 = new TabPage();
            groupBox12 = new GroupBox();
            comboBox1 = new ComboBox();
            groupBox11 = new GroupBox();
            button1 = new Button();
            button2 = new Button();
            toolTip1 = new ToolTip(components);
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            groupBox13.SuspendLayout();
            groupBox6.SuspendLayout();
            groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox3.SuspendLayout();
            groupBox2.SuspendLayout();
            tabControl2.SuspendLayout();
            tabPage4.SuspendLayout();
            tabPage5.SuspendLayout();
            groupBox1.SuspendLayout();
            tabPage2.SuspendLayout();
            groupBox14.SuspendLayout();
            groupBox10.SuspendLayout();
            groupBox9.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBoxConteinerDecode).BeginInit();
            groupBox8.SuspendLayout();
            groupBox7.SuspendLayout();
            tabPage3.SuspendLayout();
            groupBox12.SuspendLayout();
            groupBox11.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Controls.Add(tabPage3);
            tabControl1.Dock = DockStyle.Fill;
            tabControl1.Location = new Point(0, 0);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(1898, 1024);
            tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(groupBox13);
            tabPage1.Controls.Add(buttonEmbed);
            tabPage1.Controls.Add(groupBox6);
            tabPage1.Controls.Add(groupBox5);
            tabPage1.Controls.Add(groupBox4);
            tabPage1.Controls.Add(groupBox3);
            tabPage1.Controls.Add(groupBox2);
            tabPage1.Controls.Add(groupBox1);
            tabPage1.Location = new Point(4, 34);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1890, 986);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Встраивание";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox13
            // 
            groupBox13.Controls.Add(buttonForMoreInfo1);
            groupBox13.Controls.Add(textBoxLogInput);
            groupBox13.Location = new Point(453, 540);
            groupBox13.Name = "groupBox13";
            groupBox13.Size = new Size(1406, 416);
            groupBox13.TabIndex = 13;
            groupBox13.TabStop = false;
            groupBox13.Text = "Логи";
            // 
            // buttonForMoreInfo1
            // 
            buttonForMoreInfo1.Location = new Point(977, 376);
            buttonForMoreInfo1.Name = "buttonForMoreInfo1";
            buttonForMoreInfo1.Size = new Size(423, 34);
            buttonForMoreInfo1.TabIndex = 5;
            buttonForMoreInfo1.Text = "Узнать подробную информацию об алгоритмах";
            buttonForMoreInfo1.UseVisualStyleBackColor = true;
            buttonForMoreInfo1.Click += buttonForMoreInfo_Click;
            // 
            // textBoxLogInput
            // 
            textBoxLogInput.Location = new Point(6, 30);
            textBoxLogInput.Multiline = true;
            textBoxLogInput.Name = "textBoxLogInput";
            textBoxLogInput.ScrollBars = ScrollBars.Vertical;
            textBoxLogInput.Size = new Size(1394, 340);
            textBoxLogInput.TabIndex = 4;
            // 
            // buttonEmbed
            // 
            buttonEmbed.Location = new Point(16, 624);
            buttonEmbed.Name = "buttonEmbed";
            buttonEmbed.Size = new Size(412, 50);
            buttonEmbed.TabIndex = 0;
            buttonEmbed.Text = "Встроить и сохранить";
            buttonEmbed.UseVisualStyleBackColor = true;
            buttonEmbed.Click += buttonEmbed_Click;
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(progressBar1);
            groupBox6.Controls.Add(label2);
            groupBox6.Controls.Add(label1);
            groupBox6.Location = new Point(6, 701);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new Size(432, 163);
            groupBox6.TabIndex = 12;
            groupBox6.TabStop = false;
            groupBox6.Text = "Вместимость";
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(16, 111);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(399, 34);
            progressBar1.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(16, 74);
            label2.Name = "label2";
            label2.Size = new Size(397, 25);
            label2.TabIndex = 1;
            label2.Text = "Объём секретных данных:                                  ";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(16, 38);
            label1.Name = "label1";
            label1.Size = new Size(397, 25);
            label1.TabIndex = 0;
            label1.Text = "Ёмкость контейнера:                                           ";
            toolTip1.SetToolTip(label1, "Чтобы увидеть ёмкость выбранного изображения-контейнера,\r\nтребуется выбрать:\r\n1 Изображение-контейнер;\r\n2 Алгоритм;\r\n3 Хотя бы один цветовой канал.");
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(pictureBox2);
            groupBox5.Location = new Point(1159, 6);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(700, 528);
            groupBox5.TabIndex = 11;
            groupBox5.TabStop = false;
            groupBox5.Text = "Изображение после встраивания секрета";
            // 
            // pictureBox2
            // 
            pictureBox2.Location = new Point(6, 30);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(694, 481);
            pictureBox2.TabIndex = 0;
            pictureBox2.TabStop = false;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(pictureBox1);
            groupBox4.Location = new Point(453, 6);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(700, 528);
            groupBox4.TabIndex = 10;
            groupBox4.TabStop = false;
            groupBox4.Text = "Изображение до встраивания секрета";
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(6, 30);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(688, 481);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(checkedRGBInput);
            groupBox3.Controls.Add(comboBoxStegInput);
            groupBox3.Location = new Point(6, 468);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(432, 150);
            groupBox3.TabIndex = 9;
            groupBox3.TabStop = false;
            groupBox3.Text = "Выбрать параметры встраивания";
            // 
            // checkedRGBInput
            // 
            checkedRGBInput.CheckOnClick = true;
            checkedRGBInput.FormattingEnabled = true;
            checkedRGBInput.Items.AddRange(new object[] { "R", "G", "B" });
            checkedRGBInput.Location = new Point(142, 30);
            checkedRGBInput.Name = "checkedRGBInput";
            checkedRGBInput.Size = new Size(62, 88);
            checkedRGBInput.TabIndex = 1;
            checkedRGBInput.SelectedIndexChanged += checkedRGBInput_SelectedIndexChanged;
            // 
            // comboBoxStegInput
            // 
            comboBoxStegInput.AllowDrop = true;
            comboBoxStegInput.FormattingEnabled = true;
            comboBoxStegInput.Items.AddRange(new object[] { "LSB", "PVD" });
            comboBoxStegInput.Location = new Point(16, 33);
            comboBoxStegInput.Name = "comboBoxStegInput";
            comboBoxStegInput.Size = new Size(120, 33);
            comboBoxStegInput.TabIndex = 2;
            comboBoxStegInput.Text = "Алгоритм";
            comboBoxStegInput.SelectedIndexChanged += comboBoxStegInput_SelectedIndexChanged;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(tabControl2);
            groupBox2.Location = new Point(6, 125);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(432, 337);
            groupBox2.TabIndex = 8;
            groupBox2.TabStop = false;
            groupBox2.Text = "Выбрать секретный файл или ввести текст";
            // 
            // tabControl2
            // 
            tabControl2.Controls.Add(tabPage4);
            tabControl2.Controls.Add(tabPage5);
            tabControl2.Location = new Point(6, 30);
            tabControl2.Name = "tabControl2";
            tabControl2.SelectedIndex = 0;
            tabControl2.Size = new Size(420, 297);
            tabControl2.TabIndex = 6;
            tabControl2.SelectedIndexChanged += tabControl2_SelectedIndexChanged;
            // 
            // tabPage4
            // 
            tabPage4.Controls.Add(pathSecret);
            tabPage4.Controls.Add(buttonPathSecret);
            tabPage4.Location = new Point(4, 34);
            tabPage4.Name = "tabPage4";
            tabPage4.Padding = new Padding(3);
            tabPage4.Size = new Size(412, 259);
            tabPage4.TabIndex = 0;
            tabPage4.Text = "Файл";
            tabPage4.UseVisualStyleBackColor = true;
            // 
            // pathSecret
            // 
            pathSecret.Enabled = false;
            pathSecret.Location = new Point(6, 18);
            pathSecret.Name = "pathSecret";
            pathSecret.Size = new Size(399, 31);
            pathSecret.TabIndex = 2;
            // 
            // buttonPathSecret
            // 
            buttonPathSecret.Location = new Point(6, 55);
            buttonPathSecret.Name = "buttonPathSecret";
            buttonPathSecret.Size = new Size(399, 34);
            buttonPathSecret.TabIndex = 5;
            buttonPathSecret.Text = "Выбрать файл для сокрытия";
            buttonPathSecret.UseVisualStyleBackColor = true;
            buttonPathSecret.Click += buttonPathSecret_Click;
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(textBoxForSecMess);
            tabPage5.Location = new Point(4, 34);
            tabPage5.Name = "tabPage5";
            tabPage5.Padding = new Padding(3);
            tabPage5.Size = new Size(412, 259);
            tabPage5.TabIndex = 1;
            tabPage5.Text = "Текст";
            tabPage5.UseVisualStyleBackColor = true;
            // 
            // textBoxForSecMess
            // 
            textBoxForSecMess.Location = new Point(6, 6);
            textBoxForSecMess.Multiline = true;
            textBoxForSecMess.Name = "textBoxForSecMess";
            textBoxForSecMess.ScrollBars = ScrollBars.Vertical;
            textBoxForSecMess.Size = new Size(399, 247);
            textBoxForSecMess.TabIndex = 3;
            textBoxForSecMess.TextChanged += textBoxForSecMess_TextChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(pathConteiner);
            groupBox1.Controls.Add(buttonPathInputConteiner);
            groupBox1.Location = new Point(6, 6);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(432, 113);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "Выбрать контейнер";
            // 
            // pathConteiner
            // 
            pathConteiner.Enabled = false;
            pathConteiner.Location = new Point(16, 30);
            pathConteiner.Name = "pathConteiner";
            pathConteiner.Size = new Size(399, 31);
            pathConteiner.TabIndex = 1;
            // 
            // buttonPathInputConteiner
            // 
            buttonPathInputConteiner.Location = new Point(16, 67);
            buttonPathInputConteiner.Name = "buttonPathInputConteiner";
            buttonPathInputConteiner.Size = new Size(399, 34);
            buttonPathInputConteiner.TabIndex = 4;
            buttonPathInputConteiner.Text = "Выбрать изображение-контейнер";
            buttonPathInputConteiner.UseVisualStyleBackColor = true;
            buttonPathInputConteiner.Click += buttonPathInputConteiner_Click;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(groupBox14);
            tabPage2.Controls.Add(buttonDecode);
            tabPage2.Controls.Add(groupBox10);
            tabPage2.Controls.Add(groupBox9);
            tabPage2.Controls.Add(groupBox8);
            tabPage2.Controls.Add(groupBox7);
            tabPage2.Location = new Point(4, 34);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1890, 986);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Извлечене";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // groupBox14
            // 
            groupBox14.Controls.Add(button3);
            groupBox14.Controls.Add(textBoxLogDecode);
            groupBox14.Location = new Point(461, 560);
            groupBox14.Name = "groupBox14";
            groupBox14.Size = new Size(1402, 396);
            groupBox14.TabIndex = 15;
            groupBox14.TabStop = false;
            groupBox14.Text = "Логи";
            // 
            // button3
            // 
            button3.Location = new Point(973, 359);
            button3.Name = "button3";
            button3.Size = new Size(423, 34);
            button3.TabIndex = 6;
            button3.Text = "Узнать подробную информацию об алгоритмах";
            button3.UseVisualStyleBackColor = true;
            // 
            // textBoxLogDecode
            // 
            textBoxLogDecode.Location = new Point(6, 30);
            textBoxLogDecode.Multiline = true;
            textBoxLogDecode.Name = "textBoxLogDecode";
            textBoxLogDecode.ScrollBars = ScrollBars.Vertical;
            textBoxLogDecode.Size = new Size(1390, 323);
            textBoxLogDecode.TabIndex = 4;
            // 
            // buttonDecode
            // 
            buttonDecode.Location = new Point(22, 295);
            buttonDecode.Name = "buttonDecode";
            buttonDecode.Size = new Size(399, 50);
            buttonDecode.TabIndex = 14;
            buttonDecode.Text = "Извлечь";
            buttonDecode.UseVisualStyleBackColor = true;
            buttonDecode.Click += buttonDecode_Click;
            // 
            // groupBox10
            // 
            groupBox10.Controls.Add(textBoxDecode);
            groupBox10.Controls.Add(buttonSaveDecode);
            groupBox10.Location = new Point(461, 6);
            groupBox10.Name = "groupBox10";
            groupBox10.Size = new Size(696, 548);
            groupBox10.TabIndex = 13;
            groupBox10.TabStop = false;
            groupBox10.Text = "Данные из контейнера";
            // 
            // textBoxDecode
            // 
            textBoxDecode.Location = new Point(14, 30);
            textBoxDecode.Multiline = true;
            textBoxDecode.Name = "textBoxDecode";
            textBoxDecode.ScrollBars = ScrollBars.Vertical;
            textBoxDecode.Size = new Size(676, 461);
            textBoxDecode.TabIndex = 5;
            // 
            // buttonSaveDecode
            // 
            buttonSaveDecode.Location = new Point(514, 497);
            buttonSaveDecode.Name = "buttonSaveDecode";
            buttonSaveDecode.Size = new Size(176, 34);
            buttonSaveDecode.TabIndex = 1;
            buttonSaveDecode.Text = "Сохранить";
            buttonSaveDecode.UseVisualStyleBackColor = true;
            buttonSaveDecode.Click += buttonSaveDecode_Click;
            // 
            // groupBox9
            // 
            groupBox9.Controls.Add(pictureBoxConteinerDecode);
            groupBox9.Location = new Point(1163, 6);
            groupBox9.Name = "groupBox9";
            groupBox9.Size = new Size(700, 548);
            groupBox9.TabIndex = 12;
            groupBox9.TabStop = false;
            groupBox9.Text = "Изображение";
            // 
            // pictureBoxConteinerDecode
            // 
            pictureBoxConteinerDecode.Location = new Point(6, 27);
            pictureBoxConteinerDecode.Name = "pictureBoxConteinerDecode";
            pictureBoxConteinerDecode.Size = new Size(688, 515);
            pictureBoxConteinerDecode.TabIndex = 0;
            pictureBoxConteinerDecode.TabStop = false;
            // 
            // groupBox8
            // 
            groupBox8.Controls.Add(textBoxConteinerDecode);
            groupBox8.Controls.Add(buttonConteinerDecode);
            groupBox8.Location = new Point(6, 6);
            groupBox8.Name = "groupBox8";
            groupBox8.Size = new Size(432, 113);
            groupBox8.TabIndex = 11;
            groupBox8.TabStop = false;
            groupBox8.Text = "Выбрать контейнер";
            // 
            // textBoxConteinerDecode
            // 
            textBoxConteinerDecode.Enabled = false;
            textBoxConteinerDecode.Location = new Point(16, 30);
            textBoxConteinerDecode.Name = "textBoxConteinerDecode";
            textBoxConteinerDecode.Size = new Size(399, 31);
            textBoxConteinerDecode.TabIndex = 1;
            // 
            // buttonConteinerDecode
            // 
            buttonConteinerDecode.Location = new Point(16, 67);
            buttonConteinerDecode.Name = "buttonConteinerDecode";
            buttonConteinerDecode.Size = new Size(399, 34);
            buttonConteinerDecode.TabIndex = 4;
            buttonConteinerDecode.Text = "Выбрать изображение-контейнер";
            buttonConteinerDecode.UseVisualStyleBackColor = true;
            buttonConteinerDecode.Click += buttonConteinerDecode_Click;
            // 
            // groupBox7
            // 
            groupBox7.Controls.Add(checkedListBoxDecode);
            groupBox7.Controls.Add(comboBoxDecode);
            groupBox7.Location = new Point(6, 125);
            groupBox7.Name = "groupBox7";
            groupBox7.Size = new Size(432, 150);
            groupBox7.TabIndex = 10;
            groupBox7.TabStop = false;
            groupBox7.Text = "Выбрать используемые параметры встраивания";
            // 
            // checkedListBoxDecode
            // 
            checkedListBoxDecode.CheckOnClick = true;
            checkedListBoxDecode.FormattingEnabled = true;
            checkedListBoxDecode.Items.AddRange(new object[] { "R", "G", "B" });
            checkedListBoxDecode.Location = new Point(138, 33);
            checkedListBoxDecode.Name = "checkedListBoxDecode";
            checkedListBoxDecode.Size = new Size(62, 88);
            checkedListBoxDecode.TabIndex = 1;
            // 
            // comboBoxDecode
            // 
            comboBoxDecode.AllowDrop = true;
            comboBoxDecode.FormattingEnabled = true;
            comboBoxDecode.Items.AddRange(new object[] { "LSB", "PVD" });
            comboBoxDecode.Location = new Point(16, 33);
            comboBoxDecode.Name = "comboBoxDecode";
            comboBoxDecode.Size = new Size(116, 33);
            comboBoxDecode.TabIndex = 2;
            comboBoxDecode.Text = "Алгоритм";
            comboBoxDecode.SelectedIndexChanged += comboBoxDecode_SelectedIndexChanged;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(groupBox12);
            tabPage3.Controls.Add(groupBox11);
            tabPage3.Location = new Point(4, 34);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new Size(1890, 986);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Анализ";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox12
            // 
            groupBox12.Controls.Add(comboBox1);
            groupBox12.Location = new Point(3, 173);
            groupBox12.Name = "groupBox12";
            groupBox12.Size = new Size(300, 96);
            groupBox12.TabIndex = 3;
            groupBox12.TabStop = false;
            groupBox12.Text = "Настройки";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "Серый", "Красный", "Зелёный", "Синий" });
            comboBox1.Location = new Point(6, 43);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(278, 33);
            comboBox1.TabIndex = 0;
            comboBox1.Text = "Цветовой канал";
            // 
            // groupBox11
            // 
            groupBox11.Controls.Add(button1);
            groupBox11.Controls.Add(button2);
            groupBox11.Location = new Point(3, 3);
            groupBox11.Name = "groupBox11";
            groupBox11.Size = new Size(300, 164);
            groupBox11.TabIndex = 2;
            groupBox11.TabStop = false;
            groupBox11.Text = "Визуализация";
            // 
            // button1
            // 
            button1.Location = new Point(6, 30);
            button1.Name = "button1";
            button1.Size = new Size(278, 55);
            button1.TabIndex = 0;
            button1.Text = "Пиксельное представление";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(6, 91);
            button2.Name = "button2";
            button2.Size = new Size(278, 55);
            button2.TabIndex = 1;
            button2.Text = "Цветовые каналы";
            button2.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1898, 1024);
            Controls.Add(tabControl1);
            Name = "Form1";
            Text = "StegoTeacher";
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            groupBox13.ResumeLayout(false);
            groupBox13.PerformLayout();
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            tabControl2.ResumeLayout(false);
            tabPage4.ResumeLayout(false);
            tabPage4.PerformLayout();
            tabPage5.ResumeLayout(false);
            tabPage5.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            tabPage2.ResumeLayout(false);
            groupBox14.ResumeLayout(false);
            groupBox14.PerformLayout();
            groupBox10.ResumeLayout(false);
            groupBox10.PerformLayout();
            groupBox9.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBoxConteinerDecode).EndInit();
            groupBox8.ResumeLayout(false);
            groupBox8.PerformLayout();
            groupBox7.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            groupBox12.ResumeLayout(false);
            groupBox11.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private ComboBox comboBoxDecode;
        private TabControl tabControl2;
        private TabPage tabPage4;
        private TextBox pathSecret;
        private Button buttonPathSecret;
        private TabPage tabPage5;
        private Button buttonPathInputConteiner;
        private TextBox textBoxForSecMess;
        private TextBox pathConteiner;
        private GroupBox groupBox3;
        private GroupBox groupBox2;
        private GroupBox groupBox1;
        private CheckedListBox checkedRGBInput;
        private ComboBox comboBoxStegInput;
        private GroupBox groupBox5;
        private GroupBox groupBox4;
        private Button buttonEmbed;
        private GroupBox groupBox6;
        private Label label2;
        private Label label1;
        private ProgressBar progressBar1;
        private GroupBox groupBox8;
        private TextBox textBoxConteinerDecode;
        private Button buttonConteinerDecode;
        private GroupBox groupBox7;
        private CheckedListBox checkedListBoxDecode;
        private GroupBox groupBox9;
        private GroupBox groupBox10;
        private TextBox textBoxDecode;
        private TabControl tabControl3;
        private TabPage tabPage6;
        private TextBox textBoxLogDecode;
        private Button button2;
        private TabPage tabPage7;
        private PictureBox pictureBoxConteinerDecode;
        private Button buttonDecode;
        private Button buttonSaveDecode;
        private Button button1;
        private GroupBox groupBox11;
        private GroupBox groupBox12;
        private ComboBox comboBox1;
        private GroupBox groupBox13;
        private TextBox textBoxLogInput;
        private GroupBox groupBox14;
        private PictureBox pictureBox2;
        private PictureBox pictureBox1;
        private Button buttonForMoreInfo1;
        private Button button3;
        private ToolTip toolTip1;
    }
}
