using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Video;

namespace GameTemplate.Runtime
{
    public class TitoLoadingScreen : MonoBehaviour
    {
        [SerializeField] private VideoPlayer videoPlayer;
        
        public UnityEvent VideoFinished;

        private void Awake()
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }

        private void OnDestroy()
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }

        private void OnVideoFinished(VideoPlayer source)
        {
            VideoFinished?.Invoke();
        }
    }
}
