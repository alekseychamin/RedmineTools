namespace WinRedminePlaning
{
    partial class MainForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.button8 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.but_loadRedmine = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.button9 = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.rTProject = new System.Windows.Forms.RichTextBox();
            this.rBRedmineProject = new System.Windows.Forms.RadioButton();
            this.rBSelectedProject = new System.Windows.Forms.RadioButton();
            this.rBBothProject = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button8);
            this.panel1.Controls.Add(this.button3);
            this.panel1.Controls.Add(this.button2);
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.but_loadRedmine);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(381, 440);
            this.panel1.TabIndex = 1;
            // 
            // button8
            // 
            this.button8.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button8.Location = new System.Drawing.Point(29, 112);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(318, 23);
            this.button8.TabIndex = 7;
            this.button8.Text = "Проекты спец";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.but_LoadProjectUser);
            // 
            // button3
            // 
            this.button3.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button3.Location = new System.Drawing.Point(29, 89);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(318, 23);
            this.button3.TabIndex = 6;
            this.button3.Text = "Проекты ФРВ";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.but_loadProject);
            // 
            // button2
            // 
            this.button2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button2.Location = new System.Drawing.Point(29, 66);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(318, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "ФРВ групп";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.but_loadGroup_Click);
            // 
            // button1
            // 
            this.button1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button1.Location = new System.Drawing.Point(29, 43);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(318, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "ФРВ специалистов";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.but_loadUser_Click);
            // 
            // but_loadRedmine
            // 
            this.but_loadRedmine.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.but_loadRedmine.Location = new System.Drawing.Point(29, 20);
            this.but_loadRedmine.Name = "but_loadRedmine";
            this.but_loadRedmine.Size = new System.Drawing.Size(318, 23);
            this.but_loadRedmine.TabIndex = 0;
            this.but_loadRedmine.Text = "Годовой ФРВ";
            this.but_loadRedmine.UseVisualStyleBackColor = true;
            this.but_loadRedmine.Click += new System.EventHandler(this.but_loadYWH_Click);
            // 
            // button7
            // 
            this.button7.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button7.Location = new System.Drawing.Point(29, 61);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(318, 23);
            this.button7.TabIndex = 10;
            this.button7.Text = "Просроченные проекты";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.but_LoadExperiedProject);
            // 
            // button6
            // 
            this.button6.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button6.Location = new System.Drawing.Point(29, 34);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(318, 23);
            this.button6.TabIndex = 9;
            this.button6.Text = "Просроченные задания специалистов";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.but_LoadExperiedUser);
            // 
            // button5
            // 
            this.button5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button5.Location = new System.Drawing.Point(29, 16);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(318, 23);
            this.button5.TabIndex = 8;
            this.button5.Text = "Сохранить Годовой ФРВ";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button4.Location = new System.Drawing.Point(29, 11);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(318, 23);
            this.button4.TabIndex = 7;
            this.button4.Text = "Обновить данные";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.but_updateRedmineData);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rBBothProject);
            this.panel2.Controls.Add(this.rBSelectedProject);
            this.panel2.Controls.Add(this.rBRedmineProject);
            this.panel2.Controls.Add(this.rTProject);
            this.panel2.Controls.Add(this.button4);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 293);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(381, 147);
            this.panel2.TabIndex = 2;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.button9);
            this.panel3.Controls.Add(this.button7);
            this.panel3.Controls.Add(this.button6);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 201);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(381, 92);
            this.panel3.TabIndex = 3;
            // 
            // button9
            // 
            this.button9.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.button9.Location = new System.Drawing.Point(29, 6);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(318, 23);
            this.button9.TabIndex = 11;
            this.button9.Text = "Задания с задержкой";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.but_ReportIssue);
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.button5);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 146);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(381, 55);
            this.panel4.TabIndex = 4;
            // 
            // rTProject
            // 
            this.rTProject.Location = new System.Drawing.Point(29, 40);
            this.rTProject.Name = "rTProject";
            this.rTProject.Size = new System.Drawing.Size(318, 75);
            this.rTProject.TabIndex = 9;
            this.rTProject.Text = "";
            // 
            // rBRedmineProject
            // 
            this.rBRedmineProject.AutoSize = true;
            this.rBRedmineProject.Checked = true;
            this.rBRedmineProject.Location = new System.Drawing.Point(29, 121);
            this.rBRedmineProject.Name = "rBRedmineProject";
            this.rBRedmineProject.Size = new System.Drawing.Size(123, 17);
            this.rBRedmineProject.TabIndex = 10;
            this.rBRedmineProject.TabStop = true;
            this.rBRedmineProject.Text = "проекты из redmine";
            this.rBRedmineProject.UseVisualStyleBackColor = true;
            // 
            // rBSelectedProject
            // 
            this.rBSelectedProject.AutoSize = true;
            this.rBSelectedProject.Location = new System.Drawing.Point(163, 121);
            this.rBSelectedProject.Name = "rBSelectedProject";
            this.rBSelectedProject.Size = new System.Drawing.Size(126, 17);
            this.rBSelectedProject.TabIndex = 11;
            this.rBSelectedProject.Text = "указанные проекты";
            this.rBSelectedProject.UseVisualStyleBackColor = true;
            // 
            // rBBothProject
            // 
            this.rBBothProject.AutoSize = true;
            this.rBBothProject.Location = new System.Drawing.Point(304, 121);
            this.rBBothProject.Name = "rBBothProject";
            this.rBBothProject.Size = new System.Drawing.Size(43, 17);
            this.rBBothProject.TabIndex = 12;
            this.rBBothProject.Text = "все";
            this.rBBothProject.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 440);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Main";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button but_loadRedmine;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.RichTextBox rTProject;
        private System.Windows.Forms.RadioButton rBRedmineProject;
        private System.Windows.Forms.RadioButton rBBothProject;
        private System.Windows.Forms.RadioButton rBSelectedProject;
    }
}

