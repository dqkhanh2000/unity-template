using UnityEngine;
using UnityEngine.UI;
using GameTemplate.Runtime.Core;
using GameTemplate.Runtime.Core.Currencies;

namespace GameTemplate.Example
{
    /// <summary>
    /// Example script demonstrating the currency system
    /// </summary>
    public class CurrencyExample : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Text coinText;
        [SerializeField] private Text gemText;
        [SerializeField] private Text diamondText;
        
        [Header("Currency Display")]
        [SerializeField] private GameObject gemDisplay;
        [SerializeField] private GameObject diamondDisplay;
        
        [Header("Test Buttons")]
        [SerializeField] private Button addCoinButton;
        [SerializeField] private Button addGemButton;
        [SerializeField] private Button addDiamondButton;
        [SerializeField] private Button spendCoinButton;
        [SerializeField] private Button spendGemButton;
        [SerializeField] private Button spendDiamondButton;
        [SerializeField] private Button convertCoinToGemButton;
        [SerializeField] private Button convertGemToDiamondButton;
        
        private PlayerData playerData;
        
        private void Start()
        {
            // Wait for GameManager to be initialized
            if (GameManager.Instance == null)
            {
                Debug.LogWarning("GameManager not found! Waiting for initialization...");
                return;
            }
            
            playerData = GameManager.Instance.PlayerData;
            if (playerData == null)
            {
                Debug.LogError("PlayerData not found!");
                return;
            }
            
            // Setup UI
            SetupUI();
            
            // Subscribe to currency events
            SubscribeToCurrencyEvents();
            
            // Update display
            UpdateCurrencyDisplay();
        }
        
        private void SetupUI()
        {
            // Setup button listeners
            if (addCoinButton) addCoinButton.onClick.AddListener(() => AddCurrency(100, 0, 0));
            if (addGemButton) addGemButton.onClick.AddListener(() => AddCurrency(0, 10, 0));
            if (addDiamondButton) addGemButton.onClick.AddListener(() => AddCurrency(0, 0, 1));
            if (spendCoinButton) spendCoinButton.onClick.AddListener(() => SpendCurrency(50, 0, 0));
            if (spendGemButton) spendGemButton.onClick.AddListener(() => SpendCurrency(0, 5, 0));
            if (spendDiamondButton) spendDiamondButton.onClick.AddListener(() => SpendCurrency(0, 0, 1));
            if (convertCoinToGemButton) convertCoinToGemButton.onClick.AddListener(() => ConvertCoinToGem(100));
            if (convertGemToDiamondButton) convertGemToDiamondButton.onClick.AddListener(() => ConvertGemToDiamond(10));
            
            // Show/hide currency displays based on settings
            if (gemDisplay) gemDisplay.SetActive(CurrencySystem.IsGemEnabled);
            if (diamondDisplay) diamondDisplay.SetActive(CurrencySystem.IsDiamondEnabled);
        }
        
        private void SubscribeToCurrencyEvents()
        {
            // Subscribe to coin changes
            if (playerData.GetCurrency() != null)
            {
                playerData.GetCurrency().OnCoinChanged += OnCoinChanged;
                playerData.GetCurrency().OnGemChanged += OnGemChanged;
                playerData.GetCurrency().OnDiamondChanged += OnDiamondChanged;
            }
        }
        
        private void OnDestroy()
        {
            // Unsubscribe from events
            if (playerData?.GetCurrency() != null)
            {
                playerData.GetCurrency().OnCoinChanged -= OnCoinChanged;
                playerData.GetCurrency().OnGemChanged -= OnGemChanged;
                playerData.GetCurrency().OnDiamondChanged -= OnDiamondChanged;
            }
        }
        
        private void OnCoinChanged(int oldValue, int newValue)
        {
            Debug.Log($"Coin changed from {oldValue} to {newValue}");
            UpdateCurrencyDisplay();
        }
        
        private void OnGemChanged(int oldValue, int newValue)
        {
            Debug.Log($"Gem changed from {oldValue} to {newValue}");
            UpdateCurrencyDisplay();
        }
        
        private void OnDiamondChanged(int oldValue, int newValue)
        {
            Debug.Log($"Diamond changed from {oldValue} to {newValue}");
            UpdateCurrencyDisplay();
        }
        
        private void UpdateCurrencyDisplay()
        {
            if (playerData?.GetCurrency() == null) return;
            
            var currency = playerData.GetCurrency();
            
            if (coinText) coinText.text = $"Coin: {currency.Coin}";
            if (gemText) gemText.text = $"Gem: {currency.Gem}";
            if (diamondText) diamondText.text = $"Diamond: {currency.Diamond}";
        }
        
        private void AddCurrency(int coin, int gem, int diamond)
        {
            if (playerData == null) return;
            
            if (coin > 0) playerData.AddCoin(coin);
            if (gem > 0) playerData.AddGem(gem);
            if (diamond > 0) playerData.AddDiamond(diamond);
            
            Debug.Log($"Added currency - Coin: +{coin}, Gem: +{gem}, Diamond: +{diamond}");
        }
        
        private void SpendCurrency(int coin, int gem, int diamond)
        {
            if (playerData == null) return;
            
            bool success = true;
            if (coin > 0) success &= playerData.SpendCoin(coin);
            if (gem > 0) success &= playerData.SpendGem(gem);
            if (diamond > 0) success &= playerData.SpendDiamond(diamond);
            
            if (success)
            {
                Debug.Log($"Spent currency - Coin: -{coin}, Gem: -{gem}, Diamond: -{diamond}");
            }
            else
            {
                Debug.LogWarning("Insufficient funds to spend currency!");
            }
        }
        
        private void ConvertCoinToGem(int coinAmount)
        {
            if (playerData == null) return;
            
            bool success = playerData.ConvertCoinToGem(coinAmount);
            if (success)
            {
                Debug.Log($"Converted {coinAmount} coin to gem");
            }
            else
            {
                Debug.LogWarning("Failed to convert coin to gem!");
            }
        }
        
        private void ConvertGemToDiamond(int gemAmount)
        {
            if (playerData == null) return;
            
            bool success = playerData.ConvertGemToDiamond(gemAmount);
            if (success)
            {
                Debug.Log($"Converted {gemAmount} gem to diamond");
            }
            else
            {
                Debug.LogWarning("Failed to convert gem to diamond!");
            }
        }
        
        // Public methods for external calls
        public void AddCoin(int amount) => AddCurrency(amount, 0, 0);
        public void AddGem(int amount) => AddCurrency(0, amount, 0);
        public void AddDiamond(int amount) => AddCurrency(0, 0, amount);
        public void SpendCoin(int amount) => SpendCurrency(amount, 0, 0);
        public void SpendGem(int amount) => SpendCurrency(0, amount, 0);
        public void SpendDiamond(int amount) => SpendCurrency(0, 0, amount);
    }
} 