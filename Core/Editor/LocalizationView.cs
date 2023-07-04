﻿using UnityEditor;
using UnityEngine;
using UnityExtended;

namespace ResourceLocalization
{
	public class LocalizationView
	{
		private readonly Color background = new Color(0.22f, 0.22f, 0.22f);
		private LocalizationStorage LocalizationStorage { get; }
		public LocalizationTag Tag { get; set; }
		private Vector2 scrollPosition = Vector2.zero;

		public LocalizationView(LocalizationStorage storage)
		{
			LocalizationStorage = storage ?? throw new System.ArgumentNullException(nameof(storage));
		}

		public void OnGUI(Rect position)
		{
			EditorGUI.DrawRect(position, background);
			GUI.Label(position, GUIContent.none, "grey_border");

			if (Tag != null)
			{
				var changeCheck = LocalizationStorage.ContainsLocalizationTag(Tag);
				try
				{
					GUILayout.BeginArea(position);
					if (changeCheck) { EditorGUI.BeginChangeCheck(); }
					DrawHeader(Tag);

					scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUIStyle.none);
					GUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
					DrawResources(Tag, LocalizationStorage.Languages);
					GUILayout.EndVertical();
					GUILayout.EndScrollView();

					if (changeCheck && EditorGUI.EndChangeCheck()) { LocalizationStorage?.ChangeVersion(); }
					GUILayout.EndArea();
				}
				catch (System.ArgumentException){ }
			}
		}

		private void DrawHeader(LocalizationTag localization)
		{
			GUILayout.Space(2);
			GUILayout.BeginHorizontal(EditorStyles.inspectorDefaultMargins);
			GUILayout.Label("Name");
			GUILayout.FlexibleSpace();
			localization.Name = GUILayout.TextField(localization.Name);
			GUILayout.FlexibleSpace();

			if (LocalizationStorage.ContainsLocalizationTag(localization))
			{
				if (GUILayout.Button("Delete")) { LocalizationStorage.RemoveLocalizationTag(localization); EditorGUI.FocusTextInControl(null); }
			}
			else
			{
				if (GUILayout.Button("Add")) { LocalizationStorage.AddLocalizationTag(localization); EditorGUI.FocusTextInControl(null); }
			}
			GUILayout.EndHorizontal();
			ExtendedEditorGUI.DrawLine(Color.black);
		}

		public static void DrawResources(LocalizationTag tag, Language[] languages, params GUILayoutOption[] options)
		{
			GUIStyle style = new GUIStyle(EditorStyles.textArea);
			style.wordWrap = true;
			var isString = tag.Type.IsAssignableFrom(typeof(string));
			for (int i = 0; i < tag.Resources.Count; i++)
			{
				if (isString)
				{
					EditorGUILayout.LabelField(languages[i].Name);
					tag.Resources[i].Data = EditorGUILayout.TextArea((string)tag.Resources[i].Data, style, options);
				}
				else
				{
					tag.Resources[i].Data = EditorGUILayout.ObjectField(languages[i].Name, (Object)tag.Resources[i].Data, tag.Type, false, options);
				}
			}
		}
	}
}