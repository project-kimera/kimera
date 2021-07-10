using System;

namespace Kimera.Data.Entities
{
    public class Plugin
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Author { get; set; }

        public string Copyright { get; set; }

        public string Version { get; set; }

        public Plugin(Guid id)
        {
            Id = id;
        }
    }
}
