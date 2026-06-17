using UnityEngine;
using System.Collections;
using TapRythm.Enums;

namespace TapRythm.Notes
{
    public class NoteVisual : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _spriteRender;
        [SerializeField] private Sprite _tapSprite;
        [SerializeField] private Sprite _holdSprite;
        [SerializeField] private Color _tapColor = Color.blue;
        [SerializeField] private Color _holdColor = Color.green;
        [SerializeField] private ParticleSystem _hitParticle;
        
        public void SetupForType(NoteType type)
        {
            if (_spriteRender == null) return;
            
            switch (type)
            {
                case NoteType.Tap:
                    _spriteRender.sprite = _tapSprite;
                    _spriteRender.color = _tapColor;
                    _spriteRender.transform.localScale = new Vector3(0.8f, 0.3f, 0.1f);
                    break;
                case NoteType.Hold:
                    _spriteRender.sprite = _holdSprite;
                    _spriteRender.color = _holdColor;
                    _spriteRender.transform.localScale = new Vector3(0.8f, 1f, 0.1f); // Длинная!
                    break;
            }
        }
        
        public void PlayHitEffect(HitResult result)
        {
            StartCoroutine(FlashColor(GetColorForResult(result)));
            if (_hitParticle != null) _hitParticle.Play();
        }
        
        public void PlayHoldStartEffect()
        {
            StartCoroutine(FlashColor(Color.cyan));
        }
        
        public void PlayHoldEndEffect()
        {
            StartCoroutine(FlashColor(Color.green));
        }
        
        public void PlayMissEffect()
        {
            StartCoroutine(FlashColor(Color.red));
        }
        
        public void DestroyNote() => Destroy(gameObject);
        
        private IEnumerator FlashColor(Color color)
        {
            Color original = _spriteRender.color;
            _spriteRender.color = color;
            yield return new WaitForSeconds(0.1f);
            _spriteRender.color = original;
        }
        
        private Color GetColorForResult(HitResult result)
        {
            switch (result)
            {
                case HitResult.Perfect: return Color.yellow;
                case HitResult.Great: return Color.green;
                case HitResult.Good: return Color.cyan;
                default: return Color.red;
            }
        }
    }
}