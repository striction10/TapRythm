using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using TapRythm.Managers;
using TapRythm.Auth;
using System.Threading.Tasks;

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
        [SerializeField] private Button _heartButton;
        [SerializeField] private Image _heartIcon;
        
        [Header("Colors")]
        [SerializeField] private Color _unlockedColor = new Color(0.2f, 0.6f, 1f);
        [SerializeField] private Color _lockedColor = new Color(0.3f, 0.3f, 0.3f);
        [SerializeField] private Color _selectedColor = new Color(1f, 0.8f, 0f);
        [SerializeField] private Color _completedColor = new Color(0.2f, 0.8f, 0.3f);
        [SerializeField] private Color _favoriteColor = new Color(1f, 0.2f, 0.2f);
        
        [Header("Sprites")]
        [SerializeField] private Sprite _defaultCover;
        [SerializeField] private Sprite[] _coverSprites;
        [SerializeField] private Sprite _heartSprite;
        
        private SongDto _currentSong;
        private bool _isSelected = false;
        private bool _isFavorite = false;
        private float _lastClickTime = 0f;
        private const float DOUBLE_CLICK_TIME = 0.3f;
        
        public void Setup(SongDto song, Action onClick)
        {
            _currentSong = song;
            _isFavorite = song.isFavorite;
            
            _songNameText.text = song.songName;
            _artistText.text = song.artist;
            
            if (_coverImage != null)
            {
                if (song.id > 0 && song.id <= _coverSprites.Length)
                    _coverImage.sprite = _coverSprites[song.id - 1];
                else
                    _coverImage.sprite = _defaultCover;
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
            
            UpdateHeartIcon();
            
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => onClick?.Invoke());
            
            if (_heartButton != null)
            {
                _heartButton.onClick.RemoveAllListeners();
                _heartButton.onClick.AddListener(() => {
                    onClick?.Invoke();
                    OnButtonClick();
                });
            }
            
            _button.onClick.AddListener(OnButtonClick);
        }
        
        private void OnButtonClick()
        {
            float timeSinceLastClick = Time.time - _lastClickTime;
            _lastClickTime = Time.time;
            
            if (timeSinceLastClick < DOUBLE_CLICK_TIME && _currentSong != null)
            {
                _ = ToggleFavorite();
            }
        }
        
        private async Task ToggleFavorite()
        {
            if (_currentSong == null) return;
            
            _isFavorite = !_isFavorite;
            UpdateHeartIcon();
            
            var request = new ToggleFavoriteRequest
            {
                songId = _currentSong.id,
                isFavorite = _isFavorite
            };
            
            string url = $"{AuthManager.Instance.ApiUrl}/song/favorite";
            var response = await ApiClient.Post<object>(url, request);
            
            if (response != null)
            {
                _currentSong.isFavorite = _isFavorite;
            }
            else
            {
                _isFavorite = !_isFavorite;
                UpdateHeartIcon();
            }
        }
        
        private void UpdateHeartIcon()
        {
            if (_heartIcon != null)
            {
                bool shouldShow = _isFavorite;
                
                _heartIcon.gameObject.SetActive(shouldShow);
                
                if (shouldShow)
                {
                    _heartIcon.sprite = _heartSprite;
                    _heartIcon.color = _favoriteColor;
                    
                    StopAllCoroutines();
                    StartCoroutine(HeartAppearAnimation());
                }
                else
                {
                    StopAllCoroutines();
                    _heartIcon.transform.localScale = Vector3.one;
                }
            }
        }
        
        private IEnumerator HeartAppearAnimation()
        {
            _heartIcon.transform.localScale = Vector3.zero;
            float elapsed = 0f;
            float duration = 0.3f;
            
            while (elapsed < duration)
            {
                float t = elapsed / duration;
                float scale = Mathf.Sin(t * Mathf.PI / 2f);
                _heartIcon.transform.localScale = Vector3.one * scale;
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            _heartIcon.transform.localScale = Vector3.one;
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
                        _backgroundImage.color = _currentSong.isCompleted ? _completedColor : _unlockedColor;
                    else
                        _backgroundImage.color = _lockedColor;
                }
            }
        }
        
        public SongDto GetSongData() => _currentSong;
    }
}