using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text.RegularExpressions;

namespace ZetaTelnet
{

    public partial class ZetaTelnet : Form
    {
        private Socket _socket = null;
        byte[] _inBuffer = null;
        private IPHostEntry _ipHost;
        private IPAddress _ipAddress;
        private delegate void TerminalTextCallback(string text);
        private delegate void EnableConnectCallback(bool enabled);
        List<String> _scrollback = new List<String>();
        int _scrollbackPosition = -1;
        bool _stripANSI = true;

        public ZetaTelnet()
        {
            InitializeComponent();
            //txtInput.KeyPress += new KeyPressEventHandler(KeyHandler);
            txtInput.KeyDown += new KeyEventHandler(KeyHandler);
        }

        ~ZetaTelnet()
        {
            if( _socket != null )
            {
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
            }
        }

        private void KeyHandler(Object o, KeyEventArgs e)
        {
            if( e.KeyCode == Keys.Return )
            {
                e.Handled = true;
                if( _socket != null && _socket.Connected == true )
                {
                    _scrollback.Add(txtInput.Text);
                    _scrollbackPosition = _scrollback.Count;
                    byte[] sendBytes = Encoding.ASCII.GetBytes(txtInput.Text + "\n");
                    AsyncCallback onsend = new AsyncCallback( OnSend );
                    _socket.BeginSend(sendBytes, 0, sendBytes.Length, SocketFlags.None, onsend, _socket);
                    AddTerminalText(txtInput.Text + "\n");
                    txtInput.Text = "";
                }
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (_scrollbackPosition > 0 && _scrollback.Count > 0 )
                {
                    _scrollbackPosition--;
                    txtInput.Text = _scrollback[_scrollbackPosition];
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (_scrollbackPosition < (_scrollback.Count - 1) && _scrollback.Count > 0)
                {
                    _scrollbackPosition++;
                    txtInput.Text = _scrollback[_scrollbackPosition];
                }
                else
                {
                    txtInput.Text = String.Empty;
                    _scrollbackPosition = _scrollback.Count;
                }
            }
        }

        public void OnSend( IAsyncResult ar )
        {
            _socket = (Socket)ar.AsyncState;

            try
            {
                int bytesSent = _socket.EndSend(ar);
                if (bytesSent > 0)
                {

                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error processing receive buffer!");
            }            
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                _ipHost = Dns.GetHostEntry(txtIP.Text);
            }
            catch( SystemException)
            {
                MessageBox.Show("Unable to resolve IP Address");
                return;
            }
            try
            {
                foreach (IPAddress address in _ipHost.AddressList)
                {
                    if (address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        _ipAddress = address;
                        break;
                    }
                }
            }
            catch(SystemException)
            {
                MessageBox.Show("IP Address does not resolve to any known hosts.");
                return;
            }
            if (_ipAddress == null)
            {
                MessageBox.Show("Cannot find host for address " + txtIP.Text + ".");
                return;
            }
            try
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            catch (SystemException)
            {
                MessageBox.Show("Unable to create socket.");
                return;
            }
            try
            {
                _socket.Blocking = false;
                AsyncCallback onconnect = new AsyncCallback( OnConnect );
                int port;
                try
                {
                    port = Int32.Parse(txtPort.Text);
                }
                catch( SystemException)
                {
                    MessageBox.Show("Unable to connect: Bad port number (must be an integer)");
                    return;
                }
                _socket.BeginConnect(_ipAddress, port, onconnect, _socket);
            }
            catch ( SocketException ex)
            {
                int errorCode = ex.ErrorCode;
                MessageBox.Show("Socket error " + errorCode.ToString() + " on BeginConnect");
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to initiate connection: " + ex.ToString());
                return;
            }

        }

        public void OnConnect( IAsyncResult ar )
        {
            _socket = (Socket) ar.AsyncState;

            try
            {
                _socket.EndConnect(ar);
                if( _socket.Connected )
                {
                    SetupReceiveCallback(_socket);
                    EnableConnect(false);
                }
                else
                {
                    MessageBox.Show("Unable to connect to remote host.");
                    EnableConnect(true);
                    return;
                }
            }
            catch( Exception ex)
            {
                MessageBox.Show(ex.Message, "Connection Error");
                EnableConnect(true);
                return;
            }
        }

        public void SetupReceiveCallback( Socket sock )
        {
            try
            {
                AsyncCallback receiveData = new AsyncCallback(OnReceiveData);
                _inBuffer = new byte[1024];
                sock.BeginReceive(_inBuffer, 0, 1024, SocketFlags.None, receiveData, sock);
            }
            catch( Exception ex )
            {
                MessageBox.Show("Setup receive callback failed: " + ex.ToString() );
                return;
            }
        }

        /// <summary>
        /// Process data received from the socket.
        /// </summary>
        /// <param name="ar"></param>
        public void OnReceiveData( IAsyncResult ar )
        {
            _socket = (Socket) ar.AsyncState;

            if (_socket == null || !_socket.Connected)
                return;

            try
            {
                int bytesReceived = _socket.EndReceive(ar);
                if( bytesReceived > 0)
                {
                    string buffer = Encoding.ASCII.GetString(_inBuffer, 0, 1024);

                    AddTerminalText( buffer );
                    _inBuffer = null;
                }
                SetupReceiveCallback(_socket);
            }
            catch( SocketException ex)
            {
                MessageBox.Show("Error Receiving Data: " + ex.ToString());
                if (_socket != null)
                    _socket.Close();
                EnableConnect(true);
            }
            catch( Exception ex)
            {
                MessageBox.Show(ex.Message, "Error processing receive buffer!");
                return;
            }
        }

        /// <summary>
        /// Reacts to window size changes and moves/resizes controls accordingly.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSizeChanged(EventArgs e)
        {
            int previousHeight = txtTerminal.Height;
            //txtTerminal.Height = this.Height - 92;
            txtTerminal.Height = this.Height - 115;
            int newInputY = txtTerminal.Location.Y + txtTerminal.Height;
            txtInput.Location = new Point(txtInput.Location.X, newInputY);
            txtInput.Width = this.Width - 14;
            txtTerminal.Width = this.Width - 14;
            base.OnSizeChanged(e);
        }

        /// <summary>
        /// Appends text to the terminal output window.
        /// </summary>
        /// <param name="text"></param>
        private void AddTerminalText(string text)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.txtTerminal.InvokeRequired)
            {
                TerminalTextCallback d = new TerminalTextCallback(AddTerminalText);
                this.Invoke(d, new object[] { text });
            }
            else
            {
                if (_stripANSI)
                {
                    this.txtTerminal.AppendText(RemoveANSICodes(text));
                }
                else
                {
                    this.txtTerminal.AppendText(text);
                }
                this.txtTerminal.ScrollToCaret();
            }
        }

        public static String RemoveANSICodes(string text)
        { 
            text = Regex.Replace(text, @"\e\[\d*(;?\d)+m", "");
            text = text.Replace("\n\r", "\r\n");
            return text;
        }

        private void btnDisconnect_Click(object sender, EventArgs e)
        {
            _socket.Close();
            EnableConnect(true);
        }

        protected void EnableConnect(bool enabled)
        {
            if (this.btnConnect.InvokeRequired)
            {
                EnableConnectCallback d = new EnableConnectCallback(EnableConnect);
                this.Invoke(d, new object[] { enabled });
            }
            else
            {
                this.btnConnect.Enabled = enabled;
                this.btnDisconnect.Enabled = !enabled;
            }
        }

        private void txtTerminal_Click(object sender, EventArgs e)
        {
            txtInput.Focus();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
            "Zeta Telnet 3.01\nCopyright (c) 2007-2013 Zeta Centauri, Inc.\nhttp://www.zetacentauri.com\nWritten by Jason Champion.\n\nThis program is freeware and may be distributed freely.", "About Zeta Telnet");
        }

        private void saveAsMenuItem_Click(object sender, EventArgs e)
        {
            String text = txtTerminal.Text;
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "text files (*.txt)|*.txt";
            dlg.FilterIndex = 0;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                StreamWriter file = new StreamWriter(dlg.FileName);
                file.Write(text);
                file.Close();
            }
        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void stripAnsiCodesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _stripANSI = !_stripANSI;
            stripAnsiCodesToolStripMenuItem.Checked = _stripANSI;
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnConnect_Click(sender, e);
        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnDisconnect_Click(sender, e);
        }

        private void changeForegroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtTerminal.ForeColor = dlg.Color;
            }
        }

        private void changeBackgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dlg = new ColorDialog();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                txtTerminal.BackColor = dlg.Color;
            }
        }
    }
}
