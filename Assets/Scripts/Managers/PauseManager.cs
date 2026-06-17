using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TapRythm.Auth;
using TapRythm.Managers;
using System.Collections;

namespace TapRythm.Game
{
    public class PauseManager : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject _pausePanel;
        [SerializeField] private Button _pauseButton;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _menuButton;
        
        private bool _isPaused = false;
        private float _timeScaleBeforePause = 1f;
        
        private void Start()
        {
            SetupButtons();
            
            if (_pausePanel != null)
                _pausePanel.SetActive(false);
        }
        
        private void SetupButtons()
        {
            if (_pauseButton != null)
            {
                _pauseButton.onClick.RemoveAllListeners();
                _pauseButton.onClick.AddListener(PauseGame);
            }
            
            if (_resumeButton != null)
            {
                _resumeButton.onClick.RemoveAllListeners();
                _resumeButton.onClick.AddListener(ResumeGame);
            }
            
            if (_menuButton != null)
            {
                _menuButton.onClick.RemoveAllListeners();
                _menuButton.onClick.AddListener(BackToMenu);
            }
        }
        
        public void PauseGame()
        {
            if (_isPaused) return;
            
            _isPaused = true;
            _timeScaleBeforePause = Time.timeScale;
            Time.timeScale = 0f;
            
            SongManager.Instance?.PauseSong();
            
            var noteSpawner = FindFirstObjectByType<NoteSpawner>();
            noteSpawner?.StopSpawning();
            
            if (_pausePanel != null)
                _pausePanel.SetActive(true);
        }
        
        public void ResumeGame()
        {
            if (!_isPaused) return;
            
            _isPaused = false;
            Time.timeScale = _timeScaleBeforePause;
            
            SongManager.Instance?.ResumeSong();
            
            var noteSpawner = FindFirstObjectByType<NoteSpawner>();
            noteSpawner?.StartSpawning();
            
            if (_pausePanel != null)
                _pausePanel.SetActive(false);
        }
        
        private void BackToMenu()
        {
            AuthUI.SetManualLogout();
            AuthManager.Instance?.Logout();
            
            Time.timeScale = 1f;
            SceneManager.LoadScene("AuthScene");
        }
        
        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }
    }
}