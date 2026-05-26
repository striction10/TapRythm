using UnityEngine;
using TapRythm.Enums;

namespace TapRythm.Utils
{
    public static class AccuracyHelper
    {
        public static float CalculateAccuracy(float currentZ, float targetZ, float tolerance = 0.5f)
        {
            float distance = Mathf.Abs(currentZ - targetZ);
            return 1f - Mathf.Clamp01(distance / tolerance);
        }
        
        public static HitResult GetHitResultFromAccuracy(float accuracy)
        {
            if (accuracy >= 0.80f) return HitResult.Perfect;
            if (accuracy >= 0.60f) return HitResult.Great;
            if (accuracy >= 0.20f) return HitResult.Good;
            return HitResult.Miss;
        }
    }
}