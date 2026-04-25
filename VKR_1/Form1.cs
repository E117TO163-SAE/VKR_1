using NIRSteg_2;
using System.Text;

namespace VKR_1
{
    public partial class Form1 : Form
    {
        private string inputDataPath = null;
        private string inputImagePath = null;
        private string decodeImagePath = null;
        private Bitmap loadedBitmap = null;
        private Bitmap loadedBitmapForDecode = null;

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

        public Form1()
        {
            InitializeComponent();
            buttonSaveDecode.Visible = false;
            buttonSaveDecode.Enabled = false;
            LoadRangesToGrid();
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
                label1.Text = "Ёмкость контейнера: " + loadedBitmap.Width * loadedBitmap.Height * (checkedRGBInput.CheckedItems.Count) + " байт";
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
                    result = LsbSteganography.EmbedData(loadedBitmap, secretDataForEmbed,
                        checkedRGBInput.CheckedItems.Contains("R"),
                        checkedRGBInput.CheckedItems.Contains("G"),
                        checkedRGBInput.CheckedItems.Contains("B"));
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
                label1.Text = "Ёмкость контейнера: " + (loadedBitmap.Width * loadedBitmap.Height * (checkedRGBInput.CheckedItems.Count)) / 8 + " байт";

            }
            else if (_pvdEmbed && loadedBitmap != null)
            {
                label1.Text = "Ёмкость контейнера: " + PvdSteganography.GetCapacity(loadedBitmap,
                                                       checkedRGBInput.CheckedItems.Contains("R"),
                                                       checkedRGBInput.CheckedItems.Contains("G"),
                                                       checkedRGBInput.CheckedItems.Contains("B")) + " байт";
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
                    secretDataForDecode = LsbSteganography.ExtractData(loadedBitmapForDecode,
                        checkedListBoxDecode.CheckedItems.Contains("R"),
                        checkedListBoxDecode.CheckedItems.Contains("G"),
                        checkedListBoxDecode.CheckedItems.Contains("B"));
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
    }
}
