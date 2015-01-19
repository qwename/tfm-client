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
    partial class TFM
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TFM));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.newClientStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.restartStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.exitStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.viewMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.fullscreenStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.scaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noBorderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exactFitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noScaleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alignToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.leftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.topToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.bottomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.middleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.windowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.opaqueToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.transparentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.screenshotStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.flshMemStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.helpMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutStrip = new System.Windows.Forms.ToolStripMenuItem();
            this.FLUSH = new System.Windows.Forms.Timer(this.components);
            this.alertMsg = new System.Windows.Forms.TextBox();
            this.loadSwf = new System.Windows.Forms.Button();
            this.swfName = new System.Windows.Forms.TextBox();
            this.showLog = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileMenu,
            this.viewMenu,
            this.toolsMenu,
            this.helpMenu});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.menuStrip1.Size = new System.Drawing.Size(800, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileMenu
            // 
            this.fileMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newClientStrip,
            this.restartStrip,
            this.exitStrip});
            this.fileMenu.Name = "fileMenu";
            this.fileMenu.Size = new System.Drawing.Size(37, 20);
            this.fileMenu.Text = "&File";
            // 
            // newClientStrip
            // 
            this.newClientStrip.Name = "newClientStrip";
            this.newClientStrip.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.newClientStrip.Size = new System.Drawing.Size(175, 22);
            this.newClientStrip.Text = "&New Client";
            this.newClientStrip.Click += new System.EventHandler(this.newClientStrip_Click);
            // 
            // restartStrip
            // 
            this.restartStrip.Name = "restartStrip";
            this.restartStrip.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.restartStrip.Size = new System.Drawing.Size(175, 22);
            this.restartStrip.Text = "&Restart Client";
            this.restartStrip.Click += new System.EventHandler(this.restartStrip_Click);
            // 
            // exitStrip
            // 
            this.exitStrip.Name = "exitStrip";
            this.exitStrip.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.exitStrip.Size = new System.Drawing.Size(175, 22);
            this.exitStrip.Text = "&Exit";
            this.exitStrip.Click += new System.EventHandler(this.exitStrip_Click);
            // 
            // viewMenu
            // 
            this.viewMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fullscreenStrip,
            this.scaleToolStripMenuItem,
            this.alignToolStripMenuItem,
            this.windowToolStripMenuItem});
            this.viewMenu.Name = "viewMenu";
            this.viewMenu.Size = new System.Drawing.Size(44, 20);
            this.viewMenu.Text = "&View";
            // 
            // fullscreenStrip
            // 
            this.fullscreenStrip.Name = "fullscreenStrip";
            this.fullscreenStrip.ShortcutKeys = System.Windows.Forms.Keys.F11;
            this.fullscreenStrip.Size = new System.Drawing.Size(152, 22);
            this.fullscreenStrip.Text = "&Fullscreen";
            this.fullscreenStrip.Click += new System.EventHandler(this.fullscreenStrip_Click);
            // 
            // scaleToolStripMenuItem
            // 
            this.scaleToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.showAllToolStripMenuItem,
            this.noBorderToolStripMenuItem,
            this.exactFitToolStripMenuItem,
            this.noScaleToolStripMenuItem});
            this.scaleToolStripMenuItem.Name = "scaleToolStripMenuItem";
            this.scaleToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.scaleToolStripMenuItem.Text = "Scale";
            // 
            // showAllToolStripMenuItem
            // 
            this.showAllToolStripMenuItem.CheckOnClick = true;
            this.showAllToolStripMenuItem.Name = "showAllToolStripMenuItem";
            this.showAllToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.showAllToolStripMenuItem.Text = "Show All";
            this.showAllToolStripMenuItem.Click += new System.EventHandler(this.showAllToolStripMenuItem_Click);
            // 
            // noBorderToolStripMenuItem
            // 
            this.noBorderToolStripMenuItem.CheckOnClick = true;
            this.noBorderToolStripMenuItem.Name = "noBorderToolStripMenuItem";
            this.noBorderToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.noBorderToolStripMenuItem.Text = "No Border";
            this.noBorderToolStripMenuItem.Click += new System.EventHandler(this.noBorderToolStripMenuItem_Click);
            // 
            // exactFitToolStripMenuItem
            // 
            this.exactFitToolStripMenuItem.CheckOnClick = true;
            this.exactFitToolStripMenuItem.Name = "exactFitToolStripMenuItem";
            this.exactFitToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.exactFitToolStripMenuItem.Text = "Exact Fit";
            this.exactFitToolStripMenuItem.Click += new System.EventHandler(this.exactFitToolStripMenuItem_Click);
            // 
            // noScaleToolStripMenuItem
            // 
            this.noScaleToolStripMenuItem.Checked = true;
            this.noScaleToolStripMenuItem.CheckOnClick = true;
            this.noScaleToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.noScaleToolStripMenuItem.Name = "noScaleToolStripMenuItem";
            this.noScaleToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
            this.noScaleToolStripMenuItem.Text = "No Scale";
            this.noScaleToolStripMenuItem.Click += new System.EventHandler(this.noScaleToolStripMenuItem_Click);
            // 
            // alignToolStripMenuItem
            // 
            this.alignToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.leftToolStripMenuItem,
            this.rightToolStripMenuItem,
            this.topToolStripMenuItem,
            this.bottomToolStripMenuItem,
            this.middleToolStripMenuItem});
            this.alignToolStripMenuItem.Name = "alignToolStripMenuItem";
            this.alignToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.alignToolStripMenuItem.Text = "Align";
            // 
            // leftToolStripMenuItem
            // 
            this.leftToolStripMenuItem.CheckOnClick = true;
            this.leftToolStripMenuItem.Name = "leftToolStripMenuItem";
            this.leftToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.leftToolStripMenuItem.Text = "Left";
            this.leftToolStripMenuItem.Click += new System.EventHandler(this.leftToolStripMenuItem_Click);
            // 
            // rightToolStripMenuItem
            // 
            this.rightToolStripMenuItem.CheckOnClick = true;
            this.rightToolStripMenuItem.Name = "rightToolStripMenuItem";
            this.rightToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.rightToolStripMenuItem.Text = "Right";
            this.rightToolStripMenuItem.Click += new System.EventHandler(this.rightToolStripMenuItem_Click);
            // 
            // topToolStripMenuItem
            // 
            this.topToolStripMenuItem.CheckOnClick = true;
            this.topToolStripMenuItem.Name = "topToolStripMenuItem";
            this.topToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.topToolStripMenuItem.Text = "Top";
            this.topToolStripMenuItem.Click += new System.EventHandler(this.topToolStripMenuItem_Click);
            // 
            // bottomToolStripMenuItem
            // 
            this.bottomToolStripMenuItem.CheckOnClick = true;
            this.bottomToolStripMenuItem.Name = "bottomToolStripMenuItem";
            this.bottomToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.bottomToolStripMenuItem.Text = "Bottom";
            this.bottomToolStripMenuItem.Click += new System.EventHandler(this.bottomToolStripMenuItem_Click);
            // 
            // middleToolStripMenuItem
            // 
            this.middleToolStripMenuItem.Checked = true;
            this.middleToolStripMenuItem.CheckOnClick = true;
            this.middleToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.middleToolStripMenuItem.Name = "middleToolStripMenuItem";
            this.middleToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
            this.middleToolStripMenuItem.Text = "Middle";
            this.middleToolStripMenuItem.Click += new System.EventHandler(this.middleToolStripMenuItem_Click);
            // 
            // windowToolStripMenuItem
            // 
            this.windowToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.windowToolStripMenuItem1,
            this.opaqueToolStripMenuItem,
            this.transparentToolStripMenuItem});
            this.windowToolStripMenuItem.Name = "windowToolStripMenuItem";
            this.windowToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.windowToolStripMenuItem.Text = "Window";
            // 
            // windowToolStripMenuItem1
            // 
            this.windowToolStripMenuItem1.Name = "windowToolStripMenuItem1";
            this.windowToolStripMenuItem1.Size = new System.Drawing.Size(137, 22);
            this.windowToolStripMenuItem1.Text = "Window";
            this.windowToolStripMenuItem1.Click += new System.EventHandler(this.windowToolStripMenuItem1_Click);
            // 
            // opaqueToolStripMenuItem
            // 
            this.opaqueToolStripMenuItem.Name = "opaqueToolStripMenuItem";
            this.opaqueToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.opaqueToolStripMenuItem.Text = "Opaque";
            this.opaqueToolStripMenuItem.Click += new System.EventHandler(this.opaqueToolStripMenuItem_Click);
            // 
            // transparentToolStripMenuItem
            // 
            this.transparentToolStripMenuItem.Name = "transparentToolStripMenuItem";
            this.transparentToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.transparentToolStripMenuItem.Text = "Transparent";
            this.transparentToolStripMenuItem.Click += new System.EventHandler(this.transparentToolStripMenuItem_Click);
            // 
            // toolsMenu
            // 
            this.toolsMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.screenshotStrip,
            this.flshMemStrip});
            this.toolsMenu.Name = "toolsMenu";
            this.toolsMenu.Size = new System.Drawing.Size(48, 20);
            this.toolsMenu.Text = "&Tools";
            // 
            // screenshotStrip
            // 
            this.screenshotStrip.Name = "screenshotStrip";
            this.screenshotStrip.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F12)));
            this.screenshotStrip.Size = new System.Drawing.Size(207, 22);
            this.screenshotStrip.Text = "&Screenshot";
            this.screenshotStrip.Click += new System.EventHandler(this.screenshotStrip_Click);
            // 
            // flshMemStrip
            // 
            this.flshMemStrip.Name = "flshMemStrip";
            this.flshMemStrip.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F11)));
            this.flshMemStrip.Size = new System.Drawing.Size(207, 22);
            this.flshMemStrip.Text = "&Flush Memory";
            this.flshMemStrip.Click += new System.EventHandler(this.flshMemStrip_Click);
            // 
            // helpMenu
            // 
            this.helpMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutStrip});
            this.helpMenu.Name = "helpMenu";
            this.helpMenu.Size = new System.Drawing.Size(44, 20);
            this.helpMenu.Text = "&Help";
            // 
            // aboutStrip
            // 
            this.aboutStrip.Name = "aboutStrip";
            this.aboutStrip.Size = new System.Drawing.Size(107, 22);
            this.aboutStrip.Text = "&About";
            this.aboutStrip.Click += new System.EventHandler(this.aboutStrip_Click);
            // 
            // FLUSH
            // 
            this.FLUSH.Enabled = true;
            this.FLUSH.Interval = 30000;
            this.FLUSH.Tick += new System.EventHandler(this.FLUSH_Tick);
            // 
            // alertMsg
            // 
            this.alertMsg.Enabled = false;
            this.alertMsg.Location = new System.Drawing.Point(406, 2);
            this.alertMsg.Name = "alertMsg";
            this.alertMsg.Size = new System.Drawing.Size(257, 20);
            this.alertMsg.TabIndex = 5;
            this.alertMsg.TabStop = false;
            this.alertMsg.Text = "Messages from client will be displayed here";
            // 
            // loadSwf
            // 
            this.loadSwf.Enabled = false;
            this.loadSwf.Location = new System.Drawing.Point(325, -1);
            this.loadSwf.Name = "loadSwf";
            this.loadSwf.Size = new System.Drawing.Size(75, 23);
            this.loadSwf.TabIndex = 6;
            this.loadSwf.Text = "Load";
            this.loadSwf.UseVisualStyleBackColor = true;
            this.loadSwf.Click += new System.EventHandler(this.loadSwf_Click);
            // 
            // swfName
            // 
            this.swfName.Location = new System.Drawing.Point(186, 2);
            this.swfName.Name = "swfName";
            this.swfName.Size = new System.Drawing.Size(133, 20);
            this.swfName.TabIndex = 7;
            this.swfName.Text = "Plugin.swf";
            // 
            // showLog
            // 
            this.showLog.Location = new System.Drawing.Point(669, 0);
            this.showLog.Name = "showLog";
            this.showLog.Size = new System.Drawing.Size(81, 24);
            this.showLog.TabIndex = 8;
            this.showLog.Text = "Message Log";
            this.showLog.UseVisualStyleBackColor = true;
            this.showLog.Click += new System.EventHandler(this.showLog_Click);
            // 
            // TFM
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSize = true;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.showLog);
            this.Controls.Add(this.swfName);
            this.Controls.Add(this.loadSwf);
            this.Controls.Add(this.alertMsg);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "TFM";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Qwename\'s Client";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileMenu;
        private System.Windows.Forms.ToolStripMenuItem restartStrip;
        private System.Windows.Forms.ToolStripMenuItem exitStrip;
        private System.Windows.Forms.ToolStripMenuItem viewMenu;
        private System.Windows.Forms.ToolStripMenuItem fullscreenStrip;
        private System.Windows.Forms.ToolStripMenuItem helpMenu;
        private System.Windows.Forms.ToolStripMenuItem aboutStrip;
        private System.Windows.Forms.ToolStripMenuItem toolsMenu;
        private System.Windows.Forms.ToolStripMenuItem screenshotStrip;
        private System.Windows.Forms.ToolStripMenuItem newClientStrip;
        private System.Windows.Forms.ToolStripMenuItem flshMemStrip;
        private System.Windows.Forms.Timer FLUSH;
        private System.Windows.Forms.TextBox alertMsg;
        private System.Windows.Forms.Button loadSwf;
        private System.Windows.Forms.TextBox swfName;
        private System.Windows.Forms.Button showLog;
        private System.Windows.Forms.ToolStripMenuItem scaleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noBorderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exactFitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem noScaleToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alignToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem topToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bottomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem leftToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rightToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem windowToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem opaqueToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem transparentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem middleToolStripMenuItem;
    }
}

