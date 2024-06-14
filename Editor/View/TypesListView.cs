using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace EasyAssetsLocalize
{
    /// <summary>
    /// Class for displaying a list of types for localization.
    /// </summary>
    internal class TypesListView : ReorderableList
    {
        private IStorage Storage { get; }
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="storage"><see cref="Localization"/> repository</param>
        public TypesListView(IStorage storage) : base(null, typeof(System.Type), false, true, true, true)
        {
            Storage = storage;
            list = GetListFromArray(Storage.Localizations);
            drawHeaderCallback = DrawHeader;
            drawElementCallback = DrawTypeItem;
            onAddCallback = AddTypeComponent;
            onRemoveCallback = RemoveTypeComponent;
        }

        /// <summary>
        /// Method to initialize a list.
        /// </summary>
        /// <param name="array"></param>
        /// <returns>List of resource types for localization</returns>
        private List<object> GetListFromArray(Localization[] array)
        {
            return new List<object>(from l in array.Where(i => i.IsDefault) select l.Type);
        }

        /// <summary>
        /// Header render method.
        /// </summary>
        /// <param name="position"><see cref="Rect"/> for rendering</param>
        private void DrawHeader(Rect position)
        {
            EditorGUI.LabelField(position, "Types");
        }

        /// <summary>
        /// Method for display list item.
        /// </summary>
        /// <param name="position"><see cref="Rect"/> for rendering</param>
        /// <param name="index">Sequence number of the element in the list</param>
        /// <param name="isActive">Whether the list item is active</param>
        /// <param name="isFocused">Whether the list item has focus</param>
        private void DrawTypeItem(Rect position, int index, bool isActive, bool isFocused)
        {
            if(list[index] is System.Type type)
            {
                EditorGUI.LabelField(position, type.GetContent());
            }
            else
            {
                NewValueField(position, index);
            }
        }

        /// <summary>
        /// Method for adding a new <see cref="System.Type"/> to the list.
        /// </summary>
        /// <param name="position"><see cref="Rect"/> for rendering</param>
        /// <param name="index">Sequence number of the element in the list</param>
        private void NewValueField(Rect position, int index)
        {
            EditorGUI.BeginChangeCheck();
            list[index] = EditorGUI.ObjectField(position, (Object)list[index], typeof(Object), false);
            if (EditorGUI.EndChangeCheck())
            {
                if (list[index] != null)
                {
                    if(Storage.GetDefaultLocalization(list[index].GetType()) == null)
                    {
                        LocalizationBuilder.CreateComponent(Storage, list[index]);
                        list = GetListFromArray(Storage.Localizations);
                    }
                    else
                    {
                        throw new System.ArgumentException("The localization of objects " +
                            $"<color=green>{list[index].GetType()}</color> is already in storage.");
                    }
                }
            }
        }

        /// <summary>
        /// Adding a localization component to the scene.
        /// </summary>
        /// <param name="reorderable">Link to display list</param>
        private void AddTypeComponent(ReorderableList reorderable)
        {
            reorderable.list.Add(null);
            reorderable.index = list.Count - 1;
        }

        /// <summary>
        /// Removing the localization component from the scene.
        /// </summary>
        /// <param name="reorderable">Link to display list</param>
        private void RemoveTypeComponent(ReorderableList reorderable)
        {
            if(reorderable.list[reorderable.index] is System.Type type)
            {
                if(type.Equals(typeof(string)))
                {
                    Debug.LogError("Due to the complexity of adding a \"string\" type, removing it is prohibited.");
                    return;
                }
                else if(EditorExtends.DeleteConfirmation(type.Name))
                {
                    Storage.RemoveAll(type);
                    //LocalizationBuilder.RemoveComponent(type);
                }
                else { return; }
            }
            reorderable.list.RemoveAt(reorderable.index--);
        }
    }
}