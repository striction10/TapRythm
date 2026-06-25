using UnityEngine;
using TapRythm.Enums;
using TapRythm.Interfaces;
using TapRythm.UI;
using TapRythm.Managers;

namespace TapRythm.Notes
{
    public class Note : MonoBehaviour, INote
    {
        [Header("Components")]
        [SerializeField] private NoteMovement _movement;
        [SerializeField] private NoteDataComponent _data;
        [SerializeField] private NoteVisual _visual;
        
        private bool _isHolding = false;
        private float _holdStartTime = 0f;
        private float _holdDuration = 0f;
        
        public int LaneIndex => _data.LaneIndex;
        public float HitTime => _data.HitTime;
        public NoteType Type => _data.Type;
        public bool IsHit => _data.IsHit;
        
        private void EnsureComponents()
        {
            if (_movement == null)
            {
                _movement = GetComponent<NoteMovement>();
                if (_movement == null)
                    _movement = gameObject.AddComponent<NoteMovement>();
            }
            
            if (_data == null)
            {
                _data = GetComponent<NoteDataComponent>();
                if (_data == null)
                    _data = gameObject.AddComponent<NoteDataComponent>();
            }
            
            if (_visual == null)
            {
                _visual = GetComponent<NoteVisual>();
                if (_visual == null)
                    _visual = gameObject.AddComponent<NoteVisual>();
            }
        }

        public void Init(int lane, float hitTime, NoteType type, float speed, float startZ, float targetZ, float duration = 0f)
        {
            EnsureComponents();
            
            _data.Init(lane, hitTime, type);
            _movement.Init(speed, startZ, targetZ);
            
            _holdDuration = duration;
            _isHolding = false;
            
            if (_visual != null)
            {
                _visual.SetupForType(type);
            }
        }

        void Update()
        {
            Move();
            if (HasReachedTarget())
            {
                OnMiss();
            }
        }

        public void Move() => _movement.Move();
        public bool HasReachedTarget() => _movement.HasReachedTarget();
        public float GetCurrentZ() => _movement.GetCurrentZ();
        public float GetTargetZ() => _movement.GetTargetZ();

        public void OnTap()
        {
            if (_data.IsHit) return;
            
            if (_data.Type == NoteType.Hold)
            {
                _isHolding = true;
                _holdStartTime = Time.time;
                return;
            }
            
            float accuracy = CalculateAccuracy();
            HitResult result = GetHitResultFromAccuracy(accuracy);
            AccuracyText.Show(result);
            ScoreManager.Instance.AddScore(result);
            
            _data.MarkAsHit();
            _visual?.PlayHitEffect(result);
            DestroyNote();
        }
        
        public void OnHoldRelease()
        {
            if (_data.IsHit || !_isHolding) return;
            
            float holdTime = Time.time - _holdStartTime;
            bool isPerfectHold = Mathf.Abs(holdTime - _holdDuration) < 0.3f;
            
            HitResult result = isPerfectHold ? HitResult.Perfect : HitResult.Great;
            AccuracyText.Show(result);
            ScoreManager.Instance.AddScore(result);
            
            _data.MarkAsHit();
            DestroyNote();
        }

        public void OnMiss()
        {
            if (_data.IsHit) return;
            
            if (_data.Type == NoteType.Hold && _isHolding)
            {
                _isHolding = false;
            }
            
            _data.MarkAsHit();
            _visual?.PlayMissEffect();
            AccuracyText.Show(HitResult.Miss);
            ScoreManager.Instance.AddScore(HitResult.Miss);
            DestroyNote();
        }

        public float CalculateAccuracy()
        {
            float distance = Mathf.Abs(GetCurrentZ() - GetTargetZ());
            return Mathf.Clamp01(1f - (distance / 0.5f));
        }

        public HitResult GetHitResultFromAccuracy(float accuracy)
        {
            if (accuracy >= 0.80f) return HitResult.Perfect;
            if (accuracy >= 0.60f) return HitResult.Great;
            if (accuracy >= 0.20f) return HitResult.Good;
            return HitResult.Miss;
        }

        public void DestroyNote() => _visual?.DestroyNote();
    }
}