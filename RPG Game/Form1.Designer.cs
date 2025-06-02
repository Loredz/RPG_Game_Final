namespace RPG_Game
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TextBox txtPlayer1Name;
        private System.Windows.Forms.TextBox txtPlayer2Name;
        private System.Windows.Forms.ComboBox cmbPlayer1Class;
        private System.Windows.Forms.ComboBox cmbPlayer2Class;
        private System.Windows.Forms.Button btnStartBattle;
        private System.Windows.Forms.ListBox lstBattleLog;
        private System.Windows.Forms.Label lblPlayer1Health;
        private System.Windows.Forms.Label lblPlayer2Health;
        private System.Windows.Forms.Label lblWinner;
        private System.Windows.Forms.Panel pnlArena;

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
            txtPlayer1Name = new TextBox();
            txtPlayer2Name = new TextBox();
            cmbPlayer1Class = new ComboBox();
            cmbPlayer2Class = new ComboBox();
            btnStartBattle = new Button();
            lstBattleLog = new ListBox();
            lblPlayer1Health = new Label();
            lblPlayer2Health = new Label();
            lblWinner = new Label();
            pnlArena = new Panel();
            SuspendLayout();
            // 
            // txtPlayer1Name
            // 
            txtPlayer1Name.BackColor = Color.FromArgb(45, 45, 45);
            txtPlayer1Name.ForeColor = Color.White;
            txtPlayer1Name.Location = new Point(50, 30);
            txtPlayer1Name.Name = "txtPlayer1Name";
            txtPlayer1Name.Size = new Size(150, 23);
            txtPlayer1Name.TabIndex = 0;
            // 
            // txtPlayer2Name
            // 
            txtPlayer2Name.BackColor = Color.FromArgb(45, 45, 45);
            txtPlayer2Name.ForeColor = Color.White;
            txtPlayer2Name.Location = new Point(600, 30);
            txtPlayer2Name.Name = "txtPlayer2Name";
            txtPlayer2Name.Size = new Size(150, 23);
            txtPlayer2Name.TabIndex = 1;
            // 
            // cmbPlayer1Class
            // 
            cmbPlayer1Class.BackColor = Color.FromArgb(45, 45, 45);
            cmbPlayer1Class.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPlayer1Class.ForeColor = Color.White;
            cmbPlayer1Class.Location = new Point(50, 70);
            cmbPlayer1Class.Name = "cmbPlayer1Class";
            cmbPlayer1Class.Size = new Size(150, 23);
            cmbPlayer1Class.TabIndex = 2;
            // 
            // cmbPlayer2Class
            // 
            cmbPlayer2Class.BackColor = Color.FromArgb(45, 45, 45);
            cmbPlayer2Class.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPlayer2Class.ForeColor = Color.White;
            cmbPlayer2Class.Location = new Point(600, 70);
            cmbPlayer2Class.Name = "cmbPlayer2Class";
            cmbPlayer2Class.Size = new Size(150, 23);
            cmbPlayer2Class.TabIndex = 3;
            // 
            // btnStartBattle
            // 
            btnStartBattle.BackColor = Color.FromArgb(60, 60, 60);
            btnStartBattle.ForeColor = Color.White;
            btnStartBattle.Location = new Point(325, 60);
            btnStartBattle.Name = "btnStartBattle";
            btnStartBattle.Size = new Size(150, 40);
            btnStartBattle.TabIndex = 4;
            btnStartBattle.Text = "Start Battle";
            btnStartBattle.UseVisualStyleBackColor = false;
            // 
            // lstBattleLog
            // 
            lstBattleLog.BackColor = Color.FromArgb(30, 30, 30);
            lstBattleLog.ForeColor = Color.White;
            lstBattleLog.FormattingEnabled = true;
            lstBattleLog.ItemHeight = 15;
            lstBattleLog.Location = new Point(50, 489);
            lstBattleLog.Name = "lstBattleLog";
            lstBattleLog.Size = new Size(700, 184);
            lstBattleLog.TabIndex = 5;
            lstBattleLog.SelectedIndexChanged += lstBattleLog_SelectedIndexChanged;
            // 
            // lblPlayer1Health
            // 
            lblPlayer1Health.ForeColor = Color.White;
            lblPlayer1Health.Location = new Point(50, 100);
            lblPlayer1Health.Name = "lblPlayer1Health";
            lblPlayer1Health.Size = new Size(150, 23);
            lblPlayer1Health.TabIndex = 6;
            lblPlayer1Health.Text = "Health: 0";
            // 
            // lblPlayer2Health
            // 
            lblPlayer2Health.ForeColor = Color.White;
            lblPlayer2Health.Location = new Point(600, 100);
            lblPlayer2Health.Name = "lblPlayer2Health";
            lblPlayer2Health.Size = new Size(150, 23);
            lblPlayer2Health.TabIndex = 7;
            lblPlayer2Health.Text = "Health: 0";
            // 
            // lblWinner
            // 
            lblWinner.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblWinner.ForeColor = Color.LightGreen;
            lblWinner.Location = new Point(271, 416);
            lblWinner.Name = "lblWinner";
            lblWinner.Size = new Size(236, 40);
            lblWinner.TabIndex = 8;
            lblWinner.TextAlign = ContentAlignment.MiddleCenter;
            lblWinner.Click += lblWinner_Click;
            // 
            // pnlArena
            // 
            pnlArena.BackColor = Color.FromArgb(20, 20, 20);
            pnlArena.Location = new Point(50, 126);
            pnlArena.Name = "pnlArena";
            pnlArena.Size = new Size(700, 275);
            pnlArena.TabIndex = 9;
            pnlArena.Paint += pnlArena_Paint_1;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(835, 685);
            Controls.Add(lblWinner);
            Controls.Add(txtPlayer1Name);
            Controls.Add(lstBattleLog);
            Controls.Add(txtPlayer2Name);
            Controls.Add(cmbPlayer1Class);
            Controls.Add(cmbPlayer2Class);
            Controls.Add(btnStartBattle);
            Controls.Add(lblPlayer1Health);
            Controls.Add(lblPlayer2Health);
            Controls.Add(pnlArena);
            Name = "Form1";
            Text = "Classroom RPG Battle";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
