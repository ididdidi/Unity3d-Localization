﻿using System.Collections.Generic;
using UnityEngine;

namespace ResourceLocalization
{
    /// <summary>
    /// Class for providing localization to objects in the scene
    /// </summary>
    public class LocalizationController : MonoBehaviour
    {
        [SerializeField] private LocalizationStorage localizationStorage;
        [SerializeField, HideInInspector] private List<LocalizationTag> localizationTags = new List<LocalizationTag>();
        private Dictionary<string, Resource> dictionary;

        /// <summary>
        /// The current language.
        /// </summary>
        public Language Language
        {
            get => new Language(PlayerPrefs.HasKey("Language") ? PlayerPrefs.GetString("Language") : Application.systemLanguage.ToString());
            set => PlayerPrefs.SetString("Language", value.Name);
        }
        /// <summary>
        /// List of localization tags on this scene.
        /// </summary>
        public List<LocalizationTag> LocalizationTags { get => localizationTags; }
        /// <summary>
        /// Link to localization repository.
        /// </summary>
        public LocalizationStorage LocalizationStorage { get => localizationStorage; }
        
        /// <summary>
        /// Start is called before the first frame update.
        /// </summary>
        void Start()
        {
            LoadLocalization(localizationStorage);
        }

        /// <summary>
        /// Sets localization.
        /// </summary>
        /// <param name="localizationStorage">Localization repository</param>
        public void LoadLocalization(LocalizationStorage localizationStorage)
        {
            this.localizationStorage = localizationStorage;
            if (localizationStorage.ConteinsLanguage(Language)) { SetLanguage(Language); }
            else { SetLanguage(localizationStorage.Languages[0]); }
        }

        /// <summary>
        /// Sets the current language and loads localized resources.
        /// </summary>
        /// <param name="language">Localization language</param>
        public void SetLanguage(Language language)
        {
            dictionary = localizationStorage.GetDictionary(language);
            Language = language;

            for(int i=0; i < localizationTags.Count; i++)
            {
                localizationTags[i].Resource = dictionary[localizationTags[i].ID];
            }
        }

        /// <summary>
        /// Changes the language to the next one in the localization list.
        /// </summary>
        public void SetNextLanguage()
        {
            ChangeLocalzation(Direction.Next);
        }

        /// <summary>
        /// Changes the language to the previous one in the list of localizations.
        /// </summary>
        public void SetPrevLanguage()
        {
            ChangeLocalzation(Direction.Back);
        }

        private enum Direction { Next = 1, Back =-1 }
        /// <summary>
        /// Method for changing localization language. Switching is carried out in a circle.
        /// </summary>
        /// <param name="direction">Indicates which language to select: the previous one in the list or the next one</param>
        private void ChangeLocalzation(Direction direction)
        {
            var languages = new List<Language>(localizationStorage.Languages);
            if (languages.Count < 2) { return; }

            int index = languages.IndexOf(Language);
            if (index > -1)
            {
                int newIndex = (index + (int)direction) % languages.Count;
                if (newIndex >= 0) { index = newIndex; }
                else { index = languages.Count + newIndex; }
                SetLanguage(languages[index]);
            }
            else
            {
                throw new System.ArgumentException($"{Language} not found in the {localizationStorage}");
            }
        }
    }
}