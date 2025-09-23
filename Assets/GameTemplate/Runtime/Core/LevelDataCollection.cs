using System;
using UnityEngine;

namespace GameTemplate.Runtime.Core
{
    /// <summary>
    /// ScriptableObject for storing a collection of level data.
    /// This can be used as an alternative to JSON files for level configuration.
    /// </summary>
    [CreateAssetMenu(fileName = "LevelDataCollection", menuName = "GameTemplate/Level Data Collection")]
    public class LevelDataCollection : ScriptableObject
    {
        [Header("Level Collection")]
        public LevelData[] levels = Array.Empty<LevelData>();
        
        [Header("Collection Info")]
        [SerializeField] private string collectionName = "Default Level Collection";
        [SerializeField] private string description = "A collection of game levels";
        [SerializeField] private string version = "1.0.0";
        
        // Properties
        public LevelData[] Levels => levels;
        public string CollectionName => collectionName;
        public string Description => description;
        public string Version => version;
        public int TotalLevels => levels?.Length ?? 0;
        
        /// <summary>
        /// Gets a level by its index.
        /// </summary>
        /// <param name="index">The level index (0-based)</param>
        /// <returns>The level data, or null if index is out of range</returns>
        public LevelData GetLevel(int index)
        {
            if (levels == null || index < 0 || index >= levels.Length)
                return null;
                
            return levels[index];
        }
        
        /// <summary>
        /// Gets a level by its ID.
        /// </summary>
        /// <param name="levelId">The level ID to find</param>
        /// <returns>The level data, or null if not found</returns>
        public LevelData GetLevelById(int levelId)
        {
            if (levels == null || levelId < 0)
                return null;
                
            foreach (var level in levels)
            {
                if (level != null && level.LevelId == levelId)
                    return level;
            }

            // ìf not found, return by level index
            return levels.Length > levelId ? levels[levelId] : null;
        }
        
        /// <summary>
        /// Gets a level by its name.
        /// </summary>
        /// <param name="levelName">The level name to find</param>
        /// <returns>The level data, or null if not found</returns>
        public LevelData GetLevelByName(string levelName)
        {
            if (levels == null || string.IsNullOrEmpty(levelName))
                return null;
                
            foreach (var level in levels)
            {
                if (level != null && level.LevelName == levelName)
                    return level;
            }
            
            return null;
        }
        
        /// <summary>
        /// Gets the next level after the given level index.
        /// </summary>
        /// <param name="currentLevelIndex">The current level index</param>
        /// <returns>The next level data, or null if there is no next level</returns>
        public LevelData GetNextLevel(int currentLevelIndex)
        {
            var nextIndex = currentLevelIndex + 1;
            return GetLevel(nextIndex);
        }
        
        /// <summary>
        /// Gets the previous level before the given level index.
        /// </summary>
        /// <param name="currentLevelIndex">The current level index</param>
        /// <returns>The previous level data, or null if there is no previous level</returns>
        public LevelData GetPreviousLevel(int currentLevelIndex)
        {
            var previousIndex = currentLevelIndex - 1;
            return GetLevel(previousIndex);
        }
        
        /// <summary>
        /// Validates the level collection for errors.
        /// </summary>
        /// <returns>Array of validation error messages</returns>
        public string[] ValidateCollection()
        {
            var errors = new System.Collections.Generic.List<string>();
            
            if (levels == null)
            {
                errors.Add("Levels array is null");
                return errors.ToArray();
            }
            
            if (levels.Length == 0)
            {
                errors.Add("No levels in collection");
                return errors.ToArray();
            }
            
            // Check for duplicate IDs
            var usedIds = new System.Collections.Generic.HashSet<int>();
            for (int i = 0; i < levels.Length; i++)
            {
                var level = levels[i];
                if (level == null)
                {
                    errors.Add($"Level at index {i} is null");
                    continue;
                }
                
                if (level.LevelId < 0)
                {
                    errors.Add($"Level at index {i} has invalid ID: {level.LevelId}");
                    continue;
                }
                
                if (!usedIds.Add(level.LevelId))
                {
                    errors.Add($"Duplicate level ID found: {level.LevelId}");
                }
            }
            
            return errors.ToArray();
        }
        
        /// <summary>
        /// Sorts the levels by their index.
        /// </summary>
        public void SortLevelsByIndex()
        {
            if (levels == null || levels.Length <= 1)
                return;
                
            System.Array.Sort(levels, (a, b) => 
            {
                if (a == null && b == null) return 0;
                if (a == null) return 1;
                if (b == null) return -1;
                return a.levelId.CompareTo(b.levelId);
            });
        }
        
        /// <summary>
        /// Creates a copy of this level collection.
        /// </summary>
        /// <returns>A new LevelDataCollection with the same data</returns>
        public LevelDataCollection Clone()
        {
            var clone = CreateInstance<LevelDataCollection>();
            clone.collectionName = this.collectionName;
            clone.description = this.description;
            clone.version = this.version;
            
            if (levels != null)
            {
                clone.levels = new LevelData[levels.Length];
                for (int i = 0; i < levels.Length; i++)
                {
                    if (levels[i] != null)
                    {
                        clone.levels[i] = levels[i].Clone();
                    }
                }
            }
            
            return clone;
        }
        
        /// <summary>
        /// Exports the level collection to JSON.
        /// </summary>
        /// <returns>JSON string representation of the collection</returns>
        public string ToJson()
        {
            var collection = new LevelDataCollectionWrapper
            {
                levels = levels,
                collectionName = collectionName,
                description = description,
                version = version
            };
            
            return JsonUtility.ToJson(collection, true);
        }
        
        /// <summary>
        /// Imports level data from JSON.
        /// </summary>
        /// <param name="json">JSON string to import from</param>
        public void FromJson(string json)
        {
            var collection = JsonUtility.FromJson<LevelDataCollectionWrapper>(json);
            if (collection != null)
            {
                levels = collection.levels;
                collectionName = collection.collectionName;
                description = collection.description;
                version = collection.version;
            }
        }
        
        /// <summary>
        /// Wrapper class for JSON serialization.
        /// </summary>
        [System.Serializable]
        private class LevelDataCollectionWrapper
        {
            public LevelData[] levels;
            public string collectionName;
            public string description;
            public string version;
        }
        
        #if UNITY_EDITOR
        /// <summary>
        /// Editor-only method to add a level to the collection.
        /// </summary>
        /// <param name="level">The level to add</param>
        public void AddLevel(LevelData level)
        {
            if (level == null) return;
            
            var newLevels = new LevelData[levels.Length + 1];
            System.Array.Copy(levels, newLevels, levels.Length);
            newLevels[levels.Length] = level;
            levels = newLevels;
            
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        /// <summary>
        /// Editor-only method to remove a level from the collection.
        /// </summary>
        /// <param name="index">The index of the level to remove</param>
        public void RemoveLevel(int index)
        {
            if (levels == null || index < 0 || index >= levels.Length) return;
            
            var newLevels = new LevelData[levels.Length - 1];
            for (int i = 0, j = 0; i < levels.Length; i++)
            {
                if (i != index)
                {
                    newLevels[j] = levels[i];
                    j++;
                }
            }
            levels = newLevels;
            
            UnityEditor.EditorUtility.SetDirty(this);
        }
        #endif
    }
} 