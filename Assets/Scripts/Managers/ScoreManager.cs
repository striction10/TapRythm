using UnityEngine;
using TapRythm.Enums;

namespace TapRythm.Managers
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager Instance { get; private set; }
        
        [Header("Current Stats")]
        private int _currentScore;
        private int _perfectCount;
        private int _comboMultiplier;
        private int _combo;
        
        [Header("Hit Counters")]
        private int _totalPerfect;
        private int _totalGreat;
        private int _totalGood;
        private int _totalMiss;
        
        [Header("Combo Thresholds")]
        [SerializeField] private int _combo2Threshold = 10;
        [SerializeField] private int _combo3Threshold = 25;
        [SerializeField] private int _combo4Threshold = 50;
        
        [Header("Base Score Values")]
        [SerializeField] private int _perfectPoints = 100;
        [SerializeField] private int _greatPoints = 80;
        [SerializeField] private int _goodPoints = 50;
        
        [Header("Multiplier Values")]
        [SerializeField] private float _multiplier1 = 1.0f;
        [SerializeField] private float _multiplier2 = 1.2f;
        [SerializeField] private float _multiplier3 = 1.3f;
        [SerializeField] private float _multiplier4 = 1.4f;
        
        public int CurrentScore => _currentScore;
        public int ComboMultiplier => _comboMultiplier;
        public int PerfectCount => _totalPerfect;
        public int GreatCount => _totalGreat;
        public int GoodCount => _totalGood;
        public int MissCount => _totalMiss;
        public int Combo => _combo;
        
        public static event System.Action<int, int> OnScoreUpdated;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void ResetScore()
        {
            _currentScore = 0;
            _perfectCount = 0;
            _comboMultiplier = 1;
            
            _totalPerfect = 0;
            _totalGreat = 0;
            _totalGood = 0;
            _totalMiss = 0;
        }
        
        private void UpdateMultiplier()
        {
            if (_perfectCount >= _combo4Threshold)
            {
                _comboMultiplier = 4;
            }
            else if (_perfectCount >= _combo3Threshold)
            {
                _comboMultiplier = 3;
            }
            else if (_perfectCount >= _combo2Threshold)
            {
                _comboMultiplier = 2;
            }
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
                    _totalPerfect++;
                    _perfectCount++;
                    UpdateMultiplier();
                    multiplier = GetMultiplierValue();
                    points = Mathf.RoundToInt(_perfectPoints * multiplier);
                    break;
                    
                case HitResult.Great:
                    _totalGreat++;
                    points = Mathf.RoundToInt(_greatPoints * multiplier);
                    break;
                    
                case HitResult.Good:
                    _totalGood++;
                    points = Mathf.RoundToInt(_goodPoints * multiplier);
                    break;
                    
                case HitResult.Miss:
                    _totalMiss++;
                    _perfectCount = 0;
                    _comboMultiplier = 1;
                    points = 0;
                    break;
            }
            
            _currentScore += points;
            
            OnScoreUpdated?.Invoke(_currentScore, _comboMultiplier);
        }
        
        public float GetAccuracy()
        {
            int total = _totalPerfect + _totalGreat + _totalGood + _totalMiss;
            if (total == 0) 
            {
                return 0f;
            }
            return (float)(_totalPerfect * 1.0 + _totalGreat * 0.8 + _totalGood * 0.5) / total * 100f;
        }
    }
}