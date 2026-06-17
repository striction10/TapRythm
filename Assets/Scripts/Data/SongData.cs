using System;
using System.Collections.Generic;

namespace TapRythm.Data
{
    [Serializable]
    public class NoteData
    {
        public float beat;
        public int lane;
        public string type;
        public float duration;
        
        public NoteData(float beat, int lane, string type = "tap")
        {
            this.beat = beat;
            this.lane = lane;
            this.type = type;
            this.duration = 0f;
        }
    }
    
    [Serializable]
    public class SongChart
    {
        public string songName;
        public string artist;
        public float bpm;
        public float offset;
        public List<NoteData> notes;
        
        public SongChart()
        {
            notes = new List<NoteData>();
        }
    }
}