using GameTemplate.Runtime.EventSystem;

namespace GameTemplate.Runtime.Core.Currencies
{
    /// <summary>
    /// Event data for currency changes
    /// </summary>
    public struct CurrencyEvent : IEvent
    {
        public enum CurrencyType
        {
            Coin,
            Gem,
            Diamond
        }
        
        public enum ChangeType
        {
            Add,
            Spend,
            Change,
            Convert
        }
        
        public CurrencyType Currency { get; private set; }
        public ChangeType Change { get; private set; }
        public int OldValue { get; private set; }
        public int NewValue { get; private set; }
        public int Amount { get; private set; }
        
        public CurrencyEvent(CurrencyType currency, ChangeType change, int oldValue, int newValue, int amount = 0)
        {
            Currency = currency;
            Change = change;
            OldValue = oldValue;
            NewValue = newValue;
            Amount = amount;
        }
        
        public static CurrencyEvent CreateAddEvent(CurrencyType currency, int oldValue, int newValue, int amount)
        {
            return new CurrencyEvent(currency, ChangeType.Add, oldValue, newValue, amount);
        }
        
        public static CurrencyEvent CreateSpendEvent(CurrencyType currency, int oldValue, int newValue, int amount)
        {
            return new CurrencyEvent(currency, ChangeType.Spend, oldValue, newValue, amount);
        }
        
        public static CurrencyEvent CreateChangeEvent(CurrencyType currency, int oldValue, int newValue)
        {
            return new CurrencyEvent(currency, ChangeType.Change, oldValue, newValue);
        }
        
        public static CurrencyEvent CreateConvertEvent(CurrencyType fromCurrency, CurrencyType toCurrency, int fromOldValue, int fromNewValue, int toOldValue, int toNewValue, int amount)
        {
            // For conversion, we'll use the "from" currency as the main currency
            return new CurrencyEvent(fromCurrency, ChangeType.Convert, fromOldValue, fromNewValue, amount);
        }

        public string EventId => $"CurrencyEvent_{Currency}_{Change}_{OldValue}_{NewValue}";
        public bool IsValid  => true;
    }
} 