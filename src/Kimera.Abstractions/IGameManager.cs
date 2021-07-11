using Kimera.Abstractions.Bases;
using Kimera.Data.Entities;
using System.Threading.Tasks;

namespace Kimera.Abstractions
{
    public interface IGameManager : IPluginBase
    {
        public bool IsAsync { get; }

        public void AddGame(string[] files);

        public Task AddGameAsync(string[] files);

        public void RemoveGame(PackageMetadata metadata);

        public Task RemoveGameAsync(PackageMetadata metadata);

        public void PreprocessGame(ref PackageMetadata metadata);

        public Task PreprocessGameAsync(ref PackageMetadata metadata);

        public void PlayGame(PackageMetadata metadata);

        public Task PlayGameAsync(PackageMetadata metadata);
    }
}
