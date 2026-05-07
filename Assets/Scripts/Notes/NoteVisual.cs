using UnityEngine;

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
    }
}