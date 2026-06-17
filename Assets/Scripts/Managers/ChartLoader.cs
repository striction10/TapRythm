using UnityEngine;
using TapRythm.Data;
using System.IO;
using System.Collections.Generic;

namespace TapRythm.Managers
{
    public static class ChartLoader
    {
        public static SongChart LoadFromJSON(string filePath)
        {
            try
            {
                string fullPath = Path.Combine(Application.streamingAssetsPath, filePath);
                
                #if UNITY_EDITOR
                string json = File.ReadAllText(fullPath);
                #else
                string json = Resources.Load<TextAsset>(filePath).text;
                #endif
                
                SongChart chart = JsonUtility.FromJson<SongChart>(json);
                return chart;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Ошибка загрузки карты нот: {e.Message}");
                return null;
            }
        }
        
        public static SongChart CreateTestChart()
        {
            SongChart chart = new SongChart();
            chart.songName = "Test Song";
            chart.artist = "Test Artist";
            chart.bpm = 120;
            chart.offset = 0;
            chart.notes = new List<NoteData>();
            
            for (int i = 0; i < 20; i++)
            {
                chart.notes.Add(new NoteData(i + 1, i % 3, "tap"));
            }
            
            return chart;
        }
    }
}