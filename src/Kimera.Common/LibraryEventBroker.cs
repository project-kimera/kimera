using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kimera.Common
{
    public static class LibraryEventBroker
    {
        #region ::Category::

        public static event EventHandler<Guid> SelectedCategoryChangerRequestedEvent;

        public static void InvokeSelectedCategoryChangerRequestedEvent(object sender, Guid e)
        {
            SelectedCategoryChangerRequestedEvent?.Invoke(sender, e);
        }

        #endregion

        #region ::Game::

        public static event EventHandler<Guid> GameStarterRequestedEvent;

        public static event EventHandler<Guid> GameCategoryChangerRequestedEvent;

        public static event EventHandler<Guid> GameRemoverRequestedEvent;

        public static event EventHandler<Guid> GameInformationRequestedEvent;

        public static event EventHandler<Guid> MetadataEditorRequestedEvent;

        public static void InvokeGameStarterRequestedEvent(object sender, Guid e)
        {
            GameStarterRequestedEvent?.Invoke(sender, e);
        }

        public static void InvokeGameCategoryChangerRequestedEvent(object sender, Guid e)
        {
            GameCategoryChangerRequestedEvent?.Invoke(sender, e);
        }

        public static void InvokeGameRemoverRequestedEvent(object sender, Guid e)
        {
            GameRemoverRequestedEvent?.Invoke(sender, e);
        }

        public static void InvokeGameInformationRequestedEvent(object sender, Guid e)
        {
            GameInformationRequestedEvent?.Invoke(sender, e);
        }

        public static void InvokeMetadataEditorRequestedEvent(object sender, Guid e)
        {
            MetadataEditorRequestedEvent?.Invoke(sender, e);
        }

        #endregion
    }
}
