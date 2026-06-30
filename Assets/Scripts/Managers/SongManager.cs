using UnityEngine;
using TapRythm.Data;
using System.Collections.Generic;

namespace TapRythm.Managers
{
    public class SongManager : MonoBehaviour
    {
        public static SongManager Instance { get; private set; }
        public static event System.Action OnSongFinished;
        
        [SerializeField] private AudioSource _audioSource;
        
        private SongChart _currentChart;
        private float _bpm;
        private float _secPerBeat;
        private double _songStartTime;
        private double _pauseTimeOffset = 0;
        private bool _isPaused = false;
        private Queue<NoteData> _noteQueue;
        private System.Action<NoteData> _onNoteSpawn;
        
        public bool IsPlaying { get; private set; }
        public float CurrentBPM => _bpm;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                if (_audioSource == null)
                {
                    _audioSource = GetComponent<AudioSource>();
                    if (_audioSource == null)
                        _audioSource = gameObject.AddComponent<AudioSource>();
                    
                    _audioSource.playOnAwake = false;
                    _audioSource.loop = false;
                    _audioSource.enabled = true;
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Update()
        {
            if (!IsPlaying || _isPaused) return;
            
            if (_audioSource != null && !_audioSource.isPlaying && IsPlaying)
            {
                IsPlaying = false;
                OnSongFinished?.Invoke();
                return;
            }
            
            float currentBeat = CurrentBeat;
            
            while (_noteQueue != null && _noteQueue.Count > 0 && _noteQueue.Peek().beat <= currentBeat + 2f)
            {
                NoteData note = _noteQueue.Dequeue();
                _onNoteSpawn?.Invoke(note);
            }
        }
        
        public void LoadSong(SongChart chart, AudioClip audioClip, System.Action<NoteData> onNoteSpawn)
        {
            _currentChart = chart;
            _bpm = chart.bpm;
            _secPerBeat = 60f / _bpm;
            _onNoteSpawn = onNoteSpawn;
            
            if (_audioSource == null) return;
            
            _audioSource.clip = audioClip;
            _audioSource.enabled = true;
            
            var sortedNotes = new List<NoteData>(chart.notes);
            sortedNotes.Sort((a, b) => a.beat.CompareTo(b.beat));
            _noteQueue = new Queue<NoteData>(sortedNotes);
        }
        
        public void PlaySong()
        {
            if (_audioSource == null || _audioSource.clip == null)
            {
                Debug.LogError("Аудио не загружено!");
                return;
            }

            if (!_audioSource.enabled)
            {
                _audioSource.enabled = true;
            }
            
            _songStartTime = AudioSettings.dspTime + 0.5;
            _audioSource.PlayScheduled(_songStartTime);
            IsPlaying = true;
            _isPaused = false;
            _pauseTimeOffset = 0;
        }
        
        public float CurrentBeat => IsPlaying ? (float)((AudioSettings.dspTime - _songStartTime) / _secPerBeat) : 0f;
        
        public void PauseSong()
        {
            if (!IsPlaying || _isPaused) return;
            
            if (_audioSource == null || !_audioSource.enabled)
            {
                Debug.LogWarning("SongManager: AudioSource = null или отключён");
                return;
            }
            
            _isPaused = true;
            _audioSource.Pause();
            _pauseTimeOffset = AudioSettings.dspTime - _songStartTime;
            IsPlaying = false;
        }

        public void ResumeSong()
        {
            if (!_isPaused) return;
            
            if (_audioSource == null || !_audioSource.enabled)
            {
                Debug.LogWarning("SongManager: AudioSource = null или отключён");
                return;
            }
            
            _songStartTime = AudioSettings.dspTime - _pauseTimeOffset;
            _audioSource.Play();
            IsPlaying = true;
            _isPaused = false;
        }
        
        public void StopSong()
        {
            IsPlaying = false;
            _isPaused = false;
            _audioSource.Stop();
            _noteQueue?.Clear();
        }
    }
}