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

﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ABCParser;

namespace TFM_Client
{
    public partial class Replacer : Form
    {
        internal List<string[]> aobs = new List<string[]>();
        private TFM main = null;
        private string[] temp = null;
        private byte[] file = null;
        GetKey k = null;

        public Replacer(TFM t)
        {
            this.FormClosed += new FormClosedEventHandler(Replacer_FormClosed);
            main = t;
            InitializeComponent();
        }

        private void Replacer_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                k.Close();
            }
            catch { }
            main.PostChoose();
        }

        private void addAoB_Click(object sender, EventArgs e)
        {
            if (aobSearch.Text != "" && aobReplace.Text != "")
            {
                if (aobSearch.Text.Replace(" ", "").Length % 2 == 0 && aobReplace.Text.Replace(" ", "").Length % 2 == 0)
                {
                    foreach (string[] s in aobs)
                    {
                        if (s[0] == aobName.Text)
                        {
                            MessageBox.Show("Please use a different AoB name.");
                            return;
                        }
                    }
                    string[] o = new string[3];
                    o[0] = aobName.Text;
                    o[1] = aobSearch.Text;
                    o[2] = aobReplace.Text;
                    aobs.Add(o);
                    aobList.Items.Add(o[0]);
                    exporter.Enabled = true;
                    removeAoB.Enabled = true;
                    aobEdit.Enabled = true;
                    aobSearch.Text = "";
                    aobReplace.Text = "";
                    aobName.Text = "AoB" + (aobList.Items.Count + 1).ToString();
                }
                else
                {
                    MessageBox.Show("Please check your AoBs.");
                }
            }
            else
            {
                MessageBox.Show("Search/Replace fields must not be blank.");
            }
        }

        private void removeAoB_Click(object sender, EventArgs e)
        {
            if (aobList.SelectedItem != null)
            {
                try
                {
                    string[] found = null;
                    foreach (string[] o in aobs)
                    {
                        if (o[0] == aobList.SelectedItem.ToString())
                        {
                            found = o;
                            break;
                        }
                    }
                    aobs.Remove(found);
                }
                catch
                {
                    MessageBox.Show("Failed to remove item");
                }
            }
            aobList.Items.Remove(aobList.SelectedItem);
            if (aobList.Items.Count == 0)
            {
                removeAoB.Enabled = false;
                exporter.Enabled = false;
                aobEdit.Enabled = false;
            }
        }

        private void done_Click(object sender, EventArgs e)
        {
            if (aobList.Items.Count > 0)
            {
                main.Replacing = true;
            }
            this.Close();
        }

        private void aobEdit_Click(object sender, EventArgs e)
        {
            if (aobList.SelectedItem != null)
            {
                foreach (string[] s in aobs)
                {
                    if (s[0] == aobList.SelectedItem.ToString())
                    {
                        aobName.Text = s[0];
                        aobSearch.Text = s[1];
                        aobReplace.Text = s[2];
                        temp = s;
                        aobSave.Enabled = true;
                        break;
                    }
                }
            }
        }

        private void aobSave_Click(object sender, EventArgs e)
        {
            aobSave.Enabled = false;
            foreach (string[] s in aobs)
            {
                if (s[0] == temp[0])
                {
                    s[0] = aobName.Text;
                    s[1] = aobSearch.Text;
                    s[2] = aobReplace.Text;
                    temp = null;
                    aobSearch.Text = "";
                    aobReplace.Text = "";
                    aobName.Text = "AoB" + (aobList.Items.Count + 1).ToString();
                    break;
                }
            }
        }

        private void exporter_Click(object sender, EventArgs e)
        {
            if (exportName.Text != "" && exportPass.Text != "")
            {
                if (exportPass.Text.Length < 12)
                {
                    MessageBox.Show("Password must be at least 12 characters long.");
                    return;
                }
                string bef = "";
                foreach (string[] s in aobs)
                {
                    bef += s[0] + "\0" + s[1] + "\0" + s[2] + "\0";
                }
                byte[] final = ABC_Tools.StringToByteArray("TFMOOSE" + EncDec.Encrypt(bef, exportPass.Text));
                System.IO.File.WriteAllBytes(exportName.Text + ".moose", final);
                MessageBox.Show("File saved as " + exportName.Text + ".moose");
            }
            else
            {
                MessageBox.Show("File Name/Password fields must not be empty.");
            }
        }

        internal void DecData(string key, GetKey ent)
        {
            ent.Close();
            try
            {
                string[] deced = EncDec.Decrypt(ABC_Tools.ByteArrayToString(file, 7) , key).Split('\0');
                aobs = new List<string[]>();
                while (aobList.Items.Count > 0)
                {
                    aobList.Items.RemoveAt(0);
                }
                int ary_i = 0;
                string[] stemp = new string[3];
                foreach (string s in deced)
                {
                    stemp[ary_i++] = s;
                    if (ary_i >= 3)
                    {
                        ary_i = 0;
                        aobs.Add(stemp);
                        aobList.Items.Add(stemp[0]);
                        stemp = new string[3];
                    }
                }
                aobEdit.Enabled = true;
                removeAoB.Enabled = true;
                exporter.Enabled = true;
            }
            catch
            {
                MessageBox.Show("Invalid Password.");
            }
        }

        private void loadBtn_Click(object sender, EventArgs e)
        {
            if (openMOOSE.ShowDialog() == DialogResult.OK)
            {
                file = ABC_Tools.GetBytesFromFile(System.IO.Path.GetFullPath(openMOOSE.FileName));
                if (ABC_Tools.ByteArrayToString(file, 0, 7) == "TFMOOSE")
                {
                    k = new TFM_Client.GetKey(this);
                    k.Show();
                }
                else
                {
                    MessageBox.Show("Invalid .moose file.");
                }
            }
        }
    }
}
