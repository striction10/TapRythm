using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace TapRythm.Managers
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager Instance { get; private set; }

        public event Action<Vector2> OnTouchPerformed;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Update()
        {
            if (Touchscreen.current?.primaryTouch.press.wasPressedThisFrame == true)
            {
                Vector2 touchPos = Touchscreen.current.primaryTouch.position.ReadValue();
                OnTouchPerformed?.Invoke(touchPos);
            }
        }
    }
}