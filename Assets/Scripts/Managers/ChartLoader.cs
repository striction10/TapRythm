using UnityEngine;
using TapRythm.Data;
using System.IO;

namespace TapRythm.Managers
{
    public static class ChartLoader
    {
        public static SongChart LoadFromStreamingAssets(string folderPath, string fileName)
        {
            string fullPath = Path.Combine(Application.streamingAssetsPath, folderPath, fileName);
            
            #if UNITY_EDITOR
            if (File.Exists(fullPath))
            {
                string json = File.ReadAllText(fullPath);
                SongChart chart = JsonUtility.FromJson<SongChart>(json);
                return chart;
            }
            #else
            #endif
            return null;
        }
    }
}