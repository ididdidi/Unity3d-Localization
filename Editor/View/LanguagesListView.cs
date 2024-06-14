using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EasyAssetsLocalize
{
    /// <summary>
    /// Class for visualizing the list of used languages.
    /// </summary>
    internal class LanguagesListView : ReorderableList
    {
        private IStorage storage;
        private int lastIndex =-1;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="storage">Localization store instance</param>
        public LanguagesListView(IStorage storage) : base(storage.Languages, typeof(Language), true, true, true, true)
        {
            this.storage = storage;
            drawHeaderCallback = DrawHeader;
            drawElementCallback = DrawLanguage;
            onAddCallback = AddLanguage;
            onRemoveCallback = RemoveLanguage;
            onReorderCallback = ReorderLanguages;
        }

        /// <summary>
        /// Draws the title of the list of languages.
        /// </summary>
        /// <param name="position"><see cref="Rect"/> provided for drawing the header</param>
        private void DrawHeader(Rect position)
        {
            EditorGUI.LabelField(position, "Languages");

            if (list.Count > 1) { displayRemove = true; }
            else { displayRemove = false; }
        }

        /// <summary>
        /// Draws <see cref="Language"/> settings from a list
        /// </summary>
        /// <param name="position"><see cref="Rect"/>provided for rendering the language settings</param>
        /// <param name="index"><see cref="Language"/> index in the list</param>
        /// <param name="isActive">Is the list item active?</param>
        /// <param name="isFocused">Is the list item on focused?</param>
        private void DrawLanguage(Rect position, int index, bool isActive, bool isFocused)
        {
            Rect rect = new Rect(position);
            rect.width = 156f;
            rect.x -= 2f;
            rect.y += 1f;
            Language language = (Language)list[index];
            
            EditorGUI.BeginChangeCheck();
            SystemLanguage systemLanguage = (SystemLanguage)EditorGUI.EnumPopup(rect, language.SystemLanguage);
            if (EditorGUI.EndChangeCheck()) 
            {
                Undo.RecordObject((Object)storage, $"Rename {language.SystemLanguage} to {systemLanguage}");
                language.SystemLanguage = (SystemLanguage)systemLanguage;
                storage.SaveChanges();
            }
            
            if (isFocused && lastIndex != index) { lastIndex = index; }
        }

        /// <summary>
        /// Adding a new language to the list.
        /// </summary>
        /// <param name="list">This <see cref="ReorderableList"/></param>
        private void AddLanguage(ReorderableList list)
        {
            Undo.RecordObject((Object)storage, "Add new language");
            storage.AddLanguage(new Language(SystemLanguage.Unknown));
            list.index = list.count - 1;
        }

        /// <summary>
        /// Remove the selected <see cref="Language"/> from the list.
        /// </summary>
        /// <param name="list">This <see cref="ReorderableList"/></param>
        private void RemoveLanguage(ReorderableList list)
        {
            Language language = list.list[list.index] as Language;
            if (EditorExtends.DeleteConfirmation(language.SystemLanguage.ToString()))
            {
                Undo.RecordObject((Object)storage, $"Remove {language.SystemLanguage}");
                storage.RemoveLanguage(list.index--);
                EditorGUI.FocusTextInControl(null);
            }
        }

        /// <summary>
        /// Change the order of Languages in the list.
        /// </summary>
        /// <param name="list">This <see cref="ReorderableList"/></param>
        private void ReorderLanguages(ReorderableList list)
        {
            Undo.RecordObject((Object)storage, "Reorder languages");
            storage.ReorderLocalizations(lastIndex, list.index);
        }
    }
}
