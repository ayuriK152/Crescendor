using System;
using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SongManager
{
    public List<Song> songs = new List<Song>();
    public List<Song> curriculumSongs = new List<Song>();
    public int songCount = 0;
    public Song selectedSong = null;
    public Curriculum selectedCurriculum = Curriculum.Hanon;
    public Dictionary<Curriculum, int> curriculumSongMounts = new Dictionary<Curriculum, int>();
    public bool isModCurriculum = false;

    public void LoadSongsFromConvertsFolder()
    {
        foreach (Curriculum curriculum in Enum.GetValues(typeof(Curriculum)))
        {
            // Converts 폴더 내에 있는 텍스트 파일만 가져오기
            TextAsset[] originSongFiles = Resources.LoadAll<TextAsset>("Converts");
            if (curriculum == Curriculum.None)
            {
                originSongFiles = Resources.LoadAll<TextAsset>("Converts");
            }
            else
            {
                originSongFiles = Resources.LoadAll<TextAsset>($"Converts/{curriculum}");
            }
            List<KeyValuePair<string, string>> songFiles = new List<KeyValuePair<string, string>>();
            foreach (TextAsset originSongFile in originSongFiles)
            {
                songFiles.Add(new KeyValuePair<string, string>(originSongFile.name, originSongFile.text));
            }

            // 각 파일에 대해 SongManager에 곡 정보 추가
            foreach (KeyValuePair<string, string> songFile in songFiles)
            {
                // 파일 이름만 추출
                string songTitle = songFile.Key.Split('-')[0];
                songTitle = songTitle.Replace("_", " ");
                string songComposer = songFile.Key.Split('-')[1];
                songComposer = songComposer.Replace("_", " ");

                // SongManager에 중복 검사 후 곡 정보 추가
                if (!IsSongAlreadyAdded(songTitle))
                {
                    AddSong(songCount, songTitle, songComposer, curriculum);
                }
            }
        }

        songs.Sort((Song a, Song b) => a.songTitle.CompareTo(b.songTitle));
    }

    // SongManager에 이미 해당 곡이 추가되어 있는지 확인하는 메서드
    private bool IsSongAlreadyAdded(string songTitle)
    {
        foreach (Song song in songs)
        {
            if (song.songTitle == songTitle)
            {
                return true; // 이미 추가되어 있음
            }
        }
        return false; // 추가되어 있지 않음
    }

    public void AddSong(int songNum, string songTitle, string composer, Curriculum curriculum)
    {
        Song newSong = new Song(songNum, songTitle, composer, curriculum);
        songs.Add(newSong);
        songCount++;
    }
}
