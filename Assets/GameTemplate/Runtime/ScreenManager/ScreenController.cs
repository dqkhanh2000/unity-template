using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameTemplate.Runtime.ScreenManager
{
    public class ScreenController : MonoBehaviour
    {
        [SerializeField] private string key;
        [SerializeField] private ScreenManager screenManager;

        public UnityEvent Activated;
        public UnityEvent Deactivated;

        private bool isActive;
        public bool IsActive
        {
            get => isActive;
        }
        public string Key => key;

        private void Start()
        {
            isActive = gameObject.activeSelf;
        }

        public void Active()
        {
            isActive = true;
            gameObject.SetActive(true);
            Activated?.Invoke();
        }

        public void Deactive()
        {
            if (isActive)
            {
                isActive = false;
                gameObject.SetActive(false);
                Deactivated?.Invoke();
            }

        }
    }
}