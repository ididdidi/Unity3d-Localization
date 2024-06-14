using UnityEditor;
using UnityEngine;

namespace EasyAssetsLocalize
{
    internal class LocalizationPresenter
	{
		private const float MIN_WIDTH = 640f;
		private bool isWideView;

		private readonly Color LightSkin = new Color(0.77f, 0.77f, 0.77f);
		private readonly Color DarkSkin = new Color(0.22f, 0.22f, 0.22f);
		protected Color Background => EditorGUIUtility.isProSkin ? DarkSkin : LightSkin;

		protected IDisplay parent;
		protected IEditorView currentView;

		protected SearchTreeView searchView;
		protected LocalizationView localizationView;
		protected LocalizationSettingsView settingsView;

		private TypeView typeView;

		/// Animated view change
		private IEditorView animatedView;
		private long lastTime;
		private float currentAnimation, targetAnimation = 0f;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parent">An instance class that provides a Rect and implements the repaint method</param>
		/// <param name="searchView">Displaying search in the editor</param>
		/// <param name="localizationView">Displaying localization in the editor</param>
		/// <param name="settingsView">Displaying settings in the editor</param>
		public LocalizationPresenter(IDisplay parent, SearchTreeView searchView, LocalizationView localizationView, LocalizationSettingsView settingsView)
		{
			this.parent = parent ?? throw new System.ArgumentNullException(nameof(parent));
			this.searchView = searchView ?? throw new System.ArgumentNullException(nameof(searchView));
			this.localizationView = localizationView ?? throw new System.ArgumentNullException(nameof(localizationView));
			this.settingsView = settingsView ?? throw new System.ArgumentNullException(nameof(settingsView));

			this.typeView = new TypeView();
			this.searchView.OptionButton = ShowSettingsButton;
			this.searchView.OnFocusEntry = OnFocusEntry;
			this.searchView.OnSelectEntry = OnSelectEntry;
			this.localizationView.OnBackButton = CloseLocalizationView;
			this.settingsView.OnBackButton = CloseSettingsView;
		}

		/// <summary>
		/// Drawing LocalizationStorage in the inspector window.
		/// </summary>
		public void OnGUI(Rect position)
		{
			Rect rect;

			if (position.width > MIN_WIDTH)
			{
				if (!isWideView)
				{
					isWideView = true;
					if(currentView == searchView) {
						currentView = typeView;
						searchView.Refresh();
					}
				}

				rect = new Rect(0, 0, 320, position.height);
				EditorGUI.DrawRect(rect, Background);
				searchView?.OnGUI(rect);

				rect.x = 319;
				rect.width = position.width - 320;
				EditorGUI.DrawRect(rect, Background);

				currentView?.OnGUI(rect);
				
				rect.width = 1;
				EditorGUI.DrawRect(rect, Color.black);
			}
			else
			{ 
				isWideView = false;

				if(currentView == null || currentView == typeView) { currentView = searchView; }
				currentView.OnGUI(position);
				if (currentAnimation != targetAnimation) { AnimatedChangeView(position); }
			}

			rect = new Rect(position);
			rect.height = 1;
			EditorGUI.DrawRect(rect, Color.black);
			rect.y = position.height - 1;
			EditorGUI.DrawRect(rect, Color.black);
		}

		/// <summary>
		/// Button to show settings
		/// </summary>
		/// <param name="rect">Position</param>
		private void ShowSettingsButton(Rect rect)
		{
			if (GUI.Button(rect, new GUIContent(EditorGUIUtility.IconContent("_Popup").image, "Settings"), GUIStyle.none))
			{
				if (isWideView) { currentView = settingsView; }
				else { StartChangeView(settingsView, 1f, 0f); }
			}
		}

		/// <summary>
		/// Updating data to be displayed in views.
		/// </summary>
		/// <param name="data">Data to display</param>
		/// <returns>Returns a view depending on the received data</returns>
		private IEditorView UpdateView(object data)
		{
			if (data is GUIContent content)
			{
				typeView.Content = content;
				return typeView;
			}
			else if (data is Localization localization)
			{
				localizationView.Data = localization;
				return localizationView;
			}
			return currentView;
		}

		/// <summary>
		/// Method called when focus appears on a entry.
		/// </summary>
		/// <param name="data">Data in focused entry</param>
		private void OnFocusEntry(object data)
		{
			var view = UpdateView(data);
			if (isWideView && currentView != settingsView) { currentView = view; }
		}

		/// <summary>
		/// Method called when select on a entry.
		/// </summary>
		/// <param name="data">Data in selected entry</param>
		private void OnSelectEntry(object data)
		{
			var view = UpdateView(data);
			if (isWideView) { currentView = view; }
			else { StartChangeView(localizationView, 1f, 0f); }
		}

		/// <summary>
		/// Hides the localization view.
		/// </summary>
		private void CloseLocalizationView()
		{
			if (isWideView) { searchView?.GoToParent(); }
			else { StartChangeView(localizationView, 0f, 1f); }
		}

		/// <summary>
		/// Hides the settings view.
		/// </summary>
		private void CloseSettingsView()
		{
			if (isWideView) { currentView = (searchView.CurrentEntry is SearchTreeGroupEntry) ? typeView : localizationView as IEditorView; }
			else { StartChangeView(settingsView, 0f, 1f); }
		}

		/// <summary>
		/// Method that triggers the change of active view.
		/// </summary>
		/// <param name="view">Animated view</param>
		/// <param name="currentAnimation">Current animation position value</param>
		/// <param name="targetAnimation">Target animation position value</param>
		private void StartChangeView(IEditorView view, float currentAnimation, float targetAnimation)
		{
			animatedView = view;
			currentView = searchView;
			this.currentAnimation = currentAnimation;
			this.targetAnimation = targetAnimation;
			lastTime = System.DateTime.Now.Ticks;
		}

		/// <summary>
		/// View change animation
		/// </summary>
		/// <param name="position"></param>
		private void AnimatedChangeView(Rect position)
		{
			long now = System.DateTime.Now.Ticks;
			float deltaTime = (now - lastTime) / (float)System.TimeSpan.TicksPerSecond;
			lastTime = now;

			var rect = new Rect(position);
			currentAnimation = Mathf.MoveTowards(currentAnimation, targetAnimation, deltaTime * 4);
			rect.x = rect.width * currentAnimation;
			EditorGUI.DrawRect(rect, Background);
			animatedView.OnGUI(rect);

			if (currentAnimation == targetAnimation && targetAnimation == 0)
			{
				currentView = animatedView;
				animatedView = null;
			}
			parent.Repaint();
		}
	}
}