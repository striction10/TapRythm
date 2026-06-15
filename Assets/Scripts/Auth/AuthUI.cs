using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace TapRythm.Auth
{
    public class AuthUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject _loginPanel;
        [SerializeField] private GameObject _registerPanel;
        [SerializeField] private GameObject _loadingPanel;
        
        [Header("Login")]
        [SerializeField] private InputField _loginUsername;
        [SerializeField] private InputField _loginPassword;
        [SerializeField] private TMP_Text _loginError;
        [SerializeField] private Button _loginButton;
        [SerializeField] private Button _toRegisterButton;
        
        [Header("Register")]
        [SerializeField] private InputField _regEmail;
        [SerializeField] private InputField _regUsername;
        [SerializeField] private InputField _regPassword;
        [SerializeField] private InputField _regConfirmPassword;
        [SerializeField] private TMP_Text _regError;
        [SerializeField] private Button _registerButton;
        
        private Coroutine _loginErrorCoroutine;
        private Coroutine _regErrorCoroutine;
        private static bool _isManualLogout = false;
        
        private void Start()
        {
            if (AuthManager.Instance == null)
            {
                Debug.LogError("AuthManager.Instance = null!");
                return;
            }
            
            if (_isManualLogout)
            {
                _isManualLogout = false;
                ShowLoginPanel();
                SetupButtons();
                return;
            }
            
            if (AuthManager.Instance.IsAuthenticated)
            {
                AuthManager.Instance.GoToGameScene();
                return;
            }
            
            SetupButtons();
            ShowLoginPanel();
        }
        
        private void SetupButtons()
        {
            _loginButton.onClick.RemoveAllListeners();
            _registerButton.onClick.RemoveAllListeners();
            _toRegisterButton.onClick.RemoveAllListeners();
            
            _loginButton.onClick.AddListener(() => StartCoroutine(OnLogin()));
            _registerButton.onClick.AddListener(() => StartCoroutine(OnRegister()));
            _toRegisterButton.onClick.AddListener(ShowRegisterPanel);
        }
        
        public static void SetManualLogout()
        {
            _isManualLogout = true;
        }
        
        private IEnumerator OnLogin()
        {
            string username = _loginUsername.text.Trim();
            string password = _loginPassword.text;
            
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowLoginError("Заполните все поля");
                yield break;
            }
            
            SetLoading(true);
            
            yield return AuthManager.Instance.Login(username, password, (success, message, statusCode) =>
            {
                SetLoading(false);
                
                if (success)
                {
                    AuthManager.Instance.GoToGameScene();
                }
                else
                {
                    if (statusCode == 401)
                        ShowLoginError("Неверный логин или пароль");
                    else if (statusCode == 500)
                        ShowLoginError("Ошибка сервера. Попробуйте позже.");
                    else
                        ShowLoginError(message ?? "Ошибка входа");
                }
            });
        }
        
        private IEnumerator OnRegister()
        {
            string email = _regEmail.text.Trim();
            string username = _regUsername.text.Trim();
            string password = _regPassword.text;
            string confirm = _regConfirmPassword.text;
            
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowRegError("Заполните все поля");
                yield break;
            }
            
            if (!email.Contains("@") || !email.Contains("."))
            {
                ShowRegError("Введите корректный email");
                yield break;
            }
            
            if (username.Length < 3)
            {
                ShowRegError("Логин минимум 3 символа");
                yield break;
            }
            
            if (password != confirm)
            {
                ShowRegError("Пароли не совпадают");
                yield break;
            }
            
            if (password.Length < 3)
            {
                ShowRegError("Пароль минимум 3 символа");
                yield break;
            }
            
            SetLoading(true);
            
            yield return AuthManager.Instance.Register(email, username, password, (success, message, statusCode) =>
            {
                SetLoading(false);
                
                if (success)
                {
                    ShowRegError("Регистрация успешна! Теперь войдите.", false);
                    
                    _regEmail.text = "";
                    _regUsername.text = "";
                    _regPassword.text = "";
                    _regConfirmPassword.text = "";
                    
                    _loginUsername.text = username;
                    ShowLoginPanel();
                }
                else
                {
                    ShowRegError(message ?? "Ошибка регистрации");
                }
            });
        }
        
        private void ShowLoginError(string message, bool isError = true)
        {
            if (_loginErrorCoroutine != null)
                StopCoroutine(_loginErrorCoroutine);
            
            _loginError.text = message;
            _loginError.color = isError ? Color.red : Color.green;
            _loginErrorCoroutine = StartCoroutine(ClearLoginErrorAfterDelay());
        }
        
        private IEnumerator ClearLoginErrorAfterDelay()
        {
            yield return new WaitForSeconds(2f);
            _loginError.text = "";
        }
        
        private void ShowRegError(string message, bool isError = true)
        {
            if (_regErrorCoroutine != null)
                StopCoroutine(_regErrorCoroutine);
            
            _regError.text = message;
            _regError.color = isError ? Color.red : Color.green;
            _regErrorCoroutine = StartCoroutine(ClearRegErrorAfterDelay());
        }
        
        private IEnumerator ClearRegErrorAfterDelay()
        {
            yield return new WaitForSeconds(2f);
            _regError.text = "";
        }
        
        private void SetLoading(bool isLoading)
        {
            _loginPanel.SetActive(!isLoading);
            _registerPanel.SetActive(false);
            if (_loadingPanel != null) _loadingPanel.SetActive(isLoading);
        }
        
        private void ShowLoginPanel()
        {
            _loginPanel.SetActive(true);
            _registerPanel.SetActive(false);
            _loadingPanel.SetActive(false);
            _loginError.text = "";
            _regError.text = "";
        }
        
        private void ShowRegisterPanel()
        {
            _loginPanel.SetActive(false);
            _registerPanel.SetActive(true);
            _loadingPanel.SetActive(false);
            _loginError.text = "";
            _regError.text = "";
        }
    }
}