using UnityEngine;
using UnityEngine.Events;
namespace GameTemplate.Runtime.InputSystem
{
    public class TouchInputController : MonoBehaviour
    {
        public UnityEvent<Touch> OnPointerDown;
        public UnityEvent<Touch> OnPointerMove;
        public UnityEvent<Touch> OnPointerUp;

        private bool hasValidTouch;
        private int validTouchFingerId = -1;

        private void Update()
        {
            if (!hasValidTouch)
            {
                HandleInputDown();
                if (hasValidTouch)
                {
                    OnPointerDown.Invoke(GetValidTouch());
                }
            }
            else
            {
                HandleInputMoveUp();
            }
        }

        private void HandleInputDown()
        {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_WEBGL

            #region unity remote codepath

            if (Input.touchCount > 0 && hasValidTouch == false)
            {
                foreach (var touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Began &&
                        !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(touch.fingerId + 1))
                    {
                        hasValidTouch = true;
                        validTouchFingerId = touch.fingerId;
                    }
                }
            }

            #endregion

            if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject() && hasValidTouch == false)
            {
                hasValidTouch = true;
            }
#else
            if (Input.touchCount > 0 && hasValidTouch == false)
            {
                foreach (var touch in Input.touches)
                {
                    if (touch.phase == TouchPhase.Began &&
                        !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-(touch.fingerId + 1)))
                    {
                        hasValidTouch = true;
                        validTouchFingerId = touch.fingerId;
                    }
                }
            }
#endif
        }

        private void HandleInputMoveUp()
        {
            foreach (var touch in Input.touches)
            {
                if (touch.fingerId == validTouchFingerId)
                {
                    if (touch.phase == TouchPhase.Moved)
                    {
                        OnPointerMove.Invoke(touch);
                    }

                    if (touch.phase == TouchPhase.Ended)
                    {
                        OnPointerUp.Invoke(touch);
                        hasValidTouch = false;
                    }

                    return;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                OnPointerUp.Invoke(new Touch
                {
                    position = Input.mousePosition
                });
                hasValidTouch = false;
            }
            else
            {
                {
                    OnPointerMove.Invoke(new Touch
                    {
                        position = Input.mousePosition
                    });
                }
            }
        }

        public Touch GetValidTouch()
        {
            foreach (var touch in Input.touches)
            {
                if (touch.fingerId == validTouchFingerId)
                {
                    return touch;
                }
            }

            return new Touch
            {
                position = Input.mousePosition
            };
        }

        public void ResetState()
        {
            hasValidTouch = false;
        }

        public void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                ResetState();
            }
        }

        private void OnEnable()
        {
            ResetState();
        }
    }
}
