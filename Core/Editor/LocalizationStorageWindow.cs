﻿using UnityEditor;
using UnityEngine;
using UnityExtended;

namespace SimpleLocalization
{
	/// <summary>
	/// Display localization storage data in a separate inspector window.
	/// </summary>
	public class LocalizationStorageWindow : EditorWindow, IDisplay
	{
		public const float MIN_WIDTH = 240f;
		public const float MIN_HIGHT = 320f;

		// Data renderer in a editor window
		private SearchTreeView searchView;
		private LocalizationView localizationView;
		private LocalizationPropertiesView propertiesView;
		private WideLocalizationPresenter widePresentor;
		private NarrowLocalizationPresenter narrowPresentor;
		private int storageVersion;
		private static LocalizationStorage LocalizationStorage { get => LocalizationManager.Storage; }

		private void OnEnable()
		{
			titleContent = new GUIContent("Simple Localization", EditorGUIUtility.IconContent("FilterByType@2x").image);
			localizationView = new LocalizationView(LocalizationStorage);
			propertiesView = new LocalizationPropertiesView();
			searchView = new SearchTreeView(this, new LocalizationSearchProvider(LocalizationStorage));
			widePresentor = new WideLocalizationPresenter(this, searchView, localizationView, propertiesView);
			narrowPresentor = new NarrowLocalizationPresenter(this, searchView, localizationView, propertiesView);
		}

		/// <summary>
		/// Creation of initialization and display of a window on the monitor screen.
		/// </summary>
		public static LocalizationStorageWindow Show(float minWidth = MIN_WIDTH, float minHight = MIN_HIGHT)
		{
			var instance = GetWindow<LocalizationStorageWindow>(LocalizationStorage.name, true);

			instance.minSize = new Vector2(minWidth, minHight);

			return instance;
		}

		/// <summary>
		/// Creation of initialization and display of a window on the monitor screen.
		/// </summary>
		[MenuItem("Window/Localization Storage")]
		public new static LocalizationStorageWindow Show() => Show(MIN_WIDTH, MIN_HIGHT);

		/// <summary>
		/// Method for rendering window content.
		/// </summary>
		internal void OnGUI()
		{
			if(searchView != null && storageVersion != LocalizationStorage.Version)
			{
				searchView.IsChanged = true;
				storageVersion = LocalizationStorage.Version;
			}

			if(position.width > 320) { widePresentor.OnGUI(position); }
			else { narrowPresentor.OnGUI(position); }
		}
	}
}