using UnityEngine;

namespace EasyAssetsLocalize
{
    /// <summary>
    /// Static class for managing localization, providing localization storage and language to localization components in the scene.
    /// </summary>
    public static class LocalizationManager
    {
        private static IStorage storage;
        private static int storageVersion;

        /// <summary>
        /// Static constructor. Used to subscribe to Undo(Ctrl+Z).
        /// </summary>
        static LocalizationManager() => UnityEditor.Undo.undoRedoPerformed += OnUndo;        

        private static void OnUndo()
        {
            if(storageVersion != storage.Version) { SetStorage(Storage); }
        }

        /// <summary>
        /// Link to Localization Storage.
        /// </summary>
        public static IStorage Storage { get => storage ?? ResetStorage(); private set => storage = value; }

        /// <summary>
        /// The current language.
        /// </summary>
        public static Language Language
        {
            get => GetLanguage();
            set => SetLanguage(value);
        }
        
        /// <summary>
        /// Delegates subscribed to changes in the localizationtion storage.
        /// </summary>
        public static System.Action<IStorage> OnStorageChange { get; set; }
        /// <summary>
        /// Delegates subscribed to changes in the language.
        /// </summary>
        public static System.Action<Language> OnLanguageChange { get; set; }

        /// <summary>
        /// Method for changing the current localization storage to another.
        /// </summary>
        /// <param name="storage"></param>
        public static void SetStorage(IStorage storage)
        {
            Storage = storage;
            storageVersion = Storage.Version;
            OnStorageChange?.Invoke(Storage);
            OnLanguageChange?.Invoke(Language);
        }

        /// <summary>
        /// The method resets the current localization store and sets the default one from the resources.
        /// </summary>
        /// <returns>Default Localization Storage</returns>
        public static IStorage ResetStorage()
        {
            SetStorage(Resources.Load<LocalizationStorage>(nameof(LocalizationStorage)));
            return Storage;
        }

        /// <summary>
        /// Sets the current language and loads localized resources.
        /// </summary>
        /// <param name="language">Language</param>
        private static void SetLanguage(Language language)
        {
            if (storage.Languages.Contains(language))
            {
                PlayerPrefs.SetInt("Language", (int)language.SystemLanguage);
                OnLanguageChange?.Invoke(language);
            }
        }

        /// <summary>
        /// Method for getting the current localization language.
        /// </summary>
        /// <returns>Current Language</returns>
        private static Language GetLanguage()
        {
            Language language = new Language(PlayerPrefs.HasKey("Language") ? 
                (SystemLanguage)PlayerPrefs.GetInt("Language") : Application.systemLanguage);

            return Storage.Languages.Contains(language) ? language : Storage.Languages[0];
        }
    }
}