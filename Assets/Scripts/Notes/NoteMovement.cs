using UnityEngine;
using TapRythm.Interfaces;

namespace TapRythm.Notes
{
    public class NoteMovement : MonoBehaviour, IMovable
    {
        [Header("Movement Settings")]
        [SerializeField] private float _speedZ = 2f;
        [SerializeField] private float _startZ = 10f;
        [SerializeField] private float _targetZ = 2f;

        private bool _hasReachedTarget;

        public void Init(float speed, float startZ, float targetZ)
        {
            _speedZ = speed;
            _startZ = startZ;
            _targetZ = targetZ;

            Vector3 pos = transform.position;
            pos.z = _startZ;
            transform.position = pos;
        }

        public void Move(float speedMultiplier = 1f)
        {
            if (_hasReachedTarget)
            {
                return;
            }
            transform.Translate(Vector3.back * _speedZ * speedMultiplier * Time.deltaTime);
        }

        public bool HasReachedTarget()
        {
            if (!_hasReachedTarget && transform.position.z <= _targetZ - 2f)
            {
                _hasReachedTarget = true;
            }
            return _hasReachedTarget;
        }

        public float GetCurrentZ() => transform.position.z;
        public float GetTargetZ() => _targetZ;
    }
}