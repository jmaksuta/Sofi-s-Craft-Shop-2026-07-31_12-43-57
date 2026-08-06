using SofisCraftShop.Data;
using SofisCraftShop.Network;
using SofisCraftShop.UI;
using System.Threading.Tasks;
using TMPro;
using Unity.Android.Gradle.Manifest;
using UnityEngine;

public abstract class BaseSceneController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField]
    private GameObject loadingOverlay;
    [SerializeField]
    private TMP_Text loadingStatusText;

    protected async void Start()
    {
        // Check if player has a stored JWT token from a previous session
        if (ApiClient.Instance != null && ApiClient.Instance.IsLoggedIn)
        {
            await TryAutoLogin();
        }
        else
        {
            HideLoading();
            UserNotLoggedIn();
        }
    }

    public abstract void LoginSuccess(SofisCraftShop.Data.PlayerSyncDto syncData);

    public abstract void LoginFailure();

    public abstract void UserNotLoggedIn();


    public void ShowLoading(string message)
    {
        loadingStatusText.text = message;
        loadingOverlay.SetActive(true);
    }
    public void HideLoading()
    {
        loadingOverlay.SetActive(false);
    }


    //// Start is called once before the first execution of Update after the MonoBehaviour is created
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {

    }


    private async Task TryAutoLogin()
    {
        ShowLoading("Restoring session...");

        // Validate token by attempting to fetch initial sync data from backend
        string syncJson = await ApiClient.Instance.GetSyncDataAsync();

        if (!string.IsNullOrEmpty(syncJson))
        {
            SofisCraftShop.Data.PlayerSyncDto syncData = JsonUtility.FromJson<PlayerSyncDto>(syncJson);

            Debug.Log("<color=green>[Auth] Auto-login succeeded!</color>");
            LoginSuccess(syncData);
        }
        else
        {
            // Token expired or invalid — reset token and show login panel
            ApiClient.Instance.Logout();
            HideLoading();
            LoginFailure();
        }
    }

}
