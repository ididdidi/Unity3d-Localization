﻿using UnityEditor;
using UnityEngine;
using UnityExtended;

namespace SimpleLocalization
{
    public class LocalizationPropertiesView1 : IEditorView
    {
        private static readonly string helpURL = "https://ididdidi.ru/";
        private LanguagesListView languagesList;
        private TypesListView typesList;
        private System.Action onCloseButton;

        public float HeightInGUI => (21 * (languagesList.count + typesList.count)) + 128f;


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="onCloseButton">Callback to close view</param>
        public LocalizationPropertiesView1(System.Action onCloseButton)
        {
            this.onCloseButton = onCloseButton ?? throw new System.ArgumentNullException(nameof(onCloseButton));
            languagesList = new LanguagesListView(LocalizationManager.Storage);
            typesList = new TypesListView();
        }

        public void OnGUI(Rect position)
        {
            GUILayout.BeginArea(position);

            GUILayout.Space(2);
            GUILayout.BeginHorizontal(EditorStyles.inspectorFullWidthMargins);

            var content = new GUIContent(EditorGUIUtility.IconContent("tab_prev@2x").image, "Back");
            if (GUILayout.Button(content, EditorStyles.label, GUILayout.Width(20f), GUILayout.Height(20f)))
            {
                GoBack();
            }

            GUILayout.FlexibleSpace();
            GUILayout.Label("Propertyes", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();

            // Draw header
            content = new GUIContent(EditorGUIUtility.IconContent("_Help@2x").image, "Help");
            if (GUILayout.Button(content, EditorStyles.label, GUILayout.Width(20f), GUILayout.Height(20f)))
            {
                Application.OpenURL(helpURL);
            }

            GUILayout.EndHorizontal();
            GUILayout.BeginVertical(EditorStyles.inspectorFullWidthMargins);

            // Show Languages ReorderableList
            languagesList.DoLayoutList();
            GUILayout.Space(2);
            // Show supported types list
            typesList.DoLayoutList();

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        /// <summary>
        /// Method called when the Back button is clicked
        /// </summary>
        public void GoBack()
        {
            languagesList.index = typesList.index = -1;
            onCloseButton();
            EditorGUI.FocusTextInControl(null);
        }
    }
}
