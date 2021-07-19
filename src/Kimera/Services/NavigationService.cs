﻿using Kimera.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Kimera.Services
{
    public class NavigationService
    {
        private static NavigationService _instance;

        public static NavigationService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new NavigationService();
                }

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public static LibraryPage LibraryPage { get; set; } = new LibraryPage();

        public static SettingsPage SettingsPage { get; set; } = new SettingsPage();

        private Frame _shellFrame;

        public void InitializeFrame(Frame rootFrame)
        {
            _shellFrame = rootFrame;
        }

        public void NavigateTo(Page page)
        {
            _shellFrame?.Navigate(page);
        }

        public void NavigateTo(Page page, object parameter)
        {
            _shellFrame?.Navigate(page, parameter);
        }

        public void GoBack()
        {
            _shellFrame?.GoBack();
        }

        public void GoForward()
        {
            _shellFrame?.GoForward();
        }

        public bool CanGoBack()
        {
            return (bool)_shellFrame?.CanGoBack;
        }

        public bool CanGoForward()
        {
            return (bool)_shellFrame?.CanGoForward;
        }

        public void Refresh()
        {
            _shellFrame?.Refresh();
        }
    }
}