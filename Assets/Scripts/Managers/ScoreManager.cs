using UnityEngine;
using TapRythm.Enums;
using TapRythm.Data;
using TapRythm.Managers;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

namespace TapRythm.Managers
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }
        
        public static event System.Action<int, int> OnScoreUpdated;
        
        [Header("Current Stats")]
        private int _currentScore;
        private int _combo;
        private int _maxCombo;
        private int _comboMultiplier = 1;
        private int _perfectCount;
        private int _greatCount;
        private int _goodCount;
        private int _missCount;
        private int _totalNotes;
        private int _currentSongId;
        private string _currentSongName;
        
        [Header("Score Values")]
        [SerializeField] private int _perfectPoints = 100;
        [SerializeField] private int _greatPoints = 80;
        [SerializeField] private int _goodPoints = 50;
        
        [Header("Multiplier Thresholds")]
        [SerializeField] private int _multiplier2Threshold = 10;
        [SerializeField] private int _multiplier3Threshold = 20;
        [SerializeField] private int _multiplier4Threshold = 30;
        
        [Header("Multiplier Values")]
        [SerializeField] private float _multiplier1 = 1.0f;
        [SerializeField] private float _multiplier2 = 1.2f;
        [SerializeField] private float _multiplier3 = 1.3f;
        [SerializeField] private float _multiplier4 = 1.4f;
        
        public int CurrentScore => _currentScore;
        public int Combo => _combo;
        public int ComboMultiplier => _comboMultiplier;
        public int PerfectCount => _perfectCount;
        public int GreatCount => _greatCount;
        public int GoodCount => _goodCount;
        public int MissCount => _missCount;
        public int MaxCombo => _maxCombo;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                SongManager.OnSongFinished += OnSongFinished;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void OnDestroy()
        {
            SongManager.OnSongFinished -= OnSongFinished;
        }
        
        public void SetSongInfo(int songId, string songName, int totalNotes)
        {
            _currentSongId = songId;
            _currentSongName = songName;
            _totalNotes = totalNotes;
        }
        
        public void ResetScore()
        {
            _currentScore = 0;
            _combo = 0;
            _maxCombo = 0;
            _comboMultiplier = 1;
            _perfectCount = 0;
            _greatCount = 0;
            _goodCount = 0;
            _missCount = 0;
        }
        
        private void UpdateMultiplier()
        {
            if (_perfectCount >= _multiplier4Threshold)
                _comboMultiplier = 4;
            else if (_perfectCount >= _multiplier3Threshold)
                _comboMultiplier = 3;
            else if (_perfectCount >= _multiplier2Threshold)
                _comboMultiplier = 2;
            else
                _comboMultiplier = 1;
        }
        
        private float GetMultiplierValue()
        {
            switch (_comboMultiplier)
            {
                case 4: return _multiplier4;
                case 3: return _multiplier3;
                case 2: return _multiplier2;
                default: return _multiplier1;
            }
        }
        
        public void AddScore(HitResult result)
        {
            int points = 0;
            float multiplier = GetMultiplierValue();
            
            switch (result)
            {
                case HitResult.Perfect:
                    _perfectCount++;
                    UpdateMultiplier();
                    multiplier = GetMultiplierValue();
                    points = Mathf.RoundToInt(_perfectPoints * multiplier);
                    break;
                case HitResult.Great:
                    _greatCount++;
                    points = Mathf.RoundToInt(_greatPoints * multiplier);
                    break;
                case HitResult.Good:
                    _goodCount++;
                    points = Mathf.RoundToInt(_goodPoints * multiplier);
                    break;
                case HitResult.Miss:
                    _missCount++;
                    _perfectCount = 0;
                    _comboMultiplier = 1;
                    points = 0;
                    break;
            }
            
            _currentScore += points;
            
            if (result != HitResult.Miss)
                _combo++;
            else
                _combo = 0;
            
            if (_combo > _maxCombo)
                _maxCombo = _combo;
            
            OnScoreUpdated?.Invoke(_currentScore, _combo);
        }
        
        public async void OnSongFinished()
        {
            Debug.Log($"OnSongFinished: песня завершена! Очки: {_currentScore}, Perfect: {_perfectCount}, MaxCombo: {_maxCombo}");
            
            var progressResponse = await SongLibraryManager.Instance.SaveProgress(
                _currentSongId, 
                _currentScore, 
                _perfectCount, 
                _totalNotes
            );
            
            if (progressResponse != null && progressResponse.success)
            {
                Debug.Log($"Прогресс сохранён: {progressResponse.message}");
                if (progressResponse.songCompleted)
                {
                    Debug.Log($"🎉 Песня пройдена! Разблокирована: {progressResponse.unlockedSongName}");
                }
            }
            
            var statsRequest = new UpdateStatsRequest
            {
                score = _currentScore,
                perfectCount = _perfectCount,
                greatCount = _greatCount,
                goodCount = _goodCount,
                missCount = _missCount,
                maxCombo = _maxCombo,
                songId = _currentSongId
            };
            
            await UserStatsManager.Instance.UpdateStats(statsRequest);
            //SceneManager.LoadScene("SongSelectScene");
        }
    }
}