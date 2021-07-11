using System;

namespace Kimera.Abstractions.Bases
{
    public interface IPluginBase
    {
        public Guid Guid { get; }

        public string Name { get; }

        public string Author { get; }

        public string Copyright { get; }

        public Version Version { get; }
    }
}
