using System.Collections.Generic;
using UnityEngine;

namespace EasyAssetsLocalize
{
    /// <summary>
    /// Сlass that encapsulates data from languages and localized resources.
    /// </summary>
    [CreateAssetMenu(fileName = "New LocalizationStorage", menuName = "Localization Storage", order = 700)]
    internal class LocalizationStorage : ScriptableObject, IStorage
    {
        [SerializeField, HideInInspector] private int version;
        [SerializeField, HideInInspector] private List<Language> languages = new List<Language>();
        [SerializeField, HideInInspector] private List<Localization> localizations = new List<Localization>();

        /// <summary>
        /// Returns the index of the current storage version.
        /// </summary>
        public int Version { get => version; }

        /// <summary>
        /// List of languages in array format.
        /// </summary>
        public List<Language> Languages => languages;
        /// <summary>
        /// List of localizations in array format.
        /// </summary>
        public Localization[] Localizations => localizations.ToArray();
        
        /// <summary>
        /// Adds a new localization language to the repository.
        /// </summary>
        /// <param name="language">New localization language</param>
        public void AddLanguage(Language language)
        {
            for (int i = 0; i < localizations.Count; i++)
            {
                localizations[i].Resources.Add(localizations[i].Resources[0].Clone());
            }
            languages.Add(language);
            SaveChanges();
        }

        /// <summary>
        /// Removes the selected localization language from the repository.
        /// </summary>
        /// <param name="index">Index in list</param>
        public void RemoveLanguage(int index)
        {
            for (int i = 0; i < localizations.Count; i++)
            {
                localizations[i].Resources.RemoveAt(index);
            }
            languages.RemoveAt(index);
            SaveChanges();
        }

        /// <summary>
        /// Does the repository contain this localization.
        /// </summary>
        /// <param name="localization"><see cref="Localization"/></param>
        /// <returns>Does the repository contain this localization</returns>
        public bool ContainsLocalization(Localization localization) => localizations.Contains(localization);

        /// <summary>
        /// Adds a new localization object to the repository.
        /// </summary>
        /// <param name="localization">New <see cref="Localization"/></param>
        public void AddLocalization(Localization localization)
        {
            localizations.Add(localization);
            SaveChanges();
            return;
        }

        /// <summary>
        /// Used to get localized resources by localization tag ID, which is assigned when adding localization to the repository.
        /// </summary>
        /// <param name="id">Identifier of the localization tag in the repository</param>
        /// <returns>Localization</returns>
        public Localization GetLocalization(string id)
        {
            for (int i=0; i < localizations.Count; i++)
            {
                if (localizations[i].ID.Equals(id))
                {
                    return localizations[i];
                } 
            }
            throw new System.ArgumentException("Resource localization not found");
        }

        /// <summary>
        /// Returns the default localization for the given resource type
        /// </summary>
        /// <param name="type">Resource type</param>
        /// <returns><see cref="Localization"/></returns>
        public Localization GetDefaultLocalization(System.Type type)
        {
            for (int i = 0; i < localizations.Count; i++)
            {
                if (localizations[i].IsDefault && localizations[i].Type.Equals(type))
                {
                    return localizations[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Removes localization in the list of localizations.
        /// </summary>
        /// <param name="localization"><see cref="Localization"/></param>
        public void RemoveLocalization(Localization localization)
        {
            localizations.Remove(localization);
            SaveChanges();
        }

        /// <summary>
        /// Delete all localizations for a given resource type.
        /// </summary>
        /// <param name="type"></param>
        public void RemoveAll(System.Type type)
        {
            for(int i=localizations.Count-1; i > -1; i--)
            {
                if (localizations[i].Type.Equals(type))
                {
                    localizations.Remove(localizations[i]);
                }
            }
            SaveChanges();
        }

        /// <summary>
        /// Change the order of languages in localization storage.
        /// </summary>
        /// <param name="currentIndex"></param>
        /// <param name="nextIndex"></param>
        public void ReorderLocalizations(int currentIndex, int nextIndex)
        {
            for (int i = 0; i < localizations.Count; i++)
            {
                var temp = localizations[i].Resources[currentIndex];
                localizations[i].Resources.Remove(temp);
                localizations[i].Resources.Insert(nextIndex, temp);
            }
            SaveChanges();
        }

        /// <summary>
        /// Saves vault data changes in the editor.
        /// </summary>
        public void SaveChanges()
        {
            version++;
            LocalizationManager.SetStorage(this);
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
    }
}