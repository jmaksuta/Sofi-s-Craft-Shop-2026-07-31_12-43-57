using System;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace SofisCraftShop.Network
{
    [Serializable]
    public class AuthResponse
    {
        public string token;
        public string playerId;
        public string username;
    }

    [Serializable]
    public class RegisterPayload
    {
        public string username;
        public string email;
        public string password;
    }

    [Serializable]
    public class LoginPayload
    {
        public string username;
        public string password;
    }

    [Serializable]
    public class StartCraftPayload
    {
        public string recipeId;
    }

    [Serializable]
    public class ClaimCraftPayload
    {
        public string queueItemId;
    }

    public class ApiClient : MonoBehaviour
    {
        public static ApiClient Instance { get; private set; }

        [Header("Server Configuration")]
        [SerializeField]
        private string baseUrl = "https://localhost:7123/api/v1";

        private const string JWT_PREF_KEY = "SOFI_JWT_TOKEN";
        private string authToken = string.Empty;

        public bool IsLoggedIn => !string.IsNullOrEmpty(authToken);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Load saved token from storage on startup
            authToken = PlayerPrefs.GetString(JWT_PREF_KEY, string.Empty);
        }

        #region Authentication

        public async Task<bool> RegisterAsync(string username, string email, string password)
        {
            var payload = new RegisterPayload { username = username, email = email, password = password };
            string jsonBody = JsonUtility.ToJson(payload);

            string responseJson = await SendPostRequestAsync($"{baseUrl}/Auth/register", jsonBody, requireAuth: false);

            if (!string.IsNullOrEmpty(responseJson))
            {
                AuthResponse authData = JsonUtility.FromJson<AuthResponse>(responseJson);
                if (authData != null && !string.IsNullOrEmpty(authData.token))
                {
                    authToken = authData.token;
                    PlayerPrefs.SetString(JWT_PREF_KEY, authToken);
                    PlayerPrefs.Save();
                    return true;
                }
            }
            return false;
        }

        ///// <summary>
        ///// Sends login request and stores the JWT bearer token upon success.
        ///// </summary>
        //public async Task<bool> LoginAsync(string username)
        //{
        //    string jsonBody = $"{{\"username\":\"{username}\"}}";
        //    string responseJson = await SendPostRequestAsync($"{baseUrl}/Auth/login", jsonBody, requireAuth: false);

        //    if (!string.IsNullOrEmpty(responseJson))
        //    {
        //        AuthResponse authData = JsonUtility.FromJson<AuthResponse>(responseJson);
        //        if (authData != null && !string.IsNullOrEmpty(authData.token))
        //        {
        //            authToken = authData.token;
        //            PlayerPrefs.SetString(JWT_PREF_KEY, authToken);
        //            PlayerPrefs.Save();
        //            Debug.Log("<color=green>[API] Login Successful! JWT Token Saved.</color>");
        //            return true;
        //        }
        //    }

        //    Debug.LogError("[API] Login Failed.");
        //    return false;
        //}

        public async Task<bool> LoginAsync(string username, string password)
        {
            var payload = new LoginPayload { username = username, password = password };
            string jsonBody = JsonUtility.ToJson(payload);

            string responseJson = await SendPostRequestAsync($"{baseUrl}/Auth/login", jsonBody, requireAuth: false);

            if (!string.IsNullOrEmpty(responseJson))
            {
                AuthResponse authData = JsonUtility.FromJson<AuthResponse>(responseJson);
                if (authData != null && !string.IsNullOrEmpty(authData.token))
                {
                    authToken = authData.token;
                    PlayerPrefs.SetString(JWT_PREF_KEY, authToken);
                    PlayerPrefs.Save();
                    return true;
                }
            }
            return false;
        }

        public void Logout()
        {
            authToken = string.Empty;
            PlayerPrefs.DeleteKey(JWT_PREF_KEY);
            Debug.Log("[API] Logged out & token cleared.");
        }

        #endregion

        #region Crafting Endpoints

        public async Task<string> GetSyncDataAsync()
        {
            return await SendGetRequestAsync($"{baseUrl}/Crafting/sync");
        }

        public async Task<string> RequestStartCraftAsync(string recipeId)
        {
            var payload = new StartCraftPayload { recipeId = recipeId };
            string jsonBody = JsonUtility.ToJson(payload);
            return await SendPostRequestAsync($"{baseUrl}/Crafting/start", jsonBody);
        }

        public async Task<string> RequestClaimCraftAsync(string queueItemId)
        {
            var payload = new ClaimCraftPayload { queueItemId = queueItemId };
            string jsonBody = JsonUtility.ToJson(payload);
            return await SendPostRequestAsync($"{baseUrl}/Crafting/claim", jsonBody);
        }

        #endregion

        #region Core WebRequest Pipeline

        private async Task<string> SendGetRequestAsync(string uri)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(uri))
            {
#if UNITY_EDITOR
                request.certificateHandler = new BypassDevCertificateHandler();
#endif
                AttachHeaders(request, requireAuth: true);
                var operation = request.SendWebRequest();

                while (!operation.isDone) await Task.Yield();

                return HandleResponse(request);
            }
        }

        private async Task<string> SendPostRequestAsync(string uri, string jsonPayload, bool requireAuth = true)
        {
            using (UnityWebRequest request = new UnityWebRequest(uri, "POST"))
            {
#if UNITY_EDITOR
                request.certificateHandler = new BypassDevCertificateHandler();
#endif
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonPayload);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();

                AttachHeaders(request, requireAuth);

                var operation = request.SendWebRequest();

                while (!operation.isDone) await Task.Yield();

                return HandleResponse(request);
            }
        }

        private void AttachHeaders(UnityWebRequest request, bool requireAuth)
        {
            request.SetRequestHeader("Content-Type", "application/json");

            // Attach Bearer token header if available and endpoint requires authentication
            if (requireAuth && !string.IsNullOrEmpty(authToken))
            {
                request.SetRequestHeader("Authorization", $"Bearer {authToken}");
            }
        }

        private string HandleResponse(UnityWebRequest request)
        {
            if (request.result == UnityWebRequest.Result.Success)
            {
                return request.downloadHandler.text;
            }

            if (request.responseCode == 401)
            {
                Debug.LogError("[API] 401 Unauthorized - JWT Token expired or invalid.");
            }
            else
            {
                Debug.LogError($"[API] Request Failed ({request.responseCode}): {request.error}\n{request.downloadHandler.text}");
            }

            return null;
        }

        #endregion


        // Helper handler for ignoring self-signed dev certificates in Unity Editor
        public class BypassDevCertificateHandler : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                return true; // Allows local HTTPS debugging
            }
        }

    }
}