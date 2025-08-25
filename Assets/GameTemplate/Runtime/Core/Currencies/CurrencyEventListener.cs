using UnityEngine;
using GameTemplate.Runtime.EventSystem;

namespace GameTemplate.Runtime.Core.Currencies
{
    /// <summary>
    /// Listener for currency events
    /// </summary>
    public class CurrencyEventListener : MonoBehaviourEventListener<CurrencyEvent>
    {
        [Header("Currency Event Settings")]
        [SerializeField] private CurrencyEvent.CurrencyType listenToCurrency = CurrencyEvent.CurrencyType.Coin;
        [SerializeField] private CurrencyEvent.ChangeType listenToChangeType = CurrencyEvent.ChangeType.Add;
        
        // Events that can be subscribed to from Unity Inspector
        [Header("Unity Events")]
        public UnityEngine.Events.UnityEvent<int, int> onCurrencyChanged; // oldValue, newValue
        public UnityEngine.Events.UnityEvent<int> onCurrencyAmountChanged; // amount
        public UnityEngine.Events.UnityEvent onCurrencyEventTriggered;
        
        public override void OnEventTriggered(CurrencyEvent currencyEvent)
        {
            // Check if we should listen to this currency and change type
            if (currencyEvent.Currency == listenToCurrency && currencyEvent.Change == listenToChangeType)
            {
                // Trigger Unity events
                onCurrencyChanged?.Invoke(currencyEvent.OldValue, currencyEvent.NewValue);
                onCurrencyAmountChanged?.Invoke(currencyEvent.Amount);
                onCurrencyEventTriggered?.Invoke();
                    
                // Log the event
                Debug.Log($"Currency Event: {currencyEvent.Currency} {currencyEvent.Change} - " +
                          $"Old: {currencyEvent.OldValue}, New: {currencyEvent.NewValue}, Amount: {currencyEvent.Amount}");
            }
        }
        
        /// <summary>
        /// Sets which currency type to listen to
        /// </summary>
        /// <param name="currencyType">Currency type to listen to</param>
        public void SetListenToCurrency(CurrencyEvent.CurrencyType currencyType)
        {
            listenToCurrency = currencyType;
        }
        
        /// <summary>
        /// Sets which change type to listen to
        /// </summary>
        /// <param name="changeType">Change type to listen to</param>
        public void SetListenToChangeType(CurrencyEvent.ChangeType changeType)
        {
            listenToChangeType = changeType;
        }
        
        /// <summary>
        /// Gets the current currency value from PlayerData
        /// </summary>
        /// <returns>Current currency value</returns>
        public int GetCurrentCurrencyValue()
        {
            if (GameManager.Instance?.PlayerData?.GetCurrency() == null) return 0;
            
            var currency = GameManager.Instance.PlayerData.GetCurrency();
            switch (listenToCurrency)
            {
                case CurrencyEvent.CurrencyType.Coin:
                    return currency.Coin;
                case CurrencyEvent.CurrencyType.Gem:
                    return currency.Gem;
                case CurrencyEvent.CurrencyType.Diamond:
                    return currency.Diamond;
                default:
                    return 0;
            }
        }
    }
} 