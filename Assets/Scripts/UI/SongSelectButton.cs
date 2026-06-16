using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using TapRythm.Managers;

namespace TapRythm.UI
{
    public class SongSelectButton : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image _coverImage;
        [SerializeField] private TextMeshProUGUI _songNameText;
        [SerializeField] private TextMeshProUGUI _artistText;
        [SerializeField] private TextMeshProUGUI _statusText;
        [SerializeField] private TextMeshProUGUI _highScoreText;
        [SerializeField] private Image _lockIcon;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Button _button;
        
        [Header("Colors")]
        [SerializeField] private Color _unlockedColor = new Color(0.2f, 0.6f, 1f);
        [SerializeField] private Color _lockedColor = new Color(0.3f, 0.3f, 0.3f);
        [SerializeField] private Color _selectedColor = new Color(1f, 0.8f, 0f);
        [SerializeField] private Color _completedColor = new Color(0.2f, 0.8f, 0.3f);
        
        [Header("Cover Sprites")]
        [SerializeField] private Sprite _defaultCover;
        [SerializeField] private Sprite[] _coverSprites;
        
        private SongDto _currentSong;
        private bool _isSelected = false;
        
        public void Setup(SongDto song, Action onClick)
        {
            _currentSong = song;
            
            _songNameText.text = song.songName;
            _artistText.text = song.artist;
            
            if (_coverImage != null)
            {
                if (song.id > 0 && song.id <= _coverSprites.Length)
                {
                    _coverImage.sprite = _coverSprites[song.id - 1];
                }
                else
                {
                    _coverImage.sprite = _defaultCover;
                }
            }
            
            if (song.isUnlocked)
            {
                _lockIcon.gameObject.SetActive(false);
                _button.interactable = true;
                
                if (song.isCompleted)
                {
                    _statusText.text = "ПРОЙДЕНА";
                    _statusText.color = Color.green;
                    _backgroundImage.color = _completedColor;
                }
                else
                {
                    _statusText.text = "ДОСТУПНА";
                    _statusText.color = Color.white;
                    _backgroundImage.color = _unlockedColor;
                }
                
                _highScoreText.text = song.highScore > 0 ? $"{song.highScore} очков" : "Не пройдена";
                _highScoreText.color = Color.yellow;
            }
            else
            {
                _lockIcon.gameObject.SetActive(true);
                _statusText.text = "ЗАКРЫТА";
                _statusText.color = Color.gray;
                _backgroundImage.color = _lockedColor;
                _button.interactable = false;
                _highScoreText.text = "";
                _highScoreText.color = Color.gray;
            }
            
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => onClick?.Invoke());
        }
        
        public void SetSelected(bool selected)
        {
            _isSelected = selected;
            
            if (selected)
            {
                _backgroundImage.color = _selectedColor;
                transform.localScale = new Vector3(1.1f, 1.1f, 1f);
            }
            else
            {
                transform.localScale = Vector3.one;
                
                if (_currentSong != null)
                {
                    if (_currentSong.isUnlocked)
                    {
                        _backgroundImage.color = _currentSong.isCompleted ? _completedColor : _unlockedColor;
                    }
                    else
                    {
                        _backgroundImage.color = _lockedColor;
                    }
                }
            }
        }
        
        public SongDto GetSongData()
        {
            return _currentSong;
        }
    }
}