using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public class Notes : IComparable
    {
        public int keyNum;
        public int startTime;
        public int endTime;
        public int deltaTime;
        public int channel;

        public Notes(int keyNum, int startTime, int endTime, int channel)
        {
            this.keyNum = keyNum;
            this.startTime = startTime;
            this.endTime = endTime;
            this.deltaTime = endTime - startTime;
            this.channel = channel;
        }

        public int CompareTo(object obj)
        {
            Notes oppo = obj as Notes;
            return startTime.CompareTo(oppo.startTime);
        }
    }

    public class Song
    {
        public int songNum;
        public string songTitle;
        public string songComposer;

        public Song(int songNum, string songTitle, string composer)
        {
            this.songNum = songNum;
            this.songTitle = songTitle;
            this.songComposer = composer;
        }
    }

    /* ��ũ ����Ʈ�� Ŭ����
     * ���� ����Ģ�� ��߳����� DB�� �̸��� ������ϱ� ������ ���ܻ�����.
     * ���� ������ �������� ����.*/
    [Serializable]
    public class RankRecord
    {
        public string name;
        public string user_id;
        public float score;
        public string date;
        public string midi;

        public RankRecord(string name, string user_id, float score, string date, string midi)
        {
            this.name = name;
            this.user_id = user_id;
            this.score = score;
            this.date = date;
            this.midi = midi;
        }
    }

    public class RankRecordList
    {
        public List<RankRecord> records;

        public RankRecordList()
        {
            records = new List<RankRecord>();
        }
    }

    [Serializable]
    public class UserReplayRecordForParse
    {
        public int tempo;
        public Dictionary<int, List<KeyValuePair<int, int>>> noteRecords;
        public string originFileName;

        public UserReplayRecordForParse(Dictionary<int, List<KeyValuePair<int, int>>> noteRecords, int tempo, string originFileName)
        {
            this.noteRecords = noteRecords;
            this.tempo = tempo;
            this.originFileName = originFileName;
        }

        public UserReplayRecord ParseToOrigin(float accuracy)
        {
            return new UserReplayRecord(noteRecords, tempo, originFileName, accuracy);
        }
    }

    [Serializable]
    public class UserReplayRecord
    {
        public int tempo;
        public string originFileName;
        public float accuracy;

        public Dictionary<int, List<KeyValuePair<int, int>>> noteRecords;

        public UserReplayRecord(Dictionary<int, List<KeyValuePair<int, int>>> noteRecords, int tempo, string originFileName, float accuracy)
        {
            this.noteRecords = noteRecords;
            this.tempo = tempo;
            this.originFileName = originFileName;
            this.accuracy = accuracy;
        }
    }

    public enum Scene
    {
        Unknown,
        ActualModScene,
        PracticeModScene,
        ReplayModScene,
        SongSelectScene,
        ResultScene,
        MainMenuScene,
        MyPageScene,
    }

    public enum UIEvent
    {
        Click,
        Drag,
    }

    public enum InputType
    {
        OnKeyDown,
        OnKeyUp,
    }
}
