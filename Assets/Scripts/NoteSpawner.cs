using UnityEngine;
using System.Collections.Generic;
using TapRythm.Data;
using TapRythm.Notes;
using TapRythm.Enums;

namespace TapRythm.Managers
{
    public class NoteSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _notePrefab;
        [SerializeField] private Transform[] _lanes;
        
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
            if (noteData.lane >= _lanes.Length) return;
            
            NoteType noteType = NoteType.Tap;
            switch (noteData.type.ToLower())
            {
                case "hold": noteType = NoteType.Hold; break;
                case "slide": noteType = NoteType.Slide; break;
            }
            
            SpawnNote(noteData.lane, noteData.beat, noteType);
        }
        
        public void SpawnNote(int lane, float beat, NoteType type = NoteType.Tap)
        {
            if (lane >= _lanes.Length) return;
            
            Vector3 spawnPos = _lanes[lane].position;
            spawnPos.z = _startZ;
            
            GameObject noteObj = Instantiate(_notePrefab, spawnPos, Quaternion.identity);
            Note note = noteObj.GetComponent<Note>();
            
            if (note != null)
            {
                note.Init(lane, beat, type, _noteSpeed, _startZ, _targetZ);
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
    }
}