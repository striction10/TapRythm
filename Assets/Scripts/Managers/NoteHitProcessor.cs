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
            _mainCamera = Camera.main;
            InputManager.Instance.OnTouchPerformed += ProcessTap;
        }

        private void ProcessTap(Vector2 screenPos)
        {
            Ray ray = _mainCamera.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit, 100f, _noteLayerMask))
            {
                Note note = hit.collider.GetComponent<Note>();
                if (note != null)
                {
                    note.OnTap();
                }
            }
        }

        private void Oestroy()
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.OnTouchPerformed -= ProcessTap;
            }
        }
    }
}