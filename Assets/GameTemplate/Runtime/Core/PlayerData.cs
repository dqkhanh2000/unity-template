using System;
using System.Collections.Generic;
using GameTemplate.Runtime.Core.Currencies;
using UnityEngine;
using Object = System.Object;

namespace GameTemplate.Runtime.Core
{
    /// <summary>
    /// Player data containing progress, settings, and unlock information.
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        [Header("Level Progress")]
        public int currentLevelId = 0;
        public int highestLevelCompleted = -1;
        
        [Header("Unlocked Levels")]
        public List<int> unlockedLevelIds = new List<int> { 0 }; // Start with first level unlocked
        
        [Header("Game Statistics")]
        public int totalPlayTime = 0; // in seconds
        public int totalGamesPlayed = 0;
        public int totalGamesWon = 0;
        public int totalGamesLost = 0;
        
        [Header("Currency")]
        public CurrencyData currency = new();
        
        [Header("Settings")]
        public float masterVolume = 1.0f;
        public float musicVolume = 0.8f;
        public float sfxVolume = 0.8f;
        public bool vibrationEnabled = true;
        public bool musicEnabled = true;
        public bool sfxEnabled = true;
        
        [Header("Additional Data")]
        public Dictionary<string, Object> additionalData = new Dictionary<string, Object>();
        
        [Header("Meta")]
        public string lastSaveDate = "";
        public string gameVersion = "1.0.0";
        
        /// <summary>
        /// Creates a new PlayerData instance with default values.
        /// </summary>
        public PlayerData()
        {
            lastSaveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            // Ensure first level is unlocked
            if (!unlockedLevelIds.Contains(0))
            {
                unlockedLevelIds.Add(0);
            }
        }
        
        /// <summary>
        /// Gets the current level ID.
        /// </summary>
        public int CurrentLevelId => currentLevelId;
        
        /// <summary>
        /// Gets the highest level completed by the player.
        /// </summary>
        public int HighestLevelCompleted => highestLevelCompleted;
        
        /// <summary>
        /// Gets the total number of levels unlocked.
        /// </summary>
        public int TotalLevelsUnlocked => unlockedLevelIds.Count;
        
        /// <summary>
        /// Gets all unlocked level IDs.
        /// </summary>
        public List<int> UnlockedLevelIds => new List<int>(unlockedLevelIds);
        
        public Dictionary<string, Object> AdditionalData => additionalData;
        
        /// <summary>
        /// Checks if a specific level is completed.
        /// </summary>
        /// <param name="levelId">The level ID to check</param>
        /// <returns>True if the level is completed, false otherwise</returns>
        public bool IsLevelCompleted(int levelId)
        {
            return levelId <= highestLevelCompleted;
        }
        
        /// <summary>
        /// Checks if a specific level is unlocked.
        /// </summary>
        /// <param name="levelId">The level ID to check</param>
        /// <returns>True if the level is unlocked, false otherwise</returns>
        public bool IsLevelUnlocked(int levelId)
        {
            return unlockedLevelIds.Contains(levelId);
        }
        
        /// <summary>
        /// Marks a level as completed and unlocks the next level.
        /// </summary>
        /// <param name="levelId">The level ID to mark as completed</param>
        public void CompleteLevel(int levelId)
        {
            if (levelId > highestLevelCompleted)
            {
                highestLevelCompleted = levelId;
                
                // Unlock the next level
                UnlockLevel(levelId + 1);
            }
        }
        
        /// <summary>
        /// Unlocks a specific level.
        /// </summary>
        /// <param name="levelId">The level ID to unlock</param>
        public void UnlockLevel(int levelId)
        {
            if (!unlockedLevelIds.Contains(levelId))
            {
                unlockedLevelIds.Add(levelId);
                unlockedLevelIds.Sort(); // Keep the list sorted
            }
        }
        
        /// <summary>
        /// Unlocks multiple levels at once.
        /// </summary>
        /// <param name="levelIds">Array of level IDs to unlock</param>
        public void UnlockLevels(params int[] levelIds)
        {
            foreach (var levelId in levelIds)
            {
                UnlockLevel(levelId);
            }
        }
        
        /// <summary>
        /// Locks a specific level (removes it from unlocked list).
        /// </summary>
        /// <param name="levelId">The level ID to lock</param>
        public void LockLevel(int levelId)
        {
            if (levelId != 0) // Don't lock the first level
            {
                unlockedLevelIds.Remove(levelId);
            }
        }
        
        /// <summary>
        /// Sets the current level ID.
        /// </summary>
        /// <param name="levelId">The level ID to set</param>
        public void SetCurrentLevel(int levelId)
        {
            currentLevelId = Mathf.Max(0, levelId);
        }
        
        /// <summary>
        /// Updates the last save date to the current time.
        /// </summary>
        public void UpdateSaveDate()
        {
            lastSaveDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        
        /// <summary>
        /// Gets the win rate as a percentage.
        /// </summary>
        /// <returns>The win rate percentage (0-100)</returns>
        public float GetWinRate()
        {
            if (totalGamesPlayed == 0) return 0f;
            return (float)totalGamesWon / totalGamesPlayed * 100f;
        }
        
        /// <summary>
        /// Gets the formatted play time as a string.
        /// </summary>
        /// <returns>Formatted play time string (e.g., "2h 30m 15s")</returns>
        public string GetFormattedPlayTime()
        {
            int hours = totalPlayTime / 3600;
            int minutes = (totalPlayTime % 3600) / 60;
            int seconds = totalPlayTime % 60;
            
            if (hours > 0)
                return $"{hours}h {minutes}m {seconds}s";
            if (minutes > 0)
                return $"{minutes}m {seconds}s";
            return $"{seconds}s";
        }

        #region additional data
        
        /// <summary>
        /// Sets the additional data dictionary.
        /// </summary>
        /// <param name="key">The key of dictionary</param>
        /// <param name="value">The value of dictionary</param>
        public void SetAdditionalData(string key, Object value)
        {
            if (additionalData.ContainsKey(key))
            {
                additionalData[key] = value;
            }
            else
            {
                additionalData.Add(key, value);
            }
        }
        /// <summary>
        /// Gets the additional data dictionary.
        /// </summary>
        /// <param name="key">The key of dictionary</param>
        public Object GetAdditionalData(string key)
        {
            return additionalData.GetValueOrDefault(key);
        }

        #endregion
        
        #region Settings Methods
        
        /// <summary>
        /// Sets the master volume.
        /// </summary>
        /// <param name="volume">Volume level (0-1)</param>
        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
        }
        
        /// <summary>
        /// Sets the music volume.
        /// </summary>
        /// <param name="volume">Volume level (0-1)</param>
        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
        }
        
        /// <summary>
        /// Sets the SFX volume.
        /// </summary>
        /// <param name="volume">Volume level (0-1)</param>
        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
        }
        
        /// <summary>
        /// Enables or disables vibration.
        /// </summary>
        /// <param name="enabled">Whether vibration is enabled</param>
        public void SetVibrationEnabled(bool enabled)
        {
            vibrationEnabled = enabled;
        }
        
        /// <summary>
        /// Enables or disables music.
        /// </summary>
        /// <param name="enabled">Whether music is enabled</param>
        public void SetMusicEnabled(bool enabled)
        {
            musicEnabled = enabled;
        }
        
        /// <summary>
        /// Enables or disables SFX.
        /// </summary>
        /// <param name="enabled">Whether SFX is enabled</param>
        public void SetSFXEnabled(bool enabled)
        {
            sfxEnabled = enabled;
        }
        
        /// <summary>
        /// Gets the effective music volume (master * music).
        /// </summary>
        /// <returns>Effective music volume</returns>
        public float GetEffectiveMusicVolume()
        {
            return musicEnabled ? masterVolume * musicVolume : 0f;
        }
        
        /// <summary>
        /// Gets the effective SFX volume (master * sfx).
        /// </summary>
        /// <returns>Effective SFX volume</returns>
        public float GetEffectiveSFXVolume()
        {
            return sfxEnabled ? masterVolume * sfxVolume : 0f;
        }
        
        #endregion
        
        #region Currency Methods
        
        /// <summary>
        /// Gets the currency data.
        /// </summary>
        /// <returns>Currency data instance</returns>
        public CurrencyData GetCurrency() => currency;
        
        /// <summary>
        /// Adds coin to player's currency.
        /// </summary>
        /// <param name="amount">Amount of coin to add</param>
        public void AddCoin(int amount)
        {
            currency.AddCoin(amount);
        }
        
        /// <summary>
        /// Adds gem to player's currency.
        /// </summary>
        /// <param name="amount">Amount of gem to add</param>
        public void AddGem(int amount)
        {
            currency.AddGem(amount);
        }
        
        /// <summary>
        /// Adds diamond to player's currency.
        /// </summary>
        /// <param name="amount">Amount of diamond to add</param>
        public void AddDiamond(int amount)
        {
            currency.AddDiamond(amount);
        }
        
        /// <summary>
        /// Spends coin from player's currency.
        /// </summary>
        /// <param name="amount">Amount of coin to spend</param>
        /// <returns>True if successful, false if insufficient funds</returns>
        public bool SpendCoin(int amount)
        {
            return currency.SpendCoin(amount);
        }
        
        /// <summary>
        /// Spends gem from player's currency.
        /// </summary>
        /// <param name="amount">Amount of gem to spend</param>
        /// <returns>True if successful, false if insufficient funds</returns>
        public bool SpendGem(int amount)
        {
            return currency.SpendGem(amount);
        }
        
        /// <summary>
        /// Spends diamond from player's currency.
        /// </summary>
        /// <param name="amount">Amount of diamond to spend</param>
        /// <returns>True if successful, false if insufficient funds</returns>
        public bool SpendDiamond(int amount)
        {
            return currency.SpendDiamond(amount);
        }
        
        /// <summary>
        /// Checks if player can afford the specified costs.
        /// </summary>
        /// <param name="coinCost">Coin cost</param>
        /// <param name="gemCost">Gem cost (optional)</param>
        /// <param name="diamondCost">Diamond cost (optional)</param>
        /// <returns>True if player can afford, false otherwise</returns>
        public bool CanAfford(int coinCost, int gemCost = 0, int diamondCost = 0)
        {
            return currency.CanAfford(coinCost, gemCost, diamondCost);
        }
        
        /// <summary>
        /// Converts coin to gem.
        /// </summary>
        /// <param name="coinAmount">Amount of coin to convert</param>
        /// <returns>True if conversion successful, false otherwise</returns>
        public bool ConvertCoinToGem(int coinAmount)
        {
            return currency.ConvertCoinToGem(coinAmount);
        }
        
        /// <summary>
        /// Converts gem to diamond.
        /// </summary>
        /// <param name="gemAmount">Amount of gem to convert</param>
        /// <returns>True if conversion successful, false otherwise</returns>
        public bool ConvertGemToDiamond(int gemAmount)
        {
            return currency.ConvertGemToDiamond(gemAmount);
        }
        
        #endregion
    }
} 