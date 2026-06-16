using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System.Text;
using System.Collections;

namespace TapRythm.Auth
{
    [System.Serializable]
    public class RegisterRequest 
    { 
        public string email; 
        public string username; 
        public string password; 
    }
    
    [System.Serializable]
    public class LoginRequest 
    { 
        public string username; 
        public string password; 
    }
    
    [System.Serializable]
    public class AuthResponse 
    { 
        public bool success; 
        public string token; 
        public string username; 
        public int userId; 
        public string message; 
    }

    public class AuthManager : MonoBehaviour
    {
        public static AuthManager Instance { get; private set; }
        
        [Header("API Settings")]
        [SerializeField] private string _apiUrl = "http://localhost:5001/api";
        public string ApiUrl => _apiUrl;
        [SerializeField] private string _gameSceneName = "GameScene";
        
        public string Token { get; private set; }
        public int UserId { get; private set; }
        public string Username { get; private set; }
        public bool IsAuthenticated => !string.IsNullOrEmpty(Token);
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadSession();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public IEnumerator Login(string username, string password, System.Action<bool, string, long> callback)
        {
            var request = new LoginRequest { username = username, password = password };
            string json = JsonUtility.ToJson(request);
            
            using (var req = new UnityWebRequest($"{_apiUrl}/auth/login", "POST"))
            {
                byte[] body = Encoding.UTF8.GetBytes(json);
                req.uploadHandler = new UploadHandlerRaw(body);
                req.downloadHandler = new DownloadHandlerBuffer();
                req.SetRequestHeader("Content-Type", "application/json");
                
                yield return req.SendWebRequest();
                
                long statusCode = req.responseCode;
                
                if (req.result == UnityWebRequest.Result.Success)
                {
                    var resp = JsonUtility.FromJson<AuthResponse>(req.downloadHandler.text);
                    if (resp.success)
                    {
                        Token = resp.token;
                        UserId = resp.userId;
                        Username = resp.username;
                        SaveSession();
                        callback?.Invoke(true, resp.message, statusCode);
                    }
                    else
                    {
                        callback?.Invoke(false, resp.message, statusCode);
                    }
                }
                else
                {
                    string errorMessage = GetErrorMessage(statusCode, req.error);
                    callback?.Invoke(false, errorMessage, statusCode);
                }
            }
        }
        
        public IEnumerator Register(string email, string username, string password, System.Action<bool, string, long> callback)
        {
            var request = new RegisterRequest 
            { 
                email = email, 
                username = username, 
                password = password 
            };
            string json = JsonUtility.ToJson(request);
            
            using (var req = new UnityWebRequest($"{_apiUrl}/auth/register", "POST"))
            {
                byte[] body = Encoding.UTF8.GetBytes(json);
                req.uploadHandler = new UploadHandlerRaw(body);
                req.downloadHandler = new DownloadHandlerBuffer();
                req.SetRequestHeader("Content-Type", "application/json");
                
                yield return req.SendWebRequest();
                
                long statusCode = req.responseCode;
                
                if (req.result == UnityWebRequest.Result.Success)
                {
                    var resp = JsonUtility.FromJson<AuthResponse>(req.downloadHandler.text);
                    callback?.Invoke(resp.success, resp.message, statusCode);
                }
                else
                {
                    string errorMessage = GetErrorMessage(statusCode, req.error);
                    callback?.Invoke(false, errorMessage, statusCode);
                }
            }
        }
        
        private string GetErrorMessage(long statusCode, string error)
        {
            switch (statusCode)
            {
                case 401: return "Неверный логин или пароль";
                case 500: return "Ошибка сервера. Попробуйте позже.";
                case 0: return "Нет подключения к серверу";
                default: return error ?? $"Ошибка {statusCode}";
            }
        }
        
        public void GoToGameScene()
        {
            SceneManager.LoadScene(_gameSceneName);
        }
        
        public void Logout()
        {
            Token = null;
            UserId = 0;
            Username = null;
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
        
        private void SaveSession()
        {
            PlayerPrefs.SetString("Token", Token);
            PlayerPrefs.SetInt("UserId", UserId);
            PlayerPrefs.SetString("Username", Username);
            PlayerPrefs.Save();
        }
        
        private void LoadSession()
        {
            if (PlayerPrefs.HasKey("Token"))
            {
                Token = PlayerPrefs.GetString("Token");
                UserId = PlayerPrefs.GetInt("UserId");
                Username = PlayerPrefs.GetString("Username");
            }
        }
    }
}