using UnityEngine;
using TapRythm.Enums;
using TapRythm.Interfaces;
using TapRythm.UI;

namespace TapRythm.Notes
{
    public class Note : MonoBehaviour, INote
    {
        [Header("Components")]
        [SerializeField] private NoteMovement _movement;
        [SerializeField] private NoteDataComponent _data;
        [SerializeField] private NoteVisual _visual;

        public int LaneIndex => _data.LaneIndex;
        public float HitTime => _data.HitTime;
        public NoteType Type => _data.Type;
        public bool IsHit => _data.IsHit;

        public void Init(int lane, float hitTime, NoteType type, float speed, float startZ, float targetZ)
        {
            _data.Init(lane, hitTime, type);
            _movement.Init(speed, startZ, targetZ);
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
            if (_data.IsHit)
            {
                return;
            }
            float accuracy = CalculateAccuracy();
            HitResult result = GetHitResultFromAccuracy(accuracy);
            AccuracyText.Show(result);

            _data.MarkAsHit();
            _visual?.PlayHitEffect();
            DestroyNote();
        }

        public void OnMiss()
        {
            if (_data.IsHit)
            {
                return;
            }
            _data.MarkAsHit();
            _visual?.PlayHitEffect();
            DestroyNote();
        }

        public float CalculateAccuracy()
        {
            float distance = Mathf.Abs(GetCurrentZ() - GetTargetZ());
            return Mathf.Clamp01(1f - (distance / 0.5f));
        }

        public HitResult GetHitResultFromAccuracy(float accuracy)
        {
            if (accuracy >= 0.80f)
            {
                return HitResult.Perfect;
            }
            if (accuracy >= 0.60f)
            {
                return HitResult.Great;
            }
            if (accuracy >= 0.20f)
            {
                return HitResult.Good;
            }
            return HitResult.Miss;
        }

        public void DestroyNote()
        {
            _visual?.DestroyNote();
        }
    }
}