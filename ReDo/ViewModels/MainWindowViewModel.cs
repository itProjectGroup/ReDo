﻿using ReDo.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReDo.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public MainWindowViewModel() { }

        public event PropertyChangedEventHandler PropertyChanged;

        public UIElementBtnState BtnStop
        { get { return new UIElementBtnState { btnName = "Stop", btnPath = "M14 10H3V12H14V10M14 6H3V8H14V6M3 16H10V14H3V16M21.5 11.5L23 13L16 20L11.5 15.5L13 14L16 17L21.5 11.5Z" }; } }

        public UIElementBtnState BtnPlayback
        { get { return new UIElementBtnState { btnName = "PlayBack", btnPath = "M4,2A2,2 0 0,0 2,4V14H4V4H14V2H4M8,6A2,2 0 0,0 6,8V18H8V8H18V6H8M20,12V20H12V12H20M20,10H12A2,2 0 0,0 10,12V20A2,2 0 0,0 12,22H20A2,2 0 0,0 22,20V12A2,2 0 0,0 20,10M14,13V19L18,16L14,13Z" }; } }


        private UIElementBtnState _btnData;
        public UIElementBtnState BtnData
        {
            get { return _btnData; }
            set
            {
                _btnData = value;
                OnPropertyChanged("PathData");
            }
        }

        private string _btnState;

        public string BtnState
        {
            get { return _btnState; }
            set
            {
                _btnState = value;
                OnPropertyChanged("PathData");
            }
        }


        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
