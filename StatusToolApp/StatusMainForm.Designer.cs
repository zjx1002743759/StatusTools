
using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    partial class StatusMainForm:Form
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
            
        }


      



        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatusMainForm));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.StateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ExitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Namelabel = new System.Windows.Forms.Label();
            this.Addbutton = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.Deletebutton = new System.Windows.Forms.Button();
            this.NameComboBox = new System.Windows.Forms.ComboBox();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "啥都没选";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StateToolStripMenuItem,
            this.ExitToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(101, 48);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // StateToolStripMenuItem
            // 
            this.StateToolStripMenuItem.Image = global::WindowsFormsApp1.Properties.Resources.setup;
            this.StateToolStripMenuItem.Name = "StateToolStripMenuItem";
            this.StateToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.StateToolStripMenuItem.Text = "设置";
            this.StateToolStripMenuItem.Click += new System.EventHandler(this.StateToolStripMenuItem_Click);
            // 
            // ExitToolStripMenuItem
            // 
            this.ExitToolStripMenuItem.Image = global::WindowsFormsApp1.Properties.Resources.Exit;
            this.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem";
            this.ExitToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
            this.ExitToolStripMenuItem.Text = "退出";
            this.ExitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItem_Click);
            // 
            // Namelabel
            // 
            this.Namelabel.AutoSize = true;
            this.Namelabel.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.Namelabel.Image = global::WindowsFormsApp1.Properties.Resources.background;
            this.Namelabel.Location = new System.Drawing.Point(80, 113);
            this.Namelabel.Name = "Namelabel";
            this.Namelabel.Size = new System.Drawing.Size(83, 12);
            this.Namelabel.TabIndex = 14;
            this.Namelabel.Text = "ProjectName：";
            // 
            // Addbutton
            // 
            this.Addbutton.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.Addbutton.Image = global::WindowsFormsApp1.Properties.Resources.background;
            this.Addbutton.Location = new System.Drawing.Point(498, 113);
            this.Addbutton.Name = "Addbutton";
            this.Addbutton.Size = new System.Drawing.Size(89, 23);
            this.Addbutton.TabIndex = 12;
            this.Addbutton.Text = "Add";
            this.Addbutton.UseVisualStyleBackColor = true;
            this.Addbutton.Click += new System.EventHandler(this.Addbutton_Click);
            // 
            // button1
            // 
            this.button1.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.background;
            this.button1.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.button1.Location = new System.Drawing.Point(688, 398);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(89, 23);
            this.button1.TabIndex = 17;
            this.button1.Text = "Close";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.ColseButton_Click);
            // 
            // listView1
            // 
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(82, 167);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(629, 190);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // Deletebutton
            // 
            this.Deletebutton.BackgroundImage = global::WindowsFormsApp1.Properties.Resources.background;
            this.Deletebutton.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.Deletebutton.Location = new System.Drawing.Point(631, 113);
            this.Deletebutton.Name = "Deletebutton";
            this.Deletebutton.Size = new System.Drawing.Size(80, 23);
            this.Deletebutton.TabIndex = 18;
            this.Deletebutton.Text = "Delete";
            this.Deletebutton.UseVisualStyleBackColor = true;
            this.Deletebutton.Click += new System.EventHandler(this.Deletebutton_Click);
            // 
            // NameComboBox
            // 
            this.NameComboBox.FormattingEnabled = true;
            this.NameComboBox.Location = new System.Drawing.Point(184, 110);
            this.NameComboBox.Name = "NameComboBox";
            this.NameComboBox.Size = new System.Drawing.Size(274, 20);
            this.NameComboBox.TabIndex = 19;
            this.NameComboBox.DropDown += new System.EventHandler(this.NameComboBox_DropDown);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.ControlBox = false;
            this.Controls.Add(this.NameComboBox);
            this.Controls.Add(this.Deletebutton);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.Namelabel);
            this.Controls.Add(this.Addbutton);
            this.Name = "Form1";
            this.Text = "设置";
            this.Load += new System.EventHandler(this.Form1_Load);

            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;      
        private ToolStripMenuItem StateToolStripMenuItem;
        private ToolStripMenuItem ExitToolStripMenuItem;
        private Label Namelabel;
        private Button Addbutton;
        private Button button1;
        private ListView listView1;
        private Button Deletebutton;
        private ComboBox NameComboBox;
    }
}

