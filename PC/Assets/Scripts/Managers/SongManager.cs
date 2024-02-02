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
        // Converts ���� ���
        string convertsFolderPath = "Assets/Resources/Converts";

        // Converts ���� ���� �ִ� �ؽ�Ʈ ���ϸ� ��������
        string[] songFiles = Directory.GetFiles(convertsFolderPath, "*.txt");

        // �� ���Ͽ� ���� SongManager�� �� ���� �߰�
        foreach (string songFile in songFiles)
        {
            // ���� �̸��� ���� (Ȯ���� ����)
            string songTitle = Path.GetFileNameWithoutExtension(songFile);

            // SongManager�� �ߺ� �˻� �� �� ���� �߰�
            if (!IsSongAlreadyAdded(songTitle))
            {
                AddSong(songCount, songTitle, "Unknown Composer");
            }
        }

    }


    // SongManager�� �̹� �ش� ���� �߰��Ǿ� �ִ��� Ȯ���ϴ� �޼���
    private bool IsSongAlreadyAdded(string songTitle)
    {
        foreach (Song song in songs)
        {
            if (song.songTitle == songTitle)
            {
                return true; // �̹� �߰��Ǿ� ����
            }
        }
        return false; // �߰��Ǿ� ���� ����
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
