using System;
using GameTemplate.Runtime.Core.Attributes;
using UnityEngine;

namespace GameTemplate.Runtime.Core
{
    /// <summary>
    /// Basic level data structure containing essential information.
    /// Supports both Resources and Addressables loading methods.
    /// </summary>
    [Serializable]
    public class LevelData
    {
        [Header("Basic Information")]
        public int levelId;
        public string levelName;

        [Space(10)]
        [InfoBox("May be json file or prefab or anything else", InfoBoxType.Info, false)]
        public UnityEngine.Object levelObject;
        
        /// <summary>
        /// Creates a new LevelData instance with basic information.
        /// </summary>
        /// <param name="id">Level ID (index)</param>
        /// <param name="name">Level name</param>
        /// <param name="address">Addressable address for the level data</param>
        /// <param name="resourcePath">Resource path as fallback</param>
        public LevelData(int id, string name, string address = "", string resourcePath = "")
        {
            levelId = id;
            levelName = name;
        }
        
        /// <summary>
        /// Gets the level ID.
        /// </summary>
        public int LevelId => levelId;
        
        /// <summary>
        /// Gets the level name.
        /// </summary>
        public string LevelName => levelName;
        
        /// <summary>
        /// Serializes the level data to JSON.
        /// </summary>
        /// <returns>JSON string representation of the level data</returns>
        public string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
        
        /// <summary>
        /// Deserializes level data from JSON.
        /// </summary>
        /// <param name="json">JSON string to deserialize</param>
        /// <returns>LevelData instance</returns>
        public static LevelData FromJson(string json)
        {
            return JsonUtility.FromJson<LevelData>(json);
        }
        
        /// <summary>
        /// Creates a copy of this level data.
        /// </summary>
        /// <returns>A new LevelData instance with the same values</returns>
        public LevelData Clone()
        {
            return FromJson(ToJson());
        }
    }
} 