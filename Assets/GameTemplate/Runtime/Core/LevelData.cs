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
    public class LevelData : ScriptableObject
    {
        [Header("Basic Information")]
        public int levelId;
        
        /// <summary>
        /// Creates a new LevelData instance with basic information.
        /// </summary>
        /// <param name="id">Level ID (index)</param>
        public LevelData(int id)
        {
            levelId = id;
        }

        protected LevelData()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the level ID.
        /// </summary>
        public int LevelId => levelId;
        
        /// <summary>
        /// Gets the level name.
        /// </summary>
        public virtual string LevelName => $"Level {levelId}";
        
        
        /// <summary>
        /// Serializes the level data to JSON.
        /// </summary>
        /// <returns>JSON string representation of the level data</returns>
        public virtual string ToJson()
        {
            return JsonUtility.ToJson(this, true);
        }
        
        /// <summary>
        /// Deserializes level data from JSON.
        /// </summary>
        /// <param name="json">JSON string to deserialize</param>
        /// <returns>LevelData instance</returns>
        public virtual LevelData FromJson(string json)
        {
            return JsonUtility.FromJson<LevelData>(json);
        }
        
        /// <summary>
        /// Creates a copy of this level data.
        /// </summary>
        /// <returns>A new LevelData instance with the same values</returns>
        public virtual LevelData Clone()
        {
            return FromJson(ToJson());
        }
    }
} 