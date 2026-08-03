using TMPro;
using UnityEngine;
using UnityEngine.UI;
using SofisCraftShop.Network;
using System.Threading.Tasks;
using UnityEngine.Events;
using System;
using UnityEngine.EventSystems;
using NUnit.Framework.Internal;

public class LoginUIController : MonoBehaviour
{
    [Header("Mode State")]
    [SerializeField]
    private bool isRegisterMode = false;

    [Header("Input Fields")]
    [SerializeField]
    private TMP_InputField usernameInput;
    [SerializeField]
    private TMP_InputField emailInput; // Only visible during Register
    [SerializeField]
    private TMP_InputField passwordInput;
    [SerializeField]
    private GameObject emailFieldGroup;

    [Header("Buttons & UI Labels")]
    [SerializeField]
    private Button submitButton;
    [SerializeField]
    private TMP_Text submitButtonText;
    [SerializeField]
    private Button toggleModeButton;
    [SerializeField]
    private TMP_Text toggleModeText;
    [SerializeField]
    private TMP_Text statusMessageText;

    [Header("Panels")]
    [SerializeField]
    private GameObject loginPanel;
    //[SerializeField]
    //private GameObject mainGameUI; // Next UI state to show after login

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitClicked);
        }
        if (toggleModeButton != null)
        {
            toggleModeButton.onClick.AddListener(ToggleMode);
        }
        UpdateUIState();
    }

    public void AddSubmitButtonListener(UnityAction listener)
    {
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(listener);
        }
    }

    public void RemoveSubmitButtonListener(UnityAction listener)
    {
        if (submitButton != null)
        {
            submitButton.onClick.RemoveListener(listener);
        }
    }

    public event Action OnLogin;

    private void ToggleMode()
    {
        isRegisterMode = !isRegisterMode;
        statusMessageText.text = string.Empty;
        UpdateUIState();
    }

    private void UpdateUIState()
    {
        emailFieldGroup.SetActive(isRegisterMode);
        submitButtonText.text = isRegisterMode ? "Create Account" : "Login";
        toggleModeText.text = isRegisterMode
            ? "Already have an account? <color=#D68298><u>Login</u></color>"
            : "Need an account? <color=#D68298><u>Sign Up</u></color>";
    }

    private async void OnSubmitClicked()
    {
        string username = usernameInput.text.Trim();
        string email = emailInput.text.Trim();
        string password = passwordInput.text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            SetStatus("Please fill in all required fields.", isError: true);
            return;
        }

        if (isRegisterMode && string.IsNullOrEmpty(email))
        {
            SetStatus("Please provide a valid email address.", isError: true);
            return;
        }

        SetStatus("Connecting to server...", isError: false);
        submitButton.interactable = false;

        bool success;
        if (isRegisterMode)
        {
            success = await ApiClient.Instance.RegisterAsync(username, email, password);
        }
        else
        {
            success = await ApiClient.Instance.LoginAsync(username, password);
        }

        submitButton.interactable = true;

        if (success)
        {
            SetStatus("Welcome to Sofi's Craft Shop!", isError: false);

            // Transition to Main Game Loop
            loginPanel.SetActive(false);
            //if (mainGameUI != null) mainGameUI.SetActive(true);

            OnLogin?.Invoke();

        }
        else
        {
            SetStatus("Authentication failed. Please check your credentials.", isError: true);
        }
    }

    private void SetStatus(string message, bool isError)
    {
        statusMessageText.text = message;
        statusMessageText.color = isError ? new Color(0.9f, 0.3f, 0.3f) : new Color(0.3f, 0.8f, 0.4f);
    }

    private async void Test()
    {
        Console.WriteLine("Test");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show()
    {
        this.loginPanel.SetActive(true);
    }

    public void Hide()
    {
        this.loginPanel.SetActive(false);
    }
}
