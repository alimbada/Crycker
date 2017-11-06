﻿using System;
using System.Windows.Forms;

using Cricker.Controls;
using Cricker.Data;
using Cricker.Helper;
using Cricker.Properties;
using Cricker.Types;

namespace Cricker
{
    class App : ApplicationContext
    {
        private NotifyIcon TrayIcon;
        private ContextMenuControl contextMenuControl = new ContextMenuControl();
        private TickerController tickerController;

        public App()
        {
            Logger.Info("App starting...");

            InitializeComponent();

            // Spin up Controller and Update data
            tickerController = new TickerController(Settings.Default.Provider, Settings.Default.Coin, Settings.Default.Currency, Settings.Default.RefreshInterval);
            tickerController.DataUpdated += C_DataUpdated;            
        }

        private void C_DataUpdated(object sender, TickerEventArgs e)
        {
            TaskbarIconHelper.SetPrice(e.LastPrice, e.PreviousPrice, e.Coin, e.Currency, e.Provider, e.LastUpdated);
        }

        private void InitializeComponent()
        {
            Application.ApplicationExit += Application_ApplicationExit;

            // Init Context Menu
            contextMenuControl.SetProvider(Settings.Default.Provider);
            contextMenuControl.SetCoin(Settings.Default.Coin);
            contextMenuControl.SetCurrency(Settings.Default.Currency);
            contextMenuControl.SetRefreshInterval(Settings.Default.RefreshInterval);

            contextMenuControl.CurrencyChanged += ContextMenuControl_CurrencyChanged;
            contextMenuControl.CoinChanged += ContextMenuControl_CoinChanged;
            contextMenuControl.ProviderChanged += ContextMenuControl_ProviderChanged;
            contextMenuControl.RefreshIntervalChanged += ContextMenuControl_RefreshIntervalChanged;
            contextMenuControl.ExitClicked += ContextMenuControl_ExitClicked;

            // Init Tray Icon
            TrayIcon = new NotifyIcon
            {
                ContextMenuStrip = contextMenuControl.ContextMenuStrip
            };
            TrayIcon.Click += TrayIcon_Click;
            TrayIcon.DoubleClick += TrayIcon_DoubleClick;

            TaskbarIconHelper.notifyIcon = TrayIcon;

            TrayIcon.Visible = true;        
        }
      
        private void ContextMenuControl_ProviderChanged(object sender, StringEventArgs e)
        {
            // throw new NotImplementedException();
            tickerController.UpdateData();

            Settings.Default.Provider = e.Value;
            Settings.Default.Save();
        }

        private void ContextMenuControl_CoinChanged(object sender, StringEventArgs e)
        {
            // throw new NotImplementedException();
            tickerController.UpdateData();

            Settings.Default.Coin = e.Value;
            Settings.Default.Save();
        }

        private void ContextMenuControl_CurrencyChanged(object sender, StringEventArgs e)
        {
            // throw new NotImplementedException();

            tickerController.SetCurrency(e.Value);
            tickerController.UpdateData();

            Settings.Default.Currency = e.Value;
            Settings.Default.Save();
        }

        private void ContextMenuControl_RefreshIntervalChanged(object sender, IntEventArgs e)
        {
            tickerController.SetRefreshInterval(e.Value);
            tickerController.UpdateData();

            Settings.Default.RefreshInterval = e.Value;
            Settings.Default.Save();
        }

        private void ContextMenuControl_ExitClicked(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to exit?", "Are you sure?",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void Application_ApplicationExit(object sender, EventArgs e)
        {
            //Cleanup so that the icon will be removed when the application is closed
            TrayIcon.Visible = false;
        }


        private void TrayIcon_Click(object sender, EventArgs e)
        {
            // Here you can do stuff if the tray icon is clicked
            // throw new NotImplementedException();
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            // Here you can do stuff if the tray icon is doubleclicked
            // TrayIcon.ShowBalloonTip(10000);
        }

    }

}