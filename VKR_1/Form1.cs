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

        private int countColorChannel = 0;
        public Form1()
        {
            InitializeComponent();
            buttonSaveDecode.Visible = false;
            buttonSaveDecode.Enabled = false;
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
                //label1.Text = "Ёмкость контейнера: " + loadedBitmap.Width * loadedBitmap.Height * (checkedRGBInput.CheckedItems.Count) + " бит";
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
                    result = PvdSteganography.EmbedDataPvd(loadedBitmap, secretDataForEmbed);
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
                label1.Text = "Ёмкость контейнера: " + (loadedBitmap.Width * loadedBitmap.Height * (checkedRGBInput.CheckedItems.Count))/8 + " байт";

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
                    secretDataForDecode = PvdSteganography.ExtractDataPvd(loadedBitmapForDecode);
                }

                string text;
                byte[] fileBytes;
                string fileName;
                StegoPayload.Unpack(secretDataForDecode, out text, out fileBytes, out fileName);

                if (text != null)
                {
                    textBoxDecode.Text = "Извлечён текст: " + "\"" + text + "\"";
                    textBoxLogDecode.AppendText("Извлечён текст: " + "\"" + text + "\"" + Environment.NewLine);
                    buttonSaveDecode.Visible = false;
                    buttonSaveDecode.Enabled = false;
                }
                else
                {
                    buttonSaveDecode.Visible = true;
                    buttonSaveDecode.Enabled = true;
                    textBoxDecode.Text = "Извлечён файл: " + fileName;
                    textBoxLogDecode.AppendText("Извлечён файл: " + fileName + Environment.NewLine);
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

        
    }
}
