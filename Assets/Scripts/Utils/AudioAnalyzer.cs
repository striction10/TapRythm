using UnityEngine;
using System.Collections.Generic;
using TapRythm.Data;

namespace TapRythm.Utils
{
    public static class AudioAnalyzer
    {
        private static int[][] _patterns = new int[][]
        {
            new int[] { 0, 1, 2, 0 },
            new int[] { 2, 1, 0, 2 },
            new int[] { 0, 2, 0, 2 },
            new int[] { 0, 1, 0, 1 },
            new int[] { 2, 0, 1, 0 },
        };
        
        public static SongChart GenerateChartFromAudio(AudioClip clip, float bpm, int lanes = 3)
        {
            SongChart chart = new SongChart();
            chart.songName = clip.name;
            chart.artist = "Unknown";
            chart.bpm = bpm;
            chart.offset = 0;
            chart.notes = new List<NoteData>();
            
            float[] samples = new float[clip.samples * clip.channels];
            
            try
            {
                clip.GetData(samples, 0);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Не удалось получить данные аудио: {e.Message}");
                return GenerateSimpleChart(bpm, lanes);
            }
            
            float secPerBeat = 60f / bpm;
            
            // Детектируем биты с ЧУВСТВИТЕЛЬНЫМ ПОРОГОМ
            List<float> beatTimes = DetectBeats(samples, clip.frequency, clip.channels, secPerBeat);
            
            if (beatTimes.Count < 4)
            {
                Debug.LogWarning("Не удалось найти биты в песне! Используем простой ритм.");
                return GenerateSimpleChart(bpm, lanes);
            }
            
            // Увеличиваем количество нот (берём каждый второй бит вместо каждого четвёртого)
            List<float> filteredBeats = new List<float>();
            for (int i = 0; i < beatTimes.Count; i += 1)
            {
                filteredBeats.Add(beatTimes[i]);
            }
            beatTimes = filteredBeats;
            
            Debug.Log($"Обнаружено {beatTimes.Count} битов для генерации нот");
            
            int patternIndex = Random.Range(0, _patterns.Length);
            int[] pattern = _patterns[patternIndex];
            int patternCounter = 0;
            
            bool holdActive = false;
            float holdStartBeat = 0f;
            float holdDuration = 0f;
            
            for (int i = 0; i < beatTimes.Count; i++)
            {
                float beat = beatTimes[i];
                
                int lane = pattern[patternCounter % pattern.Length];
                patternCounter++;
                
                bool isHold = false;
                
                // HOLD если пауза > 1.5 удара
                if (i + 1 < beatTimes.Count)
                {
                    float nextBeat = beatTimes[i + 1];
                    float gap = nextBeat - beat;
                    
                    if (gap > 1.5f && gap < 4f && !holdActive)
                    {
                        isHold = true;
                        holdDuration = Mathf.Min(gap, 2.5f);
                        holdActive = true;
                        holdStartBeat = beat;
                    }
                }
                
                if (holdActive && beat - holdStartBeat > holdDuration)
                {
                    holdActive = false;
                }
                
                if (isHold)
                {
                    chart.notes.Add(new NoteData(beat, lane, "hold") { duration = holdDuration });
                }
                else if (!holdActive || beat - holdStartBeat < 0.5f)
                {
                    chart.notes.Add(new NoteData(beat, lane, "tap"));
                }
                
                // ДОБАВЛЯЕМ БОЛЬШЕ ДОПОЛНИТЕЛЬНЫХ НОТ
                if (i % 2 == 0)
                {
                    int lane2 = (lane + 1) % lanes;
                    chart.notes.Add(new NoteData(beat + 0.5f, lane2, "tap"));
                }
                
                if (i % 4 == 0 && i > 0)
                {
                    int lane2 = (lane + 1) % lanes;
                    chart.notes.Add(new NoteData(beat + 0.75f, lane2, "tap"));
                }
            }
            
            // Ограничиваем, чтобы не было слишком много
            if (chart.notes.Count > 250)
            {
                // Уменьшаем количество дополнительных нот
                List<NoteData> reducedNotes = new List<NoteData>();
                for (int i = 0; i < chart.notes.Count; i += 2)
                {
                    reducedNotes.Add(chart.notes[i]);
                }
                chart.notes = reducedNotes;
            }
            
            Debug.Log($"Сгенерировано {chart.notes.Count} нот для песни {clip.name}");
            return chart;
        }
        
        private static List<float> DetectBeats(float[] samples, int frequency, int channels, float secPerBeat)
        {
            List<float> beats = new List<float>();
            
            int sampleStep = Mathf.FloorToInt(secPerBeat / 6 * frequency * channels);
            
            // НИЗКИЙ ПОРОГ для большей чувствительности
            float threshold = 0.035f;  // Было 0.04 → стало 0.035
            
            for (int i = 0; i < samples.Length - sampleStep; i += sampleStep / 2)
            {
                float energy = 0f;
                for (int j = 0; j < sampleStep; j++)
                {
                    if (i + j < samples.Length)
                        energy += Mathf.Abs(samples[i + j]);
                }
                
                energy /= sampleStep;
                
                if (energy > threshold)
                {
                    float beatTime = (float)i / (frequency * channels);
                    if (beats.Count == 0 || beatTime - beats[beats.Count - 1] > secPerBeat * 0.25f)
                    {
                        beats.Add(beatTime);
                    }
                }
            }
            
            return beats;
        }
        
        public static SongChart GenerateSimpleChart(float bpm, int lanes = 3)
        {
            SongChart chart = new SongChart();
            chart.songName = "Auto Generated";
            chart.artist = "Unknown";
            chart.bpm = bpm;
            chart.offset = 0;
            chart.notes = new List<NoteData>();
            
            int[] pattern = { 0, 1, 2, 0 };
            int counter = 0;
            
            for (int i = 1; i < 100; i += 2)
            {
                int lane = pattern[counter % pattern.Length];
                counter++;
                
                if (i % 8 == 0 && i > 0)
                {
                    chart.notes.Add(new NoteData(i, lane, "hold") { duration = 2f });
                }
                else
                {
                    chart.notes.Add(new NoteData(i, lane, "tap"));
                }
                
                if (i % 4 == 0)
                {
                    int lane2 = (lane + 1) % lanes;
                    chart.notes.Add(new NoteData(i + 1, lane2, "tap"));
                }
            }
            
            return chart;
        }
    }
}