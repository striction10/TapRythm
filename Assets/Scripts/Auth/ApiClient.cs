using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using System.Threading.Tasks;

namespace TapRythm.Auth
{
    public static class ApiClient
    {
        private static string GetToken()
        {
            return AuthManager.Instance?.Token ?? "";
        }

        public static async Task<T> Get<T>(string url) where T : class
        {
            using var request = UnityWebRequest.Get(url);
            request.SetRequestHeader("Authorization", $"Bearer {GetToken()}");

            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success)
            {
                return JsonUtility.FromJson<T>(request.downloadHandler.text);
            }

            Debug.LogError($"GET error: {request.error}");
            return null;
        }

        public static async Task<T> Post<T>(string url, object data) where T : class
        {
            string json = JsonUtility.ToJson(data);
            byte[] body = Encoding.UTF8.GetBytes(json);

            using var request = new UnityWebRequest(url, "POST");
            request.uploadHandler = new UploadHandlerRaw(body);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {GetToken()}");

            var operation = request.SendWebRequest();

            while (!operation.isDone)
                await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success)
            {
                return JsonUtility.FromJson<T>(request.downloadHandler.text);
            }

            Debug.LogError($"POST error: {request.error}");
            return null;
        }
    }
}