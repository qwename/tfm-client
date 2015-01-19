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

﻿//Product of Transformoose Productions
//Copyright Qwename 2011

#define release

using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using System.Drawing.Imaging;
using ShockwaveFlashObjects;
using ABCParser;

namespace TFM_Client
{
    public partial class TFM : Form
    {
        private double version = 2.8;
        private int minor = 3;
        FormState formState = new FormState();
        private bool maximized = false;
        private System.Diagnostics.Process p;
        private MemoryWriter editor = new MemoryWriter(System.Diagnostics.Process.GetCurrentProcess());
        private byte[] data;
        private Stream dataStm;
        private AxShockwaveFlashObjects.AxShockwaveFlash tfmHolder;
        internal static string logData = "";
        private bool logging = false;
        private logHistory logger;
        private ABCParser.SwfStruct swf;
        private bool VERBOSE = true;
        internal bool Replacing = false;
        private Replacer r;
        private string[] browser = { "5.0 (Windows NT 6.1; WOW64) AppleWebKit/534.30 (KHTML, like Gecko) Chrome/12.0.742.91 Safari/534.30-Netscape", "5.0 (Windows)-Netscape", "9.80 (Windows NT 6.1; U; en)-Opera", "5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0; SLCC2; .NET CLR 2.0.50727; .NET CLR 3.5.30729; .NET CLR 3.0.30729; Media Center PC 6.0; .NET4.0C; .NET4.0E; InfoPath.3)-Microsoft Internet Explorer" };

        public TFM()
        {
            /*
            using (var needed = Assembly.GetExecutingAssembly().GetManifestResourceStream("TFM_Client.Resources.Interop.ShockwaveFlashObjects.dll"))
            {
                byte[] neededData = new byte[needed.Length];
                needed.Read(neededData, 0, (int)needed.Length);
                AppDomain.CurrentDomain.Load(neededData);
            }
            */
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
                {
                    string resourceName = new AssemblyName(args.Name).Name + ".dll";
                    string resource = Array.Find(this.GetType().Assembly.GetManifestResourceNames(), element => element.EndsWith(resourceName));

                    using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource))
                    {
                        Byte[] assemblyData = new Byte[stream.Length];
                        stream.Read(assemblyData, 0, assemblyData.Length);
                        return Assembly.Load(assemblyData);
                    }

                };
            InitializeComponent();
            this.Text += " v" + version.ToString() + "," + minor.ToString();
            p = new System.Diagnostics.Process();
            p.StartInfo.FileName = Application.ExecutablePath;
        }

        internal void showStrip()
        {
            swfName.Visible = loadSwf.Visible = alertMsg.Visible = showLog.Visible = menuStrip1.Visible = true;
        }

        internal void hideStrip()
        {
            swfName.Visible = loadSwf.Visible = alertMsg.Visible = showLog.Visible = menuStrip1.Visible = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            //this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //this.TransparencyKey = this.BackColor = Color.Transparent;
            #if !release
            this.Text += " BETA";
            swfName.Enabled = false;
            swfName.Visible = false;
            loadSwf.Enabled = false;
            loadSwf.Visible = false;
            alertMsg.Visible = false;
            #endif
            startClient();
        }

        private void startClient()
        {
            /*
             * 0 ——相当于 Quality2 取 "Low"

                1 ——相当于 Quality2 取 "High"

                2 ——相当于 Quality2 取 "AutoLow"

                3 ——相当于 Quality2 取 "AutoHigh"
             */
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TFM));
            this.tfmHolder = new AxShockwaveFlashObjects.AxShockwaveFlash();
            this.tfmHolder.BeginInit();
            this.tfmHolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tfmHolder.Enabled = true;
            this.tfmHolder.Location = new System.Drawing.Point(0, 27);
            this.tfmHolder.Name = "tfmHolder";
            this.tfmHolder.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("tfmHolder.OcxState")));
            this.tfmHolder.Size = new System.Drawing.Size(800, 600);
            this.tfmHolder.TabIndex = 0;
            this.tfmHolder.TabStop = false;
            this.Controls.Add(this.tfmHolder);
            this.tfmHolder.EndInit();
            
            tfmHolder.Quality = 3;
            tfmHolder.FlashCall += new AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEventHandler(
                FlashCallEventHandler);
            tfmHolder.PreviewKeyDown += new PreviewKeyDownEventHandler(tfmHolder_PreviewKeyDown);
            dataStm = Assembly.GetExecutingAssembly().GetManifestResourceStream("TFM_Client.Resources.DeathStar");
            data = new byte[dataStm.Length];
            dataStm.Read(data, 0, (int)dataStm.Length);
            InitFlashMovie(tfmHolder, data);
            dataStm = null;
            data = null;
            MemoryWriter.FlushMemory();
        }

        private void tfmHolder_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Escape:
                    if (maximized)
                    {
                        boostUp(this);
                    }
                    break;
            }
        }

        private void boostUp(Form target)
        {
            formState.Restore(target);
            menuStrip1.Visible = !menuStrip1.Visible;
            maximized = !maximized;
        }

        private void saveScreenshot()
        {
            Rectangle bounds = tfmHolder.RectangleToScreen(tfmHolder.ClientRectangle);
            using (Bitmap screen = new Bitmap(bounds.Width, bounds.Height, tfmHolder.CreateGraphics()))
            {
                using (Graphics gfx = Graphics.FromImage(screen))
                {
                    IntPtr gfxthing = gfx.GetHdc();
                    MemoryWriter.PrintWindow(tfmHolder.Handle, gfxthing, 0);
                    gfx.ReleaseHdc(gfxthing);
                }
                screen.Save(Application.StartupPath + "\\" + "Transformice_" + getTime("yyyyMMddHHmmssffff") + ".jpg", ImageFormat.Jpeg);
            }
        }

        private static string getTime()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff");
        }

        private static string getTime(string format)
        {
            return DateTime.Now.ToString(format);
        }


        private void FlashCallEventHandler(object sender, AxShockwaveFlashObjects._IShockwaveFlashEvents_FlashCallEvent e)
        {
            // message is in xml format so we need to parse it
            XmlDocument document = new XmlDocument();
            document.LoadXml(e.request);
            // get attributes to see which command flash is trying to call
            XmlAttributeCollection attributes = document.FirstChild.Attributes;
            String command = attributes.Item(0).InnerText;
            // get parameters
            XmlNodeList list = document.GetElementsByTagName("arguments");
            // Interpret command
            switch (command)
            {
                case "recupLangue":
                    tfmHolder.SetReturnValue("<string>EN</string>");
                    break;
                case "peekaboo":
                    if (editor.Patch("file:///", "http://www.transformice.com/Transformice.swf?n=" + list[0].InnerText + "\0\0\0\0")) boot("elif", "qwename");
                    else boot("elif", "emanewq");
                    break;
                case "cows":
                    if (editor.Patch("/[[DYNAMIC]]/1", "\0\0\0")) boot("override", "qwename");
                    break;
                case "test":
                    log(list[0].InnerText);
                    break;
                case "debug":
                    MessageBox.Show(list[0].InnerText);
                    break;
                case "loaded":
                    loadSwf.Enabled = true;
                    break;
                case "Arise":
                    GlobalMouseHandler globalClick = new GlobalMouseHandler();
                    globalClick.INIT(this, menuStrip1.ClientRectangle);
                    menuStrip1.Visible = false;
                    Application.AddMessageFilter(globalClick);
                    swf = new SwfStruct(Convert.FromBase64String(list[0].InnerText), this);
                    try
                    {
                        //File.WriteAllBytes("ola.swf", GetTFMSwf(swf));
                        boot("DDoS", Convert.ToBase64String(GetTFMSwf(swf)));
                    }
                    catch (Exception E)
                    {
                        update(E.ToString());
                    }
                    break;
                case "AOB":
                    if (list[0].InnerText == "Clearance")
                    {
                        r = new Replacer(this);
                        r.Show();
                    }
                    break;
                case "function(){return navigator.appVersion+\'-\'+navigator.appName;}":
                    tfmHolder.SetReturnValue("<string>" + browser[((int)NextDouble(new Random(), 0, 3))] + "</string>");
                    //boot("something", "5.0 (Windows NT 6.1; WOW64) AppleWebKit/534.30 (KHTML, like Gecko) Chrome/12.0.742.91 Safari/534.30-Netscape");
                    break;
                case "window.location.href.toString":
                    tfmHolder.SetReturnValue("<string>http://www.transformice.com/</string");
                    break;
                default:
                    update("Unknown " + command);
                    break;
            }
        }

        double NextDouble(Random rng, double min, double max)
        {
            return min + (rng.NextDouble() * (max - min));
        }

        internal void PostChoose()
        {
            boot("BOB", "BOA");
        }

        private byte[] GetTFMSwf(SwfStruct s)
        {
            byte[][] preFinal = new byte[3][];
            int dataStart = 0, finalLen = 0, i = 0, check = -1, prev = 0, buffer = 0;
            int[] sequence = new int[3], indexSeq = new int[3];
            foreach (ABCParser.SwfStruct.Tag ta in s.GetTags)
            {
                if (ta.Header.TagType == 87)
                {
                    dataStart = (int)(ta.Offset + ta.Header.TagLength - ta.Header.Length + 6);
                    finalLen += (int)ta.Header.Length - 6;
                    preFinal[i] = new byte[ta.Header.Length - 6];
                    Buffer.BlockCopy(s.GetSwf, dataStart, preFinal[i], 0, (int)ta.Header.Length - 6);
                    indexSeq[i] = dataStart;
                    update("Part " + (i + 1).ToString() + " found.");
                    if (++i >= preFinal.Length) break;
                }
            }
            i = 0;
            int previ = 0;
            byte[] final = null, final2 = null;
            update("Combining all parts...");
            while (true)
            {
                if (check == -1 && (ABC_Tools.ByteArrayToString(preFinal[i], 0, 3) == "CWS" || ABC_Tools.ByteArrayToString(preFinal[i], 0, 3) == "FWS"))
                {
                    update("Found First");
                    final = new byte[finalLen];
                    final2 = new byte[final.Length];
                    Buffer.BlockCopy(preFinal[i], 0, final, 0, preFinal[i].Length);
                    Buffer.BlockCopy(preFinal[i], 0, final2, 0, preFinal[i].Length);
                    check = 0;
                    prev += preFinal[i].Length;
                    sequence[0] = i;
                }
                else if (check == 0)//&& ABC_Tools.ByteArrayToHexString(preFinal[i], (uint)preFinal[i].Length - 5, 5) != "0040000000")
                {
                    update("Found Second " + prev.ToString());
                    Buffer.BlockCopy(preFinal[i], 0, final, prev, preFinal[i].Length);
                    check = 1;
                    prev += preFinal[i].Length;
                    sequence[1] = i;
                }
                else if (check == 1)//&& ABC_Tools.ByteArrayToHexString(preFinal[i], (uint)preFinal[i].Length - 5, 5) == "0040000000")
                {
                    update("Found Third " + prev.ToString());
                    Buffer.BlockCopy(preFinal[i], 0, final, prev, preFinal[i].Length);
                    Buffer.BlockCopy(preFinal[i], 0, final2, prev - preFinal[previ].Length, preFinal[i].Length);
                    Buffer.BlockCopy(preFinal[previ], 0, final2, prev - preFinal[previ].Length + preFinal[i].Length, preFinal[previ].Length);
                    check = 2;
                    sequence[2] = i;
                }
                previ = i;
                i++;
                if (check == 2)
                {
                    File.WriteAllBytes("final.swf", final);
                    File.WriteAllBytes("final2.swf", final2);
                    bool compressed = false;
                    if (final[0] == 0x43) compressed = true;
                    SwfStruct tfm = null;
                    try
                    {
                        tfm = new SwfStruct(final, this);
                    }
                    catch
                    {
                        tfm = new SwfStruct(final2, this);
                    }
                    if (Replacing)
                    {
                        log("Start Replacing");
                        foreach (string[] aob in r.aobs)
                        {
                            Replace(ref tfm.newSwf, aob[1], aob[2], aob[0]);
                        }
                        log("end");
                    }
                    /*
                    //No ads
                    Replace(ref tfm.newSwf, "d0 30 d0 66 ?? ?? 27 61 ?? ?? 60 ?? ?? 4f ?? ?? ?? 47", "47 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02");
                    //No Explosions
                    Replace(ref tfm.newSwf, "D0 66 ?? ?? 14 ?? ?? ?? 60 ?? ?? ?? ?? 24 0A A3 46 ?? ?? 01 ?? ?? ??", "D0 66 ?? ?? 14 00 00 00 02 02 02 ?? ?? 24 00 02 02 02 02 02 02 ?? ??");
                    //Anti AFK
                    Replace(ref tfm.newSwf, "D0 66 ?? ?? A1 2D ?? AF 2A 12 ?? ?? ??", "D0 66 ?? ?? A1 24 00 AD 2A 12 ?? ?? ??");
                    //Jump hack
                    //-12
                    //Replace(ref tfm.newSwf, "60 ?? 66 ?? ?? 66 ?? ?? 66 ?? ?? 66 ?? ?? 24 FB", "60 ?? 66 ?? ?? 66 ?? ?? 66 ?? ?? 66 ?? ?? 24 F4");
                    //-8
                    Replace(ref tfm.newSwf, "60 ?? 66 ?? ?? 66 ?? ?? 66 ?? ?? 66 ?? ?? 24 FB", "60 ?? 66 ?? ?? 66 ?? ?? 66 ?? ?? 66 ?? ?? 24 F8");
                    //No Death
                    Replace(ref tfm.newSwf, "D0 66 ?? ?? 26 61 ?? ?? 60 ?? ?? 66 ?? ?? 5D ?? ?? D0 66 ?? ?? 4A ?? ?? 01 4F ?? ?? 01", "02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02 02");
                    //Fly Hack
                    Replace(ref tfm.newSwf, "0E ?? ?? ?? D0 24 FF 68 ?? ?? D0 D0", "0E ?? ?? ?? D0 24 01 68 ?? ?? D0 D0");
                    //Spawn Anywhere
                    Replace(ref tfm.newSwf, "D0 66 ?? ?? 76 2A 11 ?? ?? ?? 29 ?? ?? D0 60 ?? ?? 66 ?? ?? 66 ?? ?? AD 12 ?? ?? ??", "D0 66 ?? ?? 76 2A 11 ?? ?? ?? 29 ?? ?? D0 60 ?? ?? 66 ?? ?? 66 ?? ?? AD 12 00 00 00");
                    //Instant Spawn
                    Replace(ref tfm.newSwf, "D0 60 ?? ?? 66 ?? ?? 66 ?? ?? 0E ?? ?? ?? D0 4F ?? ?? 00 10 ?? ?? ??", "D0 60 ?? ?? 66 ?? ?? 66 ?? ?? 0E 00 00 00 D0 4F ?? ?? 00 10 ?? ?? ??");
                    //Crazy Spawn
                    Replace(ref tfm.newSwf, "62 06 66 ?? ?? 24 03 a1 4f ?? ?? ?? d0 4f ?? ?? ?? d0 4f ?? ?? ?? 47", "62 06 66 ?? ?? 24 03 a1 4f ?? ?? ?? 02 02 02 02 02 02 02 02 02 02 47");
                    //
                    //Replace(ref tfm.newSwf, "", "");
                     */
                    if (compressed)
                    {
                        tfm.newSwf = ABC_Tools.CompressSwf(tfm.newSwf);
                    }
                    //byte[] fin = new byte[finalLen];
                    byte[] fin = new byte[tfm.newSwf.Length];
                    Buffer.BlockCopy(tfm.newSwf, 0, fin, 0, tfm.newSwf.Length);
                    //File.WriteAllBytes("ola2.swf", fin);
                    byte[] ret = s.newSwf;
                    i = 0;
                    Buffer.BlockCopy(fin, 0, preFinal[sequence[0]], 0, preFinal[sequence[0]].Length);
                    Buffer.BlockCopy(fin, preFinal[sequence[0]].Length, preFinal[sequence[1]], 0, preFinal[sequence[1]].Length);
                    Buffer.BlockCopy(fin, preFinal[sequence[0]].Length + preFinal[sequence[1]].Length, preFinal[sequence[2]], 0, preFinal[sequence[2]].Length);
                    while (i < preFinal.Length)
                    {
                        Buffer.BlockCopy(preFinal[sequence[i]], 0, ret, indexSeq[sequence[i]], preFinal[sequence[i]].Length);
                        preFinal[sequence[i++]] = null;
                    }
                    
                    fin = null;
                    preFinal = null;
                    sequence = null;
                    indexSeq = null;
                    s.Delete();
                    tfm.Delete();
                    return ret;
                }
                else if (i >= preFinal.Length) i = 0;
                if (buffer++ > 20)
                {
                    update("Could not get Transformice swf from downloaded swf.");
                    break;
                }
            }
            return null;
        }

        private void Replace(ref byte[] b, string search, string replace, string name)
        {
            bool success = ABC_Tools.Replace(ref b, search, replace);
            log("\n" + name + "\n" + search + "\n>\n" + replace + "\n" + (success ? "Replaced" : "Not found") + "\n");
        }

        private void Replace(ref byte[] b, string search, string replace)
        {
            bool success = ABC_Tools.Replace(ref b, search, replace);
            log("\n" + search + "\n>\n" + replace + "\n" + (success ? "Replaced" : "Not found") + "\n");
        }

        public void update(string s)
        {
            if (VERBOSE)
            log(s);
        }

        private void log(string msg)
        {
            msg = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:ffff ") + msg;
            alertMsg.Text = msg;
            logData += msg + "\n";
            if (logging) logger.update(msg);
        }

        private void InitFlashMovie(AxShockwaveFlashObjects.AxShockwaveFlash flashObj, byte[] swfFile)
        {
            using (MemoryStream stm = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stm))
                {
                    /* Write length of stream for AxHost.State */
                    writer.Write(8 + swfFile.Length);
                    /* Write Flash magic 'fUfU' */
                    writer.Write(0x55665566);
                    /* Length of swf file */
                    writer.Write(swfFile.Length);
                    writer.Write(swfFile);
                    stm.Seek(0, SeekOrigin.Begin);
                    /* 1 == IPeristStreamInit */
                    flashObj.OcxState = new AxHost.State(stm, 1, false, null);
                }
            }
        }

        private void InitFlashMovie(AxShockwaveFlashObjects.AxShockwaveFlash flashObj, Stream stm)
        {
            using (MemoryStream data = new MemoryStream())
            {
                using (StreamWriter writer = new StreamWriter(data))
                {
                    /* Write length of stream for AxHost.State */
                    writer.Write(8 + stm.Length);
                    /* Write Flash magic 'fUfU' */
                    writer.Write(0x55665566);
                    /* Length of swf file */
                    writer.Write(stm);
                    data.Seek(0, SeekOrigin.Begin);
                    MessageBox.Show(data.Length.ToString());
                    /* 1 == IPeristStreamInit */
                    flashObj.OcxState = new AxHost.State(data, 1, false, null);
                }
            }
        }

        private void boot(String function, String validate)
        {
            tfmHolder.CallFunction("<invoke name=\"" + function + "\" returntype=\"xml\"><arguments><string>" + validate + "</string></arguments></invoke>");
        }

        private void restartStrip_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to restart?", "Exit", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                System.Diagnostics.Process.Start(Application.ExecutablePath);
                Application.Exit();
                //Application.Restart();
            }
        }

        private void exitStrip_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit?", "Exit", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        private void fullscreenStrip_Click(object sender, EventArgs e)
        {
            if (!maximized)
            {
                formState.Maximize(this);
                menuStrip1.Visible = false;
                maximized = true;
            }
            else
            {
                formState.Restore(this);
                menuStrip1.Visible = true;
                maximized = false;
            }
        }

        private void aboutStrip_Click(object sender, EventArgs e)
        {
            About info = new About();
            info.Show();
        }

        private void screenshotStrip_Click(object sender, EventArgs e)
        {
            saveScreenshot();
        }

        private void newClientStrip_Click(object sender, EventArgs e)
        {
            p = new System.Diagnostics.Process();
            p.StartInfo.FileName = Application.ExecutablePath;
            p.Start();
        }

        private void flshMemStrip_Click(object sender, EventArgs e)
        {
            MemoryWriter.FlushMemory();
        }

        private void FLUSH_Tick(object sender, EventArgs e)
        {
            MemoryWriter.FlushMemory();
        }

        private void loadSwf_Click(object sender, EventArgs e)
        {
            loadSwf.Enabled = false;
            if (swfName.Text.EndsWith(".swf"))
            {
                boot("swf", swfName.Text);
            }
            else
            {
                GetKey k = new GetKey(this);
                k.Show();
            }
        }

        internal void DecData(string key, GetKey ent)
        {
            ent.Hide();
            ent.Close();
            try
            {
                string deced = Convert.ToBase64String(EncDec.Decrypt(ABC_Tools.GetBytesFromFile(swfName.Text), key));
                boot("bytes", deced);
            }
            catch
            {
                log("Failed to load " + swfName.Text);
                loadSwf.Enabled = true;
            }
        }

        internal void CancelDec()
        {
            log("Loading cancelled ny user");
            loadSwf.Enabled = true;
        }

        private void showLog_Click(object sender, EventArgs e)
        {
            showLog.Enabled = false;
            logging = true;
            logger = null;
            logger = new logHistory();
            logger.FormClosing += new FormClosingEventHandler(logger_FormClosing);
            logger.Show();
        }

        private void logger_FormClosing(object sender, FormClosingEventArgs e)
        {
            logging = false;
            showLog.Enabled = true;
        }

        private void ScaleCheck()
        {
            showAllToolStripMenuItem.Checked = false;
            noBorderToolStripMenuItem.Checked = false;
            exactFitToolStripMenuItem.Checked = false;
            noScaleToolStripMenuItem.Checked = false;
        }

        private void showAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
             * 0 ——相当于 Scale 取 "ShowAll"

                1 ——相当于 Scale 取 "NoBorder"

                2 ——相当于 Scale 取 "ExactFit"
        
                3 - - No Scale
             */
            tfmHolder.ScaleMode = 0;
            ScaleCheck();
            showAllToolStripMenuItem.Checked = true;
        }

        private void noBorderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tfmHolder.ScaleMode = 1;
            ScaleCheck();
            noScaleToolStripMenuItem.Checked = true;
        }

        private void exactFitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tfmHolder.ScaleMode = 2;
            ScaleCheck();
            exactFitToolStripMenuItem.Checked = true;
        }

        private void noScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tfmHolder.ScaleMode = 3;
            ScaleCheck();
            noScaleToolStripMenuItem.Checked = true;
        }

        private void AlignCheck()
        {
            topToolStripMenuItem.Checked = false;
            leftToolStripMenuItem.Checked = false;
            rightToolStripMenuItem.Checked = false;
            bottomToolStripMenuItem.Checked = false;
            middleToolStripMenuItem.Checked = false;
        }

        private void topToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tfmHolder.AlignMode = 4;
        }

        private void leftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
             * Default - 0
             * Left - 1
             * Right - 2
             * Top - 4
             * Bottom - 8
             */
            tfmHolder.AlignMode = 1;
            AlignCheck();
            leftToolStripMenuItem.Checked = true;
        }

        private void rightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tfmHolder.AlignMode = 2;
            AlignCheck();
            rightToolStripMenuItem.Checked = true;
        }

        private void bottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tfmHolder.AlignMode = 8;
            AlignCheck();
            bottomToolStripMenuItem.Checked = true;
        }

        private void middleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tfmHolder.AlignMode = 0;
            AlignCheck();
            middleToolStripMenuItem.Checked = true;
        }

        private void windowToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            tfmHolder.WMode = "Window";
        }

        private void opaqueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tfmHolder.WMode = "Opaque";
        }

        private void transparentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tfmHolder.WMode = "Transparent";
        }

    }

    public class GlobalMouseHandler : IMessageFilter
    {
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_CHAR = 0x102;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONDOWN = 0x201;
        private const int WM_LBUTTONUP = 0x0202;
        private TFM main;
        private Rectangle r;

        public void INIT(TFM t, Rectangle rec)
        {
            r = rec;
            main = t;
        }

        public bool PreFilterMessage(ref Message m)
        {
            switch (m.Msg)
            {
                case WM_MOUSEMOVE:
                    Point p = new Point(Get_MOUSE_X_LPARAM(m.LParam), Get_MOUSE_Y_LPARAM(m.LParam));
                    if (r.Contains(p))
                    {
                        main.showStrip();
                    }
                    else
                    {
                        main.hideStrip();
                    }
                    break;
                default:
                    break;
            }
            return false;
        }

        private int Get_MOUSE_X_LPARAM(IntPtr i)
        {
            return (int)i & 0xFFFF;
        }

        private int Get_MOUSE_Y_LPARAM(IntPtr i)
        {
            return (int)((int)i & 0xFFFF0000) >> 16;
        }
    }
}
