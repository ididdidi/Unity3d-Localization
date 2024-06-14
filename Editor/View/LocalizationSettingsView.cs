using UnityEditor;
using UnityEngine;

namespace EasyAssetsLocalize
{
    /// <summary>
    /// Class for displaying the settings of this plugin in the Unity editor.
    /// </summary>
    internal class LocalizationSettingsView : IEditorView
    {
        private const float MIN_WIDTH = 380f;
        private const float BTN_SIZE = 20f;

        private static readonly string helpURL = "https://ididdidi.ru/projects/unity3d-easy-assets-localize";
        private LanguagesListView languagesList;
        private TypesListView typesList;
        private Vector2 scrollPosition;

        public System.Action OnBackButton;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="onCloseButton">Callback to close view</param>
        public LocalizationSettingsView(IStorage storage)
        {
            languagesList = new LanguagesListView(storage);
            typesList = new TypesListView(storage);
        }

        /// <summary>
        /// Method to display in an inspector or editor window.
        /// </summary>
        /// <param name="position"><see cref="Rect"/></param>
        public void OnGUI(Rect position)
        {
            GUILayout.BeginArea(position);

            GUILayout.BeginVertical();
            
            DrawHeader();
            DrawSettings(position.width > MIN_WIDTH);

            GUILayout.EndVertical();
            GUILayout.EndArea();

            HandleKeyboard(Event.current);
        }

        /// <summary>
        /// Draws a header for the view.
        /// </summary>
        private void DrawHeader()
        {
            GUILayout.Space(2);
            GUILayout.BeginHorizontal(EditorStyles.inspectorFullWidthMargins);

            var content = new GUIContent(EditorGUIUtility.IconContent("tab_prev").image, "Back");
            if (GUILayout.Button(content, EditorStyles.label, GUILayout.Width(BTN_SIZE), GUILayout.Height(BTN_SIZE)))
            {
                GoBack();
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label("Settings", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();

            // Draw header
            content = new GUIContent(EditorGUIUtility.IconContent("_Help").image, "Help");
            if (GUILayout.Button(content, EditorStyles.label, GUILayout.Width(BTN_SIZE), GUILayout.Height(BTN_SIZE)))
            {
                Application.OpenURL(helpURL);
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(4);
        }

        /// <summary>
        /// Renders the current plugin settings.
        /// </summary>
        /// <param name="horizontalPositioning">Horizontal arrangement of settings lists</param>
        private void DrawSettings(bool horizontalPositioning)
        {
            scrollPosition = GUILayout.BeginScrollView(scrollPosition, EditorStyles.inspectorFullWidthMargins);
            if (horizontalPositioning) { GUILayout.BeginHorizontal(); }

            // Show Languages ReorderableList
            GUILayout.BeginVertical();
            languagesList.DoLayoutList();
            GUILayout.EndVertical();
            GUILayout.Space(2);
            // Show supported types list
            GUILayout.BeginVertical();
            typesList.DoLayoutList();
            GUILayout.EndVertical();

            if (horizontalPositioning) { GUILayout.EndHorizontal(); }
            GUILayout.EndScrollView();
        }

        /// <summary>
        /// Method called when the Back button is clicked
        /// </summary>
        public void GoBack()
        {
            languagesList.index = typesList.index = -1;
            OnBackButton?.Invoke();
            EditorGUI.FocusTextInControl(null);
        }

        /// <summary>
        /// Handles keystrokes
        /// </summary>
        /// <param name="curentEvent"></param>
        private void HandleKeyboard(Event curentEvent)
        {
            if (curentEvent.type == EventType.KeyDown)
            {
                switch (curentEvent.keyCode)
                {
                    case KeyCode.LeftArrow:
                        {
                            GoBack();
                            curentEvent.Use();
                        }
                        return;
                }
            }
        }
    }
}
