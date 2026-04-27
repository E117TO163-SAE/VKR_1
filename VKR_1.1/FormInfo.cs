using PdfiumViewer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace VKR_1
{
    public partial class FormInfo : Form
    {
        private PdfDocument _currentDocument;

        public FormInfo()
        {
            InitializeComponent();
            string pdfPath = Path.Combine(Application.StartupPath, "info", "PVD_inform.pdf");
            OpenPdfFile(pdfPath);
        }

        private void OpenPdfFile(string filePath)
        {
            // Проверяем, существует ли файл
            if (!System.IO.File.Exists(filePath))
            {
                MessageBox.Show("Файл не найден!");
                return;
            }

            // Закрываем предыдущий документ, если есть
            if (_currentDocument != null)
            {
                _currentDocument.Dispose();
            }

            // Загружаем новый документ и сохраняем ссылку
            _currentDocument = PdfDocument.Load(filePath);
            pdfViewer1.Document = _currentDocument;
        }

        // Не забываем освободить ресурсы при закрытии формы
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _currentDocument?.Dispose();
            base.OnFormClosing(e);
        }
    }
}
