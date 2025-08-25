using System;
using UnityEngine;
using GameTemplate.Runtime.EventSystem;

namespace GameTemplate.Runtime.Core.Currencies
{
    [Serializable]
    public class CurrencyData
    {
        [SerializeField] private int coin;
        [SerializeField] private int gem;
        [SerializeField] private int diamond;
        
        // Events
        public event Action<int, int> OnCoinChanged; // oldValue, newValue
        public event Action<int, int> OnGemChanged; // oldValue, newValue
        public event Action<int, int> OnDiamondChanged; // oldValue, newValue
        
        // Properties
        public int Coin
        {
            get => coin;
            set
            {
                if (coin != value)
                {
                    int oldValue = coin;
                    coin = Mathf.Max(0, value);
                    OnCoinChanged?.Invoke(oldValue, coin);
                }
            }
        }
        
        public int Gem
        {
            get => gem;
            set
            {
                if (gem != value)
                {
                    int oldValue = gem;
                    gem = Mathf.Max(0, value);
                    OnGemChanged?.Invoke(oldValue, gem);
                }
            }
        }
        
        public int Diamond
        {
            get => diamond;
            set
            {
                if (diamond != value)
                {
                    int oldValue = diamond;
                    diamond = Mathf.Max(0, value);
                    OnDiamondChanged?.Invoke(oldValue, diamond);
                }
            }
        }
        
        // Constructor
        public CurrencyData(int initialCoin = 0, int initialGem = 0, int initialDiamond = 0)
        {
            coin = Mathf.Max(0, initialCoin);
            gem = Mathf.Max(0, initialGem);
            diamond = Mathf.Max(0, initialDiamond);
        }
        
        // Add currency methods
        public void AddCoin(int amount)
        {
            if (amount > 0)
            {
                int oldValue = coin;
                Coin += amount;
                
                // Trigger currency event
                var currencyEvent = CurrencyEvent.CreateAddEvent(CurrencyEvent.CurrencyType.Coin, oldValue, coin, amount);
                EventManager.TriggerEvent(currencyEvent);
            }
        }
        
        public void AddGem(int amount)
        {
            if (amount > 0 && CurrencySystem.IsGemEnabled)
            {
                int oldValue = gem;
                Gem += amount;
                
                // Trigger currency event
                var currencyEvent = CurrencyEvent.CreateAddEvent(CurrencyEvent.CurrencyType.Gem, oldValue, gem, amount);
                EventManager.TriggerEvent(currencyEvent);
            }
        }
        
        public void AddDiamond(int amount)
        {
            if (amount > 0 && CurrencySystem.IsDiamondEnabled)
            {
                int oldValue = diamond;
                Diamond += amount;
                
                // Trigger currency event
                var currencyEvent = CurrencyEvent.CreateAddEvent(CurrencyEvent.CurrencyType.Diamond, oldValue, diamond, amount);
                EventManager.TriggerEvent(currencyEvent);
            }
        }
        
        // Spend currency methods
        public bool SpendCoin(int amount)
        {
            if (amount > 0 && coin >= amount)
            {
                int oldValue = coin;
                Coin -= amount;
                
                // Trigger currency event
                var currencyEvent = CurrencyEvent.CreateSpendEvent(CurrencyEvent.CurrencyType.Coin, oldValue, coin, amount);
                EventManager.TriggerEvent(currencyEvent);
                return true;
            }
            return false;
        }
        
        public bool SpendGem(int amount)
        {
            if (amount > 0 && CurrencySystem.IsGemEnabled && gem >= amount)
            {
                int oldValue = gem;
                Gem -= amount;
                
                // Trigger currency event
                var currencyEvent = CurrencyEvent.CreateSpendEvent(CurrencyEvent.CurrencyType.Gem, oldValue, gem, amount);
                EventManager.TriggerEvent(currencyEvent);
                return true;
            }
            return false;
        }
        
        public bool SpendDiamond(int amount)
        {
            if (amount > 0 && CurrencySystem.IsDiamondEnabled && diamond >= amount)
            {
                int oldValue = diamond;
                Diamond -= amount;
                
                // Trigger currency event
                var currencyEvent = CurrencyEvent.CreateSpendEvent(CurrencyEvent.CurrencyType.Diamond, oldValue, diamond, amount);
                EventManager.TriggerEvent(currencyEvent);
                return true;
            }
            return false;
        }
        
        // Check if can afford
        public bool CanAfford(int coinCost, int gemCost = 0, int diamondCost = 0)
        {
            return coin >= coinCost && 
                   (gemCost == 0 || (CurrencySystem.IsGemEnabled && gem >= gemCost)) &&
                   (diamondCost == 0 || (CurrencySystem.IsDiamondEnabled && diamond >= diamondCost));
        }
        
        // Convert currency methods
        public bool ConvertCoinToGem(int coinAmount)
        {
            if (!CurrencySystem.IsGemEnabled || coinAmount <= 0 || coin < coinAmount)
                return false;
                
            int gemAmount = CurrencySystem.ConvertCoinToGem(coinAmount);
            if (gemAmount > 0)
            {
                int oldCoinValue = coin;
                int oldGemValue = gem;
                
                SpendCoin(coinAmount);
                AddGem(gemAmount);
                
                // Trigger conversion event
                var currencyEvent = CurrencyEvent.CreateConvertEvent(CurrencyEvent.CurrencyType.Coin, CurrencyEvent.CurrencyType.Gem, 
                    oldCoinValue, coin, oldGemValue, gem, coinAmount);
                EventManager.TriggerEvent(currencyEvent);
                return true;
            }
            return false;
        }
        
        public bool ConvertGemToDiamond(int gemAmount)
        {
            if (!CurrencySystem.IsDiamondEnabled || gemAmount <= 0 || gem < gemAmount)
                return false;
                
            int diamondAmount = CurrencySystem.ConvertGemToDiamond(gemAmount);
            if (diamondAmount > 0)
            {
                int oldGemValue = gem;
                int oldDiamondValue = diamond;
                
                SpendGem(gemAmount);
                AddDiamond(diamondAmount);
                
                // Trigger conversion event
                var currencyEvent = CurrencyEvent.CreateConvertEvent(CurrencyEvent.CurrencyType.Gem, CurrencyEvent.CurrencyType.Diamond, 
                    oldGemValue, gem, oldDiamondValue, diamond, gemAmount);
                EventManager.TriggerEvent(currencyEvent);
                return true;
            }
            return false;
        }
        
        // Reset all currency
        public void Reset()
        {
            Coin = 0;
            Gem = 0;
            Diamond = 0;
        }
        
        // Clone currency data
        public CurrencyData Clone()
        {
            return new CurrencyData(coin, gem, diamond);
        }
    }
} 