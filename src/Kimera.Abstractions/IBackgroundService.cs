using Kimera.Abstractions.Bases;

namespace Kimera.Abstractions
{
    public interface IBackgroundService : IPluginBase
    {
        public void OnStartup();

        public void OnActivated();

        public void OnDeactived();

        public void OnExit();
    }
}
