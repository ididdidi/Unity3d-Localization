﻿using UnityEditor;
using UnityEngine;

namespace EasyAssetsLocalize
{
    internal class LocalizationSettingsView : IEditorView
    {
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
            GUILayout.Space(2);
            GUILayout.BeginHorizontal(EditorStyles.inspectorFullWidthMargins);

            var content = new GUIContent(EditorGUIUtility.IconContent("tab_prev").image, "Back");
            if (GUILayout.Button(content, EditorStyles.label, GUILayout.Width(20f), GUILayout.Height(20f)))
            {
                GoBack();
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label("Settings", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();

            // Draw header
            content = new GUIContent(EditorGUIUtility.IconContent("_Help").image, "Help");
            if (GUILayout.Button(content, EditorStyles.label, GUILayout.Width(20f), GUILayout.Height(20f)))
            {
                Application.OpenURL(helpURL);
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(4);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, EditorStyles.inspectorFullWidthMargins);

            var horizontal = position.width > 370;
            if (horizontal) { GUILayout.BeginHorizontal(); }

            // Show Languages ReorderableList
            GUILayout.BeginVertical();
            languagesList.DoLayoutList();
            GUILayout.EndVertical();
            GUILayout.Space(2);
            // Show supported types list
            GUILayout.BeginVertical();
            typesList.DoLayoutList();
            GUILayout.EndVertical();

            if (horizontal) { GUILayout.EndHorizontal(); }
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            GUILayout.EndArea();

            HandleKeyboard(Event.current);
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