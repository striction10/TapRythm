using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TapRythm.Managers;
using TapRythm.Auth;
using UnityEngine.SceneManagement;

namespace TapRythm.UI
{
    public class ProfileUI : MonoBehaviour
    {
        [Header("Stats")]
        [SerializeField] private TextMeshProUGUI _usernameText;
        [SerializeField] private TextMeshProUGUI _totalScoreText;
        [SerializeField] private TextMeshProUGUI _totalPlaysText;
        [SerializeField] private TextMeshProUGUI _accuracyText;
        [SerializeField] private TextMeshProUGUI _maxComboText;
        [SerializeField] private TextMeshProUGUI _songsCompletedText;
        [SerializeField] private TextMeshProUGUI _songsUnlockedText;
        [SerializeField] private TextMeshProUGUI _favoriteSongText;
        
        [Header("Details")]
        [SerializeField] private TextMeshProUGUI _perfectText;
        [SerializeField] private TextMeshProUGUI _greatText;
        [SerializeField] private TextMeshProUGUI _goodText;
        [SerializeField] private TextMeshProUGUI _missText;
        
        [Header("Buttons")]
        [SerializeField] private Button _backButton;
        [SerializeField] private Button _logoutButton;
        
        private async void Start()
        {
            await LoadProfile();
            
            if (_backButton != null)
                _backButton.onClick.AddListener(() => gameObject.SetActive(false));
            
            if (_logoutButton != null)
                _logoutButton.onClick.AddListener(Logout);
        }
        
        private async System.Threading.Tasks.Task LoadProfile()
        {
            bool success = await UserStatsManager.Instance.LoadStats();
            
            if (!success)
            {
                Debug.LogError("Не удалось загрузить статистику!");
                return;
            }
            
            var stats = UserStatsManager.Instance.Stats;
            
            if (_usernameText != null)
                _usernameText.text = AuthManager.Instance.Username;
            
            if (_totalScoreText != null)
                _totalScoreText.text = stats.totalScore.ToString();
            
            if (_totalPlaysText != null)
                _totalPlaysText.text = stats.totalPlays.ToString();
            
            if (_accuracyText != null)
                _accuracyText.text = $"{stats.averageAccuracy:F1}%";
            
            if (_maxComboText != null)
                _maxComboText.text = stats.maxCombo.ToString();
            
            if (_songsCompletedText != null)
                _songsCompletedText.text = stats.songsCompleted.ToString();
            
            if (_songsUnlockedText != null)
                _songsUnlockedText.text = stats.songsUnlocked.ToString();
            
            if (_favoriteSongText != null)
                _favoriteSongText.text = stats.favoriteSong ?? "Нет любимой песни";
            
            if (_perfectText != null)
                _perfectText.text = stats.totalPerfect.ToString();
            
            if (_greatText != null)
                _greatText.text = stats.totalGreat.ToString();
            
            if (_goodText != null)
                _goodText.text = stats.totalGood.ToString();
            
            if (_missText != null)
                _missText.text = stats.totalMiss.ToString();
        }
        
        private void Logout()
        {
            AuthManager.Instance.Logout();
            SceneManager.LoadScene("AuthScene");
        }
    }
}