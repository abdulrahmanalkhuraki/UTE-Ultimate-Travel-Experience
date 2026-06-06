namespace Infrastructure.Notifications
{
    public class FirebaseSettings
    {
        public const string SectionName = "Firebase";

        /// <summary>
        /// Absolute or relative path to the Firebase service-account JSON file used to
        /// authenticate with FCM. When empty/missing, real-time push is skipped (the
        /// notification is still saved to the database).
        /// </summary>
        public string? CredentialsPath { get; set; }
    }
}
