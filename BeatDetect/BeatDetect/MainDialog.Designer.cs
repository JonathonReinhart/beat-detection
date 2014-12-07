namespace BeatDetect
{
    partial class MainDialog
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
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.textBoxHistLength = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxConstant = new System.Windows.Forms.TextBox();
            this.comboBoxBeatTrack = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxSubbandThreshold = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonOK
            // 
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Location = new System.Drawing.Point(245, 231);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 0;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonCancel
            // 
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(326, 231);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 1;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // textBoxHistLength
            // 
            this.textBoxHistLength.Location = new System.Drawing.Point(155, 46);
            this.textBoxHistLength.Name = "textBoxHistLength";
            this.textBoxHistLength.Size = new System.Drawing.Size(100, 20);
            this.textBoxHistLength.TabIndex = 2;
            this.textBoxHistLength.Text = "10";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(68, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "History Length:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(44, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(102, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Sensitivity Constant:";
            // 
            // textBoxConstant
            // 
            this.textBoxConstant.Location = new System.Drawing.Point(155, 72);
            this.textBoxConstant.Name = "textBoxConstant";
            this.textBoxConstant.Size = new System.Drawing.Size(100, 20);
            this.textBoxConstant.TabIndex = 4;
            this.textBoxConstant.Text = "250";
            // 
            // comboBoxBeatTrack
            // 
            this.comboBoxBeatTrack.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxBeatTrack.FormattingEnabled = true;
            this.comboBoxBeatTrack.Location = new System.Drawing.Point(128, 172);
            this.comboBoxBeatTrack.Name = "comboBoxBeatTrack";
            this.comboBoxBeatTrack.Size = new System.Drawing.Size(192, 21);
            this.comboBoxBeatTrack.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(59, 175);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(63, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Beat Track:";
            // 
            // textBoxSubbandThreshold
            // 
            this.textBoxSubbandThreshold.Location = new System.Drawing.Point(155, 112);
            this.textBoxSubbandThreshold.Name = "textBoxSubbandThreshold";
            this.textBoxSubbandThreshold.Size = new System.Drawing.Size(100, 20);
            this.textBoxSubbandThreshold.TabIndex = 8;
            this.textBoxSubbandThreshold.Text = "1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(21, 115);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(128, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Subband Beat Threshold:";
            // 
            // MainDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 266);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBoxSubbandThreshold);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBoxBeatTrack);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxConstant);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxHistLength);
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MainDialog";
            this.Text = "Automatic Beat Detection";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.TextBox textBoxHistLength;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxConstant;
        private System.Windows.Forms.ComboBox comboBoxBeatTrack;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxSubbandThreshold;
        private System.Windows.Forms.Label label4;
    }
}