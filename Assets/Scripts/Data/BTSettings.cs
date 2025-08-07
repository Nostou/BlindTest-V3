using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "BTSettings", menuName = "Data/BTSettings")]
    public class BTSettings : ScriptableObject
    {
        public int Time;
        public int ScoreGolden;
        public int ScoreFirst;
        public int ScoreSecond;
        public bool StreakEnabled;
        public int StreakValue;
        public int StreakMax;
        public int StreakFreeze;
        public bool MusicSmartRandom;
        public bool RankingsSecretMode;
    }
}