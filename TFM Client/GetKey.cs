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
    public partial class GetKey : Form
    {
        private TFM tfm = null;
        private Replacer replace = null;

        public GetKey(TFM t)
        {
            tfm = t;
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(enterKey_KeyDown);
            keyValue.KeyDown += new KeyEventHandler(enterKey_KeyDown);
            this.FormClosed += new FormClosedEventHandler(GetKey_FormClosed);
        }

        public GetKey(Replacer r)
        {
            replace = r;
            InitializeComponent();
            this.KeyDown += new KeyEventHandler(enterKey_KeyDown);
            keyValue.KeyDown += new KeyEventHandler(enterKey_KeyDown);
            this.FormClosed += new FormClosedEventHandler(GetKey_FormClosed);
        }

        private void GetKey_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (tfm != null)
            {
                tfm.CancelDec();
            }
        }

        private void enterKey_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                if (tfm != null)
                {
                    tfm.DecData(keyValue.Text, this);
                }
                else
                {
                    replace.DecData(keyValue.Text, this);
                }
            }
        }
    }
}
