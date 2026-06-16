namespace TapRythm.Data
{
    [System.Serializable]
    public class UserStatsResponse
    {
        public int totalScore;
        public int totalPlays;
        public int totalPerfect;
        public int totalGreat;
        public int totalGood;
        public int totalMiss;
        public int maxCombo;
        public int songsCompleted;
        public int songsUnlocked;
        public string favoriteSong;
        public int favoriteSongId;
        public float averageAccuracy;
    }

    [System.Serializable]
    public class UpdateStatsRequest
    {
        public int score;
        public int perfectCount;
        public int greatCount;
        public int goodCount;
        public int missCount;
        public int maxCombo;
        public int songId;
    }
}