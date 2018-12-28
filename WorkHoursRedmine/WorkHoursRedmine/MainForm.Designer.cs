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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listViewUser = new System.Windows.Forms.ListView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labelUserName = new System.Windows.Forms.Label();
            this.listViewTimeEntry = new System.Windows.Forms.ListView();
            this.panel3 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.comboMounth = new System.Windows.Forms.ComboBox();
            this.butReportExcel = new System.Windows.Forms.Button();
            this.but_SaveExcel = new System.Windows.Forms.Button();
            this.but_loadRedmine = new System.Windows.Forms.Button();
            this.lab_MonthHours = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listViewUser);
            this.splitContainer1.Panel1.Controls.Add(this.panel2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.listViewTimeEntry);
            this.splitContainer1.Panel2.Controls.Add(this.panel3);
            this.splitContainer1.Size = new System.Drawing.Size(821, 680);
            this.splitContainer1.SplitterDistance = 273;
            this.splitContainer1.TabIndex = 0;
            // 
            // listViewUser
            // 
            this.listViewUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewUser.FullRowSelect = true;
            this.listViewUser.GridLines = true;
            this.listViewUser.HideSelection = false;
            this.listViewUser.Location = new System.Drawing.Point(0, 29);
            this.listViewUser.Name = "listViewUser";
            this.listViewUser.Size = new System.Drawing.Size(273, 651);
            this.listViewUser.TabIndex = 1;
            this.listViewUser.UseCompatibleStateImageBehavior = false;
            this.listViewUser.View = System.Windows.Forms.View.Details;
            this.listViewUser.SelectedIndexChanged += new System.EventHandler(this.listViewUser_SelectedIndexChanged);
            this.listViewUser.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listViewUser_MouseClick);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.labelUserName);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(273, 29);
            this.panel2.TabIndex = 0;
            // 
            // labelUserName
            // 
            this.labelUserName.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelUserName.AutoSize = true;
            this.labelUserName.Location = new System.Drawing.Point(85, 8);
            this.labelUserName.Name = "labelUserName";
            this.labelUserName.Size = new System.Drawing.Size(89, 13);
            this.labelUserName.TabIndex = 0;
            this.labelUserName.Text = "Пользователь: -";
            // 
            // listViewTimeEntry
            // 
            this.listViewTimeEntry.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewTimeEntry.FullRowSelect = true;
            this.listViewTimeEntry.GridLines = true;
            this.listViewTimeEntry.HideSelection = false;
            this.listViewTimeEntry.Location = new System.Drawing.Point(0, 29);
            this.listViewTimeEntry.Name = "listViewTimeEntry";
            this.listViewTimeEntry.Size = new System.Drawing.Size(544, 651);
            this.listViewTimeEntry.TabIndex = 2;
            this.listViewTimeEntry.UseCompatibleStateImageBehavior = false;
            this.listViewTimeEntry.View = System.Windows.Forms.View.Details;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.label2);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(544, 29);
            this.panel3.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(221, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(101, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Перечень заданий";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.lab_MonthHours);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.comboMounth);
            this.panel1.Controls.Add(this.butReportExcel);
            this.panel1.Controls.Add(this.but_SaveExcel);
            this.panel1.Controls.Add(this.but_loadRedmine);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 648);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(821, 32);
            this.panel1.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(318, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Месяц";
            // 
            // comboMounth
            // 
            this.comboMounth.FormattingEnabled = true;
            this.comboMounth.Location = new System.Drawing.Point(364, 7);
            this.comboMounth.Name = "comboMounth";
            this.comboMounth.Size = new System.Drawing.Size(100, 21);
            this.comboMounth.TabIndex = 2;
            this.comboMounth.SelectionChangeCommitted += new System.EventHandler(this.comboMounth_SelectionChangeCommitted);
            // 
            // butReportExcel
            // 
            this.butReportExcel.Location = new System.Drawing.Point(671, 5);
            this.butReportExcel.Name = "butReportExcel";
            this.butReportExcel.Size = new System.Drawing.Size(138, 23);
            this.butReportExcel.TabIndex = 1;
            this.butReportExcel.Text = "Отчет в Excel";
            this.butReportExcel.UseVisualStyleBackColor = true;
            this.butReportExcel.Click += new System.EventHandler(this.butReportExcel_Click);
            // 
            // but_SaveExcel
            // 
            this.but_SaveExcel.Location = new System.Drawing.Point(161, 5);
            this.but_SaveExcel.Name = "but_SaveExcel";
            this.but_SaveExcel.Size = new System.Drawing.Size(138, 23);
            this.but_SaveExcel.TabIndex = 1;
            this.but_SaveExcel.Text = "Трудозатраты в Excel";
            this.but_SaveExcel.UseVisualStyleBackColor = true;
            this.but_SaveExcel.Click += new System.EventHandler(this.but_SaveExcel_Click);
            // 
            // but_loadRedmine
            // 
            this.but_loadRedmine.Location = new System.Drawing.Point(12, 6);
            this.but_loadRedmine.Name = "but_loadRedmine";
            this.but_loadRedmine.Size = new System.Drawing.Size(141, 23);
            this.but_loadRedmine.TabIndex = 0;
            this.but_loadRedmine.Text = "Загрузить из Redmine";
            this.but_loadRedmine.UseVisualStyleBackColor = true;
            this.but_loadRedmine.Click += new System.EventHandler(this.but_loadRedmine_Click);
            // 
            // lab_MonthHours
            // 
            this.lab_MonthHours.AutoSize = true;
            this.lab_MonthHours.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lab_MonthHours.Location = new System.Drawing.Point(470, 11);
            this.lab_MonthHours.Name = "lab_MonthHours";
            this.lab_MonthHours.Size = new System.Drawing.Size(114, 13);
            this.lab_MonthHours.TabIndex = 4;
            this.lab_MonthHours.Text = "Кол-во раб. часов";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(821, 680);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MainForm";
            this.Text = "Main";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label labelUserName;
        private System.Windows.Forms.ListView listViewUser;
        private System.Windows.Forms.ListView listViewTimeEntry;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button but_loadRedmine;
        private System.Windows.Forms.Button but_SaveExcel;
        private System.Windows.Forms.ComboBox comboMounth;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button butReportExcel;
        private System.Windows.Forms.Label lab_MonthHours;
    }
}

