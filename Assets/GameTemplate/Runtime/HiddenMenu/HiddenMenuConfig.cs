using GameTemplate.Runtime.Core.Attributes;
using UnityEngine;

namespace GameTemplate.Runtime.HiddenMenu
{
    [CreateAssetMenu(fileName = "HiddenMenuConfig", menuName = "GameTemplate/HiddenMenu/Config")]
    public class HiddenMenuConfig : ScriptableObject
    {
        [Header("Hidden Menu Settings")]
        [SerializeField] private bool usePassword = true;
        [ShowIf("usePassword")]
        [SerializeField] private string password = "123456";
        [SerializeField] private int clickThreshold = 10;
        [SerializeField] private float clickTimeWindow = 5f;
        [SerializeField] private bool enableLevelDebug = true;
        
        [Header("Currency Types")]
        [SerializeField] private bool enableCoins = true;
        [SerializeField] private bool enableGems = true;
        [SerializeField] private bool enableDiamonds = true;
        
        public string Password => password;
        public int ClickThreshold => clickThreshold;
        public float ClickTimeWindow => clickTimeWindow;
        public bool EnableLevelDebug => enableLevelDebug;
        public bool EnableCoins => enableCoins;
        public bool EnableGems => enableGems;
        public bool EnableDiamonds => enableDiamonds;
        
        public bool UsePassword => usePassword;
    }
} 