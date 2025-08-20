using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameTemplate.Runtime
{
    public class DelayMonobehavior : MonoBehaviour
    {
        [SerializeField] private float time;
        [SerializeField] private bool delayOnStart;
        public UnityEvent OnDelayCompleted;
        private void Start()
        {
            if (delayOnStart)
            {
                StarDelay();
            }
        }

        public void StarDelay()
        {
            StartCoroutine(DelayCoroutine());
        }
        
        private IEnumerator DelayCoroutine()
        {
            yield return new WaitForSeconds(time);
            OnDelayCompleted?.Invoke();
        }
    }
}
