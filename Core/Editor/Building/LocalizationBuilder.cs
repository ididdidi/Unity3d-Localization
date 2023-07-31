﻿using UnityEditor;
using UnityEngine;

namespace ResourceLocalization
{
    /// <summary>
    /// Creates an instance of a script object of type LocalizationStorage
    /// </summary>
    public static class LocalizationBuilder
    {

        [InitializeOnLoadMethod]
        private static void Initialize()
        {
            CreateLocalizationStorage();
        }

        public static void CreateLocalizationStorage()
        {
            if (!Resources.Load<LocalizationStorage>(nameof(LocalizationStorage)))
            {
                var path = GetDirectory($"{nameof(LocalizationStorage)}.cs")
                    .Replace("/Core", "/Resources")
                    .Replace(Application.dataPath, "Assets");

                var storage = AssetCreator.Create<LocalizationStorage>(path);
                storage.AddLanguage(new Language(Application.systemLanguage));
                CreateComponent(storage, "Text");
            }
        }

        public static void CreateComponent(LocalizationStorage storage, object defaultValue)
        {
            if (defaultValue == null) { throw new System.ArgumentNullException(nameof(defaultValue)); }

            var path = GetDirectory($"{typeof(LocalizationComponentEditor).Name}.cs").Replace("/Core/Editor", "/Components");
            if (!System.IO.Directory.Exists($"{path}Editor/"))
            {
                System.IO.Directory.CreateDirectory($"{path}Editor/");
            }

            var type = defaultValue.GetType();
            ClassCreator.CreateClass(type.Name + "Localization", path, new LocalizationComponentPrototype(type).Code);
            ClassCreator.CreateClass(type.Name + "LocalizationEditor", path + "Editor/", new LocalizationEditorPrototype(type).Code);

            var local = new LocalizationTag($"Default {defaultValue.GetType().Name} Localization", defaultValue, storage.Languages.Count);
            storage.AddLocalizationTag(local);

            EditorUtility.SetDirty(storage);
            AssetDatabase.Refresh();
        }

        public static bool Conteins(System.Type type) => string.IsNullOrEmpty(GetDirectory($"{type.Name}LocalizationEditor.cs"));

        public static void Remove(System.Type type)
        {
            var fileName = $"{type.Name}LocalizationEditor.cs";
            var path = GetDirectory(fileName);
            if (!string.IsNullOrEmpty(path))
            {
                System.IO.File.Delete($"{path}{fileName}");
                System.IO.File.Delete($"{path.Replace("/Editor", "")}{fileName.Replace("Editor", "")}");
                AssetDatabase.Refresh();
            }
            else throw new System.ArgumentNullException(fileName);
        }

        /// <summary>
        /// Method for finding a file in the Assets directory.
        /// </summary>
        /// <param name="fileName">File fullname</param>
        /// <returns>Path to the first file found, or <see cref="null"/></returns>
        public static string GetDirectory(string fileName)
        {
            string[] res = System.IO.Directory.GetFiles(Application.dataPath, fileName, System.IO.SearchOption.AllDirectories);
            if (res.Length == 0) { return null; }
            string path = res[0].Replace(fileName, "").Replace("\\", "/");
            return path;
        }
    }
}