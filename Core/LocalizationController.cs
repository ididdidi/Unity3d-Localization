﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace EasyAssetsLocalize
{
    public class LocalizationController : MonoBehaviour
    {
        private static LocalizationController instance;
        private List<LocalizationComponent> components = new List<LocalizationComponent>();

        [SerializeField] private LocalizationStorage localizationStorage;
        [System.Serializable] public class Handler : UnityEvent<string> { }
        [SerializeField] private Handler OnChangingLanguage;

        public IStorage Storage { get => localizationStorage ?? (localizationStorage = Resources.Load<LocalizationStorage>(nameof(LocalizationStorage))); }

        /// <summary>
        /// The current language.
        /// </summary>
        public static Language Language
        {
            get => new Language(PlayerPrefs.HasKey("Language") ? (SystemLanguage)PlayerPrefs.GetInt("Language") : Application.systemLanguage);
            set => PlayerPrefs.SetInt("Language", (int)value.SystemLanguage);
        }

        public static LocalizationController GetInstance(bool dontDestroy = false)
        {
            if (!instance) { instance = FindObjectOfType<LocalizationController>(); }
            if (!instance)
            {
                var @object = new GameObject($"{nameof(LocalizationController)}");
                instance = @object.AddComponent<LocalizationController>();

                if (dontDestroy) { DontDestroyOnLoad(@object); }
            }
            return instance;
        }

        private void Start() => OnChangingLanguage?.Invoke(Language.ToString());

        /// <summary>
        /// Method for get localization resource data depending on the current language.
        /// </summary>
        /// <param name="component"><see cref="LocalizationComponent"/></param>
        /// <returns>Resource data</returns>
        private object GetLocalizationData(LocalizationComponent component)
        {
            var localization = Storage.GetLocalization(component);
            return localization.Resources[Storage.Languages.IndexOf(Language)].Data;
        }

        /// <summary>
        /// Subscribe to localization changes.
        /// </summary>
        /// <param name="component"><see cref="LocalizationComponent"/></param>
        public void Subscribe(LocalizationComponent component)
        {
            components.Add(component);
            component.SetData(GetLocalizationData(component));
        }

        /// <summary>
        /// Unsubscribe to localization changes.
        /// </summary>
        /// <param name="component"><see cref="LocalizationComponent"/></param>
        public void Unsubscribe(LocalizationComponent component)
        {
            if (!string.IsNullOrEmpty(component.ID) && components.Contains(component)) { components.Remove(component); }
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

        private enum Direction { Next = 1, Back = -1 }
        /// <summary>
        /// Method for changing localization language. Switching is carried out in a circle.
        /// </summary>
        /// <param name="direction">Indicates which language to select: the previous one in the list or the next one</param>
        private void ChangeLocalzation(Direction direction)
        {
            var languages = new List<Language>(Storage.Languages);
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

        /// <summary>
        /// Sets the current language and loads localized resources.
        /// </summary>
        /// <param name="language">Language</param>
        private void SetLanguage(Language language)
        {
            Language = language;
            OnChangingLanguage?.Invoke(language.ToString());
            for (int i = 0; i < components.Count; i++)
            {
                components[i].SetData(GetLocalizationData(components[i]));
            }
        }
    }
}