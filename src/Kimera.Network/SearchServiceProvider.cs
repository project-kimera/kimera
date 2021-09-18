using Kimera.Network.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Network
{
    /// <summary>
    /// Provides the metadata service.
    /// </summary>
    public static class SearchServiceProvider
    {
        public static List<ISearchService> Services { get; private set; } = new List<ISearchService>();

        /// <summary>
        /// Initializes the internal service of the <see cref="SearchServiceProvider"/>.
        /// </summary>
        public static void InitializeService()
        {
            Services = Assembly.GetExecutingAssembly()
                .GetTypes()  // Gets all types
                .Where(type => typeof(ISearchService).IsAssignableFrom(type)) // Ensures that object can be cast to interface
                .Where(type =>
                    !type.IsAbstract &&
                    !type.IsGenericType &&
                     type.GetConstructor(new Type[0]) != null) // Ensures that type can be instantiated
                .Select(type => (ISearchService)Activator.CreateInstance(type)) // Create instances
                .ToList();
        }
    }
}
