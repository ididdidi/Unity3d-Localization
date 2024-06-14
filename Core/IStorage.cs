using System.Collections.Generic;

namespace EasyAssetsLocalize
{
    /// <summary>
    /// Interface for interacting with the localization storage
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Returns the index of the current repository version.
        /// </summary>
        int Version { get; }

        /// <summary>
        /// List of localizations.
        /// </summary>
        List<Language> Languages { get; }
        /// <summary>
        /// List of localizations in array format.
        /// </summary>
        Localization[] Localizations { get; }

        /// <summary>
        /// Adds a new localization language to the repository.
        /// </summary>
        /// <param name="language">New localization language</param>
        void AddLanguage(Language language);

        /// <summary>
        /// Removes the selected localization language from the repository.
        /// </summary>
        /// <param name="index">Index in list</param>
        void RemoveLanguage(int index);

        /// <summary>
        /// Does the repository contain this localization.
        /// </summary>
        /// <param name="localization"><see cref="Localization"/></param>
        /// <returns>Does the repository contain this localization</returns>
        bool ContainsLocalization(Localization localization);

        /// <summary>
        /// Adds a new localization object to the repository.
        /// </summary>
        /// <param name="localization">New <see cref="Localization"/></param>
        void AddLocalization(Localization localization);

        /// <summary>
        /// Used to get localized resources by localization tag ID, which is assigned when adding localization to the repository.
        /// </summary>
        /// <param name="id">Identifier of the localization tag in the repository</param>
        /// <returns>Localization</returns>
        Localization GetLocalization(string id);

        /// <summary>
        /// Returns the default localization for the given resource type
        /// </summary>
        /// <param name="type">Resource type</param>
        /// <returns><see cref="Localization"/></returns>
        Localization GetDefaultLocalization(System.Type type);

        /// <summary>
        /// Removes localization in the list of localizations.
        /// </summary>
        /// <param name="localization"><see cref="Localization"/></param>
        void RemoveLocalization(Localization localization);

        /// <summary>
        /// Delete all localizations for a given resource type.
        /// </summary>
        /// <param name="type"></param>
        void RemoveAll(System.Type type);

        /// <summary>
        /// Change the order of languages in localization storage.
        /// </summary>
        /// <param name="currentIndex"></param>
        /// <param name="nextIndex"></param>
        void ReorderLocalizations(int currentIndex, int nextIndex);

        /// <summary>
        /// Saves vault data changes in the editor.
        /// </summary>
        void SaveChanges();
    }
}