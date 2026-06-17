using UnityEngine;
using UnityEngine.UI;
using TapRythm.Managers;

namespace TapRythm.UI
{
    public class CountComboUI : MonoBehaviour
    {
        [SerializeField] private Text _scoreText;
        [SerializeField] private Text _comboText;
        [SerializeField] private GameObject _comboPanel;
        
        private void OnEnable()
        {
            ScoreManager.OnScoreUpdated += UpdateUI;
        }
        
        private void OnDisable()
        {
            ScoreManager.OnScoreUpdated -= UpdateUI;
        }
        
        private void UpdateUI(int score, int combo)
        {
            if (_scoreText != null)
                _scoreText.text = score.ToString();
            
            if (_comboText != null)
            {
                int multiplier = ScoreManager.Instance.ComboMultiplier;
                
                if (multiplier > 1)
                {
                    _comboText.text = $"x{multiplier}";
                    _comboText.gameObject.SetActive(true);
                    
                    if (multiplier >= 4)
                        _comboText.color = new Color(0.8f, 0.2f, 0.8f);
                    else if (multiplier >= 3)
                        _comboText.color = new Color(1f, 0.5f, 0f);
                    else if (multiplier >= 2)
                        _comboText.color = Color.green;
                    
                    if (_comboPanel != null)
                        _comboPanel.SetActive(true);
                }
                else
                {
                    _comboText.gameObject.SetActive(false);
                    if (_comboPanel != null)
                        _comboPanel.SetActive(false);
                }
            }
        }
    }
}