using Kimera.Data.Entities;
using Kimera.Network.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Kimera.Network
{
    /// <summary>
    /// Provides the metadata service.
    /// </summary>
    public static class MetadataServiceProvider
    {
        public static List<IMetadataService> Services { get; private set; } = new List<IMetadataService>();

        /// <summary>
        /// Initializes the internal service of the <see cref="MetadataServiceProvider"/>.
        /// </summary>
        public static void InitializeService()
        {
            Services = Assembly.GetExecutingAssembly()
                .GetTypes()  // Gets all types
                .Where(type => typeof(IMetadataService).IsAssignableFrom(type)) // Ensures that object can be cast to interface
                .Where(type =>
                    !type.IsAbstract &&
                    !type.IsGenericType &&
                     type.GetConstructor(new Type[0]) != null) // Ensures that type can be instantiated
                .Select(type => (IMetadataService)Activator.CreateInstance(type)) // Create instances
                .ToList();
        }

        /// <summary>
        /// Get a product code from the specific path.
        /// </summary>
        /// <param name="path">The path to get a product code.</param>
        /// <returns>A product code if the path contains it; otherwise, null.</returns>
        public static string GetProductCodeFromPath(string path)
        {
            string[] pathChunks = path.Split(new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = pathChunks.Length - 1; i >= 0; i--)
            {
                foreach (IMetadataService service in Services)
                {
                    foreach (string regex in service.ProductCodeRegexs)
                    {
                        Match match = Regex.Match(pathChunks[i], regex, RegexOptions.IgnoreCase);

                        if (match.Success)
                        {
                            return match.Value;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// If the product code is valid, this returns true and type name of target service; otherwise, false and null.
        /// </summary>
        /// <param name="productCode">The product code.</param>
        /// <param name="serviceType">The string where the service type is stored.</param>
        /// <returns>true if the product code is valid; otherwise, false.</returns>
        public static bool TryValidProductCode(string productCode, out Type serviceType)
        {
            foreach(IMetadataService service in Services)
            {
                foreach (string regex in service.ProductCodeRegexs)
                {
                    if (Regex.IsMatch(productCode, regex, RegexOptions.IgnoreCase))
                    {
                        serviceType = service.GetType();
                        return true;
                    }
                }
            }

            serviceType = null;
            return false;
        }

        /// <summary>
        /// Checks availablity of the product.
        /// </summary>
        /// <param name="serviceType">The type of service to use.</param>
        /// <param name="productCode">The product code to be used.</param>
        /// <returns>true if the product is available; otherwise, false.</returns>
        public static async Task<bool> IsAvailableProductAsync(Type serviceType, string productCode)
        {
            try
            {
                IMetadataService service = Services.Where(s => s.GetType().Name == serviceType.Name).First();

                if (service != null)
                {
                    bool result = await service.IsAvailableProductAsync(productCode).ConfigureAwait(false);
                    return result;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Get the metadata of the product.
        /// </summary>
        /// <param name="serviceType">The type of service to use.</param>
        /// <param name="productCode">The product code to be used.</param>
        /// <returns>A <see cref="GameMetadata"/> if the product is available; otherwise, null.</returns>
        public static async Task<GameMetadata> GetMetadataAsync(Type serviceType, string productCode)
        {
            try
            {
                IMetadataService service = Services.Where(s => s.GetType().Name == serviceType.Name).First();

                if (service != null)
                {
                    GameMetadata result = await service.GetMetadataAsync(productCode).ConfigureAwait(false);
                    return result;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
