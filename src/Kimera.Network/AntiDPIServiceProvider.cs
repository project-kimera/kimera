using Kimera.Network.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Network
{
    public static class AntiDPIServiceProvider
    {
        private static AntiDPIService _service;

        public static void InitializeService(string directory)
        {
            try
            {
                _service = new AntiDPIService(directory);

                if (!_service.CheckResources())
                {
                    _service.EnsureResources();
                }

                _service.Start();
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException("Failed to start the AntiDPI.", ex);
            }
        }
    }
}
