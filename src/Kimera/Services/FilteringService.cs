using Caliburn.Micro;
using Kimera.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Services
{
    public class FilteringService : PropertyChangedBase
    {

        #region ::Constructors::

        public FilteringService()
        {
            InitializeService();
        }

        private async void InitializeService()
        {
            Log.Information("The filtering service has been initialized.");
        }

        #endregion
    }
}
