using UnityEngine;
using TapRythm.Enums;

namespace TapRythm.Notes
{
    public class NoteDataComponent : MonoBehaviour
    {
        [Header("Note Data")]
        [SerializeField] private int _laneIndex;
        [SerializeField] private float _hitTime;
        [SerializeField] private NoteType _type;

        private bool _isHit;

        public int LaneIndex => _laneIndex;
        public float HitTime => _hitTime;
        public NoteType Type => _type;
        public bool IsHit => _isHit;

        public void Init(int lane, float hitTime, NoteType type)
        {
            _laneIndex = lane;
            _hitTime = hitTime;
            _type = type;
            _isHit = false;
        }

        public void MarkAsHit()
        {
            _isHit = true;
        }
    }
}