using Melanchall.DryWetMidi.Core;
using SmfLite;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static SongManager;

public class SongManager : MonoBehaviour
{

    private static SongManager instance;
    public static SongManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<SongManager>();
                if (instance == null)
                {
                    GameObject singleton = new GameObject("SongManagerSingleton");
                    instance = singleton.AddComponent<SongManager>();
                }
            }
            return instance;
        }
    }

    public void LoadSongsFromConvertsFolder()
    {
        // Converts 폴더 경로
        string convertsFolderPath = "Assets/Resources/Converts";

        // Converts 폴더 내에 있는 텍스트 파일만 가져오기
        string[] songFiles = Directory.GetFiles(convertsFolderPath, "*.txt");

        // 각 파일에 대해 SongManager에 곡 정보 추가
        foreach (string songFile in songFiles)
        {
            // 파일 이름만 추출 (확장자 제외)
            string songTitle = Path.GetFileNameWithoutExtension(songFile);

            // SongManager에 중복 검사 후 곡 정보 추가
            if (!IsSongAlreadyAdded(songTitle))
            {
                AddSong(songCount, songTitle, "Unknown Composer");
            }
        }

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
    public class Song
    {
        public int songNum;
        public string songTitle;
        public string composer;

        public Song(int songNum, string songTitle, string composer)
        {
            this.songNum = songNum;
            this.songTitle = songTitle;
            this.composer = composer;
        }
    }
    
    public List<Song> songs = new List<Song>();
    public int songCount = 0;

    public void AddSong(int songNum, string songTitle, string composer)
    {
        Song newSong = new Song(songNum, songTitle, composer);
        songs.Add(newSong);
        songCount++;
    }

   


}
