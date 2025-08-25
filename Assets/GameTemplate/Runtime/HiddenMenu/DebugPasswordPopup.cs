using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameTemplate.Runtime.HiddenMenu
{
    public class DebugPasswordPopup : MonoBehaviour
    {
        [SerializeField] private Button btnClose;
        [SerializeField] private Button btnEnter;
        [SerializeField] private TMP_InputField inputField;
        
        private void Awake()
        {
            btnClose.onClick.AddListener(ClosePopup);
            btnEnter.onClick.AddListener(OnEnterClicked);
        }
        
        private void ClosePopup()
        {
            Destroy(gameObject);
        }
        
        private void OnEnterClicked()
        {
            string password = inputField.text;
            if (password == HiddenMenuManager.Instance.Config.Password)
            {
                Debug.Log("Debug mode activated!");
                ClosePopup();
                HiddenMenuManager.Instance.OpenHiddenMenu(password);
            }
            else
            {
                Debug.LogWarning("Incorrect password!");
                inputField.text = "";
            }
        }
    }
}