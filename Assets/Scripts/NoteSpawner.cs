using UnityEngine;
using System.Collections.Generic;
using TapRythm.Data;
using TapRythm.Notes;
using TapRythm.Enums;

namespace TapRythm.Managers
{
    public class NoteSpawner : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _notePrefab;
        [SerializeField] private Transform[] _lanes;
        
        [Header("Spawn Settings")]
        [SerializeField] private float _startZ = 10f;
        [SerializeField] private float _targetZ = 2f;
        [SerializeField] private float _noteSpeed = 2f;
        
        private bool _spawningEnabled = true;
        private Queue<NoteData> _pendingNotes = new Queue<NoteData>();
        
        public void SpawnNoteFromData(NoteData noteData)
        {
            if (!_spawningEnabled)
            {
                _pendingNotes.Enqueue(noteData);
                return;
            }
            
            SpawnNoteInternal(noteData);
        }
        
        private void SpawnNoteInternal(NoteData noteData)
        {
            if (noteData.lane >= _lanes.Length)
            {
                Debug.LogWarning($"Нет дорожки {noteData.lane}");
                return;
            }
            
            NoteType noteType = NoteType.Tap;
            float duration = 0f;
            
            switch (noteData.type.ToLower())
            {
                case "hold": 
                    noteType = NoteType.Hold; 
                    duration = noteData.duration > 0 ? noteData.duration : 2f;
                    break;
                case "slide": 
                    noteType = NoteType.Slide; 
                    break;
                default: 
                    noteType = NoteType.Tap; 
                    break;
            }
            
            SpawnNote(noteData.lane, noteData.beat, noteType, duration);
        }
        
        public void SpawnNote(int lane, float beat, NoteType type = NoteType.Tap, float duration = 0f)
        {
            if (lane >= _lanes.Length)
            {
                Debug.LogWarning($"Нет дорожки {lane}");
                return;
            }
            
            Vector3 spawnPos = _lanes[lane].position;
            spawnPos.z = _startZ;
            
            GameObject noteObj = Instantiate(_notePrefab, spawnPos, Quaternion.identity);
            Note note = noteObj.GetComponent<Note>();
            
            if (note != null)
            {
                note.Init(lane, beat, type, _noteSpeed, _startZ, _targetZ, duration);
            }
            else
            {
                Debug.LogError("NotePrefab не содержит компонент Note!");
            }
        }
        
        public void StopSpawning()
        {
            _spawningEnabled = false;
        }
        
        public void StartSpawning()
        {
            _spawningEnabled = true;
            
            while (_pendingNotes.Count > 0)
            {
                SpawnNoteInternal(_pendingNotes.Dequeue());
            }
        }
        
        public void ClearAllNotes()
        {
            Note[] notes = FindObjectsByType<Note>(FindObjectsSortMode.None);
            foreach (Note note in notes)
            {
                Destroy(note.gameObject);
            }
        }
        
        public void SetNoteSpeed(float speed) => _noteSpeed = speed;
        public float TargetZ => _targetZ;
        public float StartZ => _startZ;
        public float NoteSpeed => _noteSpeed;
    }
}