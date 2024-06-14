using UnityEditor;
using UnityEngine;

namespace EasyAssetsLocalize
{
	/// <summary>
	/// Class for displaying localization resources.
	/// </summary>
	internal class LocalizationView : IEditorView
	{
		private object data;
		private Vector2 scrollPosition;
		private bool editable = false;
		private NoticeView noticeView;

		private IStorage Storage { get; }
		public object Data { 
			get => data;
			set {
				if(data != value)
				{
					data = value;
					editable = false;
				}
			}
		}
		public System.Action OnBackButton { get; set; }

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="storage"><see cref="Localization"/> repository</param>
		/// <param name="noticeView">View to display notifications</param>
		public LocalizationView(IStorage storage, NoticeView noticeView)
		{
			this.Storage = storage ?? throw new System.ArgumentNullException(nameof(storage));
			this.noticeView = noticeView ?? throw new System.ArgumentNullException(nameof(noticeView));
		}

		private Localization tempLocal;
		/// <summary>
		/// Method to display in an inspector or editor window.
		/// </summary>
		/// <param name="position"><see cref="Rect"/></param>
		public void OnGUI(Rect position)
		{
			if (Data is Localization localization)
			{
				GUILayout.BeginArea(position);
				GUILayout.BeginVertical();
				GUILayout.Space(4);

				DrawHeader(position, localization, localization.IsDefault);

				bool isNotNew = Storage.ContainsLocalization(localization);
				localization.CopyTo(ref tempLocal);

				if (isNotNew) { EditorGUI.BeginChangeCheck(); }
				scrollPosition = GUILayout.BeginScrollView(scrollPosition);
				EditorGUI.BeginDisabledGroup(localization.IsDefault && !editable);
				DrawResources(tempLocal, Storage.Languages.ToArray());
				EditorGUI.EndDisabledGroup();
				GUILayout.EndScrollView();
				
				if (isNotNew && EditorGUI.EndChangeCheck())
				{
					Undo.RecordObject((Object)Storage, $"Changed data for {localization.Name} in {Storage} v{Storage.Version}");
					tempLocal.CopyTo(ref localization);
					Storage?.SaveChanges();
				}
				GUILayout.EndVertical();
				GUILayout.EndArea();
				
				HandleKeyboard(Event.current);
			}
		}

		/// <summary>
		/// Header render method.
		/// </summary>
		/// <param name="position"><see cref="Rect"/></param>
		/// <param name="localization"><see cref="Localization"/></param>
		private void DrawHeader(Rect position, Localization localization, bool isDefault = false)
		{
			GUILayout.BeginHorizontal(EditorStyles.inspectorFullWidthMargins);

			GUIContent content = new GUIContent(EditorGUIUtility.IconContent("tab_prev").image, "Back");
			if (GUILayout.Button(content, EditorStyles.label, GUILayout.Width(20f), GUILayout.Height(20f)))
			{
				GoBack();
			}
		
			GUILayout.FlexibleSpace();

			EditorGUI.BeginDisabledGroup(isDefault);
			EditorGUI.BeginChangeCheck();

			string name = GUILayout.TextField(localization.Name);

			if (EditorGUI.EndChangeCheck())
			{
				Undo.RecordObject((Object)Storage, $"Changed name {localization.Name} to {name} in {Storage} v{Storage.Version}");
				if (!string.IsNullOrWhiteSpace(name)) { localization.Name = name; }
				Storage?.SaveChanges();
			}
			EditorGUI.EndDisabledGroup();

			GUILayout.FlexibleSpace();
		
			if (isDefault)
			{
				content = new GUIContent(
					EditorGUIUtility.IconContent(editable ? "AssemblyLock" : "CustomTool").image,
					editable ? "Lock" : "Edit");
				editable = GUILayout.Toggle(editable, content, EditorStyles.label, GUILayout.Width(20), GUILayout.Height(20));
			}
			else if (Storage.ContainsLocalization(localization))
			{
				content = new GUIContent(EditorGUIUtility.IconContent("winbtn_win_close").image, "Delete");
				if (GUILayout.Button(content, EditorStyles.label, GUILayout.Width(20), GUILayout.Height(20)) && EditorExtends.DeleteConfirmation(localization.Name))
				{
					Undo.RecordObject((Object)Storage, $"Remove {localization.Name} from {Storage} v{Storage.Version}");
					Storage.RemoveLocalization(localization);
					noticeView.Show(position, new GUIContent($"{localization.Name} has been deleted"));
					GoBack();
				}
			}
			else
			{
				content = new GUIContent(EditorGUIUtility.IconContent("CreateAddNew").image, "Add");
				if (GUILayout.Button(content, EditorStyles.label, GUILayout.Width(20), GUILayout.Height(20)))
				{
					Undo.RecordObject((Object)Storage, $"Add {localization.Name} to {Storage} v{Storage.Version}");
					Storage.AddLocalization(localization);
					noticeView.Show(position, new GUIContent($"{localization.Name} has been added"));
					GoBack();
				}
			}

			GUILayout.EndHorizontal();
			EditorExtends.DrawLine(Color.black);
		}

		/// <summary>
		/// The method displays localization data.
		/// </summary>
		/// <param name="storage"><see cref="Localization"/> repository</param>
		/// <param name="localization"><see cref="Localization"/> data</param>
		/// <param name="languages"><see cref="Language"/> data</param>
		/// <param name="options">Options for displaying fields</param>
		public static void DrawResources(Localization localization, Language[] languages, params GUILayoutOption[] options)
		{
			GUIStyle style = new GUIStyle(EditorStyles.textArea);
			style.wordWrap = true;
			bool isString = typeof(string).IsAssignableFrom(localization.Type);

			for (int i = 0; i < localization.Resources.Count; i++)
			{
				if (isString)
				{
					EditorGUILayout.LabelField(languages[i].ToString());
					localization.Resources[i].Data = EditorGUILayout.TextArea((string)localization.Resources[i].Data, style, options);
				}
				else
				{
					localization.Resources[i].Data = EditorGUILayout.ObjectField(languages[i].ToString(),
						(Object)localization.Resources[i].Data, localization.Type, false, options);
				}
			}
		}

		/// <summary>
		/// Method called when the Back button is clicked
		/// </summary>
		public void GoBack()
		{
			OnBackButton?.Invoke();
			scrollPosition = Vector2.zero;
			EditorGUI.FocusTextInControl(null);
		}

		/// <summary>
		/// Handles keystrokes
		/// </summary>
		/// <param name="curentEvent"></param>
		private void HandleKeyboard(Event curentEvent)
		{
			if (curentEvent.type == EventType.KeyDown && curentEvent.keyCode == KeyCode.LeftArrow)
			{
				GoBack();
				curentEvent.Use();
			}
		}
	}
}