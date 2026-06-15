using UnityEngine;
using TapRythm.Notes;

namespace TapRythm.Managers
{
    public class NoteHitProcessor : MonoBehaviour
    {
        [SerializeField] private LayerMask _noteLayerMask;
        private Camera _mainCamera;
        
        private void Start()
        {
            UpdateCameraReference();
            
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnTouchPerformed += OnTouchPerformed;
            }
        }
        
        private void OnDestroy()
        {
            if (InputManager.Instance != null)
                InputManager.Instance.OnTouchPerformed -= OnTouchPerformed;
        }
        
        private void OnEnable()
        {
            UpdateCameraReference();
        }
        
        private void UpdateCameraReference()
        {
            _mainCamera = Camera.main;
        }
        
        private void Update()
        {
            if (_mainCamera == null)
            {
                UpdateCameraReference();
            }
        }
        
        private void ProcessTap(Vector2 screenPos)
        {
            if (_mainCamera == null)
            {
                UpdateCameraReference();
                if (_mainCamera == null) return;
            }
            
            Ray ray = _mainCamera.ScreenPointToRay(screenPos);
            
            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _noteLayerMask))
            {
                Note note = hit.collider.GetComponent<Note>();
                if (note != null && !note.IsHit)
                {
                    note.OnTap();
                }
            }
        }
        
        public void OnTouchPerformed(Vector2 screenPos)
        {
            ProcessTap(screenPos);
        }
    }
}