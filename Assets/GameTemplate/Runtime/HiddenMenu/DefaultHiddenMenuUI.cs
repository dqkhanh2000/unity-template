using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GameTemplate.Runtime.Core;
using GameTemplate.Runtime.HiddenMenu;

namespace GameTemplate.Runtime.HiddenMenu
{
    public class DefaultHiddenMenuUI : MonoBehaviour
    {
        [SerializeField] private Button btnClose;
        [SerializeField] private Button btnClearPlayerData;
        [SerializeField] private Button btnSetLevel;
        [SerializeField] private Button btnAddCoins;
        [SerializeField] private Button btnAddGems;
        [SerializeField] private Button btnAddDiamonds;
        [SerializeField] private TMP_Dropdown levelDropdown;

        private LevelData selectedLevel;

        private void Awake()
        {
            PopulateLevelDropdown();
            btnClose.onClick.AddListener(CloseMenu);
            btnClearPlayerData.onClick.AddListener(HiddenMenuManager.Instance.ClearPlayerData);
            btnSetLevel.onClick.AddListener(() => HiddenMenuManager.Instance.SetPlayerLevel(selectedLevel));
            btnAddCoins.onClick.AddListener(AddCoins);
            btnAddGems.onClick.AddListener(AddGems);
            btnAddDiamonds.onClick.AddListener(AddDiamonds);
            levelDropdown.onValueChanged.AddListener(OnLevelDropdownChanged);

            var config = HiddenMenuManager.Instance.Config;
            if (GameManager.Instance.ApplicationSettings.EnableDiamond && config.EnableDiamonds)
            {
                btnAddDiamonds.gameObject.SetActive(true);
            }
            else
            {
                btnAddDiamonds.gameObject.SetActive(false);
            }
            
            if (GameManager.Instance.ApplicationSettings.EnableGem && config.EnableGems)
            {
                btnAddGems.gameObject.SetActive(true);
            }
            else
            {
                btnAddGems.gameObject.SetActive(false);
            }
            
            if(config.EnableCoins)
            {
                btnAddCoins.gameObject.SetActive(true);
            }
            else
            {
                btnAddCoins.gameObject.SetActive(false);
            }

            if (config.EnableLevelDebug)
            {
                btnSetLevel.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                btnSetLevel.transform.parent.gameObject.SetActive(false);
            }
        }
        
        void PopulateLevelDropdown()
        {
            if (!LevelManager.Instance)
            {
                Debug.LogWarning("[DefaultHiddenMenuUI] LevelManager instance not found in scene!");
                return;
            }
            selectedLevel = LevelManager.Instance.CurrentLevelData;
            levelDropdown.options.Clear();
            foreach (var level in LevelManager.Instance.LevelDataCollection.levels)
            {
                levelDropdown.options.Add(new TMP_Dropdown.OptionData(level.LevelName));
            }
            
            int currentLevelIndex = levelDropdown.options.FindIndex(option => option.text == selectedLevel.LevelName);
            if (currentLevelIndex >= 0)
            {
                levelDropdown.value = currentLevelIndex;
            }
            else
            {
                levelDropdown.value = 0;
                selectedLevel = LevelManager.Instance.LevelDataCollection.levels[0];
            }
        }
        
        void CloseMenu()
        {
            Destroy(gameObject);
        }
        
        
        
        void AddCoins()
        {
            GameManager.Instance.PlayerData.currency.AddCoin(1000);
        }
        
        void AddGems()
        {
            GameManager.Instance.PlayerData.currency.AddGem(1000);
        }
        
        void AddDiamonds()
        {
            GameManager.Instance.PlayerData.currency.AddDiamond(1000);
        }
        
        void OnLevelDropdownChanged(int index)
        {
            selectedLevel = LevelManager.Instance.LevelDataCollection.levels[index];
        }

        private void OnDestroy()
        {
            HiddenMenuManager.Instance.IsHiddenMenuOpen = false;
        }
    }
} 