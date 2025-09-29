using System.Threading.Tasks;
using GameTemplate.Runtime.Core;
using UnityEngine;

namespace GameTemplate.Example
{
    public class BasicLevelLoader : LevelLoader
    {
        public override Task<Level> LoadLevel(LevelData levelData, Transform levelContainer = null)
        {
            // Create a new Level instance
            Level level = new GameObject("Level_" + levelData.LevelName).AddComponent<Level>();

            level.transform.parent = levelContainer;
            level.Initialize(levelData);
            
            // Return the loaded level
            return Task.FromResult(level);
        }
    }
}