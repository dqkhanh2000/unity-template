using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameTemplate.Runtime.ScreenManager
{
    [CreateAssetMenu(fileName = "Screen", menuName = "GameTemplate/Screen/ScreenConfig")]
    public class ScreenConfig : ScriptableObject
    {
        [SerializeField] private List<string> screenKeyList;

        public List<string> ScreenKeyList
        {
            get => screenKeyList;
            set => screenKeyList = value;
        }
    }
}