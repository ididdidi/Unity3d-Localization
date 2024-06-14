using UnityEditor;
using UnityEngine;

namespace EasyAssetsLocalize
{
    /// <summary>
    /// Class for displaying localization component.
    /// </summary>
    [CustomEditor(typeof(LocalizationComponent))]
    public class LocalizationComponentEditor : Editor
    {
        private IStorage storage;
        private LocalizationComponent component;
        private Localization localization;
        private SearchDropDownWindow dropDownWindow;
        private SerializedProperty handler;

        /// <summary>
        /// Only checks localization instance replacement.
        /// </summary>
        private bool IsChanged { get => component?.ID != localization?.ID; }

        /// <summary>
        /// Method for displaying a list of initialization handlers and changing localization.
        /// </summary>
        private void DrawHandler()
        {
            serializedObject.Update();
            if (handler == null) { handler = serializedObject.FindProperty("handler"); }
            if (handler != null) { EditorGUILayout.PropertyField(handler); }
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Standard method for initialization
        /// </summary>
        private void OnEnable()
        {
            component = target as LocalizationComponent;
            SetStorage(LocalizationManager.Storage);
            LocalizationManager.OnStorageChange += SetStorage;

            /// When adding a component to the scene
            if (string.IsNullOrEmpty(component.ID)) { 
                var defaultLocal = storage.GetDefaultLocalization(component.Type);
                localization = defaultLocal;
                component.ID = defaultLocal.ID;
            }
        }

        /// <summary>
        /// Method to set the current localization store.
        /// </summary>
        /// <param name="storage">Localization storage</param>
        private void SetStorage(IStorage storage)
        {
            this.storage = storage;
            UpdateLocalization();
        }

        /// <summary>
        /// Standard Method for displaying fields in an Inspector Window.
        /// </summary>
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawLocalization();
            DrawHandler();
        }

        private Localization tempLocal;
        /// <summary>
        /// Drawing localization fields in the inspector window.
        /// </summary>
        private void DrawLocalization()
        {
            if (localization != null)
            {
                if (IsChanged) { UpdateLocalization(); }
                localization.CopyTo(ref tempLocal);
                EditorGUI.BeginChangeCheck();
                EditorGUI.BeginDisabledGroup(localization.IsDefault);
                tempLocal.Name = EditorGUILayout.TextField("Localization name", tempLocal.Name);
                LocalizationView.DrawResources(tempLocal, storage.Languages.ToArray(), GUILayout.Height(50f));
                EditorGUI.EndDisabledGroup();

                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject((Object)storage, $"Changed data for {localization.Name} in {storage} v{storage.Version}");
                    if (string.IsNullOrWhiteSpace(tempLocal.Name)) { tempLocal.Name = localization.Name; }
                    tempLocal.CopyTo(ref localization);
                    storage?.SaveChanges(); 
                }
            }
            else
            {
                EditorExtends.DisplayMessage("The localization of this object in the repository was not found. " +
                    "The default localization will be used at runtime.", MessageType.Error);
            }

            if (EditorExtends.CenterButton("Change Localization")) { ShowSearchWindow(); }
        }

        /// <summary>
        /// Opens a dropdown window with a list of localizations.
        /// </summary>
        private void ShowSearchWindow()
        {
            dropDownWindow = SearchDropDownWindow.Show(new LocalizationSearchProvider(storage, component.Type), SetLocaloization);
        }

        /// <summary>
        /// Method for handling localization selection from the list.
        /// </summary>
        /// <param name="data">Localization instance as <see cref="object"/></param>
        /// <returns>Close window</returns>
        private void SetLocaloization(object data)
        {
            if (data is Localization localization)
            {
                Undo.RecordObjects(new Object[] { component, (Object)storage }, $"Set {localization.Name} in {component.name}");
                if (!storage.ContainsLocalization(localization))
                {
                    storage.AddLocalization(localization);
                }
                component.ID = localization.ID;
                UpdateLocalization();
                EditorUtility.SetDirty(component);
                dropDownWindow?.Close();
            }
        }

        /// <summary>
        /// Method for displaying data changes.
        /// </summary>
        private void UpdateLocalization()
        {
            try { localization = storage.GetLocalization(component.ID); }
            catch (System.ArgumentException) { localization = null; }
            finally { Repaint(); }
        }

        /// <summary>
        /// This function is called when the behaviour becomes disabled.
        /// </summary>
        private void OnDisable() => LocalizationManager.OnStorageChange -= SetStorage;
    }
}