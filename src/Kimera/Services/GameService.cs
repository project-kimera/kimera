using Kimera.Data.Entities;
using Kimera.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Services
{
    public class GameService
    {
        #region ::Singleton Members::

        private static GameService _instance;

        public static GameService Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameService();
                }

                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        #endregion

        #region ::Variables & Properties::



        #endregion

        #region ::Methods::

        public void AddGame(PackageType type, GameMetadata gameMetadata, PackageMetadata packageMetadata, List<Component> components, List<Guid> categories)
        {
            if (type == PackageType.Executable)
            {

            }
            else if (type == PackageType.Archive)
            {

            }
            else if (type == PackageType.Multiple)
            {

            }
        }

        public void RemoveGame(Guid gameGuid)
        {

        }

        public bool ValidateGame(Guid gameGuid)
        {
            return false;
        }

        public void StartGame(Guid gameGuid)
        {

        }

        public bool ValidateComponents(Guid gameGuid)
        {
            return false;
        }

        public void ProcessComponents(Guid gameGuid)
        {

        }

        public void RemoveComponents(Guid gameGuid)
        {

        }

        #endregion
    }
}
