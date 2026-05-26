using UnityEngine;
using UnityEngine.UI;
using TapRythm.Managers;

namespace TapRythm.UI
{
    public class CountComboUI : MonoBehaviour
    {
        [SerializeField] private Text _scoreText;
        [SerializeField] private Text _comboText;
        
        [Header("Multiplier Colors")]
        [SerializeField] private Color _multiplier2Color = new Color(0.2f, 0.8f, 0.2f);
        [SerializeField] private Color _multiplier3Color = new Color(1f, 0.5f, 0f);
        [SerializeField] private Color _multiplier4Color = new Color(0.7f, 0.2f, 0.8f);
        
        private void OnEnable()
        {
            ScoreManager.OnScoreUpdated += UpdateUI;
        }
        
        private void OnDisable()
        {
            ScoreManager.OnScoreUpdated -= UpdateUI;
        }
        
        private void UpdateUI(int score, int multiplier)
        {
            if (_scoreText != null)
                _scoreText.text = score.ToString();
            
            if (_comboText != null)
            {
                if (multiplier > 1)
                {
                    _comboText.text = $"x{multiplier}";
                    
                    switch (multiplier)
                    {
                        case 2:
                            _comboText.color = _multiplier2Color;
                            break;
                        case 3:
                            _comboText.color = _multiplier3Color;
                            break;
                        case 4:
                            _comboText.color = _multiplier4Color;
                            break;
                        default:
                            _comboText.color = Color.white;
                            break;
                    }
                }
                else
                {
                    _comboText.text = "";
                }
            }
        }
    }
}