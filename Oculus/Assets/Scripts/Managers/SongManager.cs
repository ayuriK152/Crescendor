using System.Collections.Generic;
using UnityEngine;
using static Define;

public class SongManager
{
    public List<Song> songs = new List<Song>();
    public int songCount = 0;

    public void LoadSongsFromConvertsFolder()
    {
        // Converts ���� ���� �ִ� �ؽ�Ʈ ���ϸ� ��������
        TextAsset[] originSongFiles = Resources.LoadAll<TextAsset>("Converts");
        List<KeyValuePair<string, string>> songFiles = new List<KeyValuePair<string, string>>();
        foreach (TextAsset originSongFile in originSongFiles)
        {
            songFiles.Add(new KeyValuePair<string, string>(originSongFile.name, originSongFile.text));
        }

        // �� ���Ͽ� ���� SongManager�� �� ���� �߰�
        foreach (KeyValuePair<string, string> songFile in songFiles)
        {
            // ���� �̸��� ����
            string songTitle = songFile.Key;

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
    
    public void AddSong(int songNum, string songTitle, string composer)
    {
        Song newSong = new Song(songNum, songTitle, composer);
        songs.Add(newSong);
        songCount++;
    }
}
