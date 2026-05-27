using UnityEngine;
using UnityEngine.UI;
using TapRythm.Enums;
using System.Collections;

namespace TapRythm.UI
{
    public class AccuracyText : MonoBehaviour
    {
        [SerializeField] private Text _text;
        [SerializeField] private float _flyDuration = 0.5f;
        [SerializeField] private float _flyDistance = 100f;
        
        private static AccuracyText _instance;
        private RectTransform _rectTransform;
        private Vector3 _startPosition;
        
        void Awake()
        {
            _instance = this;
            _rectTransform = GetComponent<RectTransform>();
            _text.enabled = false;
        }
        
        public static void Show(HitResult result)
        {
            if (_instance == null) return;
            _instance.Display(result);
        }
        
        void Display(HitResult result)
        {
            StopAllCoroutines();
            
            switch (result)
            {
                case HitResult.Perfect:
                    _text.text = "PERFECT!";
                    _text.color = new Color(1f, 0.8f, 0f);
                    _text.fontSize = 124;
                    break;
                case HitResult.Great:
                    _text.text = "GREAT!";
                    _text.color = Color.green;
                    _text.fontSize = 124;
                    break;
                case HitResult.Good:
                    _text.text = "GOOD!";
                    _text.color = Color.cyan;
                    _text.fontSize = 124;
                    break;
                case HitResult.Miss:
                    _text.text = "MISS!";
                    _text.color = Color.red;
                    _text.fontSize = 124;
                    break;
            }
            
            _rectTransform.anchoredPosition = Vector3.zero;
            _text.enabled = true;
            StartCoroutine(FlyAndFadeAnimation());
        }
        
        IEnumerator FlyAndFadeAnimation()
        {
            float elapsed = 0f;
            Vector3 startPos = _rectTransform.anchoredPosition;
            Vector3 endPos = startPos + new Vector3(0, _flyDistance, 0);
            Color startColor = _text.color;
            
            while (elapsed < _flyDuration)
            {
                float t = elapsed / _flyDuration;
                
                _rectTransform.anchoredPosition = Vector3.Lerp(startPos, endPos, t);
                
                Color color = startColor;
                color.a = Mathf.Lerp(1f, 0f, t);
                _text.color = color;
                
                float scale = 1f + Mathf.Sin(t * Mathf.PI) * 0.3f;
                _rectTransform.localScale = Vector3.one * scale;
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            _text.enabled = false;
            _rectTransform.localScale = Vector3.one;
            _rectTransform.anchoredPosition = startPos;
        }
    }
}