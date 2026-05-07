using TapRythm.Enums;

namespace TapRythm.Interfaces
{
    public interface INote
    {
        int LaneIndex { get; }
        float HitTime { get; }
        NoteType Type { get; }
        bool IsHit { get; }
        void OnTap();
        void OnMiss();
    }
}