using UnityEngine;

namespace GameTemplate.Runtime.Core.Currencies
{
    public static class CurrencySystem
    {
        private static ApplicationSettings _settings;
        
        public static void Initialize(ApplicationSettings settings)
        {
            _settings = settings;
        }
        
        // Convert coin to gem
        public static int ConvertCoinToGem(int coinAmount)
        {
            if (_settings == null || !_settings.EnableGem) return 0;
            return coinAmount / _settings.CoinToGemRatio;
        }
        
        // Convert gem to diamond
        public static int ConvertGemToDiamond(int gemAmount)
        {
            if (_settings == null || !_settings.EnableDiamond) return 0;
            return gemAmount / _settings.GemToDiamondRatio;
        }
        
        // Convert gem to coin
        public static int ConvertGemToCoin(int gemAmount)
        {
            if (_settings == null || !_settings.EnableGem) return 0;
            return gemAmount * _settings.CoinToGemRatio;
        }
        
        // Convert diamond to gem
        public static int ConvertDiamondToGem(int diamondAmount)
        {
            if (_settings == null || !_settings.EnableDiamond) return 0;
            return diamondAmount * _settings.GemToDiamondRatio;
        }
        
        // Check if gem is enabled
        public static bool IsGemEnabled => _settings != null && _settings.EnableGem;
        
        // Check if diamond is enabled
        public static bool IsDiamondEnabled => _settings != null && _settings.EnableDiamond;
    }
} 