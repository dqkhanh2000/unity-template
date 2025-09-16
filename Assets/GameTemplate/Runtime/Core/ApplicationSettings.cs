using UnityEngine;

namespace GameTemplate.Runtime.Core
{
    [CreateAssetMenu(fileName = "ApplicationSettings", menuName = "GameTemplate/Application Settings")]
    public class ApplicationSettings : ScriptableObject
    {
        [Header("Performance Settings")]
        [SerializeField] private int targetFPS = 60;
        
        [Header("Currency Settings")]
        [SerializeField] private bool enableGem = false;
        [SerializeField] private bool enableDiamond = false;
        [SerializeField] private int coinToGemRatio = 10;
        [SerializeField] private int gemToDiamondRatio = 10;
        
        // Properties
        public int TargetFPS => targetFPS;
        public bool EnableGem => enableGem;
        public bool EnableDiamond => enableDiamond;
        public int CoinToGemRatio => coinToGemRatio;
        public int GemToDiamondRatio => gemToDiamondRatio;
        
        private void OnValidate()
        {
            // Ensure ratios are positive
            coinToGemRatio = Mathf.Max(1, coinToGemRatio);
            gemToDiamondRatio = Mathf.Max(1, gemToDiamondRatio);
            
            // Ensure target FPS is reasonable
            targetFPS = Mathf.Clamp(targetFPS, 30, 120);
        }
    }
} 