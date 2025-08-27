using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameTemplate.Runtime
{
    public class UnityMainThreadDispatcher: MonoBehaviour
    {
        private static UnityMainThreadDispatcher instance;
        private readonly Queue<Action> executionQueue = new Queue<Action>();

        public static UnityMainThreadDispatcher Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("UnityMainThreadDispatcher").AddComponent<UnityMainThreadDispatcher>();
                    DontDestroyOnLoad(instance.gameObject);
                }
                return instance;
            }
        }

        private void Update()
        {
            lock (executionQueue)
            {
                while (executionQueue.Count > 0)
                {
                    executionQueue.Dequeue()?.Invoke();
                }
            }
        }

        public void Enqueue(Action action)
        {
            lock (executionQueue)
            {
                executionQueue.Enqueue(action);
            }
        }
    }
}