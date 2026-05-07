using UnityEngine;
using TapRythm.Notes;
using TapRythm.Enums;

public class TestSpawner : MonoBehaviour
{
    [SerializeField] private GameObject _notePrefab;
    [SerializeField] private Transform[] _lanes;
    
    private float _testBeat = 1f;
    
    void Start()
    {
        InvokeRepeating(nameof(SpawnTestNote), 1f, 1f);
    }
    
    void SpawnTestNote()
    {
        if (_notePrefab == null || _lanes.Length == 0) return;
        
        int randomLane = Random.Range(0, _lanes.Length);
        Vector3 spawnPos = _lanes[randomLane].position;
        spawnPos.z = 10f;
        
        GameObject noteObj = Instantiate(_notePrefab, spawnPos, Quaternion.identity);
        Note note = noteObj.GetComponent<Note>();
        
        note.Init(randomLane, _testBeat, NoteType.Tap, 2f, 10f, 2f);
        
        _testBeat += 1f;
    }
}