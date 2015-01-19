/*
Copyright 2015 Yixin Zhang

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

SWF file decompression support is provided by the zlib 1.2.8 library,
distributed under zlib license 1.2.8.
*/

﻿namespace TFM_Client
{
    partial class Replacer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Replacer));
            this.aobSearch = new System.Windows.Forms.TextBox();
            this.aobReplace = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.addAoB = new System.Windows.Forms.Button();
            this.removeAoB = new System.Windows.Forms.Button();
            this.aobName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.done = new System.Windows.Forms.Button();
            this.aobList = new System.Windows.Forms.ListBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.exportName = new System.Windows.Forms.TextBox();
            this.exportPass = new System.Windows.Forms.TextBox();
            this.exporter = new System.Windows.Forms.Button();
            this.loadBtn = new System.Windows.Forms.Button();
            this.aobEdit = new System.Windows.Forms.Button();
            this.aobSave = new System.Windows.Forms.Button();
            this.openMOOSE = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // aobSearch
            // 
            this.aobSearch.Location = new System.Drawing.Point(18, 81);
            this.aobSearch.Name = "aobSearch";
            this.aobSearch.Size = new System.Drawing.Size(322, 20);
            this.aobSearch.TabIndex = 0;
            // 
            // aobReplace
            // 
            this.aobReplace.Location = new System.Drawing.Point(18, 120);
            this.aobReplace.Name = "aobReplace";
            this.aobReplace.Size = new System.Drawing.Size(322, 20);
            this.aobReplace.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Search";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Replace";
            // 
            // addAoB
            // 
            this.addAoB.Location = new System.Drawing.Point(131, 146);
            this.addAoB.Name = "addAoB";
            this.addAoB.Size = new System.Drawing.Size(97, 44);
            this.addAoB.TabIndex = 6;
            this.addAoB.Text = "Add";
            this.addAoB.UseVisualStyleBackColor = true;
            this.addAoB.Click += new System.EventHandler(this.addAoB_Click);
            // 
            // removeAoB
            // 
            this.removeAoB.Enabled = false;
            this.removeAoB.Location = new System.Drawing.Point(474, 146);
            this.removeAoB.Name = "removeAoB";
            this.removeAoB.Size = new System.Drawing.Size(97, 44);
            this.removeAoB.TabIndex = 7;
            this.removeAoB.Text = "Remove";
            this.removeAoB.UseVisualStyleBackColor = true;
            this.removeAoB.Click += new System.EventHandler(this.removeAoB_Click);
            // 
            // aobName
            // 
            this.aobName.Location = new System.Drawing.Point(18, 42);
            this.aobName.Name = "aobName";
            this.aobName.Size = new System.Drawing.Size(322, 20);
            this.aobName.TabIndex = 8;
            this.aobName.Text = "AoB1";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(21, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Name";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(152, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(128, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "Example : 00 ?? 00 00 00";
            // 
            // done
            // 
            this.done.Location = new System.Drawing.Point(397, 211);
            this.done.Name = "done";
            this.done.Size = new System.Drawing.Size(143, 59);
            this.done.TabIndex = 12;
            this.done.Text = "Continue";
            this.done.UseVisualStyleBackColor = true;
            this.done.Click += new System.EventHandler(this.done_Click);
            // 
            // aobList
            // 
            this.aobList.FormattingEnabled = true;
            this.aobList.Location = new System.Drawing.Point(356, 6);
            this.aobList.Name = "aobList";
            this.aobList.Size = new System.Drawing.Size(215, 134);
            this.aobList.TabIndex = 13;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 195);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(54, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "File Name";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 234);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(161, 13);
            this.label6.TabIndex = 15;
            this.label6.Text = "Password (at least12 characters)";
            // 
            // exportName
            // 
            this.exportName.Location = new System.Drawing.Point(18, 211);
            this.exportName.Name = "exportName";
            this.exportName.Size = new System.Drawing.Size(164, 20);
            this.exportName.TabIndex = 16;
            // 
            // exportPass
            // 
            this.exportPass.Location = new System.Drawing.Point(18, 250);
            this.exportPass.Name = "exportPass";
            this.exportPass.Size = new System.Drawing.Size(164, 20);
            this.exportPass.TabIndex = 17;
            // 
            // exporter
            // 
            this.exporter.Enabled = false;
            this.exporter.Location = new System.Drawing.Point(202, 211);
            this.exporter.Name = "exporter";
            this.exporter.Size = new System.Drawing.Size(138, 59);
            this.exporter.TabIndex = 18;
            this.exporter.Text = "Export to .moose file";
            this.exporter.UseVisualStyleBackColor = true;
            this.exporter.Click += new System.EventHandler(this.exporter_Click);
            // 
            // loadBtn
            // 
            this.loadBtn.Location = new System.Drawing.Point(18, 146);
            this.loadBtn.Name = "loadBtn";
            this.loadBtn.Size = new System.Drawing.Size(97, 44);
            this.loadBtn.TabIndex = 19;
            this.loadBtn.Text = "Load from .moose file";
            this.loadBtn.UseVisualStyleBackColor = true;
            this.loadBtn.Click += new System.EventHandler(this.loadBtn_Click);
            // 
            // aobEdit
            // 
            this.aobEdit.Enabled = false;
            this.aobEdit.Location = new System.Drawing.Point(356, 146);
            this.aobEdit.Name = "aobEdit";
            this.aobEdit.Size = new System.Drawing.Size(97, 44);
            this.aobEdit.TabIndex = 20;
            this.aobEdit.Text = "Edit";
            this.aobEdit.UseVisualStyleBackColor = true;
            this.aobEdit.Click += new System.EventHandler(this.aobEdit_Click);
            // 
            // aobSave
            // 
            this.aobSave.Enabled = false;
            this.aobSave.Location = new System.Drawing.Point(243, 146);
            this.aobSave.Name = "aobSave";
            this.aobSave.Size = new System.Drawing.Size(97, 44);
            this.aobSave.TabIndex = 21;
            this.aobSave.Text = "Save";
            this.aobSave.UseVisualStyleBackColor = true;
            this.aobSave.Click += new System.EventHandler(this.aobSave_Click);
            // 
            // openMOOSE
            // 
            this.openMOOSE.FileName = "file.moose";
            this.openMOOSE.Filter = "Moose Files (*.moose)|*.moose";
            // 
            // Replacer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(583, 290);
            this.Controls.Add(this.aobSave);
            this.Controls.Add(this.aobEdit);
            this.Controls.Add(this.loadBtn);
            this.Controls.Add(this.exporter);
            this.Controls.Add(this.exportPass);
            this.Controls.Add(this.exportName);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.aobList);
            this.Controls.Add(this.done);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.aobName);
            this.Controls.Add(this.removeAoB);
            this.Controls.Add(this.addAoB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.aobReplace);
            this.Controls.Add(this.aobSearch);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Replacer";
            this.Text = "AoB Editor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox aobSearch;
        private System.Windows.Forms.TextBox aobReplace;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button addAoB;
        private System.Windows.Forms.Button removeAoB;
        private System.Windows.Forms.TextBox aobName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button done;
        private System.Windows.Forms.ListBox aobList;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox exportName;
        private System.Windows.Forms.TextBox exportPass;
        private System.Windows.Forms.Button exporter;
        private System.Windows.Forms.Button loadBtn;
        private System.Windows.Forms.Button aobEdit;
        private System.Windows.Forms.Button aobSave;
        private System.Windows.Forms.OpenFileDialog openMOOSE;
    }
}