using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Threading.Tasks;
using TapRythm.Managers;
using TapRythm.Auth;
using UnityEngine.SceneManagement;

namespace TapRythm.UI
{
    public class SongSelectUI : MonoBehaviour
    {
        [Header("Profile")]
        [SerializeField] private TextMeshProUGUI _profileUsernameText;
        [SerializeField] private TextMeshProUGUI _profileScoreText;
        
        [Header("Song List")]
        [SerializeField] private Transform _songListContainer;
        [SerializeField] private GameObject _songButtonPrefab;
        
        [Header("Buttons")]
        [SerializeField] private Button _playButton;
        
        private List<SongDto> _songs;
        private int _selectedIndex = -1;
        private List<SongSelectButton> _buttons = new List<SongSelectButton>();
        
        private async void Start()
        {
            
            if (AuthManager.Instance == null)
            {
                Debug.LogError("AuthManager.Instance = null!");
                SceneManager.LoadScene("AuthScene");
                return;
            }
            
            if (!AuthManager.Instance.IsAuthenticated)
            {
                Debug.LogWarning("Пользователь не авторизован!");
                SceneManager.LoadScene("AuthScene");
                return;
            }
            
            await LoadSongs();
            CreateSongButtons();
            
            _playButton.onClick.AddListener(OnPlaySelected);
            SelectFirstAvailable();
        }
        
        private async Task LoadSongs()
        {
            bool success = await SongLibraryManager.Instance.LoadSongs();
            if (!success)
            {
                Debug.LogError("Не удалось загрузить список песен!");
                return;
            }
            
            _songs = SongLibraryManager.Instance.Songs;
            
            if (_profileUsernameText != null)
                _profileUsernameText.text = AuthManager.Instance.Username;
            
            if (_profileScoreText != null)
                _profileScoreText.text = $"{SongLibraryManager.Instance.TotalScore}";
        }
        
        private void CreateSongButtons()
        {
            
            if (_songs == null || _songs.Count == 0)
            {
                Debug.LogError("CreateSongButtons: список песен пуст!");
                return;
            }
            
            foreach (Transform child in _songListContainer)
                Destroy(child.gameObject);
            _buttons.Clear();
            
            for (int i = 0; i < _songs.Count; i++)
            {
                int index = i;
                GameObject buttonObj = Instantiate(_songButtonPrefab, _songListContainer);
                SongSelectButton button = buttonObj.GetComponent<SongSelectButton>();
                
                if (button != null)
                {
                    button.Setup(_songs[i], () => SelectSong(index));
                    _buttons.Add(button);
                }
            }
        }
        
        private void SelectSong(int index)
        {
            
            if (_songs[index].isUnlocked == false)
            {
                Debug.LogWarning($"SelectSong: песня {_songs[index].songName} закрыта! Игнорируем клик.");
                return;
            }
            
            _selectedIndex = index;
            
            foreach (var btn in _buttons)
                btn.SetSelected(false);
            
            if (index < _buttons.Count)
                _buttons[index].SetSelected(true);
            
            SongDto song = _songs[index];
            _playButton.interactable = song.isUnlocked;
        }
        
        private void SelectFirstAvailable()
        {
            
            for (int i = 0; i < _songs.Count; i++)
            {
                if (_songs[i].isUnlocked)
                {
                    SelectSong(i);
                    return;
                }
            }
            
            if (_songs != null && _songs.Count > 0)
            {
                Debug.LogWarning("Нет открытых песен! Принудительно открываем Level 1");
                _songs[0].isUnlocked = true;
                SelectSong(0);
            }
        }
        
        private void OnPlaySelected()
        {
            
            if (_selectedIndex < 0)
            {
                Debug.LogError("_selectedIndex = -1!");
                return;
            }
            
            if (_selectedIndex >= _songs.Count)
            {
                Debug.LogError($"_selectedIndex={_selectedIndex} >= _songs.Count={_songs.Count}");
                return;
            }
            
            SongDto song = _songs[_selectedIndex];
            
            if (!song.isUnlocked)
            {
                Debug.LogWarning("Песня закрыта!");
                return;
            }
            
            SongLibraryManager.Instance.SelectSong(song.id);
            SceneManager.LoadScene("TimeToGame");
        }
    }
}