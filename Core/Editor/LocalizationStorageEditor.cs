﻿using UnityEditor;
using UnityEngine;
using UnityExtended;

namespace SimpleLocalization
{
	/// <summary>
	/// Responsible for presenting the Localization Storage in the inspector.
	/// </summary>
	[CustomEditor(typeof(LocalizationStorage))]
	public partial class LocalizationStorageEditor : Editor, IDisplay
	{
		private LocalizationStorage storage;
		private SearchTreeView searchView;
		private LocalizationView localizationView;
		private LocalizationPropertiesView propertiesView;
		private LocalizationPresenter localizationPresentor;
		private int storageVersion;

		/// <summary>
		/// Storage link caching.
		/// </summary>
		public void OnEnable()
		{
			storage = this.target as LocalizationStorage;
			localizationView = new LocalizationView(storage);
			propertiesView = new LocalizationPropertiesView();
			searchView = new SearchTreeView(this, new LocalizationSearchProvider(storage), false);
			localizationPresentor = new NarrowLocalizationPresenter(this, searchView, localizationView, propertiesView);
		}

		/// <summary>
		/// Drawing LocalizationStorage in the inspector window.
		/// </summary>
		public override void OnInspectorGUI()
		{
			if (searchView != null && storageVersion != storage.Version)
			{
				searchView.IsChanged = true;
				storageVersion = storage.Version;
			}

			var width = EditorGUIUtility.currentViewWidth;
			var height = Screen.height * (width / Screen.width) - 160;
			GUILayoutUtility.GetRect(width, height);
			var position = new Rect(0, 0, width, height);

			localizationPresentor.OnGUI(position);
		}
	}
}