using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace GameTemplate.Runtime.HiddenMenu
{
    /// <summary>
    /// Component to attach to any button to trigger hidden menu
    /// </summary>
    public class HiddenMenuTrigger : MonoBehaviour, IPointerClickHandler
    {
        [Header("Hidden Menu Trigger")]
        [SerializeField] private bool enableTrigger = true;
        [SerializeField] private bool showDebugLogs = false;
        
        private Button button;
        private int clickCount = 0;
        private float lastClickTime = 0f;
        
        private void Awake()
        {
            button = GetComponent<Button>();
            if (button == null)
            {
                Debug.LogWarning($"[HiddenMenuTrigger] No Button component found on {gameObject.name}");
            }
        }
        
        private void Start()
        {
            if (HiddenMenuManager.Instance == null)
            {
                Debug.LogWarning("[HiddenMenuTrigger] HiddenMenuManager not found in scene!");
            }
        }
        
        public void OnPointerClick(PointerEventData eventData)
        {
            if (!enableTrigger || HiddenMenuManager.Instance == null) return;
            
            clickCount++;
            lastClickTime = Time.time;
            
            if (showDebugLogs)
                Debug.Log($"[HiddenMenuTrigger] Click count: {clickCount}");
            
            // Call the hidden menu manager
            HiddenMenuManager.Instance.OnHiddenMenuTrigger();
        }
        
        /// <summary>
        /// Manually trigger hidden menu (can be called from other scripts)
        /// </summary>
        public void ManualTrigger()
        {
            if (HiddenMenuManager.Instance != null)
            {
                HiddenMenuManager.Instance.OnHiddenMenuTrigger();
            }
        }
        
        /// <summary>
        /// Enable or disable the trigger
        /// </summary>
        public void SetTriggerEnabled(bool enabled)
        {
            enableTrigger = enabled;
        }
        
        /// <summary>
        /// Reset click count manually
        /// </summary>
        public void ResetClickCount()
        {
            clickCount = 0;
            lastClickTime = 0f;
        }
    }
} 