using UnityEngine;
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
        
        /// <summary>
        /// Спавн ноты по данным из карты (используется SongManager)
        /// </summary>
        public void SpawnNoteFromData(NoteData noteData)
        {
            if (noteData.lane >= _lanes.Length)
            {
                return;
            }
            
            NoteType noteType = NoteType.Tap;
            switch (noteData.type.ToLower())
            {
                case "hold": noteType = NoteType.Hold; break;
                case "slide": noteType = NoteType.Slide; break;
                default: noteType = NoteType.Tap; break;
            }
            
            SpawnNote(noteData.lane, noteData.beat, noteType);
        }
        
        /// <summary>
        /// Базовый метод спавна ноты
        /// </summary>
        public void SpawnNote(int lane, float beat, NoteType type = NoteType.Tap)
        {
            if (lane >= _lanes.Length)
            {
                return;
            }
            
            Vector3 spawnPos = _lanes[lane].position;
            spawnPos.z = _startZ;
            
            GameObject noteObj = Instantiate(_notePrefab, spawnPos, Quaternion.identity);
            Note note = noteObj.GetComponent<Note>();
            
            if (note != null)
            {
                note.Init(lane, beat, type, _noteSpeed, _startZ, _targetZ);
            }
        }
        
        /// <summary>
        /// Очистка всех нот на сцене
        /// </summary>
        public void ClearAllNotes()
        {
            Note[] notes = FindObjectsByType<Note>(FindObjectsSortMode.None);
            foreach (Note note in notes)
            {
                Destroy(note.gameObject);
            }
        }
        
        /// <summary>
        /// Обновить скорость движения нот
        /// </summary>
        public void SetNoteSpeed(float speed)
        {
            _noteSpeed = speed;
        }
        
        /// <summary>
        /// Получить позицию зоны попадания
        /// </summary>
        public float TargetZ => _targetZ;
        
        /// <summary>
        /// Получить стартовую позицию
        /// </summary>
        public float StartZ => _startZ;
        
        /// <summary>
        /// Получить скорость нот
        /// </summary>
        public float NoteSpeed => _noteSpeed;
    }
}