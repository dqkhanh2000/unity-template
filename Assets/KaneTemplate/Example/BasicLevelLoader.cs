using System.Threading.Tasks;
using KaneTemplate.Core;
using UnityEngine;

namespace KaneTemplate.Example
{
    public class BasicLevelLoader : LevelLoader
    {
        public override Task<Level> LoadLevel(LevelData levelData, Transform levelContainer = null)
        {
            // Create a new Level instance
            Level level = new GameObject("Level_" + levelData.levelName).AddComponent<Level>();

            level.transform.parent = levelContainer;
            level.Initialize(levelData);
            
            // Return the loaded level
            return Task.FromResult(level);
        }
    }
}