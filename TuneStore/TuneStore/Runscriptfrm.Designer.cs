namespace TuneStore
{
    partial class Runscriptfrm
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
            this.consoleBox = new System.Windows.Forms.RichTextBox();
            this.startButton = new System.Windows.Forms.Button();
            this.doneBut = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.errorBox = new System.Windows.Forms.RichTextBox();
            this.errLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // consoleBox
            // 
            this.consoleBox.Location = new System.Drawing.Point(15, 25);
            this.consoleBox.Name = "consoleBox";
            this.consoleBox.ReadOnly = true;
            this.consoleBox.Size = new System.Drawing.Size(412, 279);
            this.consoleBox.TabIndex = 0;
            this.consoleBox.Text = "";
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(12, 338);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(105, 39);
            this.startButton.TabIndex = 1;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // doneBut
            // 
            this.doneBut.Location = new System.Drawing.Point(708, 338);
            this.doneBut.Name = "doneBut";
            this.doneBut.Size = new System.Drawing.Size(105, 39);
            this.doneBut.TabIndex = 2;
            this.doneBut.Text = "Close";
            this.doneBut.TextImageRelation = System.Windows.Forms.TextImageRelation.TextBeforeImage;
            this.doneBut.UseVisualStyleBackColor = true;
            this.doneBut.Click += new System.EventHandler(this.doneBut_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Console Output:";
            // 
            // errorBox
            // 
            this.errorBox.Location = new System.Drawing.Point(433, 25);
            this.errorBox.Name = "errorBox";
            this.errorBox.ReadOnly = true;
            this.errorBox.Size = new System.Drawing.Size(380, 279);
            this.errorBox.TabIndex = 4;
            this.errorBox.Text = "";
            // 
            // errLabel
            // 
            this.errLabel.AutoSize = true;
            this.errLabel.Location = new System.Drawing.Point(430, 9);
            this.errLabel.Name = "errLabel";
            this.errLabel.Size = new System.Drawing.Size(67, 13);
            this.errLabel.TabIndex = 5;
            this.errLabel.Text = "Error Output:";
            // 
            // Runscriptfrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(825, 456);
            this.Controls.Add(this.errLabel);
            this.Controls.Add(this.errorBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.doneBut);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.consoleBox);
            this.Name = "Runscriptfrm";
            this.Text = "Create Database";
            this.Load += new System.EventHandler(this.Runscriptfrm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox consoleBox;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.Button doneBut;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RichTextBox errorBox;
        private System.Windows.Forms.Label errLabel;
    }
}