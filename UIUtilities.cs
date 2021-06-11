﻿using System.Windows.Forms;
using System;
using System.Drawing;

namespace Instagram
{
    class UIUtilities
    {
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        PictureBox btn_Close, btn_Minimize, btn_Maximize;
        Panel panel;
        bool lightModeOn, isMaximized;
        int formWidth, formHeight;
        private Main formRef;

        public UIUtilities(Main f,bool formLightModeOn)
        {
            formWidth = f.Width;
            formHeight = f.Height;
            lightModeOn = formLightModeOn;
            formRef = f;
            if (f.WindowState == FormWindowState.Normal)
                isMaximized = false;
            else
                isMaximized = true;
            Initialize_Top_Left_Buttons();
            Initialize_Side_Panel();
        }

        private void Initialize_Top_Left_Buttons()
        {
            int x = formWidth, btn_Width = 45, btn_Height = 20;
            string location;
            Color backColor, hoverColor;
            location = return_ui_location();
            if (lightModeOn)
            {
                backColor = Color.FromArgb(255, 255, 255);
                hoverColor = Color.FromArgb(217, 217, 217);
            }
            else
            {
                backColor = Color.FromArgb(0, 0, 0);
                hoverColor = Color.FromArgb(57, 57, 57);
            }
            // Initializing
            btn_Close = Create_Button("close", new int[] {btn_Width, btn_Height}, new int[] { x - btn_Width, 0 }, backColor, false);
            btn_Maximize = Create_Button("maximize", new int[] {btn_Width, btn_Height}, new int[] { x - (btn_Width*2), 0 }, backColor, false);
            btn_Minimize = Create_Button("minimize", new int[] {btn_Width, btn_Height}, new int[] { x - (btn_Width*3), 0 }, backColor, false);
            // Setting up special settings
            btn_Close.MouseClick += new MouseEventHandler((o, a) => Application.Exit());
            btn_Close.MouseHover += new EventHandler((o, a) => btn_Close.BackColor = Color.Red);
            btn_Close.MouseLeave += new EventHandler((o, a) => btn_Close.BackColor = backColor);
            btn_Maximize.MouseClick += new MouseEventHandler((o, a) =>
            {
                if (!isMaximized)
                {
                    formRef.WindowState = FormWindowState.Maximized;
                    isMaximized = true;
                }
                else
                {
                    formRef.WindowState = FormWindowState.Normal;
                    isMaximized = false;
                }
            }
            );
            btn_Maximize.MouseHover += new EventHandler((o, a) => btn_Maximize.BackColor = hoverColor);
            btn_Maximize.MouseLeave += new EventHandler((o, a) => btn_Maximize.BackColor = backColor);
            btn_Minimize.MouseClick += new MouseEventHandler((o, a) =>
            {
                formRef.WindowState = FormWindowState.Minimized;
            }
            );
            btn_Minimize.MouseHover += new EventHandler((o, a) => btn_Minimize.BackColor = hoverColor);
            btn_Minimize.MouseLeave += new EventHandler((o, a) => btn_Minimize.BackColor = backColor);
        }

        public void Initialize_Side_Panel()
        {
            int y = formHeight / 2 - 110, btn_Width = 65, btn_Height = 30;
            Color panelBackgroundColor;
            if(!lightModeOn)
                panelBackgroundColor = Color.FromArgb(43, 43, 43);
            else
                panelBackgroundColor = Color.FromArgb(242, 242, 242);
            // Initializing
            panel = Create_Panel(new int[] { btn_Width, formHeight}, new int[] { 0, 0}, panelBackgroundColor);
            panel.Controls.Add(Create_Button("home", new int[] {btn_Width, btn_Height - 5}, new int[] {0, y + (int)Math.Round(btn_Height * 0.5) }, panelBackgroundColor));
            panel.Controls.Add(Create_Button("search", new int[] {btn_Width, (btn_Height-4)}, new int[] {0, (y + btn_Height *2 )}, panelBackgroundColor));
            panel.Controls.Add(Create_Button("plus", new int[] {btn_Width, btn_Height + 10}, new int[] {0, (y + (int)Math.Round((btn_Height * 3.5), MidpointRounding.AwayFromZero))}, panelBackgroundColor));
            panel.Controls.Add(Create_Button("heart", new int[] {btn_Width, btn_Height - 5}, new int[] {0, (y + (int)Math.Round((btn_Height * 5.5), MidpointRounding.AwayFromZero))}, panelBackgroundColor));
            panel.Controls.Add(Create_Button("account", new int[] {btn_Width, btn_Height - 4}, new int[] {0, (y + (int)Math.Round((btn_Height * 7.0), MidpointRounding.AwayFromZero))}, panelBackgroundColor));
        }

        public Panel Create_Panel(int[] size, int[] location, Color backColor)
        {
            Panel panel = new Panel();
            panel.Width = size[0];
            panel.Height = size[1];
            panel.Location = new System.Drawing.Point(location[0], location[1]);
            panel.BackColor = backColor;
            return panel;
        }

        private string return_ui_location()
        {
            if (lightModeOn)
                return Environment.CurrentDirectory + @"\Assets\Light Mode\UI Icons\";
            else
                return Environment.CurrentDirectory + @"\Assets\Dark Mode\UI Icons\";
        }

        public PictureBox Create_Button(string name, int[] size, int[] location, Color backColor,bool add_effects = true)
        {
            PictureBox btn = new PictureBox();
            btn.Width = size[0];
            btn.Height = size[1];
            btn.ImageLocation = return_ui_location() + name + ".png";
            btn.Location = new System.Drawing.Point(location[0], location[1]);
            btn.SizeMode = PictureBoxSizeMode.Zoom;
            btn.BringToFront();
            if(add_effects)
            {
                btn.MouseHover += new EventHandler((o, a) =>
                {
                    btn.Image.Dispose();
                    btn.Image = Image.FromFile(Environment.CurrentDirectory + @"\Assets\Selected Mode\" + name + ".png");
                });
                btn.MouseLeave += new EventHandler((o, a) =>
                {
                    btn.Image.Dispose();
                    btn.Image = Image.FromFile(return_ui_location() + name + ".png");
                });
            }
            btn.BackColor = backColor;
            return btn;
        }

        public void MouseDown(IntPtr obj, object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(obj, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        public PictureBox Get_CloseBtn()
        {
            return btn_Close;
        }

        public PictureBox Get_MaximizeBtn()
        {
            return btn_Maximize;
        }

        public PictureBox Get_MinimizeBtn()
        {
            return btn_Minimize;
        }
        public Panel Get_Panel()
        {
            return panel;
        }
    }
}
