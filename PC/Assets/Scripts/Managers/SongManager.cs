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
    public List<KeyValuePair<string, int>> curriculumIdx = new List<KeyValuePair<string, int>>();
    public bool isModCurriculum = false;

    public void LoadSongsFromConvertsFolder()
    {
        foreach (Curriculum curriculum in Enum.GetValues(typeof(Curriculum)))
        {
            // Converts ���� ���� �ִ� �ؽ�Ʈ ���ϸ� ��������
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

            // �� ���Ͽ� ���� SongManager�� �� ���� �߰�
            foreach (KeyValuePair<string, string> songFile in songFiles)
            {
                // ���� �̸��� ����
                string songTitle = songFile.Key.Split('-')[0];
                songTitle = songTitle.Replace("_", " ");
                string songComposer = songFile.Key.Split('-')[1];
                songComposer = songComposer.Replace("_", " ");
                // SongManager�� �ߺ� �˻� �� �� ���� �߰� 
                if (!IsSongAlreadyAdded(songTitle))
                {
                    AddSong(songCount, songTitle, songComposer, curriculum);
                }
            }
        }

        songs.Sort((Song a, Song b) => a.songTitle.CompareTo(b.songTitle));

        for (int i = 0; i < songs.Count; i++)
        {
            if (songs[i].curriculum == Curriculum.None)
                continue;
            curriculumIdx.Add(new KeyValuePair<string, int>(songs[i].songTitle, i));
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
    
    public void AddSong(int songNum, string songTitle, string composer, Curriculum curriculum)
    {
        Song newSong = new Song(songNum, songTitle, composer, curriculum);
        songs.Add(newSong);
        songCount++;
    }
}
