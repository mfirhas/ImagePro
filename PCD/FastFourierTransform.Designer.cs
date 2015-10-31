namespace PCD
{
    partial class FastFourierTransform
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
            this.components = new System.ComponentModel.Container();
            this.ImageInput = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FourierMag = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.scalepercentage = new System.Windows.Forms.TextBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.ImSelected = new System.Windows.Forms.PictureBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ImageInput)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.FourierMag)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImSelected)).BeginInit();
            this.SuspendLayout();
            // 
            // ImageInput
            // 
            this.ImageInput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ImageInput.ContextMenuStrip = this.contextMenuStrip1;
            this.ImageInput.Location = new System.Drawing.Point(12, 49);
            this.ImageInput.Name = "ImageInput";
            this.ImageInput.Size = new System.Drawing.Size(331, 152);
            this.ImageInput.TabIndex = 0;
            this.ImageInput.TabStop = false;
            this.ImageInput.MouseMove += new System.Windows.Forms.MouseEventHandler(this.ImageInput_MouseMove);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectImageToolStripMenuItem,
            this.previewToolStripMenuItem,
            this.closeToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(142, 70);
            // 
            // selectImageToolStripMenuItem
            // 
            this.selectImageToolStripMenuItem.Name = "selectImageToolStripMenuItem";
            this.selectImageToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.selectImageToolStripMenuItem.Text = "Select Image";
            this.selectImageToolStripMenuItem.Click += new System.EventHandler(this.selectImageToolStripMenuItem_Click);
            // 
            // previewToolStripMenuItem
            // 
            this.previewToolStripMenuItem.Name = "previewToolStripMenuItem";
            this.previewToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.previewToolStripMenuItem.Text = "Preview";
            this.previewToolStripMenuItem.Click += new System.EventHandler(this.previewToolStripMenuItem_Click);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.closeToolStripMenuItem.Text = "Close";
            // 
            // FourierMag
            // 
            this.FourierMag.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FourierMag.Location = new System.Drawing.Point(346, 49);
            this.FourierMag.Name = "FourierMag";
            this.FourierMag.Size = new System.Drawing.Size(330, 363);
            this.FourierMag.TabIndex = 1;
            this.FourierMag.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(601, 25);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "Preview FFT";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(343, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "FFT :";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(520, 425);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "Save";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(601, 425);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 425);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(106, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Scaling Percentage :";
            // 
            // scalepercentage
            // 
            this.scalepercentage.Location = new System.Drawing.Point(116, 425);
            this.scalepercentage.Name = "scalepercentage";
            this.scalepercentage.Size = new System.Drawing.Size(32, 20);
            this.scalepercentage.TabIndex = 8;
            this.scalepercentage.Text = "25";
            // 
            // ImSelected
            // 
            this.ImSelected.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ImSelected.Location = new System.Drawing.Point(12, 232);
            this.ImSelected.Name = "ImSelected";
            this.ImSelected.Size = new System.Drawing.Size(328, 180);
            this.ImSelected.TabIndex = 9;
            this.ImSelected.TabStop = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 216);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Selected";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 12;
            this.label1.Text = "Source";
            // 
            // FastFourierTransform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(688, 455);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ImSelected);
            this.Controls.Add(this.scalepercentage);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.FourierMag);
            this.Controls.Add(this.ImageInput);
            this.Name = "FastFourierTransform";
            this.Text = "FastFourierTransform";
            this.Load += new System.EventHandler(this.FastFourierTransform_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ImageInput)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.FourierMag)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ImSelected)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox ImageInput;
        private System.Windows.Forms.PictureBox FourierMag;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox scalepercentage;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem selectImageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.PictureBox ImSelected;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
    }
}