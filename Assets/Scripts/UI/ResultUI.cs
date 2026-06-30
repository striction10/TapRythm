using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TapRythm.Managers;
using UnityEngine.SceneManagement;

namespace TapRythm.UI
{
    public class ResultUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject _resultPanel;
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private TextMeshProUGUI _statsText;
        [SerializeField] private Button _retryButton;
        [SerializeField] private Button _menuButton;
        
        [Header("Star Images")]
        [SerializeField] private Image _star1;
        [SerializeField] private Image _star2;
        [SerializeField] private Image _star3;
        [SerializeField] private Sprite _starFull;
        [SerializeField] private Sprite _starEmpty;
        
        [Header("Star Colors")]
        [SerializeField] private Color _starGold = new Color(1f, 0.8f, 0f);
        [SerializeField] private Color _starGray = new Color(0.2f, 0.2f, 0.2f);
        
        private void Start()
        {
            if (_resultPanel != null)
                _resultPanel.SetActive(false);
            
            SongManager.OnSongFinished += ShowResults;
            
            if (_retryButton != null)
                _retryButton.onClick.AddListener(RetrySong);
            
            if (_menuButton != null)
                _menuButton.onClick.AddListener(GoToMenu);
        }
        
        private void OnDestroy()
        {
            SongManager.OnSongFinished -= ShowResults;
        }
        
        private void ShowResults()
        {
            Debug.Log("ResultUI: показ результатов");
            
            var scoreManager = ScoreManager.Instance;
            if (scoreManager == null) return;
            
            if (_scoreText != null)
                _scoreText.text = $"Очки: {scoreManager.CurrentScore}";
            
            if (_statsText != null)
            {
                _statsText.text = $"Perfect: {scoreManager.PerfectCount} " +
                                  $"Great: {scoreManager.GreatCount} " +
                                  $"Good: {scoreManager.GoodCount} " +
                                  $"Miss: {scoreManager.MissCount}";
            }
            
            int total = scoreManager.PerfectCount + scoreManager.GreatCount + 
                        scoreManager.GoodCount + scoreManager.MissCount;
            
            float perfectPercent = total > 0 ? (float)scoreManager.PerfectCount / total * 100f : 0f;
            int stars = GetStarsCount(perfectPercent);
            
            UpdateStars(stars);
            
            if (_resultPanel != null)
                _resultPanel.SetActive(true);
        }
        
        private int GetStarsCount(float perfectPercent)
        {
            if (perfectPercent >= 95f) return 3;
            if (perfectPercent >= 80f) return 2;
            if (perfectPercent >= 65f) return 1;
            return 0;
        }
        
        private void UpdateStars(int stars)
        {
            if (_star1 != null)
            {
                _star1.sprite = stars >= 1 ? _starFull : _starEmpty;
                _star1.color = stars >= 1 ? _starGold : _starGray;
            }
            
            if (_star2 != null)
            {
                _star2.sprite = stars >= 2 ? _starFull : _starEmpty;
                _star2.color = stars >= 2 ? _starGold : _starGray;
            }
            
            if (_star3 != null)
            {
                _star3.sprite = stars >= 3 ? _starFull : _starEmpty;
                _star3.color = stars >= 3 ? _starGold : _starGray;
            }
        }
        
        private void RetrySong()
        {
            SceneManager.LoadScene("TimeToGame");
        }
        
        private void GoToMenu()
        {
            SceneManager.LoadScene("SongSelectScene");
        }
    }
}