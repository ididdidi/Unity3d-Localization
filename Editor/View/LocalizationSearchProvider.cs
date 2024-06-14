using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace EasyAssetsLocalize
{
    /// <summary>
    /// Search Localization Data Provider.
    /// </summary>
    internal class LocalizationSearchProvider : ISearchTreeProvider
    {
        private SearchTree searchTree = new SearchTree();
        private IStorage Storage { get; }
        private System.Type Type { get; }
        private Dictionary<System.Type, List<Localization>> types;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="storage">Localization storage</param>
        /// <param name="type">Target type</param>
        public LocalizationSearchProvider(IStorage storage, System.Type type = null)
        {
            Storage = storage;
            Type = type;
        }

        /// <summary>
        /// Creates and returns a search tree.
        /// </summary>
        /// <returns><see cref="SearchTree"/> as a list of localizations</returns>
        public SearchTree GetSearchTree()
        {
            if (Storage == null) { throw new System.ArgumentNullException(nameof(Component)); }

            List<SearchTreeEntry> searchList = new List<SearchTreeEntry>();
            searchList.Add(new SearchTreeGroupEntry(new GUIContent("Localizations")));

            searchList.AddRange(GetItems());
            searchList.AddRange(GetNewItems());

            searchTree.BuildStack(searchList.ToArray());
            return searchTree;
        }

        /// <summary>
        /// Creates a list of <see cref="SearchTreeEntry"/> from list of <see cref="Localization"/>
        /// </summary>
        /// <param name="locals"></param>
        /// <returns>List of localizations as SearchTreeEntry</returns>
        private List<SearchTreeEntry> GetItems()
        {
            // Group by Type
            Dictionary<System.Type, List<SearchTreeEntry>> entries = new Dictionary<System.Type, List<SearchTreeEntry>>();

            var locals = new List<Localization>(Storage.Localizations);
            locals.Sort((tag0, tag1) => tag0.Name.CompareTo(tag1.Name));

            for (int i = 0; i < locals.Count; i++)
            {
                // If the target type is specified and does not match the type for item, then skip this iteration.
                if (Type != null && !Type.IsAssignableFrom(locals[i].Type)) { continue; }

                // If the list for this type has not yet been added to the dictionary, then we create it and add.
                if (!entries.ContainsKey(locals[i].Type)) { entries.Add(locals[i].Type, new List<SearchTreeEntry>()); }
                // Adding the created item to the dictionary
                var content = new GUIContent(locals[i].Type.GetContent());
                content.text = locals[i].Name;
                entries[locals[i].Type].Add(new SearchTreeEntry(content, Type == null ? 2 : 1, locals[i]));
            }

            // Create a list with tree leaves
            List<SearchTreeEntry> searchList = new List<SearchTreeEntry>();
            foreach (var entry in entries)
            {
                // If the target type is not set, create a group for the item type.
                if (Type == null)
                {
                    var cover = new GUIContent(entry.Key.Name, entry.Value[0].Content.image);
                    searchList.Add(new SearchTreeGroupEntry(new GUIContent(entry.Key.Name), 1, cover));
                }
                // Adding items to the end of the list
                searchList.AddRange(entry.Value);
            }

            return searchList;
        }

        /// <summary>
        /// Creates new elements for the list for a specific type or for all.
        /// </summary>
        /// <returns>List of new localizations as SearchTreeEntry</returns>
        private List<SearchTreeEntry> GetNewItems()
        {
            List<SearchTreeEntry> searchList = new List<SearchTreeEntry>();

            // If the target type is not initialized, then create a group for new localizations.
            if (Type == null)
            {
                var cover = new GUIContent("Add New", EditorGUIUtility.IconContent("CreateAddNew@2x").image);
                searchList.Add(new SearchTreeGroupEntry(new GUIContent("New Localization"), 1, cover));
            }

            var icon = EditorGUIUtility.IconContent("CreateAddNew").image;
            var level = 1;

            // Loop through all types of resources for localization.
            var newEntries = new List<SearchTreeEntry>();
            var defaults = (from l in Storage.Localizations.Where(i => i.IsDefault) select l).ToArray();
            for (int i = 0; i < defaults.Length; i++)
            {
                // If the target type is defined.
                if (Type != null) { if (!Type.Equals(defaults[i].Type)) { continue; } } // But this type does not correspond to it - we skip the iteration.
                else { icon = defaults[i].Type.GetContent().image; level = 2; } // Initialize the icon according to the resource type and specify the level as 2.

                // Find the default value for this type, create a new element and add it to the list of localizations.
                var newValue = Storage.GetDefaultLocalization(defaults[i].Type).Clone();
                newValue.Name = $"{newValue.Type.Name} Localization";
                newEntries.Add(new SearchTreeEntry(new GUIContent($"New {newValue.Type.Name} Localization", icon), level, newValue));
            }

            newEntries.Sort((item0, item1) => item0.Name.CompareTo(item1.Name));
            searchList.AddRange(newEntries);
            return searchList;
        }
    }
}