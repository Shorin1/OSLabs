
namespace Lab3WinForm
{
    partial class CliForm
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
            this.logTextBox = new System.Windows.Forms.TextBox();
            this.commandTextBox = new System.Windows.Forms.TextBox();
            this.executeBtn = new System.Windows.Forms.Button();
            this.currentDirTextBox = new System.Windows.Forms.TextBox();
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.SuspendLayout();
            // 
            // logTextBox
            // 
            this.logTextBox.Location = new System.Drawing.Point(12, 12);
            this.logTextBox.Multiline = true;
            this.logTextBox.Name = "logTextBox";
            this.logTextBox.ReadOnly = true;
            this.logTextBox.Size = new System.Drawing.Size(776, 397);
            this.logTextBox.TabIndex = 0;
            // 
            // commandTextBox
            // 
            this.commandTextBox.Location = new System.Drawing.Point(167, 415);
            this.commandTextBox.Name = "commandTextBox";
            this.commandTextBox.Size = new System.Drawing.Size(474, 23);
            this.commandTextBox.TabIndex = 1;
            // 
            // executeBtn
            // 
            this.executeBtn.Location = new System.Drawing.Point(647, 415);
            this.executeBtn.Name = "executeBtn";
            this.executeBtn.Size = new System.Drawing.Size(141, 23);
            this.executeBtn.TabIndex = 2;
            this.executeBtn.Text = "Ввод";
            this.executeBtn.UseVisualStyleBackColor = true;
            this.executeBtn.Click += new System.EventHandler(this.executeBtn_Click);
            // 
            // currentDirTextBox
            // 
            this.currentDirTextBox.Location = new System.Drawing.Point(12, 415);
            this.currentDirTextBox.Name = "currentDirTextBox";
            this.currentDirTextBox.ReadOnly = true;
            this.currentDirTextBox.Size = new System.Drawing.Size(149, 23);
            this.currentDirTextBox.TabIndex = 3;
            // 
            // CliForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 446);
            this.Controls.Add(this.currentDirTextBox);
            this.Controls.Add(this.executeBtn);
            this.Controls.Add(this.commandTextBox);
            this.Controls.Add(this.logTextBox);
            this.Name = "CliForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox logTextBox;
        private System.Windows.Forms.TextBox commandTextBox;
        private System.Windows.Forms.Button executeBtn;
        private System.Windows.Forms.TextBox currentDirTextBox;
    }
}

