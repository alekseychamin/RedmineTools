﻿namespace WinRedminePlaning
{
    partial class IssueDayForm
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.listLoadDWH = new System.Windows.Forms.ListView();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 453);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(765, 55);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(765, 52);
            this.panel2.TabIndex = 1;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.listLoadDWH);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 52);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(765, 401);
            this.panel3.TabIndex = 2;
            // 
            // listLoadDWH
            // 
            this.listLoadDWH.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listLoadDWH.FullRowSelect = true;
            this.listLoadDWH.GridLines = true;
            this.listLoadDWH.HideSelection = false;
            this.listLoadDWH.Location = new System.Drawing.Point(0, 0);
            this.listLoadDWH.Name = "listLoadDWH";
            this.listLoadDWH.Size = new System.Drawing.Size(765, 401);
            this.listLoadDWH.TabIndex = 0;
            this.listLoadDWH.UseCompatibleStateImageBehavior = false;
            this.listLoadDWH.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listLoadDWH_ColumnClick);
            // 
            // IssueDayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 508);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "IssueDayForm";
            this.Text = "IssueMonthForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.IssueDayForm_FormClosing);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.ListView listLoadDWH;
    }
}