using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Services
{
    public class GameService : PropertyChangedBase
    {
        #region ::Variables & Properties::

        private bool _isRunning = false;

        public bool IsRunning
        {
            get => _isRunning;
            set => Set(ref _isRunning, value);
        }

        #endregion
    }
}
