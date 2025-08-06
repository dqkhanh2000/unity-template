using UnityEngine;
using UnityEngine.Events;

namespace GameTemplate.Runtime.InputSystem
{
    public class Pickable : MonoBehaviour
    {
        public UnityEvent<Touch> OnPointerDown;
        public UnityEvent<Touch> OnPointerMove;
        public UnityEvent<Touch> OnPointerUp;
        public UnityEvent<Touch> OnPointerExit;
        public void PointerDown(Touch touch)
        {
            OnPointerDown.Invoke(touch);
            Debug.Log("Pointer Down on: " + gameObject.name);
        }

        public void PointerMove(Touch touch)
        {
            OnPointerMove.Invoke(touch);
            Debug.Log("Pointer Move on: " + gameObject.name);
        }

        public void PointerUp(Touch touch)
        {
            OnPointerUp.Invoke(touch);
            Debug.Log("Pointer Up on: " + gameObject.name);
        }

        public void PointerExit(Touch touch)
        {
            OnPointerExit.Invoke(touch);
            Debug.Log("Pointer Exit on: " + gameObject.name);
        }
    }
}
