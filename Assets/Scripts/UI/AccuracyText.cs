using UnityEngine;
using UnityEngine.UI;
using TapRythm.Enums;
using System.Collections;

namespace TapRythm.UI
{
    public class AccuracyText : MonoBehaviour
    {
        [SerializeField] private Text _text;
        [SerializeField] private float _displayDuration = 0.5f;
        
        private static AccuracyText _instance;
        
        void Awake()
        {
            _instance = this;
            if (_text != null) _text.enabled = false;
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
                    _text.color = Color.yellow;
                    break;
                case HitResult.Great:
                    _text.text = "GREAT!";
                    _text.color = Color.green;
                    break;
                case HitResult.Good:
                    _text.text = "GOOD!";
                    _text.color = Color.blue;
                    break;
                case HitResult.Miss:
                    _text.text = "MISS!";
                    _text.color = Color.red;
                    break;
            }
            
            _text.enabled = true;
            StartCoroutine(HideAfterDelay());
        }
        
        IEnumerator HideAfterDelay()
        {
            yield return new WaitForSeconds(_displayDuration);
            _text.enabled = false;
        }
    }
}