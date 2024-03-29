﻿using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MineSharp
{
    public partial class Main : Form
    {
        MinecraftServer server;
        List<string> PlayerList = new List<string>();
        FunctionsForm FuncForm;
        public string path;

        public Main()
        {
            InitializeComponent();
            this.FormClosing += CloseCommandSent;
        }

        private void CloseCommandSent(object sender, FormClosingEventArgs e)
        {
            try { server.Stop(); } catch { }
        }

        private void FolderPicker(object sender, EventArgs e)
        {
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = "C:\\Users";
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                workingDirectoryDisplayBox.Text = dialog.FileName;
            }
        }

        private void StartServer(object sender, EventArgs e)
        {
            serverOutput.Clear();
            server = new MinecraftServer();
            server.OnOutputReceived += ReceivedServerOutput;
            server.Launch(workingDirectoryDisplayBox.Text, javaPath.Text, javaArgs.Text);
            path = workingDirectoryDisplayBox.Text;
            statusIndicator.BackColor = Color.YellowGreen;
            FuncForm = new FunctionsForm();
            FuncForm.Show();
            FuncForm.parentForm = this;
        }

        private void ReceivedServerOutput(OutputEventArgs e)
        {
            try
            {
                Invoke((MethodInvoker)delegate { serverOutput.Text = serverOutput.Text + Environment.NewLine + e.Output; });
                if(e.Output.Contains("For help"))//
                {
                    Invoke((MethodInvoker)delegate{ statusIndicator.BackColor = Color.Green; });
                }
                else if (e.Output.Contains("Stopping server"))//
                {
                    Invoke((MethodInvoker)delegate { statusIndicator.BackColor = Color.DarkRed; });
                    Invoke((MethodInvoker)delegate { FuncForm.Players.Clear(); });
                }
                else if (e.Output.Contains("joined the game"))//
                {
                    if (e.Output.Contains("minecraft/DedicatedServer"))
                    {
                        string newUser = e.Output.Substring(61);
                        int firstSpace = newUser.IndexOf(' ');
                        string name = newUser.Substring(0, firstSpace);
                        Invoke((MethodInvoker)delegate { FuncForm.Players.Add(name); });
                    }
                    else
                    {
                        string newUser = e.Output.Substring(33);
                        int firstSpace = newUser.IndexOf(' ');
                        string name = newUser.Substring(0, firstSpace);
                        Invoke((MethodInvoker)delegate { FuncForm.Players.Add(name); });
                    }
                }
                else if (e.Output.Contains("left the game"))//
                {
                    if (e.Output.Contains("minecraft/DedicatedServer"))
                    {
                        string newUser = e.Output.Substring(61);
                        int firstSpace = newUser.IndexOf(' ');
                        string name = newUser.Substring(0, firstSpace);
                        Invoke((MethodInvoker)delegate { FuncForm.Players.Remove(name); });
                    }
                    else
                    {
                        string newUser = e.Output.Substring(33);
                        int firstSpace = newUser.IndexOf(' ');
                        string name = newUser.Substring(0, firstSpace);
                        Invoke((MethodInvoker)delegate { FuncForm.Players.Remove(name); });
                    }
                }
            }
            catch { }
        }

        private void StopServer(object sender, EventArgs e)
        {
            server.Stop();
            server.OnOutputReceived -= ReceivedServerOutput;
            statusIndicator.BackColor = Color.DarkRed;
            FuncForm.Close();
        }

        public void SendMessageToServer(string message)
        {
            server.SendCommand(message);
        }

        private void SendCommand(object sender, EventArgs e)
        {
            SendMessageToServer(commandBox.Text);
            commandBox.Text = "";
        }

        private void issueFound(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/jasonTheHuman2101/MineSharp/issues/new");
        }
    }
}
