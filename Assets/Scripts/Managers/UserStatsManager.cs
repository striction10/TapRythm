using UnityEngine;
using TapRythm.Auth;
using TapRythm.Data;
using System.Threading.Tasks;

namespace TapRythm.Managers
{
    public class UserStatsManager : MonoBehaviour
    {
        public static UserStatsManager Instance { get; private set; }
        
        public UserStatsResponse Stats { get; private set; }
        public bool IsLoaded { get; private set; }
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public async Task<bool> LoadStats()
        {
            string url = $"{AuthManager.Instance.ApiUrl}/stats";
            var response = await ApiClient.Get<UserStatsResponse>(url);
            
            if (response != null)
            {
                Stats = response;
                IsLoaded = true;
                return true;
            }
            
            return false;
        }
        
        public async Task<bool> UpdateStats(UpdateStatsRequest request)
        {
            string url = $"{AuthManager.Instance.ApiUrl}/stats/update";
            var response = await ApiClient.Post<object>(url, request);
            
            if (response != null)
            {
                await LoadStats();
                return true;
            }
            
            return false;
        }
    }
}