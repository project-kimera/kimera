using Kimera.Common;
using Kimera.Common.Commands;
using Kimera.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Input;

namespace Kimera.Data.Entities
{
    /// <summary>
    /// Stores the information of the game.
    /// </summary>
    public class Game
    {
        /// <summary>
        /// The Primary Key(PK) of the game.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Principal Key(PK) of the game, which used to interact with other entities.
        /// </summary>
        public Guid SystemId { get; set; }

        /// <summary>
        /// The Foreign Key(FK) of the game metadata.
        /// </summary>
        public Guid GameMetadata { get; set; }

        /// <summary>
        /// The Foreign Key(FK) of the package metadata.
        /// </summary>
        public Guid PackageMetadata { get; set; }

        /// <summary>
        /// The package status.
        /// </summary>
        public PackageStatus PackageStatus { get; set; }

        /// <summary>
        /// The navigation property of the game metadata.
        /// </summary>
        public virtual GameMetadata GameMetadataNavigation { get; set; }

        /// <summary>
        /// The navigation property of the package metadata.
        /// </summary>
        public virtual PackageMetadata PackageMetadataNavigation { get; set; }

        /// <summary>
        /// The collection of <see cref="CategorySubscription"/>, which used to check the subscription status of category and game.
        /// </summary>
        public virtual ICollection<CategorySubscription> CategorySubscriptions { get; set; } = new HashSet<CategorySubscription>();

        /// <summary>
        /// The command to start the game.
        /// </summary>
        [NotMapped]
        public ICommand StartGameCommand { get; private set; }

        /// <summary>
        /// The command to move the game to another category.
        /// </summary>
        [NotMapped]
        public ICommand ChangeGameCategoryCommand { get; private set; }

        /// <summary>
        /// The command to remove the game.
        /// </summary>
        [NotMapped]
        public ICommand RemoveGameCommand { get; private set; }

        /// <summary>
        /// The command to show the game information.
        /// </summary>
        [NotMapped]
        public ICommand ShowGameInformationCommand { get; private set; }

        /// <summary>
        /// The command to edit the metadata of the game.
        /// </summary>
        [NotMapped]
        public ICommand EditMetadataCommand { get; private set; }

        /// <summary>
        /// The command to edit the settings of the game.
        /// </summary>
        [NotMapped]
        public ICommand EditSettingsCommand { get; private set; }

        /// <summary>
        /// Creates a new instance of Game.
        /// </summary>
        public Game()
        {
            InitializeCommands();
        }

        /// <summary>
        /// Creates a new instance of Game.
        /// </summary>
        /// <param name="gameMetadataGuid">The Foreign Key(FK) of the game metadata.</param>
        /// <param name="packageMetadataGuid">The Foreign Key(FK) of the package metadata.</param>
        /// <param name="packageStatus">The package status.</param>
        public Game(Guid gameMetadataGuid, Guid packageMetadataGuid, PackageStatus packageStatus)
        {
            SystemId = Guid.NewGuid();
            GameMetadata = gameMetadataGuid;
            PackageMetadata = packageMetadataGuid;
            PackageStatus = packageStatus;

            InitializeCommands();
        }

        private void InitializeCommands()
        {
            StartGameCommand = new DelegateCommand(() => LibraryEventBroker.InvokeGameStarterRequestedEvent(this, SystemId));
            ChangeGameCategoryCommand = new DelegateCommand(() => LibraryEventBroker.InvokeGameCategoryChangerRequestedEvent(this, SystemId));
            RemoveGameCommand = new DelegateCommand(() => LibraryEventBroker.InvokeGameRemoverRequestedEvent(this, SystemId));
            ShowGameInformationCommand = new DelegateCommand(() => LibraryEventBroker.InvokeGameInformationRequestedEvent(this, SystemId));
            EditMetadataCommand = new DelegateCommand(() => LibraryEventBroker.InvokeMetadataEditorRequestedEvent(this, SystemId));
            EditSettingsCommand = new DelegateCommand(() => LibraryEventBroker.InvokeSettingsEditorRequestedEvent(this, SystemId));
        }
    }
}
