using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using GreenApp.Annotations;
using Plugin.Connectivity;
using Xamarin.Forms;

namespace GreenApp.Models
{
    class CheckInternetModel : INotifyPropertyChanged
    {

        private string _conn;

        public string Conn
        {
            get => _conn;
            set
            {
                _conn = value; 
                OnPropertyChanged();
            }
        }

        public CheckInternetModel()
        {
            CheckWifiOnStart();
            CheckWifiContinuously();
        }

        public void CheckWifiOnStart()
        {
            Conn = CrossConnectivity.Current.IsConnected ? "true" : "false";
            //Conn=CrossConnectivity.Current.IsConnected ? await navigation
        }

        public void CheckWifiContinuously()
        {
            CrossConnectivity.Current.ConnectivityChanged += Current_ConnectivityChanged;
        }

        private void Current_ConnectivityChanged(object sender, Plugin.Connectivity.Abstractions.ConnectivityChangedEventArgs e)
        {
            Conn = e.IsConnected ? "true" : "false";
            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
