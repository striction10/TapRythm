using UnityEngine;
using System.Collections.Generic;
using TapRythm.Auth;
using System.Threading.Tasks;

namespace TapRythm.Managers
{
    [System.Serializable]
    public class SongDto
    {
        public int id;
        public string songName;
        public string artist;
        public string folderPath;
        public int unlockLevel;
        public bool isUnlocked;
        public int highScore;
        public int perfectCount;
        public bool isCompleted;
        public string bpm;
        public bool isFavorite;
    }

    [System.Serializable]
    public class UserSongsResponse
    {
        public List<SongDto> songs;
        public int totalCompleted;
        public int totalScore;
    }

    [System.Serializable]
    public class SongProgressRequest
    {
        public int songId;
        public int score;
        public int perfectCount;
        public int totalNotes;
    }

    [System.Serializable]
    public class SongProgressResponse
    {
        public bool success;
        public bool isNewRecord;
        public bool songCompleted;
        public int unlockedSongId;
        public string unlockedSongName;
        public string message;
    }

    public class SongLibraryManager : MonoBehaviour
    {
        public static SongLibraryManager Instance { get; private set; }
        
        private List<SongDto> _songs = new List<SongDto>();
        private SongDto _currentSong;
        
        public List<SongDto> Songs => _songs;
        public SongDto CurrentSong => _currentSong;
        public int TotalCompleted { get; private set; }
        public int TotalScore { get; private set; }
        
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
        
        public async Task<bool> LoadSongs()
        {
            string url = $"{AuthManager.Instance.ApiUrl}/song/list";
            var response = await ApiClient.Get<UserSongsResponse>(url);
            
            if (response != null)
            {
                _songs = response.songs;
                TotalCompleted = response.totalCompleted;
                TotalScore = response.totalScore;
                return true;
            }
            
            return false;
        }
        
        public SongDto GetSongById(int id)
        {
            return _songs.Find(s => s.id == id);
        }
        
        public void SelectSong(int songId)
        {
            _currentSong = GetSongById(songId);
        }
        
        public async Task<SongProgressResponse> SaveProgress(int songId, int score, int perfectCount, int totalNotes)
        {
            var request = new SongProgressRequest
            {
                songId = songId,
                score = score,
                perfectCount = perfectCount,
                totalNotes = totalNotes
            };
            
            string url = $"{AuthManager.Instance.ApiUrl}/song/progress";
            return await ApiClient.Post<SongProgressResponse>(url, request);
        }
    }
}