namespace PADIClient
{
    partial class UserInterface
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
            this.transaction = new System.Windows.Forms.GroupBox();
            this.button4 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.serverControl = new System.Windows.Forms.GroupBox();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.padintOperation = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button9 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.transaction.SuspendLayout();
            this.serverControl.SuspendLayout();
            this.padintOperation.SuspendLayout();
            this.SuspendLayout();
            // 
            // transaction
            // 
            this.transaction.Controls.Add(this.button4);
            this.transaction.Controls.Add(this.button3);
            this.transaction.Controls.Add(this.button2);
            this.transaction.Controls.Add(this.button1);
            this.transaction.Location = new System.Drawing.Point(12, 12);
            this.transaction.Name = "transaction";
            this.transaction.Size = new System.Drawing.Size(233, 115);
            this.transaction.TabIndex = 0;
            this.transaction.TabStop = false;
            this.transaction.Text = "Transaction";
            // 
            // button4
            // 
            this.button4.Enabled = false;
            this.button4.ForeColor = System.Drawing.Color.Red;
            this.button4.Location = new System.Drawing.Point(115, 59);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(103, 34);
            this.button4.TabIndex = 3;
            this.button4.Text = "TxAbort";
            this.button4.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Enabled = false;
            this.button3.Location = new System.Drawing.Point(6, 60);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(103, 34);
            this.button3.TabIndex = 2;
            this.button3.Text = "Status";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.Control;
            this.button2.Enabled = false;
            this.button2.ForeColor = System.Drawing.Color.Green;
            this.button2.Location = new System.Drawing.Point(115, 19);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(103, 34);
            this.button2.TabIndex = 1;
            this.button2.Text = "TxCommit";
            this.button2.UseVisualStyleBackColor = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 19);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(103, 35);
            this.button1.TabIndex = 0;
            this.button1.Text = "TxBegin";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // serverControl
            // 
            this.serverControl.Controls.Add(this.label3);
            this.serverControl.Controls.Add(this.textBox4);
            this.serverControl.Controls.Add(this.button7);
            this.serverControl.Controls.Add(this.button5);
            this.serverControl.Controls.Add(this.button6);
            this.serverControl.Location = new System.Drawing.Point(12, 133);
            this.serverControl.Name = "serverControl";
            this.serverControl.Size = new System.Drawing.Size(233, 135);
            this.serverControl.TabIndex = 1;
            this.serverControl.TabStop = false;
            this.serverControl.Text = "Server Control";
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(62, 91);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(103, 35);
            this.button7.TabIndex = 6;
            this.button7.Text = "Freeze";
            this.button7.UseVisualStyleBackColor = true;
            // 
            // button6
            // 
            this.button6.Enabled = false;
            this.button6.ForeColor = System.Drawing.Color.Green;
            this.button6.Location = new System.Drawing.Point(115, 50);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(103, 35);
            this.button6.TabIndex = 5;
            this.button6.Text = "Recover";
            this.button6.UseVisualStyleBackColor = true;
            // 
            // button5
            // 
            this.button5.ForeColor = System.Drawing.Color.Red;
            this.button5.Location = new System.Drawing.Point(6, 50);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(103, 35);
            this.button5.TabIndex = 4;
            this.button5.Text = "Fail";
            this.button5.UseVisualStyleBackColor = true;
            // 
            // padintOperation
            // 
            this.padintOperation.Controls.Add(this.label2);
            this.padintOperation.Controls.Add(this.textBox3);
            this.padintOperation.Controls.Add(this.button11);
            this.padintOperation.Controls.Add(this.button10);
            this.padintOperation.Controls.Add(this.textBox2);
            this.padintOperation.Controls.Add(this.label1);
            this.padintOperation.Controls.Add(this.textBox1);
            this.padintOperation.Controls.Add(this.button9);
            this.padintOperation.Controls.Add(this.button8);
            this.padintOperation.Location = new System.Drawing.Point(12, 274);
            this.padintOperation.Name = "padintOperation";
            this.padintOperation.Size = new System.Drawing.Size(233, 202);
            this.padintOperation.TabIndex = 2;
            this.padintOperation.TabStop = false;
            this.padintOperation.Text = "PADInt Operation";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(73, 22);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(92, 20);
            this.textBox1.TabIndex = 2;
            // 
            // button9
            // 
            this.button9.Enabled = false;
            this.button9.Location = new System.Drawing.Point(9, 48);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(75, 23);
            this.button9.TabIndex = 1;
            this.button9.Text = "Access";
            this.button9.UseVisualStyleBackColor = true;
            // 
            // button8
            // 
            this.button8.Enabled = false;
            this.button8.Location = new System.Drawing.Point(90, 48);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(75, 23);
            this.button8.TabIndex = 0;
            this.button8.Text = "Create";
            this.button8.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "PADInt uid:";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(9, 77);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(156, 112);
            this.textBox2.TabIndex = 4;
            // 
            // button10
            // 
            this.button10.Enabled = false;
            this.button10.Location = new System.Drawing.Point(171, 119);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(56, 26);
            this.button10.TabIndex = 5;
            this.button10.Text = "Read";
            this.button10.UseVisualStyleBackColor = true;
            // 
            // button11
            // 
            this.button11.Enabled = false;
            this.button11.Location = new System.Drawing.Point(171, 163);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(56, 26);
            this.button11.TabIndex = 6;
            this.button11.Text = "Write";
            this.button11.UseVisualStyleBackColor = true;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(172, 93);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(53, 20);
            this.textBox3.TabIndex = 7;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(172, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Value:";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(73, 24);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(152, 20);
            this.textBox4.TabIndex = 7;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Server URL:";
            // 
            // UserInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(264, 488);
            this.Controls.Add(this.padintOperation);
            this.Controls.Add(this.serverControl);
            this.Controls.Add(this.transaction);
            this.Name = "UserInterface";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.UserInterface_Load);
            this.transaction.ResumeLayout(false);
            this.serverControl.ResumeLayout(false);
            this.serverControl.PerformLayout();
            this.padintOperation.ResumeLayout(false);
            this.padintOperation.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox transaction;
        private System.Windows.Forms.GroupBox serverControl;
        private System.Windows.Forms.GroupBox padintOperation;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox4;
    }
}

