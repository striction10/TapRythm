using UnityEngine;
using TapRythm.Data;
using TapRythm.Managers;
using TapRythm.Notes;
using System.Collections;
using System.IO;
using UnityEngine.Networking;

namespace TapRythm.UI
{
    public class SongPicker : MonoBehaviour
    {
        [Header("Song Settings")]
        [SerializeField] private string _songFolder = "Songs/Level1";
        [SerializeField] private string _songFileName = "song.mp3";
        [SerializeField] private string _chartFileName = "chart.json";
        [SerializeField] private NoteSpawner _noteSpawner;
        
        private void Start()
        {
            StartCoroutine(LoadAndPlaySong());
        }
        
        private IEnumerator LoadAndPlaySong()
        {
            SongChart chart = ChartLoader.LoadFromStreamingAssets(_songFolder, _chartFileName);
            if (chart == null)
            {
                yield break;
            }
            
            string audioPath = Path.Combine(Application.streamingAssetsPath, _songFolder, _songFileName);
            
            string url = "file://" + audioPath;
            
            using (UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG))
            {
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    AudioClip clip = DownloadHandlerAudioClip.GetContent(request);
                    
                    SongManager.Instance.LoadSong(chart, clip, OnNoteSpawn);
                    SongManager.Instance.PlaySong();
                    ScoreManager.Instance.ResetScore();
                }
            }
        }
        
        private void OnNoteSpawn(NoteData noteData)
        {
            _noteSpawner.SpawnNoteFromData(noteData);
        }
    }
}