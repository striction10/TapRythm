using UnityEngine;
using TapRythm.Data;
using System.Collections.Generic;

namespace TapRythm.Managers
{
    public class SongManager : MonoBehaviour
    {
        public static SongManager Instance { get; private set; }
        
        [Header("Audio")]
        private AudioSource _audioSource;
        
        [Header("Current Song")]
        private SongChart _currentChart;
        private float _bpm;
        private float _secPerBeat;
        private double _songStartTime;
        
        [Header("Note Spawning")]
        private Queue<NoteData> _noteQueue;
        private System.Action<NoteData> _onNoteSpawn;
        
        public bool IsPlaying { get; private set; }
        public float CurrentSongTime => (float)(AudioSettings.dspTime - _songStartTime);
        public float CurrentBeat => IsPlaying ? (float)((AudioSettings.dspTime - _songStartTime) / _secPerBeat) : 0f;
        
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
            
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
                _audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        private void Update()
        {
            if (!IsPlaying) return;
            
            float currentBeat = CurrentBeat;
            
            while (_noteQueue.Count > 0 && _noteQueue.Peek().beat <= currentBeat + 2f)
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
            
            _audioSource.clip = audioClip;
            
            var sortedNotes = new List<NoteData>(chart.notes);
            sortedNotes.Sort((a, b) => a.beat.CompareTo(b.beat));
            _noteQueue = new Queue<NoteData>(sortedNotes);
            
        }
        
        public void PlaySong()
        {
            if (_audioSource.clip == null)
            {
                return;
            }
            
            _songStartTime = AudioSettings.dspTime + 0.5;
            _audioSource.PlayScheduled(_songStartTime);
            IsPlaying = true;
        }
        
        public void StopSong()
        {
            IsPlaying = false;
            _audioSource.Stop();
            _noteQueue?.Clear();
        }
        
        public void PauseSong()
        {
            IsPlaying = false;
            _audioSource.Pause();
        }
        
        public void ResumeSong()
        {
            IsPlaying = true;
            _audioSource.UnPause();
        }
    }
}