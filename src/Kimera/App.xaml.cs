using Caliburn.Micro;
using Kimera.Data.Contexts;
using Kimera.Data.Extensions;
using Kimera.Network;
using Kimera.Utilities;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Kimera
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly static bool _debugMode = true;

        public static bool DebugMode
        {
            get => _debugMode;
        }

        private static KimeraContext _databaseContext = null;

        public static KimeraContext DatabaseContext
        {
            get => _databaseContext;
            set
            {
                _databaseContext = value;
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (!_debugMode && !PrivilegeHelper.IsAdministrator())
            {
                Log.Warning("The current process does not have administrator privileges. The process will be restart with administrator privileges.");
                PrivilegeHelper.RunAsAdiministrator();
                return;
            }

            LoadResources();

            base.OnStartup(e);
        }

        private void LoadResources()
        {
            bool useDarkTheme = IoC.Get<Settings>().UseDarkTheme;

            var paletteHelper = new PaletteHelper();

            ResourceDictionary themeDict = new ResourceDictionary();

            if (useDarkTheme)
            {
                ITheme theme = paletteHelper.GetTheme();
                theme.SetBaseTheme(Theme.Dark);
                theme.SetPrimaryColor(SwatchHelper.Lookup[(MaterialDesignColor)PrimaryColor.Blue]);
                theme.SetSecondaryColor(SwatchHelper.Lookup[(MaterialDesignColor)SecondaryColor.Blue]);
                paletteHelper.SetTheme(theme);

                themeDict.Source = new Uri(@"../Styles/Brushes.Dark.xaml", UriKind.Relative);
            }
            else
            {
                ITheme theme = paletteHelper.GetTheme();
                theme.SetBaseTheme(Theme.Light);
                theme.SetPrimaryColor(SwatchHelper.Lookup[(MaterialDesignColor)PrimaryColor.Blue]);
                theme.SetSecondaryColor(SwatchHelper.Lookup[(MaterialDesignColor)SecondaryColor.Blue]);
                paletteHelper.SetTheme(theme);

                themeDict.Source = new Uri(@"../Styles/Brushes.Light.xaml", UriKind.Relative);
            }

            App.Current.Resources.MergedDictionaries.Add(themeDict);

            ResourceDictionary buttonDict = new ResourceDictionary();
            buttonDict.Source = new Uri(@"../Styles/ButtonStyles.xaml", UriKind.Relative);
            App.Current.Resources.MergedDictionaries.Add(buttonDict);

            ResourceDictionary toggleDict = new ResourceDictionary();
            toggleDict.Source = new Uri(@"../Styles/ToggleStyles.xaml", UriKind.Relative);
            App.Current.Resources.MergedDictionaries.Add(toggleDict);

            ResourceDictionary progressBarDict = new ResourceDictionary();
            progressBarDict.Source = new Uri(@"../Styles/ProgressBarStyles.xaml", UriKind.Relative);
            App.Current.Resources.MergedDictionaries.Add(progressBarDict);

            ResourceDictionary layoutDict = new ResourceDictionary();
            layoutDict.Source = new Uri(@"../Styles/Layouts.xaml", UriKind.Relative);
            App.Current.Resources.MergedDictionaries.Add(layoutDict);

            ResourceDictionary listViewItemDict = new ResourceDictionary();
            listViewItemDict.Source = new Uri(@"../Styles/ListViewItemStyles.xaml", UriKind.Relative);
            App.Current.Resources.MergedDictionaries.Add(listViewItemDict);

            ResourceDictionary windowDict = new ResourceDictionary();
            windowDict.Source = new Uri(@"../Styles/WindowStyles.xaml", UriKind.Relative);
            App.Current.Resources.MergedDictionaries.Add(windowDict);
        }
    }
}
