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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TFM_Client
{
    public partial class logHistory : Form
    {
        public logHistory()
        {
            InitializeComponent();
            logBox.Text = TFM.logData;
        }

        internal void update(string msg)
        {
            logBox.Text = TFM.logData;
        }

        private void clrLog_Click(object sender, EventArgs e)
        {
            TFM.logData = "";
            logBox.Text = "";
        }
    }
}
