using UnityEngine;
using System.Collections;
using TapRythm.Enums;

namespace TapRythm.Notes
{
    public class NoteVisual : MonoBehaviour
    {
        [SerializeField] private ParticleSystem _hitParticle;
        [SerializeField] private SpriteRenderer _spriteRender;

        public void PlayHitEffect()
        {
            if (_hitParticle != null)
            {
                _hitParticle.Play();
            }
            if (_spriteRender != null)
            {
                _spriteRender.color = Color.green;
            }
        }

        public void PlayMissEffect()
        {
            if (_spriteRender != null)
            {
                _spriteRender.color = Color.red;
            }
        }

        public void DestroyNote()
        {
            Destroy(gameObject);
        }
        public void PlayHitEffect(HitResult result)
        {
            StartCoroutine(HitFlash(result));
            
            if (_hitParticle != null) _hitParticle.Play();
        }

        private Color GetColorForResult(HitResult result)
        {
            switch (result)
            {
                case HitResult.Perfect:
                    return new Color(1f, 0.8f, 0f);
                case HitResult.Great:
                    return Color.green;
                case HitResult.Good:
                    return Color.cyan;
                default:
                    return Color.red;
            }
        }

        private IEnumerator HitFlash(HitResult result)
        {
            Color flashColor = GetColorForResult(result);
            float duration = 0.1f;
            float elapsed = 0f;
            
            Color originalColor = _spriteRender.color;
            
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                _spriteRender.color = Color.Lerp(flashColor, originalColor, t);
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            _spriteRender.color = originalColor;
        }
    }
}