using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using SofisCraftShop.Network;
using System;

namespace SofisCraftShop.UI
{
    public class LoginSceneManager : MonoBehaviour
    {
        [Header("UI Panels")]
        [SerializeField] private GameObject authPanel;
        [SerializeField] private GameObject loadingOverlay;
        [SerializeField] private TMP_Text loadingStatusText;

        [Header("Authentication")]
        [SerializeField]
        private LoginUIController loginUIController;

        //[Header("Auth Input Fields")]
        //[SerializeField] private TMP_InputField usernameInput;
        //[SerializeField] private TMP_InputField emailInput;
        //[SerializeField] private TMP_InputField passwordInput;
        //[SerializeField] private GameObject emailGroup;

        //[Header("Buttons & Labels")]
        //[SerializeField] private Button submitButton;
        //[SerializeField] private TMP_Text submitButtonText;
        //[SerializeField] private Button toggleModeButton;
        //[SerializeField] private TMP_Text toggleModeText;
        [SerializeField] private TMP_Text errorText;

        [Header("Scene Config")]
        [SerializeField]
        private string mainGameSceneName = "Scene_MainGame";

        //private bool isRegisterMode = false;

        private async void Start()
        {
            if (this.loginUIController != null)
            {
                this.loginUIController.OnLogin += OnLoginControllerLogin;


                //this.loginUIController.SetActive(true);
            }
            //submitButton.onClick.AddListener(OnSubmitClicked);
            //toggleModeButton.onClick.AddListener(ToggleAuthMode);

            //UpdateAuthModeUI();
            errorText.text = string.Empty;

            // Check if player has a stored JWT token from a previous session
            if (ApiClient.Instance != null && ApiClient.Instance.IsLoggedIn)
            {
                await TryAutoLogin();
            }
            else
            {
                HideLoading();
                ShowLoginUI();
            }
        }

        private void OnLoginControllerLogin()
        {
            TransitionToGame();
        }

        private void OnLoginControllerFailed()
        {
            HideLoading();
            ShowError("Authentication failed. Please check your credentials.");
        }

        private async Task TryAutoLogin()
        {
            ShowLoading("Restoring session...");

            // Validate token by attempting to fetch initial sync data from backend
            string syncJson = await ApiClient.Instance.GetSyncDataAsync();

            if (!string.IsNullOrEmpty(syncJson))
            {
                Debug.Log("<color=green>[Auth] Auto-login succeeded!</color>");
                OnLoginControllerLogin();
            }
            else
            {
                // Token expired or invalid — reset token and show login panel
                ApiClient.Instance.Logout();
                HideLoading();
            }
        }

        //private void ToggleAuthMode()
        //{
        //    isRegisterMode = !isRegisterMode;
        //    errorText.text = string.Empty;
        //    UpdateAuthModeUI();
        //}

        //private void UpdateAuthModeUI()
        //{
        //    emailGroup.SetActive(isRegisterMode);
        //    submitButtonText.text = isRegisterMode ? "Create Account" : "Enter Shop";
        //    toggleModeText.text = isRegisterMode
        //        ? "Already have an account? <color=#D68298><u>Login</u></color>"
        //        : "New around here? <color=#D68298><u>Create Account</u></color>";
        //}

        //private async void OnSubmitClicked()
        //{
        //    string username = usernameInput.text.Trim();
        //    string email = emailInput.text.Trim();
        //    string password = passwordInput.text;

        //    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        //    {
        //        ShowError("Please enter both username and password.");
        //        return;
        //    }

        //    if (isRegisterMode && string.IsNullOrEmpty(email))
        //    {
        //        ShowError("Please enter a valid email address.");
        //        return;
        //    }

        //    ShowLoading(isRegisterMode ? "Creating account..." : "Logging in...");

        //    bool success;
        //    if (isRegisterMode)
        //    {
        //        success = await ApiClient.Instance.RegisterAsync(username, email, password);
        //    }
        //    else
        //    {
        //        success = await ApiClient.Instance.LoginAsync(username, password);
        //    }

        //    if (success)
        //    {
        //        TransitionToGame();
        //    }
        //    else
        //    {
        //        HideLoading();
        //        ShowError("Authentication failed. Please check your credentials.");
        //    }
        //}



        private void TransitionToGame()
        {
            ShowLoading("Loading shop...");
            SceneManager.LoadScene(mainGameSceneName);
        }

        private void ShowLoading(string message)
        {
            loadingStatusText.text = message;
            loadingOverlay.SetActive(true);
            authPanel.SetActive(false);
        }

        private void HideLoading()
        {
            loadingOverlay.SetActive(false);
            authPanel.SetActive(true);
        }

        private void ShowLoginUI()
        {
            if (this.loginUIController != null)
            {
                this.loginUIController.Show();
            }
        }

        private void HideLoginUI()
        {
            if (this.loginUIController != null)
            {
                this.loginUIController.Hide();
            }
        }

        private void ShowError(string message)
        {
            errorText.text = message;
        }
    }
}