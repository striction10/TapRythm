using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TapRythm.Auth;
using System.Collections;

namespace TapRythm.Managers
{
    public class PauseManager : MonoBehaviour
    {
        private GameObject _pausePanel;
        private Button _pauseButton;
        private Button _resumeButton;
        private Button _menuButton;
        
        private bool _isPaused = false;
        private float _timeScaleBeforePause = 1f;
        
        private void Start()
        {
            FindUIElements();
            SetupButtons();
            
            if (_pausePanel != null)
                _pausePanel.SetActive(false);
        }
        
        private void OnEnable()
        {
            FindUIElements();
            SetupButtons();
        }
        
        private void FindUIElements()
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas == null) return;
            
            // Ищем кнопку паузы
            Transform pauseBtn = canvas.transform.Find("PauseButton");
            if (pauseBtn != null)
                _pauseButton = pauseBtn.GetComponent<Button>();
            
            // Ищем панель паузы
            Transform panel = canvas.transform.Find("PausePanel");
            if (panel != null)
            {
                _pausePanel = panel.gameObject;
                
                Transform resumeBtn = panel.Find("ResumeGameButton");
                if (resumeBtn != null)
                    _resumeButton = resumeBtn.GetComponent<Button>();
                
                Transform exitBtn = panel.Find("ExitGameButton");
                if (exitBtn != null)
                    _menuButton = exitBtn.GetComponent<Button>();
            }
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
            
            SongManager songManager = FindFirstObjectByType<SongManager>();
            if (songManager != null)
                songManager.PauseSong();
            
            NoteSpawner noteSpawner = FindFirstObjectByType<NoteSpawner>();
            if (noteSpawner != null)
                noteSpawner.StopSpawning();
            
            if (_pausePanel != null)
                _pausePanel.SetActive(true);
        }
        
        public void ResumeGame()
        {
            if (!_isPaused) return;
            
            _isPaused = false;
            Time.timeScale = _timeScaleBeforePause;
            
            SongManager songManager = FindFirstObjectByType<SongManager>();
            if (songManager != null)
                songManager.ResumeSong();
            
            NoteSpawner noteSpawner = FindFirstObjectByType<NoteSpawner>();
            if (noteSpawner != null)
                noteSpawner.StartSpawning();
            
            if (_pausePanel != null)
                _pausePanel.SetActive(false);
        }
        
        private void BackToMenu()
        {
            AuthUI.SetManualLogout();
            
            if (AuthManager.Instance != null)
                AuthManager.Instance.Logout();
            
            Time.timeScale = 1f;
            StartCoroutine(DelayedLoadAuthScene());
        }
        
        private IEnumerator DelayedLoadAuthScene()
        {
            yield return new WaitForSeconds(0.1f);
            SceneManager.LoadScene("AuthScene");
        }
        
        private void OnDestroy()
        {
            Time.timeScale = 1f;
        }
    }
}