namespace VKR_1
{
    partial class FormInfo
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
            folderBrowserDialog1 = new FolderBrowserDialog();
            pdfViewer1 = new PdfiumViewer.PdfViewer();
            SuspendLayout();
            // 
            // pdfViewer1
            // 
            pdfViewer1.Dock = DockStyle.Fill;
            pdfViewer1.Location = new Point(0, 0);
            pdfViewer1.Margin = new Padding(5, 6, 5, 6);
            pdfViewer1.Name = "pdfViewer1";
            pdfViewer1.Size = new Size(1435, 1204);
            pdfViewer1.TabIndex = 0;
            // 
            // FormInfo
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1435, 1204);
            Controls.Add(pdfViewer1);
            Name = "FormInfo";
            Text = "FormInfo";
            ResumeLayout(false);
        }

        #endregion

        private FolderBrowserDialog folderBrowserDialog1;
        private PdfiumViewer.PdfViewer pdfViewer1;
    }
}