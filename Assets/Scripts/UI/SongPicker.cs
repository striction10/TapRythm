using UnityEngine;
using TapRythm.Data;
using TapRythm.Managers;
using TapRythm.Utils;

namespace TapRythm.UI
{
    public class SongPicker : MonoBehaviour
    {
        [SerializeField] private NoteSpawner _noteSpawner;
        [SerializeField] private AudioClip _audioClip;
        [SerializeField] private TextAsset _chartFile;
        [SerializeField] private bool _autoGenerate = true;
        
        private SongChart _currentChart;
        private int _currentSongId;
        private string _currentSongName;
        
        private void Start()
        {     
            if (SongLibraryManager.Instance == null)
            {
                Debug.LogError("SongPicker: SongLibraryManager.Instance = null!");
                return;
            }
            
            var currentSong = SongLibraryManager.Instance.CurrentSong;
            
            if (currentSong == null && PlayerPrefs.HasKey("SelectedSongId"))
            {
                int songId = PlayerPrefs.GetInt("SelectedSongId");
                currentSong = SongLibraryManager.Instance.GetSongById(songId);
                if (currentSong != null)
                {
                    SongLibraryManager.Instance.SelectSong(songId);
                }
            }
            
            if (currentSong == null)
            {
                var songs = SongLibraryManager.Instance.Songs;
                if (songs != null && songs.Count > 0)
                {
                    currentSong = songs[0];
                    SongLibraryManager.Instance.SelectSong(currentSong.id);
                }
                else
                {
                    Debug.LogError("SongPicker: нет доступных песен!");
                    return;
                }
            }
            
            _currentSongId = currentSong.id;
            _currentSongName = currentSong.songName;
            LoadChart();
            
            StartSong();
        }
        
        private void LoadChart()
        {
            if (_autoGenerate && _audioClip != null)
            {
                float bpm = 120f;
                
                var currentSong = SongLibraryManager.Instance.CurrentSong;
                if (currentSong != null && !string.IsNullOrEmpty(currentSong.bpm))
                {
                    bpm = float.Parse(currentSong.bpm);
                }
                
                _currentChart = AudioAnalyzer.GenerateChartFromAudio(_audioClip, bpm);
                return;
            }
            
            if (_chartFile != null)
            {
                _currentChart = JsonUtility.FromJson<SongChart>(_chartFile.text);
            }
            else
            {
                _currentChart = ChartLoader.CreateTestChart();
            }
        }
        
        public void StartSong()
        {
            if (_currentChart == null)
            {
                Debug.LogError("Карта нот не загружена!");
                return;
            }
            
            if (_audioClip == null)
            {
                Debug.LogError("AudioClip не загружен!");
                return;
            }
            
            int totalNotes = _currentChart.notes.Count;
            ScoreManager.Instance.SetSongInfo(_currentSongId, _currentSongName, totalNotes);
            ScoreManager.Instance.ResetScore();
            
            SongManager.Instance.LoadSong(_currentChart, _audioClip, OnNoteSpawn);
            SongManager.Instance.PlaySong();
        }
        
        private void OnNoteSpawn(NoteData noteData)
        {
            _noteSpawner.SpawnNoteFromData(noteData);
        }
    }
}