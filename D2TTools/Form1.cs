using Gma.System.MouseKeyHook;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace D2TTools
{
    public partial class Form1 : Form
    {
        private IKeyboardMouseEvents m_GlobalHook;

        public Form1()
        {
            InitializeComponent();
            try
            {
                Visible = false;
                label1.Text = "";
                //m_GlobalHook = Hook.GlobalEvents();
                //m_GlobalHook.KeyDown += GlobalHookKeyPress;
                UnregisterHotKey(Handle, ID_HOTKEY_CONVERT);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                RegisterHotKey(Handle, ID_HOTKEY_CONVERT, (int)KeyModifier.Alt, Keys.V.GetHashCode());
            }
        }

        #region Global Hook Key
        private void GlobalHookKeyPress(object sender, KeyEventArgs e)
        {
            IntPtr activeWindow = GetForegroundWindow();
            int a, b;
            SendMessage(activeWindow, WM_COPY, out a, out b );
            string s = Clipboard.GetText();

            MessageBox.Show(string.Format("Data Clipboard: \t{0}", s ?? ""));
        }
        #endregion

        #region event
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            ShowForm();
        }



        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseForm();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TextBox txt = sender as TextBox;
            label1.Text = txt.Text.ToVietnamese();
        }
        #endregion

        #region MyMethod

        private void ShowForm()
        {
            if (!Visible)
            {
                Show();
                WindowState = FormWindowState.Normal;
                BringToFront();
            }
        }

        private void CloseForm()
        {
            notifyIcon1.Visible = false;
            Application.ExitThread();
            UnregisterHotKey(Handle, ID_HOTKEY_CONVERT);
        }

        private void ConvertVietnamse()
        {
            string s = Clipboard.GetText();
            label1.Text = s;
            Clipboard.Clear();
            Clipboard.SetDataObject(s.ToVietnamese());
            Thread.Sleep(1000);
            SendKeys.Send("^(V)");
        }
        #endregion

        #region HotKey
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        private const int WM_HOTKEY = 0x0312;
        private const int WM_COPY = 0x0301;

        private const int ID_HOTKEY_CONVERT = 1;

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                /* Note that the three lines below are not needed if you only want to register one hotkey.
                 * The below lines are useful in case you want to register multiple keys, which you can use a switch with the id as argument, or if you want to know which key/modifier was pressed for some particular reason. */

                Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);                  // The key of the hotkey that was pressed.
                int modifier = (int)m.LParam & 0xFFFF;       // The modifier of the hotkey that was pressed.
                int id = m.WParam.ToInt32();                                        // The id of the hotkey that was pressed.

                switch (id)
                {
                    case ID_HOTKEY_CONVERT:
                        ConvertVietnamse();
                        break;
                    default:
                        break;
                }


            }
            base.WndProc(ref m);
        }
        #endregion

        #region GetText
        //Get the text of the focused control
        private string GetTextFromFocusedControl()
        {
            try
            {
                int activeWinPtr = GetForegroundWindow().ToInt32();
                int activeThreadId = 0, processId;
                activeThreadId = GetWindowThreadProcessId(activeWinPtr, out processId);
                int currentThreadId = GetCurrentThreadId();
                if (activeThreadId != currentThreadId)
                    AttachThreadInput(activeThreadId, currentThreadId, true);
                IntPtr activeCtrlId = GetFocus();

                return GetText(activeCtrlId);
            }
            catch (Exception exp)
            {
                return exp.Message;
            }
        }
        //Get the text of the control at the mouse position
        private string GetTextFromControlAtMousePosition()
        {
            try
            {
                Point p;
                if (GetCursorPos(out p))
                {
                    IntPtr ptr = WindowFromPoint(p);
                    if (ptr != IntPtr.Zero)
                    {
                        return GetText(ptr);
                    }
                }
                return "";
            }
            catch (Exception exp)
            {
                return exp.Message;
            }
        }
        //Get the text of a control with its handle
        private string GetText(IntPtr handle)
        {
            int maxLength = 100;
            IntPtr buffer = Marshal.AllocHGlobal((maxLength + 1) * 2);
            SendMessage(activeWindow, WM_COPY, out a, out b);
            SendMessageW(handle, WM_GETTEXT, maxLength, buffer);
            int selectionStart = -1;
            int selectionEnd = -1;
            SendMessage(handle, EM_GETSEL, out selectionStart, out selectionEnd);
            string w = Marshal.PtrToStringUni(buffer);
            Marshal.FreeHGlobal(buffer);
            if (selectionStart > 0 && selectionEnd > 0)
            {
                w = w.Substring(selectionStart, selectionEnd - selectionStart); //We need to send the length
            }
            return w;
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, out int wParam, out int lParam);

        public const uint EM_GETSEL = 0xB0;

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point pt);

        [DllImport("user32.dll", EntryPoint = "WindowFromPoint", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr WindowFromPoint(Point pt);

        [DllImport("user32.dll", EntryPoint = "SendMessageW")]
        public static extern int SendMessageW([InAttribute] System.IntPtr hWnd, int Msg, int wParam, IntPtr lParam);
        public const int WM_GETTEXT = 13;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        internal static extern IntPtr GetFocus();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetWindowThreadProcessId(int handle, out int processId);

        [DllImport("user32", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        internal static extern int AttachThreadInput(int idAttach, int idAttachTo, bool fAttach);
        [DllImport("kernel32.dll")]
        internal static extern int GetCurrentThreadId();

        [DllImport("user32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern int GetWindowText(IntPtr hWnd, [Out, MarshalAs(UnmanagedType.LPTStr)] StringBuilder lpString, int nMaxCount);
        #endregion
    }
}
