using Kimera.Data.Contexts;
using Kimera.Data.Extensions;
using Kimera.Network;
using Kimera.Utilities;
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
    }
}
