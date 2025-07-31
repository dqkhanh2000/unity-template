using System.Threading.Tasks;
using UnityEngine;

namespace KaneTemplate.Core
{
    public abstract class LevelLoader : MonoBehaviour
    {
        public string LoaderName { get; set; } = "Default Level Loader";
        
        public abstract Task<Level> LoadLevel(LevelData levelData, Transform levelContainer = null);
    }
}