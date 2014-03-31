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
            this.abortTx_button = new System.Windows.Forms.Button();
            this.statusTx_button = new System.Windows.Forms.Button();
            this.commitTx_button = new System.Windows.Forms.Button();
            this.beginTx_button = new System.Windows.Forms.Button();
            this.serverControl = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.serverURL_textBox = new System.Windows.Forms.TextBox();
            this.serverFreeze_button = new System.Windows.Forms.Button();
            this.serverFail_button = new System.Windows.Forms.Button();
            this.serverRecover_button = new System.Windows.Forms.Button();
            this.padintOperation = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.valuePADInt_textBox = new System.Windows.Forms.TextBox();
            this.writePADInt_button = new System.Windows.Forms.Button();
            this.readPADInt_button = new System.Windows.Forms.Button();
            this.listPADInt_textBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.uidPADInt_textBox = new System.Windows.Forms.TextBox();
            this.accessPADInt_button = new System.Windows.Forms.Button();
            this.createPADInt_button = new System.Windows.Forms.Button();
            this.log_textBox = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.transaction.SuspendLayout();
            this.serverControl.SuspendLayout();
            this.padintOperation.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // transaction
            // 
            this.transaction.Controls.Add(this.abortTx_button);
            this.transaction.Controls.Add(this.statusTx_button);
            this.transaction.Controls.Add(this.commitTx_button);
            this.transaction.Controls.Add(this.beginTx_button);
            this.transaction.Location = new System.Drawing.Point(12, 12);
            this.transaction.Name = "transaction";
            this.transaction.Size = new System.Drawing.Size(233, 115);
            this.transaction.TabIndex = 0;
            this.transaction.TabStop = false;
            this.transaction.Text = "Transaction";
            // 
            // abortTx_button
            // 
            this.abortTx_button.Enabled = false;
            this.abortTx_button.ForeColor = System.Drawing.Color.Red;
            this.abortTx_button.Location = new System.Drawing.Point(115, 59);
            this.abortTx_button.Name = "abortTx_button";
            this.abortTx_button.Size = new System.Drawing.Size(103, 34);
            this.abortTx_button.TabIndex = 3;
            this.abortTx_button.Text = "TxAbort";
            this.abortTx_button.UseVisualStyleBackColor = true;
            // 
            // statusTx_button
            // 
            this.statusTx_button.Enabled = false;
            this.statusTx_button.Location = new System.Drawing.Point(6, 60);
            this.statusTx_button.Name = "statusTx_button";
            this.statusTx_button.Size = new System.Drawing.Size(103, 34);
            this.statusTx_button.TabIndex = 2;
            this.statusTx_button.Text = "Status";
            this.statusTx_button.UseVisualStyleBackColor = true;
            // 
            // commitTx_button
            // 
            this.commitTx_button.BackColor = System.Drawing.SystemColors.Control;
            this.commitTx_button.Enabled = false;
            this.commitTx_button.ForeColor = System.Drawing.Color.Green;
            this.commitTx_button.Location = new System.Drawing.Point(115, 19);
            this.commitTx_button.Name = "commitTx_button";
            this.commitTx_button.Size = new System.Drawing.Size(103, 34);
            this.commitTx_button.TabIndex = 1;
            this.commitTx_button.Text = "TxCommit";
            this.commitTx_button.UseVisualStyleBackColor = false;
            // 
            // beginTx_button
            // 
            this.beginTx_button.Location = new System.Drawing.Point(6, 19);
            this.beginTx_button.Name = "beginTx_button";
            this.beginTx_button.Size = new System.Drawing.Size(103, 35);
            this.beginTx_button.TabIndex = 0;
            this.beginTx_button.Text = "TxBegin";
            this.beginTx_button.UseVisualStyleBackColor = true;
            this.beginTx_button.Click += new System.EventHandler(this.beginTx_button_Click);
            // 
            // serverControl
            // 
            this.serverControl.Controls.Add(this.label3);
            this.serverControl.Controls.Add(this.serverURL_textBox);
            this.serverControl.Controls.Add(this.serverFreeze_button);
            this.serverControl.Controls.Add(this.serverFail_button);
            this.serverControl.Controls.Add(this.serverRecover_button);
            this.serverControl.Location = new System.Drawing.Point(12, 133);
            this.serverControl.Name = "serverControl";
            this.serverControl.Size = new System.Drawing.Size(233, 135);
            this.serverControl.TabIndex = 1;
            this.serverControl.TabStop = false;
            this.serverControl.Text = "Server Control";
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
            // serverURL_textBox
            // 
            this.serverURL_textBox.Location = new System.Drawing.Point(73, 24);
            this.serverURL_textBox.Name = "serverURL_textBox";
            this.serverURL_textBox.Size = new System.Drawing.Size(152, 20);
            this.serverURL_textBox.TabIndex = 7;
            this.serverURL_textBox.TextChanged += new System.EventHandler(this.serverURL_textBox_TextChanged);
            // 
            // serverFreeze_button
            // 
            this.serverFreeze_button.Enabled = false;
            this.serverFreeze_button.Location = new System.Drawing.Point(62, 91);
            this.serverFreeze_button.Name = "serverFreeze_button";
            this.serverFreeze_button.Size = new System.Drawing.Size(103, 35);
            this.serverFreeze_button.TabIndex = 6;
            this.serverFreeze_button.Text = "Freeze";
            this.serverFreeze_button.UseVisualStyleBackColor = true;
            // 
            // serverFail_button
            // 
            this.serverFail_button.Enabled = false;
            this.serverFail_button.ForeColor = System.Drawing.Color.Red;
            this.serverFail_button.Location = new System.Drawing.Point(6, 50);
            this.serverFail_button.Name = "serverFail_button";
            this.serverFail_button.Size = new System.Drawing.Size(103, 35);
            this.serverFail_button.TabIndex = 4;
            this.serverFail_button.Text = "Fail";
            this.serverFail_button.UseVisualStyleBackColor = true;
            this.serverFail_button.Click += new System.EventHandler(this.serverFail_button_Click);
            // 
            // serverRecover_button
            // 
            this.serverRecover_button.Enabled = false;
            this.serverRecover_button.ForeColor = System.Drawing.Color.Green;
            this.serverRecover_button.Location = new System.Drawing.Point(115, 50);
            this.serverRecover_button.Name = "serverRecover_button";
            this.serverRecover_button.Size = new System.Drawing.Size(103, 35);
            this.serverRecover_button.TabIndex = 5;
            this.serverRecover_button.Text = "Recover";
            this.serverRecover_button.UseVisualStyleBackColor = true;
            this.serverRecover_button.Click += new System.EventHandler(this.serverRecover_button_Click);
            // 
            // padintOperation
            // 
            this.padintOperation.Controls.Add(this.label2);
            this.padintOperation.Controls.Add(this.valuePADInt_textBox);
            this.padintOperation.Controls.Add(this.writePADInt_button);
            this.padintOperation.Controls.Add(this.readPADInt_button);
            this.padintOperation.Controls.Add(this.listPADInt_textBox);
            this.padintOperation.Controls.Add(this.label1);
            this.padintOperation.Controls.Add(this.uidPADInt_textBox);
            this.padintOperation.Controls.Add(this.accessPADInt_button);
            this.padintOperation.Controls.Add(this.createPADInt_button);
            this.padintOperation.Location = new System.Drawing.Point(12, 274);
            this.padintOperation.Name = "padintOperation";
            this.padintOperation.Size = new System.Drawing.Size(233, 202);
            this.padintOperation.TabIndex = 2;
            this.padintOperation.TabStop = false;
            this.padintOperation.Text = "PADInt Operation";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Value:";
            // 
            // valuePADInt_textBox
            // 
            this.valuePADInt_textBox.Location = new System.Drawing.Point(73, 48);
            this.valuePADInt_textBox.Name = "valuePADInt_textBox";
            this.valuePADInt_textBox.Size = new System.Drawing.Size(92, 20);
            this.valuePADInt_textBox.TabIndex = 7;
            // 
            // writePADInt_button
            // 
            this.writePADInt_button.Location = new System.Drawing.Point(171, 163);
            this.writePADInt_button.Name = "writePADInt_button";
            this.writePADInt_button.Size = new System.Drawing.Size(56, 26);
            this.writePADInt_button.TabIndex = 6;
            this.writePADInt_button.Text = "Write";
            this.writePADInt_button.UseVisualStyleBackColor = true;
            // 
            // readPADInt_button
            // 
            this.readPADInt_button.Location = new System.Drawing.Point(171, 134);
            this.readPADInt_button.Name = "readPADInt_button";
            this.readPADInt_button.Size = new System.Drawing.Size(56, 26);
            this.readPADInt_button.TabIndex = 5;
            this.readPADInt_button.Text = "Read";
            this.readPADInt_button.UseVisualStyleBackColor = true;
            // 
            // listPADInt_textBox
            // 
            this.listPADInt_textBox.Location = new System.Drawing.Point(9, 77);
            this.listPADInt_textBox.Multiline = true;
            this.listPADInt_textBox.Name = "listPADInt_textBox";
            this.listPADInt_textBox.Size = new System.Drawing.Size(156, 112);
            this.listPADInt_textBox.TabIndex = 4;
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
            // uidPADInt_textBox
            // 
            this.uidPADInt_textBox.Location = new System.Drawing.Point(73, 22);
            this.uidPADInt_textBox.Name = "uidPADInt_textBox";
            this.uidPADInt_textBox.Size = new System.Drawing.Size(92, 20);
            this.uidPADInt_textBox.TabIndex = 2;
            this.uidPADInt_textBox.TextChanged += new System.EventHandler(this.uidPADInt_textBox_TextChanged);
            // 
            // accessPADInt_button
            // 
            this.accessPADInt_button.Enabled = false;
            this.accessPADInt_button.Location = new System.Drawing.Point(171, 107);
            this.accessPADInt_button.Name = "accessPADInt_button";
            this.accessPADInt_button.Size = new System.Drawing.Size(56, 23);
            this.accessPADInt_button.TabIndex = 1;
            this.accessPADInt_button.Text = "Access";
            this.accessPADInt_button.UseVisualStyleBackColor = true;
            this.accessPADInt_button.Click += new System.EventHandler(this.accessPADInt_button_Click);
            // 
            // createPADInt_button
            // 
            this.createPADInt_button.Enabled = false;
            this.createPADInt_button.Location = new System.Drawing.Point(171, 77);
            this.createPADInt_button.Name = "createPADInt_button";
            this.createPADInt_button.Size = new System.Drawing.Size(56, 26);
            this.createPADInt_button.TabIndex = 0;
            this.createPADInt_button.Text = "Create";
            this.createPADInt_button.UseVisualStyleBackColor = true;
            this.createPADInt_button.Click += new System.EventHandler(this.createPADInt_button_Click);
            // 
            // log_textBox
            // 
            this.log_textBox.Location = new System.Drawing.Point(16, 19);
            this.log_textBox.Multiline = true;
            this.log_textBox.Name = "log_textBox";
            this.log_textBox.Size = new System.Drawing.Size(286, 432);
            this.log_textBox.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.log_textBox);
            this.groupBox1.Location = new System.Drawing.Point(251, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(320, 464);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Log";
            // 
            // UserInterface
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(583, 488);
            this.Controls.Add(this.groupBox1);
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
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox transaction;
        private System.Windows.Forms.GroupBox serverControl;
        private System.Windows.Forms.GroupBox padintOperation;
        private System.Windows.Forms.Button abortTx_button;
        private System.Windows.Forms.Button statusTx_button;
        private System.Windows.Forms.Button commitTx_button;
        private System.Windows.Forms.Button beginTx_button;
        private System.Windows.Forms.Button serverFreeze_button;
        private System.Windows.Forms.Button serverRecover_button;
        private System.Windows.Forms.Button serverFail_button;
        private System.Windows.Forms.TextBox uidPADInt_textBox;
        private System.Windows.Forms.Button accessPADInt_button;
        private System.Windows.Forms.Button createPADInt_button;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button writePADInt_button;
        private System.Windows.Forms.Button readPADInt_button;
        private System.Windows.Forms.TextBox listPADInt_textBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox valuePADInt_textBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox serverURL_textBox;
        private System.Windows.Forms.TextBox log_textBox;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}

