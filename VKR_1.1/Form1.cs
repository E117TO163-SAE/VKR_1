using NIRSteg_2;
using System.Text;
using System.Drawing.Drawing2D;

namespace VKR_1
{
    public partial class Form1 : Form
    {
        private string inputDataPath = null;
        private string inputImagePath = null;
        private string decodeImagePath = null;
        private Bitmap loadedBitmap = null;
        private Bitmap loadedBitmapForDecode = null;
        private Bitmap analysisPict = null;
        private Bitmap? analysisOriginalBitmap = null;
        private Bitmap? analysisStegoBitmap = null;

        private string pathStegoImg = null;

        private byte[] secretDataForEmbed = null;
        private byte[] secretDataForDecode = null;

        private bool _text = false;
        private bool _file = true;

        private bool _lsbEmbed = false;
        private bool _pvdEmbed = false;
        private bool _lsbDecode = false;
        private bool _pvdDecode = false;

        private bool _embed = true;
        private bool _decode = false;
        private bool _syncingAnalysisScroll = false;

        private const int DefaultAnalysisCellSize = 18;
        private const int SelectionBorderWidth = 3;

        private int _analysisCellSize = DefaultAnalysisCellSize;
        private Point? _selectedAnalysisPoint = null;

        public Form1()
        {
            InitializeComponent();
            buttonSaveDecode.Visible = false;
            buttonSaveDecode.Enabled = false;

            LoadRangesToGrid();
            LoadRangesToAnalysisGrid();
            ConfigureAnalysisVisualizer();
        }
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage1)
            {
                _embed = true;
                _decode = false;
                LoadRangesToGrid();
            }
            else if (tabControl1.SelectedTab == tabPage2)
            {
                _embed = false;
                _decode = true;
                LoadRangesToGrid();
            }
            else if (tabControl1.SelectedTab == tabPage3)
            {
                checkedListBoxRGBAnalysis.SetItemChecked(0, true);

                for (int i = 0; i < checkedListBoxBitAnalysis.Items.Count; i++)
                {
                    checkedListBoxBitAnalysis.SetItemChecked(i, true);
                }

                if (tabControl4.SelectedTab == tabPage9)
                {
                    groupBox16.Text = "Оригинальное изображение";  
                    groupBox15.Text = "Стегоизображение";
                    RenderAnalysisVisualizer();
                }
            }
        }

        //
        //
        //
        // !!!!!!!!!!  Встраивание  !!!!!!!!!!
        //
        //
        //

        private void buttonPathInputConteiner_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "PNG files|*.png|TIFF files|*.tiff";
            if (ofd.ShowDialog() != DialogResult.OK) return;
            inputImagePath = ofd.FileName;
            loadedBitmap = new Bitmap(inputImagePath);

            pathConteiner.Text = Path.GetFileName(inputImagePath);

            pictureBox2.Image = null;
            pictureBox1.Image = null;
            pictureBox1.Image = Image.FromFile(ofd.FileName);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            if (_lsbEmbed)
            {
                List<int> selectedBits = new List<int>();
                foreach (var item in checkedListBoxBitEmbed.CheckedItems)
                {
                    selectedBits.Add(int.Parse(item.ToString()));
                }
                label1.Text = "Ёмкость контейнера: " + (loadedBitmap.Width * loadedBitmap.Height * (checkedRGBInput.CheckedItems.Count) * selectedBits.Count) / 8 + " байт";
            }
            else if (_pvdEmbed)
            {
                label1.Text = "Ёмкость контейнера: " + PvdSteganography.GetCapacity(loadedBitmap,
                                                       checkedRGBInput.CheckedItems.Contains("R"),
                                                       checkedRGBInput.CheckedItems.Contains("G"),
                                                       checkedRGBInput.CheckedItems.Contains("B")) + " байт";
            }

        }

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl2.SelectedTab.Text == "Текст")
            {
                _text = true;
                _file = false;

                if (textBoxForSecMess != null)
                {
                    secretDataForEmbed = StegoPayload.PackText(textBoxForSecMess.Text);
                }
            }
            else if (tabControl2.SelectedTab.Text == "Файл")
            {
                _text = false;
                _file = true;

                if (inputDataPath != null)
                {
                    secretDataForEmbed = StegoPayload.PackFile(inputDataPath);
                }
            }
        }

        private void comboBoxStegInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxStegInput.Text == "LSB")
            {
                _lsbEmbed = true;
                _pvdEmbed = false;

                textBoxLogInput.Clear();
                textBoxLogInput.AppendText("Метод наименьшего значащего бита или Least Significant Bit (LSB)." + Environment.NewLine + Environment.NewLine +
                    "Принцип LSB основан на замене младшего бита в каждом байте цвета изображения на бит скрываемого сообщения." + Environment.NewLine
                    + "Процесс встраивания выглядит следующим образом: из изображения-контейнера последовательно извлекаются " +
                    "байты пикселей, в каждом из них младший бит обнуляется (операцией AND с 0xFE), " +
                    "затем на его место записывается бит скрываемого сообщения (операцией OR с 0 или 1). " +
                    "Извлечение сообщения происходит аналогично: из каждого байта выделяется младший бит, и из " +
                    "полученной битовой последовательности собирается исходное сообщение." + Environment.NewLine);

                tableLayoutPanel1.Enabled = false;
                tableLayoutPanel1.Visible = false;

                checkedListBoxBitEmbed.Visible = true;
                checkedListBoxBitEmbed.Enabled = true;

            }
            else if (comboBoxStegInput.Text == "PVD")
            {
                _lsbEmbed = false;
                _pvdEmbed = true;

                textBoxLogInput.Clear();
                textBoxLogInput.AppendText("Метод PVD (Pixel Value Differencing) основан на анализе разницы между соседними " +
                    "пикселями изображения для определения количества бит, " +
                    "которые можно безопасно встроить без заметного искажения." + Environment.NewLine + Environment.NewLine +
                    "Процесс встраивания начинается с разделения изображения на блоки пикселей. Для каждого блока вычисляется " +
                    "разница между значениями соседних пикселей. В зависимости от величины этой разницы определяется, сколько бит " +
                    "скрываемого сообщения можно встроить в эти пиксели: чем больше разница, тем больше бит можно встроить. " +
                    "Затем эти биты встраиваются в младшие биты соответствующих пикселей. Извлечение сообщения происходит аналогично: " +
                    "анализируются разницы между пикселями, определяется количество бит для извлечения, и из этих бит восстанавливается " +
                    "исходное сообщение." + Environment.NewLine);

                checkedListBoxBitEmbed.Visible = false;
                checkedListBoxBitEmbed.Enabled = false;

                tableLayoutPanel1.Enabled = true;
                tableLayoutPanel1.Visible = true;
            }
        }

        private void buttonEmbed_Click(object sender, EventArgs e)
        {
            if (inputImagePath == null)
            {
                MessageBox.Show("Не выбрано изображение", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (_file && inputDataPath == null)
            {
                MessageBox.Show("Не выбран скрываемый файл", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (_text && string.IsNullOrEmpty(textBoxForSecMess.Text))
            {
                MessageBox.Show("Введите текст скрываемого сообщения", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (comboBoxStegInput.SelectedItem == null)
            {
                MessageBox.Show("Выберите алгоритм стеганографии", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (_lsbEmbed && checkedRGBInput.CheckedItems.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы один цветовой канал", "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (_file && pathSecret != null)
            {
                secretDataForEmbed = StegoPayload.PackFile(inputDataPath);

            }
            else if (_text && textBoxForSecMess != null)
            {
                secretDataForEmbed = StegoPayload.PackText(textBoxForSecMess.Text);
            }
            else
            {
                MessageBox.Show("Нет данных для встраивания", "Ошибка",
                                 MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG files|*.png|TIFF files|*.tiff";
                if (inputImagePath.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                {
                    sfd.FileName = "stego_image.png";
                }


                sfd.OverwritePrompt = false;

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    pathStegoImg = sfd.FileName;
                }
            }
            if (File.Exists(pathStegoImg))
            {
                DialogResult overwrite = MessageBox.Show(
                    $"Файл \"{Path.GetFileName(pathStegoImg)}\" уже существует.\n" +
                    "Перезаписать его?",
                    "Файл существует",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question,
                    MessageBoxDefaultButton.Button2);

                if (overwrite == DialogResult.No)
                    return;
            }

            try
            {
                Bitmap result;

                if (_lsbEmbed)
                {

                    List<int> selectedBits = new List<int>();
                    foreach (var item in checkedListBoxBitEmbed.CheckedItems)
                    {
                        selectedBits.Add(int.Parse(item.ToString()));
                    }

                    result = LsbSteganography2.EmbedData(loadedBitmap, secretDataForEmbed,
                        checkedRGBInput.CheckedItems.Contains("R"),
                        checkedRGBInput.CheckedItems.Contains("G"),
                        checkedRGBInput.CheckedItems.Contains("B"),
                        selectedBits);
                    result.Save(pathStegoImg, System.Drawing.Imaging.ImageFormat.Png);
                    result.Dispose();
                }
                else if (_pvdEmbed)
                {
                    result = PvdSteganography.EmbedDataPvd(loadedBitmap, secretDataForEmbed,
                        checkedRGBInput.CheckedItems.Contains("R"),
                        checkedRGBInput.CheckedItems.Contains("G"),
                        checkedRGBInput.CheckedItems.Contains("B"));
                    result.Save(pathStegoImg, System.Drawing.Imaging.ImageFormat.Png);
                    result.Dispose();
                }

                MessageBox.Show("Успешно сохранено!", "Готово",
                                   MessageBoxButtons.OK, MessageBoxIcon.Information);

                pictureBox2.Image = null;
                pictureBox2.Image = Image.FromFile(pathStegoImg);
                pictureBox2.SizeMode = PictureBoxSizeMode.Zoom;

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при встраивании: {ex.Message}\n\n{ex.StackTrace}",
                                "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void buttonPathSecret_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK) return;
            inputDataPath = ofd.FileName;
            pathSecret.Text = Path.GetFileName(inputDataPath);

            secretDataForEmbed = StegoPayload.PackFile(inputDataPath);

            label2.Text = "Объём секретных данных: " + (secretDataForEmbed.Length + 4) + " байт";
        }

        private void textBoxForSecMess_TextChanged(object sender, EventArgs e)
        {

            if (_text)
            {
                secretDataForEmbed = StegoPayload.PackText(textBoxForSecMess.Text);
                label2.Text = "Объём секретных данных: " + (secretDataForEmbed.Length + 4) + " байт";
            }
        }

        private void buttonForMoreInfo_Click(object sender, EventArgs e)
        {
            FormInfo form2 = new FormInfo();
            form2.Show();
        }

        private void checkedRGBInput_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_lsbEmbed && loadedBitmap != null)
            {
                List<int> selectedBits = new List<int>();
                foreach (var item in checkedListBoxBitEmbed.CheckedItems)
                {
                    selectedBits.Add(int.Parse(item.ToString()));
                }
                label1.Text = "Ёмкость контейнера: " + (loadedBitmap.Width * loadedBitmap.Height * (checkedRGBInput.CheckedItems.Count) * selectedBits.Count) / 8 + " байт";

            }
            else if (_pvdEmbed && loadedBitmap != null)
            {
                label1.Text = "Ёмкость контейнера: " + PvdSteganography.GetCapacity(loadedBitmap,
                                                       checkedRGBInput.CheckedItems.Contains("R"),
                                                       checkedRGBInput.CheckedItems.Contains("G"),
                                                       checkedRGBInput.CheckedItems.Contains("B")) + " байт";
            }
        }

        private void checkedListBoxBitEmbed_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_lsbEmbed && loadedBitmap != null)
            {
                List<int> selectedBits = new List<int>();
                foreach (var item in checkedListBoxBitEmbed.CheckedItems)
                {
                    selectedBits.Add(int.Parse(item.ToString()));
                }
                label1.Text = "Ёмкость контейнера: " + (loadedBitmap.Width * loadedBitmap.Height * (checkedRGBInput.CheckedItems.Count) * selectedBits.Count) / 8 + " байт";
            }
        }


        //
        //
        //
        // !!!!!!!!!!  Извлечение  !!!!!!!!!!
        //
        //
        //

        private void comboBoxDecode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxDecode.Text == "LSB")
            {
                _lsbDecode = true;
                _pvdDecode = false;

                textBoxLogDecode.Clear();
                textBoxLogDecode.AppendText("Метод наименьшего значащего бита или Least Significant Bit (LSB)." + Environment.NewLine + Environment.NewLine +
                    "Принцип LSB основан на замене младшего бита в каждом байте цвета изображения на бит скрываемого сообщения." + Environment.NewLine
                    + "Процесс встраивания выглядит следующим образом: из изображения-контейнера последовательно извлекаются байты пикселей, в каждом из них младший бит обнуляется (операцией AND с 0xFE), " +
                    "затем на его место записывается бит скрываемого сообщения (операцией OR с 0 или 1). Извлечение сообщения происходит аналогично: из каждого байта выделяется младший бит, и из " +
                    "полученной битовой последовательности собирается исходное сообщение." + Environment.NewLine);


                tableLayoutPanel3.Enabled = false;
                tableLayoutPanel3.Visible = false;

                checkedListBoxBitDecode.Visible = true;
                checkedListBoxBitDecode.Enabled = true;

            }
            else if (comboBoxDecode.Text == "PVD")
            {
                _lsbDecode = false;
                _pvdDecode = true;


                textBoxLogDecode.Clear();
                textBoxLogDecode.AppendText("Метод PVD (Pixel Value Differencing) основан на анализе разницы между " +
                    "соседними пикселями изображения для определения количества бит, которые можно безопасно встроить без заметного искажения." + Environment.NewLine + Environment.NewLine +
                    "Процесс встраивания начинается с разделения изображения на блоки пикселей. " +
                    "Для каждого блока вычисляется разница между значениями соседних пикселей. В зависимости от величины этой " +
                    "разницы определяется, сколько бит скрываемого сообщения можно встроить в эти пиксели: чем больше разница, " +
                    "тем больше бит можно встроить. Затем эти биты встраиваются в младшие биты соответствующих пикселей. " +
                    "Извлечение сообщения происходит аналогично: анализируются разницы между пикселями, определяется количество " + Environment.NewLine +
                    "Суть классического метода PVD заключается в последовательной модификации значений яркости двух соседних пикселей P_i и P_(i+1), " +
                    "для которых определяется абсолютная разность d_i = |P_i - P_(i+1)|, где d_i в диапазоне [0, 255]. На основании полученного значения d_i" +
                    "в соответствии с таблицей диапазонов квантования определяются нижняя и верхняя границы [lower_i, upper_i] региона R_i и количество встраиваемых бит " +
                    "t = floor[ log2(upper_i - lower_i + 1) ]. Последовательность бит сообщения длиной t преобразуется в десятичное значение t_d, после чего " +
                    "вычисляется новое значение d'_i = t_d  + lower_i.");


                tableLayoutPanel3.Enabled = true;
                tableLayoutPanel3.Visible = true;

                checkedListBoxBitEmbed.Visible = false;
                checkedListBoxBitEmbed.Enabled = false;

            }
        }

        private void buttonConteinerDecode_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "PNG files|*.png|TIFF files|*.tiff";
            if (ofd.ShowDialog() != DialogResult.OK) return;
            decodeImagePath = ofd.FileName;
            loadedBitmapForDecode = new Bitmap(decodeImagePath);

            textBoxConteinerDecode.Text = Path.GetFileName(decodeImagePath);

            pictureBoxConteinerDecode.Image = null;
            pictureBoxConteinerDecode.Image = Image.FromFile(ofd.FileName);
            pictureBoxConteinerDecode.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void buttonDecode_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(decodeImagePath))
            {
                MessageBox.Show("Не выбрано изображение", "Ошибка",
                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrEmpty(comboBoxStegInput.Text))
            {
                MessageBox.Show("Выберите алгоритм стеганографии", "Ошибка",
                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (_lsbDecode && checkedListBoxDecode.CheckedItems.Count == 0)
            {
                MessageBox.Show("Выберите хотя бы один цветовой канал", "Ошибка",
                                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                if (_lsbDecode)
                {
                    List<int> selectedBits = new List<int>();
                    foreach (var item in checkedListBoxBitDecode.CheckedItems)
                    {
                        selectedBits.Add(int.Parse(item.ToString()));
                    }

                    secretDataForDecode = LsbSteganography2.ExtractData(loadedBitmapForDecode,
                        checkedListBoxDecode.CheckedItems.Contains("R"),
                        checkedListBoxDecode.CheckedItems.Contains("G"),
                        checkedListBoxDecode.CheckedItems.Contains("B"),
                        selectedBits);
                }
                else if (_pvdDecode)
                {
                    secretDataForDecode = PvdSteganography.ExtractDataPvd(loadedBitmapForDecode,
                        checkedListBoxDecode.CheckedItems.Contains("R"),
                        checkedListBoxDecode.CheckedItems.Contains("G"),
                        checkedListBoxDecode.CheckedItems.Contains("B"));
                }

                string text;
                byte[] fileBytes;
                string fileName;
                StegoPayload.Unpack(secretDataForDecode, out text, out fileBytes, out fileName);

                if (text != null)
                {
                    textBoxDecode.Text = "Извлечён текст: " + "\"" + text + "\"";
                    textBoxLogDecode.AppendText(Environment.NewLine + "Извлечён текст: " + "\"" + text + "\"" + Environment.NewLine);
                    buttonSaveDecode.Visible = false;
                    buttonSaveDecode.Enabled = false;
                }
                else
                {
                    buttonSaveDecode.Visible = true;
                    buttonSaveDecode.Enabled = true;
                    textBoxDecode.Text = "Извлечён файл: " + fileName;
                    textBoxLogDecode.AppendText(Environment.NewLine + "Извлечён файл: " + fileName + Environment.NewLine);
                    buttonSaveDecode.Tag = new Tuple<string, byte[]>(fileName, fileBytes);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при извлечении: {ex.Message}\n\n{ex.StackTrace}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSaveDecode_Click(object sender, EventArgs e)
        {
            var tup = buttonSaveDecode.Tag as Tuple<string, byte[]>;
            if (tup == null) return;

            SaveFileDialog sfd = new SaveFileDialog();

            sfd.FileName = tup.Item1;
            if (sfd.ShowDialog() != DialogResult.OK) return;

            File.WriteAllBytes(sfd.FileName, tup.Item2);
            MessageBox.Show("Файл сохранён!", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);

        }

        //
        //
        //
        //
        //
        //
        //

        private void LoadRangesToGrid()
        {
            if (_embed)
                dataGridView1.Rows.Clear();
            else if (_decode)
                dataGridView2.Rows.Clear();

            foreach (var (L, U) in PvdSteganography.Ranges)
            {
                int rowIndex = -1;
                if (_embed)
                    rowIndex = dataGridView1.Rows.Add(L, U);
                else if (_decode)
                    rowIndex = dataGridView2.Rows.Add(L, U);
                UpdateTRow(rowIndex);
            }
        }

        private void UpdateTRow(int rowIndex)
        {
            if (rowIndex < 0 || (_embed && rowIndex >= dataGridView1.Rows.Count) || (_decode && rowIndex >= dataGridView2.Rows.Count)) return;

            var row = _embed ? dataGridView1.Rows[rowIndex] : dataGridView2.Rows[rowIndex];
            var lStr = row.Cells[0].Value?.ToString();
            var uStr = row.Cells[1].Value?.ToString();

            if (int.TryParse(lStr, out int L) && int.TryParse(uStr, out int U) && L <= U)
            {
                int t = (int)Math.Floor(Math.Log(U - L + 1, 2));
                row.Cells[2].Value = t;
            }
            else
            {
                row.Cells[2].Value = null;
            }
        }

        private bool TryParseAndValidate(out (int L, int U)[] ranges, out string error)
        {
            ranges = null;

            if ((_embed && dataGridView1.Rows.Count == 0) || (_decode && dataGridView2.Rows.Count == 0))
            {
                error = "Нет диапазонов.";
                return false;
            }

            var list = new List<(int L, int U)>();

            for (int i = 0; i < (_embed ? dataGridView1.Rows.Count : dataGridView2.Rows.Count); i++)
            {
                var row = _embed ? dataGridView1.Rows[i] : dataGridView2.Rows[i];

                if (!int.TryParse(row.Cells[0].Value?.ToString(), out int L) ||
                    !int.TryParse(row.Cells[1].Value?.ToString(), out int U))
                {
                    error = $"Строка {i + 1}: введите целые числа.";
                    return false;
                }

                if (L < 0 || U > 255 || L > U)
                {
                    error = $"Строка {i + 1}: некорректный диапазон.";
                    return false;
                }

                list.Add((L, U));
            }

            if (list[0].L != 0)
            {
                error = "Первый диапазон должен начинаться с 0.";
                return false;
            }

            if (list[^1].U != 255)
            {
                error = "Последний диапазон должен заканчиваться на 255.";
                return false;
            }

            for (int i = 0; i < list.Count - 1; i++)
            {
                if (list[i].U + 1 != list[i + 1].L)
                {
                    error = $"Разрыв между диапазонами {i + 1} и {i + 2}.";
                    return false;
                }
            }

            ranges = list.ToArray();
            error = null;
            return true;
        }

        //
        // Работа с таблицами для PVD
        //

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
                UpdateTRow(i);
        }

        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && dataGridView1.Rows.Count > 1)
                dataGridView1.Rows.RemoveAt(dataGridView1.CurrentRow.Index);
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Add(0, 0);
        }

        private void buttonApply_Click(object sender, EventArgs e)
        {
            if (TryParseAndValidate(out var ranges, out string error))
            {
                PvdSteganography.Ranges = ranges;
                MessageBox.Show("Диапазоны обновлены.", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(error, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            /*if (e.ColumnIndex == 0 || e.ColumnIndex == 1) 
                UpdateTRow(e.RowIndex);*/

            if (e.ColumnIndex == 1) // U
            {
                UpdateTRow(e.RowIndex);
                // Обновить L у следующей строки
                if (e.RowIndex + 1 < dataGridView1.Rows.Count)
                {
                    var row = dataGridView1.Rows[e.RowIndex];
                    if (int.TryParse(row.Cells[1].Value?.ToString(), out int U))
                        dataGridView1.Rows[e.RowIndex + 1].Cells[0].Value = U + 1;
                }
            }
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null || dataGridView1.CurrentRow.Index == 0)
                return;

            int index = dataGridView1.CurrentRow.Index;

            // Клонируем строку и копируем данные
            DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[index].Clone();
            for (int i = 0; i < dataGridView1.Rows[index].Cells.Count; i++)
            {
                row.Cells[i].Value = dataGridView1.Rows[index].Cells[i].Value;
            }

            // Удаляем и вставляем
            dataGridView1.Rows.RemoveAt(index);
            dataGridView1.Rows.Insert(index - 1, row);

            // Выделяем перемещенную строку
            dataGridView1.ClearSelection();
            dataGridView1.Rows[index - 1].Selected = true;
            dataGridView1.CurrentCell = dataGridView1.Rows[index - 1].Cells[0];
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null ||
                dataGridView1.CurrentRow.Index == dataGridView1.Rows.Count - 1)
                return;

            int index = dataGridView1.CurrentRow.Index;

            // Клонируем строку и копируем данные
            DataGridViewRow row = (DataGridViewRow)dataGridView1.Rows[index].Clone();
            for (int i = 0; i < dataGridView1.Rows[index].Cells.Count; i++)
            {
                row.Cells[i].Value = dataGridView1.Rows[index].Cells[i].Value;
            }

            // Удаляем и вставляем
            dataGridView1.Rows.RemoveAt(index);
            dataGridView1.Rows.Insert(index + 1, row);

            // Выделяем перемещенную строку
            dataGridView1.ClearSelection();
            dataGridView1.Rows[index + 1].Selected = true;
            dataGridView1.CurrentCell = dataGridView1.Rows[index + 1].Cells[0];
        }

        private void dataGridView2_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = e.RowIndex; i < e.RowIndex + e.RowCount; i++)
                UpdateTRow(i);
        }

        private void dataGridView2_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1) // U
            {
                UpdateTRow(e.RowIndex);
                // Обновить L у следующей строки
                if (e.RowIndex + 1 < dataGridView2.Rows.Count)
                {
                    var row = dataGridView2.Rows[e.RowIndex];
                    if (int.TryParse(row.Cells[1].Value?.ToString(), out int U))
                        dataGridView2.Rows[e.RowIndex + 1].Cells[0].Value = U + 1;
                }
            }

        }

        private void buttonApply1_Click(object sender, EventArgs e)
        {
            if (TryParseAndValidate(out var ranges, out string error))
            {
                PvdSteganography.Ranges = ranges;
                MessageBox.Show("Диапазоны обновлены.", "Успех",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(error, "Ошибка",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void buttonDelete1_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow != null && dataGridView2.Rows.Count > 1)
                dataGridView2.Rows.RemoveAt(dataGridView2.CurrentRow.Index);

        }

        private void buttonAdd1_Click(object sender, EventArgs e)
        {
            dataGridView2.Rows.Add(0, 0);
        }

        private void buttonDown1_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow == null ||
                dataGridView2.CurrentRow.Index == dataGridView2.Rows.Count - 1)
                return;

            int index = dataGridView2.CurrentRow.Index;
            // Клонируем строку и копируем данные
            DataGridViewRow row = (DataGridViewRow)dataGridView2.Rows[index].Clone();
            for (int i = 0; i < dataGridView2.Rows[index].Cells.Count; i++)
            {
                row.Cells[i].Value = dataGridView2.Rows[index].Cells[i].Value;
            }

            // Удаляем и вставляем
            dataGridView2.Rows.RemoveAt(index);
            dataGridView2.Rows.Insert(index + 1, row);

            // Выделяем перемещенную строку
            dataGridView2.ClearSelection();
            dataGridView2.Rows[index + 1].Selected = true;
            dataGridView2.CurrentCell = dataGridView2.Rows[index + 1].Cells[0];
        }

        private void buttonUp1_Click(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow == null || dataGridView2.CurrentRow.Index == 0)
                return;

            int index = dataGridView2.CurrentRow.Index;

            // Клонируем строку и копируем данные
            DataGridViewRow row = (DataGridViewRow)dataGridView2.Rows[index].Clone();
            for (int i = 0; i < dataGridView2.Rows[index].Cells.Count; i++)
            {
                row.Cells[i].Value = dataGridView2.Rows[index].Cells[i].Value;
            }

            // Удаляем и вставляем
            dataGridView2.Rows.RemoveAt(index);
            dataGridView2.Rows.Insert(index - 1, row);

            // Выделяем перемещенную строку
            dataGridView2.ClearSelection();
            dataGridView2.Rows[index - 1].Selected = true;
            dataGridView2.CurrentCell = dataGridView2.Rows[index - 1].Cells[0];


        }

        //
        //
        // Анализы
        //
        //

        private void buttonConteinerAnalysis_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "PNG files|*.png|TIFF files|*.tiff";
            if (ofd.ShowDialog() != DialogResult.OK) return;
            string analysisImagePath = ofd.FileName;
            analysisPict = new Bitmap(analysisImagePath);

            textBoxConteinerAnalysis.Text = Path.GetFileName(analysisImagePath);


            pictureBox4.Image = null;
            pictureBox4.Image = Image.FromFile(ofd.FileName);
            pictureBox4.SizeMode = PictureBoxSizeMode.Zoom;


            bitPlan();
        }

        private void bitPlan()
        {
            if (analysisPict == null)
            {
                pictureBox3.Image = null;
                return;
            }

            pictureBox3.Image = null;
            bool useGrayscale = checkedListBoxRGBAnalysis.GetItemChecked(3);
            bool useRed = checkedListBoxRGBAnalysis.GetItemChecked(0);
            bool useGreen = checkedListBoxRGBAnalysis.GetItemChecked(1);
            bool useBlue = checkedListBoxRGBAnalysis.GetItemChecked(2);

            bool[] selectedBits = new bool[8];
            for (int i = 0; i < 8; i++)
            {
                selectedBits[i] = checkedListBoxBitAnalysis.GetItemChecked(i);
            }


            pictureBox3.Image = BitPlan.ApplyBitFilter(analysisPict, useGrayscale, useRed, useGreen, useBlue, selectedBits);
            pictureBox3.SizeMode = PictureBoxSizeMode.Zoom;
        }


        private void checkedListBoxRGBAnalysis_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Unchecked)
            {
                if (checkedListBoxRGBAnalysis.CheckedItems.Count == 1 && checkedListBoxRGBAnalysis.CheckedItems.Contains(checkedListBoxRGBAnalysis.Items[e.Index]))
                {
                    e.NewValue = CheckState.Checked;
                }
            }
        }

        private void checkedListBoxBitAnalysis_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (e.NewValue == CheckState.Unchecked)
            {
                if (checkedListBoxBitAnalysis.CheckedItems.Count == 1 && checkedListBoxBitAnalysis.CheckedItems.Contains(checkedListBoxBitAnalysis.Items[e.Index]))
                {
                    e.NewValue = CheckState.Checked;
                }
            }
        }

        private void checkedListBoxRGBAnalysis_SelectedIndexChanged(object sender, EventArgs e)
        {
            bitPlan();
        }

        private void checkedListBoxBitAnalysis_SelectedIndexChanged(object sender, EventArgs e)
        {
            bitPlan();
        }

        private void ConfigureAnalysisVisualizer()
        {
            pictureBox3.SizeMode = PictureBoxSizeMode.Normal;
            pictureBox4.SizeMode = PictureBoxSizeMode.Normal;

            panel8.AutoScroll = true;
            panel9.AutoScroll = true;
            panel8.Scroll += AnalysisPanel_Scroll;
            panel9.Scroll += AnalysisPanel_Scroll;
            panel8.Paint += AnalysisPanel_Paint;
            panel9.Paint += AnalysisPanel_Paint;
            panel8.MouseClick += AnalysisPanel_MouseClick;
            panel9.MouseClick += AnalysisPanel_MouseClick;

            buttonOirgA.Click += buttonOirgA_Click;
            buttonConteinerA.Click += buttonConteinerA_Click;
            comboBoxMethod.SelectedIndexChanged += comboBoxMethod_SelectedIndexChanged;
            button2.Click += buttonForMoreInfo_Click;
            tabControl4.SelectedIndexChanged += tabControl4_SelectedIndexChanged;
            numericUpDownAnalysisScale.ValueChanged += numericUpDownAnalysisScale_ValueChanged;

            textBox2.ReadOnly = true;
            numericUpDownAnalysisScale.Value = DefaultAnalysisCellSize;
        }

        private void LoadRangesToAnalysisGrid()
        {
            dataGridView3.Rows.Clear();
            foreach (var (l, u) in PvdSteganography.Ranges)
            {
                int t = (int)Math.Floor(Math.Log(u - l + 1, 2));
                dataGridView3.Rows.Add(l, u, t);
            }
        }

        private void buttonOirgA_Click(object? sender, EventArgs e)
        {
            Bitmap? loaded = LoadBitmapForAnalysis(out string fileName);
            if (loaded == null)
            {
                return;
            }

            analysisOriginalBitmap?.Dispose();
            analysisOriginalBitmap = loaded;
            textBox1.Text = fileName;
            ResetAnalysisSelection();
            RenderAnalysisVisualizer();
        }

        private void buttonConteinerA_Click(object? sender, EventArgs e)
        {
            Bitmap? loaded = LoadBitmapForAnalysis(out string fileName);
            if (loaded == null)
            {
                return;
            }

            analysisStegoBitmap?.Dispose();
            analysisStegoBitmap = loaded;
            textBox3.Text = fileName;
            ResetAnalysisSelection();
            RenderAnalysisVisualizer();
        }

        private Bitmap? LoadBitmapForAnalysis(out string fileName)
        {
            fileName = string.Empty;

            using OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "PNG files|*.png|TIFF files|*.tiff";
            if (ofd.ShowDialog() != DialogResult.OK)
            {
                return null;
            }

            fileName = Path.GetFileName(ofd.FileName);
            using Bitmap temp = new Bitmap(ofd.FileName);
            return new Bitmap(temp);
        }

        private void comboBoxMethod_SelectedIndexChanged(object? sender, EventArgs e)
        {
            bool isPvd = IsPvdAnalysisMode();
            tableLayoutPanel5.Visible = isPvd;
            tableLayoutPanel5.Enabled = isPvd;

            ResetAnalysisSelection();
            RenderAnalysisVisualizer();
        }

        private void numericUpDownAnalysisScale_ValueChanged(object? sender, EventArgs e)
        {
            _analysisCellSize = (int)numericUpDownAnalysisScale.Value;
            RenderAnalysisVisualizer();
        }

        private void tabControl4_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (tabControl4.SelectedTab == tabPage8)
            {
                pictureBox3.Visible = true;
                pictureBox4.Visible = true;
                panel8.Invalidate();
                panel9.Invalidate();
                bitPlan();
                return;
            }

            if (tabControl4.SelectedTab == tabPage9)
            {
                pictureBox3.Visible = false;
                pictureBox4.Visible = false;
                RenderAnalysisVisualizer();
            }
        }

        private void AnalysisPanel_MouseClick(object? sender, MouseEventArgs e)
        {
            if (tabControl4.SelectedTab != tabPage9)
            {
                return;
            }

            Panel? panel = sender as Panel;
            Bitmap? reference = panel == panel9 ? analysisOriginalBitmap : analysisStegoBitmap;
            reference ??= GetAnalysisReferenceBitmap();
            if (reference == null)
            {
                return;
            }

            int groupWidth = GetAnalysisGroupWidth();
            Point scrollOffset = panel?.AutoScrollPosition ?? Point.Empty;
            int virtualX = e.X - scrollOffset.X;
            int virtualY = e.Y - scrollOffset.Y;
            int pixelX = virtualX / _analysisCellSize;
            int pixelY = virtualY / _analysisCellSize;

            if (pixelX < 0 || pixelY < 0 || pixelY >= reference.Height || pixelX >= reference.Width)
            {
                return;
            }

            int startX = groupWidth == 2 ? pixelX - (pixelX % 2) : pixelX;
            if (startX >= reference.Width)
            {
                return;
            }

            _selectedAnalysisPoint = new Point(startX, pixelY);
            RenderAnalysisVisualizer();
            UpdateAnalysisInfo();
        }

        private void AnalysisPanel_Paint(object? sender, PaintEventArgs e)
        {
            if (tabControl4.SelectedTab != tabPage9)
            {
                return;
            }

            Panel? panel = sender as Panel;
            if (panel == null)
            {
                return;
            }

            Bitmap? source = panel == panel9 ? analysisOriginalBitmap : analysisStegoBitmap;
            DrawAnalysisSurface(panel, e.Graphics, source);
        }

        private void AnalysisPanel_Scroll(object? sender, ScrollEventArgs e)
        {
            if (_syncingAnalysisScroll || tabControl4.SelectedTab != tabPage9)
            {
                return;
            }

            Panel? source = sender as Panel;
            Panel target = source == panel8 ? panel9 : panel8;
            SyncAnalysisScroll(source, target);
        }

        private void SyncAnalysisScroll(Panel? source, Panel? target)
        {
            if (source == null || target == null)
            {
                return;
            }

            _syncingAnalysisScroll = true;
            try
            {
                Point sourceOffset = source.AutoScrollPosition;
                target.AutoScrollPosition = new Point(-sourceOffset.X, -sourceOffset.Y);
            }
            finally
            {
                _syncingAnalysisScroll = false;
            }
        }

        private void ResetAnalysisSelection()
        {
            _selectedAnalysisPoint = null;
            textBox2.Clear();
        }

        private bool IsPvdAnalysisMode()
        {
            return string.Equals(comboBoxMethod.Text, "PVD", StringComparison.OrdinalIgnoreCase);
        }

        private int GetAnalysisGroupWidth()
        {
            return IsPvdAnalysisMode() ? 2 : 1;
        }

        private Bitmap? GetAnalysisReferenceBitmap()
        {
            return analysisOriginalBitmap ?? analysisStegoBitmap;
        }

        private void RenderAnalysisVisualizer()
        {
            if (tabControl4.SelectedTab != tabPage9)
            {
                return;
            }

            pictureBox3.Visible = false;
            pictureBox4.Visible = false;

            UpdateAnalysisScrollArea(panel9, analysisOriginalBitmap);
            UpdateAnalysisScrollArea(panel8, analysisStegoBitmap);

            panel9.Invalidate();
            panel8.Invalidate();

            if (_selectedAnalysisPoint.HasValue)
            {
                UpdateAnalysisInfo();
            }
        }

        private void UpdateAnalysisScrollArea(Panel panel, Bitmap? source)
        {
            if (source == null)
            {
                panel.AutoScrollMinSize = Size.Empty;
                return;
            }

            panel.AutoScrollMinSize = new Size(
                source.Width * _analysisCellSize + 1,
                source.Height * _analysisCellSize + 1);
        }

        private void DrawAnalysisSurface(Panel panel, Graphics graphics, Bitmap? source)
        {
            graphics.Clear(SystemColors.ControlDarkDark);

            if (source == null)
            {
                return;
            }

            int groupWidth = GetAnalysisGroupWidth();
            graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            graphics.PixelOffsetMode = PixelOffsetMode.Half;
            Point scrollOffset = panel.AutoScrollPosition;

            int startX = Math.Max(0, (-scrollOffset.X) / _analysisCellSize);
            int startY = Math.Max(0, (-scrollOffset.Y) / _analysisCellSize);
            int endX = Math.Min(source.Width - 1, (-scrollOffset.X + panel.ClientSize.Width) / _analysisCellSize + 1);
            int endY = Math.Min(source.Height - 1, (-scrollOffset.Y + panel.ClientSize.Height) / _analysisCellSize + 1);

            graphics.TranslateTransform(scrollOffset.X, scrollOffset.Y);

            for (int y = startY; y <= endY; y++)
            {
                for (int x = startX; x <= endX; x++)
                {
                    using SolidBrush brush = new SolidBrush(source.GetPixel(x, y));
                    graphics.FillRectangle(
                        brush,
                        x * _analysisCellSize,
                        y * _analysisCellSize,
                        _analysisCellSize,
                        _analysisCellSize);
                }
            }

            using Pen gridPen = new Pen(Color.FromArgb(110, Color.White), 1f);
            for (int y = startY; y <= Math.Min(source.Height, endY + 1); y++)
            {
                int py = y * _analysisCellSize;
                graphics.DrawLine(gridPen, startX * _analysisCellSize, py, (endX + 1) * _analysisCellSize, py);
            }

            for (int x = startX; x <= Math.Min(source.Width, endX + 1); x++)
            {
                int px = x * _analysisCellSize;
                graphics.DrawLine(gridPen, px, startY * _analysisCellSize, px, (endY + 1) * _analysisCellSize);
            }

            if (groupWidth == 2)
            {
                using Pen pairPen = new Pen(Color.FromArgb(190, Color.DeepSkyBlue), 2f);
                int pairStartX = startX - (startX % 2);
                for (int x = Math.Max(0, pairStartX); x <= Math.Min(source.Width, endX + 1); x += 2)
                {
                    int px = x * _analysisCellSize;
                    graphics.DrawLine(pairPen, px, startY * _analysisCellSize, px, (endY + 1) * _analysisCellSize);
                }
            }

            if (_selectedAnalysisPoint.HasValue)
            {
                Point selected = _selectedAnalysisPoint.Value;
                int selectedWidth = Math.Min(groupWidth, source.Width - selected.X);
                if (selected.Y >= 0 && selected.Y < source.Height && selectedWidth > 0)
                {
                    Rectangle selection = new Rectangle(
                        selected.X * _analysisCellSize,
                        selected.Y * _analysisCellSize,
                        selectedWidth * _analysisCellSize,
                        _analysisCellSize);

                    using SolidBrush fill = new SolidBrush(Color.FromArgb(60, Color.Fuchsia));
                    using Pen border = new Pen(Color.Fuchsia, SelectionBorderWidth);
                    graphics.FillRectangle(fill, selection);
                    graphics.DrawRectangle(border, selection);
                }
            }
        }

        private void UpdateAnalysisInfo()
        {
            if (!_selectedAnalysisPoint.HasValue)
            {
                textBox2.Clear();
                return;
            }

            Bitmap? original = analysisOriginalBitmap;
            Bitmap? stego = analysisStegoBitmap;
            Point point = _selectedAnalysisPoint.Value;

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Метод: {(IsPvdAnalysisMode() ? "PVD" : "LSB")}");
            sb.AppendLine($"Координаты блока: X={point.X}, Y={point.Y}");
            sb.AppendLine();

            if (IsPvdAnalysisMode())
            {
                AppendPvdPixelInfo(sb, "Оригинал", original, point);
                sb.AppendLine();
                AppendPvdPixelInfo(sb, "Стего-контейнер", stego, point);
            }
            else
            {
                AppendLsbPixelInfo(sb, "Оригинал", original, point);
                sb.AppendLine();
                AppendLsbPixelInfo(sb, "Стего-контейнер", stego, point);
            }

            textBox2.Text = sb.ToString().TrimEnd();
        }

        private void AppendLsbPixelInfo(StringBuilder sb, string title, Bitmap? bitmap, Point point)
        {
            sb.AppendLine(title + ":");
            if (bitmap == null)
            {
                sb.AppendLine("Изображение не загружено.");
                return;
            }

            if (point.X >= bitmap.Width || point.Y >= bitmap.Height)
            {
                sb.AppendLine("Для выбранного блока нет соответствующего пикселя.");
                return;
            }

            Color pixel = bitmap.GetPixel(point.X, point.Y);
            sb.AppendLine($"Пиксель ({point.X}, {point.Y})");
            sb.AppendLine($"ARGB: ({pixel.A}, {pixel.R}, {pixel.G}, {pixel.B})");
            sb.AppendLine($"R: {ToBinary(pixel.R)}");
            sb.AppendLine($"G: {ToBinary(pixel.G)}");
            sb.AppendLine($"B: {ToBinary(pixel.B)}");
        }

        private void AppendPvdPixelInfo(StringBuilder sb, string title, Bitmap? bitmap, Point point)
        {
            sb.AppendLine(title + ":");
            if (bitmap == null)
            {
                sb.AppendLine("Изображение не загружено.");
                return;
            }

            if (point.X >= bitmap.Width || point.Y >= bitmap.Height)
            {
                sb.AppendLine("Для выбранного блока нет соответствующей пары пикселей.");
                return;
            }

            Color left = bitmap.GetPixel(point.X, point.Y);
            sb.AppendLine($"Пиксель 1 ({point.X}, {point.Y})");
            sb.AppendLine($"ARGB: ({left.A}, {left.R}, {left.G}, {left.B})");
            sb.AppendLine($"R: {ToBinary(left.R)}");
            sb.AppendLine($"G: {ToBinary(left.G)}");
            sb.AppendLine($"B: {ToBinary(left.B)}");

            if (point.X + 1 < bitmap.Width)
            {
                Color right = bitmap.GetPixel(point.X + 1, point.Y);
                sb.AppendLine($"Пиксель 2 ({point.X + 1}, {point.Y})");
                sb.AppendLine($"ARGB: ({right.A}, {right.R}, {right.G}, {right.B})");
                sb.AppendLine($"R: {ToBinary(right.R)}");
                sb.AppendLine($"G: {ToBinary(right.G)}");
                sb.AppendLine($"B: {ToBinary(right.B)}");
                sb.AppendLine($"|ΔR|={Math.Abs(left.R - right.R)}, |ΔG|={Math.Abs(left.G - right.G)}, |ΔB|={Math.Abs(left.B - right.B)}");
            }
            else
            {
                sb.AppendLine("Второй пиксель пары отсутствует.");
            }
        }

        private static string ToBinary(byte value)
        {
            return Convert.ToString(value, 2).PadLeft(8, '0');
        }





    }
}
