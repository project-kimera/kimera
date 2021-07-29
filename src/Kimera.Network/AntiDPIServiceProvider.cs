using Kimera.Network.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Network
{
    /// <summary>
    /// Provides a solution for DPI.
    /// </summary>
    public static class AntiDPIServiceProvider
    {
        private static AntiDPIService _service;

        /// <summary>
        /// Initializes the internal service of the <see cref="AntiDPIServiceProvider"/>.
        /// </summary>
        /// <param name="directory">The directory to be used for AntiDPI service.</param>
        public static void InitializeService(string directory)
        {
            try
            {
                _service = new AntiDPIService(directory);

                if (!_service.CheckResources())
                {
                    _service.EnsureResourcesExist();
                }

                _service.Start();
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException("Failed to start the AntiDPI service.", ex);
            }
        }

        public static void DisposeService()
        {
            try
            {
                if (_service != null)
                {
                    _service.Dispose();
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to dispose the AntiDPI service.", ex);
            }
        }
    }
}
