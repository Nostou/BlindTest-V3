using System.Collections.Generic;
using Extensions;
using Managers;
using UnityEngine;

namespace Utils
{
    public class AudioShuffler
    {
        public void CustomShuffle(List<Music> musicList)
        {
            bool isSmartRandom = GameManager.Instance.GetCurrentSettings().MusicSmartRandom;
            
            if (!isSmartRandom)
            {
                musicList.Shuffle();
                return;
            }

            List<BTPlayer> players = GameManager.Instance.Players;
            int nbMusicPerPlayer = musicList.Count / players.Count;
            int half = nbMusicPerPlayer / 2;
            
            Dictionary<BTPlayer, List<Music>> firstHalf = new Dictionary<BTPlayer, List<Music>>();
            Dictionary<BTPlayer, List<Music>> secondHalf = new Dictionary<BTPlayer, List<Music>>();
            
            foreach (BTPlayer player in players)
            {
                firstHalf.Add(player, new List<Music>());
                secondHalf.Add(player, new List<Music>());
            }

            foreach (Music music in musicList)
            {
                BTPlayer player = players.Find(p => p.Name.Equals(music.Author));
                if (firstHalf[player].Count < half) firstHalf[player].Add(music);
                else secondHalf[player].Add(music);
            }
            
            List<Music> firstList = new List<Music>();
            List<Music> secondList = new List<Music>();
            
            foreach (BTPlayer player in players)
            {
                firstList.AddRange(firstHalf[player]);
                secondList.AddRange(secondHalf[player]);
            }
            
            firstList.Shuffle();
            secondList.Shuffle();
            
            List<Music> endList = new List<Music>();
            foreach (BTPlayer player in players)
            {
                for (int i = 0; i < secondList.Count; i++)
                {
                    if (!secondList[i].Author.Equals(player.Name)) continue;
                    endList.Add(secondList[i]);
                    secondList.RemoveAt(i);
                    break;
                }
            }
            endList.Shuffle();
            
            musicList.Clear();
            musicList.AddRange(firstList);
            musicList.AddRange(secondList);
            musicList.AddRange(endList);
            PreventOccurrencesInARow(musicList, 2);
        }
        
        private void PreventOccurrencesInARow(List<Music> musicList, int maxOccurrences)
        {
            int consecutiveCount = 1;
            for (int i = 1; i < musicList.Count; i++) 
            {
                if (musicList[i].Author.Equals(musicList[i - 1].Author)) consecutiveCount++;
                else consecutiveCount = 1;

                if (consecutiveCount <= maxOccurrences) continue;
                
                Debug.Log($"[AudioManager] Found more than {maxOccurrences} musics in a row for {musicList[i].Author}");

                for (int j = i + 1; j < musicList.Count; j++)
                {
                    if (musicList[j].Author.Equals(musicList[i].Author)) continue;
                    
                    (musicList[i], musicList[j]) = (musicList[j], musicList[i]);
                    consecutiveCount = 1;
                    break;
                }
            }
        }
    }
}