using Kimera.Data.Enums;
using System;

namespace Kimera.Data.Entities
{
    /// <summary>
    /// Stores the game metadata.
    /// </summary>
    public class GameMetadata
    {
        /// <summary>
        /// The Primary Key(PK) of the game metadata.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The Principal Key(PK) of the game metadata, which used to interact with other entities.
        /// </summary>
        public Guid SystemId { get; set; }

        /// <summary>
        /// The Foreign Key(FK) of the game.
        /// </summary>
        public Guid Game { get; set; }

        /// <summary>
        /// The name of the game.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The description of the game.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The creator of the game.
        /// </summary>
        public string Creator { get; set; }

        /// <summary>
        /// The age which is admitted to play this game.
        /// </summary>
        public Age AdmittedAge { get; set; }

        /// <summary>
        /// The genres of the game. It should be separated by comma.
        /// </summary>
        public string Genres { get; set; }

        /// <summary>
        /// The tags of the game. It should be separated by comma.
        /// </summary>
        public string Tags { get; set; }

        /// <summary>
        /// The supported languages of the game. It should be separated by comma.
        /// </summary>
        public string SupportedLanguages { get; set; }

        /// <summary>
        /// The score of the game.
        /// </summary>
        public double Score { get; set; }

        /// <summary>
        /// The memo about the game.
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// The total play time(seconds) of this game. 
        /// </summary>
        public int PlayTime { get; set; }

        /// <summary>
        /// Whether the game is finished or not.
        /// </summary>
        public bool IsFinished { get; set; }

        /// <summary>
        /// The time when the game was first started.
        /// </summary>
        public DateTime FirstTime { get; set; }

        /// <summary>
        /// The time when the game was exited.
        /// </summary>
        public DateTime LastTime { get; set; }

        /// <summary>
        /// The URI of game icon image.
        /// </summary>
        public string IconUri { get; set; }

        /// <summary>
        /// The URI of thumbnail image.
        /// </summary>
        public string ThumbnailUri { get; set; }

        /// <summary>
        /// The URL of homepage.
        /// </summary>
        public string HomepageUrl { get; set; }

        /// <summary>
        /// The navigation property of the game.
        /// </summary>
        public virtual Game GameNavigation { get; set; }

        /// <summary>
        /// Copies and returns current game metadata.
        /// </summary>
        /// <returns>The copyed game metadata.</returns>
        public GameMetadata Copy()
        {
            return (GameMetadata)this.MemberwiseClone();
        }
    }
}
