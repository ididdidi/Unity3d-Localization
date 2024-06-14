using UnityEditor;
using UnityEngine;

namespace EasyAssetsLocalize
{
	/// <summary>
	/// Display localization storage data in a separate inspector window.
	/// </summary>
	internal class LocalizationStorageWindow : EditorWindow, IDisplay
	{
		public const float MIN_WIDTH = 240f;
		public const float MIN_HIGHT = 320f;

		// Data renderer in a editor window
		private IStorage storage;
		private NoticeView noticeView;
		private SearchTreeView searchView;
		private LocalizationView localizationView;
		private LocalizationSettingsView settingsView;
		private LocalizationPresenter localizationPresentor;

		/// <summary>
		/// Creation of initialization and display of a window on the monitor screen.
		/// </summary>
		public static LocalizationStorageWindow Show(float minWidth = MIN_WIDTH, float minHight = MIN_HIGHT)
		{
			var instance = GetWindow<LocalizationStorageWindow>();
			instance.titleContent = new GUIContent("Easy Assets Localize", EditorGUIUtility.IconContent("FilterByType@2x").image);
			instance.minSize = new Vector2(minWidth, minHight);
			return instance;
		}

		/// <summary>
		/// Creation of initialization and display of a window on the monitor screen.
		/// </summary>
		[MenuItem("Window/Localization Storage")]
		public new static LocalizationStorageWindow Show() => Show(MIN_WIDTH, MIN_HIGHT);

		private void OnEnable()
		{
			SetStorage(LocalizationManager.Storage);
			LocalizationManager.OnStorageChange += SetStorage;
		}

		/// <summary>
		/// Method for displaying data changes.
		/// </summary>
		private void SetStorage(IStorage storage)
		{
			if (this.storage != storage && storage != null)
			{
				this.storage = storage;
				noticeView = new NoticeView(this);
				localizationView = new LocalizationView(storage, noticeView);
				settingsView = new LocalizationSettingsView(storage);
				searchView = new SearchTreeView(this, new LocalizationSearchProvider(storage));
				localizationPresentor = new LocalizationPresenter(this, searchView, localizationView, settingsView);
			}
			searchView.Refresh();
			Repaint();
		}

		/// <summary>
		/// Method for rendering window content.
		/// </summary>
		internal void OnGUI()
		{
			Rect rect = new Rect(0, 0, position.width, position.height);
			if (localizationPresentor != null)
			{
				localizationPresentor.OnGUI(rect);
				noticeView?.OnGUI();
			}
			else
			{
				ShowEroor(rect, "Localization storage not found!");
			}
		}

		private void ShowEroor(Rect position, string text)
		{
			var rect = new Rect(0f, 0f, 128f, 128f);
			rect.center = position.center;
			rect.y -= 25f;

			var content = new GUIContent(text, EditorGUIUtility.IconContent("console.erroricon@2x").image);
			GUI.DrawTexture(rect, content.image, ScaleMode.ScaleToFit);

			position.y += 55f;
			var style = new GUIStyle("AM MixerHeader");
			style.alignment = TextAnchor.MiddleCenter;
			style.fontSize = 32;
			GUI.Label(position, content.text, style);
		}

		/// <summary>
		/// This function is called when the behaviour becomes disabled.
		/// </summary>
		private void OnDisable() => LocalizationManager.OnStorageChange -= SetStorage;
	}
}