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
        public Curriculum curriculum;

        public Song(int songNum, string songTitle, string songComposer, Curriculum curriculum)
        {
            this.songNum = songNum;
            this.songTitle = songTitle;
            this.songComposer = songComposer;
            this.curriculum = curriculum;
        }
    }

    /* 랭크 리스트용 클래스
     * 변수 명명규칙에 어긋나지만 DB와 이름을 맞춰야하기 때문에 예외사항임.
     * 절대 변수명 수정하지 말것.*/
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

    public class SheetNote
    {
        public int keyNum;
        public float timing;
        public bool isUpper;
        public bool isShapeFourth;
        public NoteKind noteKind;
        public Vector2 position;

        public SheetNote(int keyNum, float timing, bool isUpper, NoteKind noteKind)
        {
            this.keyNum = keyNum;
            this.timing = timing;
            this.isUpper = isUpper;
            this.noteKind = noteKind;
            isShapeFourth = false;
            if (isUpper)
            {
                int yPosOffset = (keyNum - 3) / 12;
                position = new Vector2(timing, (Managers.Midi.NoteKeyPosOrder(keyNum) + yPosOffset * 7 - 32) * 7.5f);
            }
            else
            {
                int yPosOffset = (keyNum - 3) / 12;
                position = new Vector2(timing, (Managers.Midi.NoteKeyPosOrder(keyNum) + yPosOffset * 7 - 20) * 7.5f);
            }
        }
    }

    public class Bar
    {
        public List<SheetNote> notes;

        public Bar()
        {
            notes = new List<SheetNote>();
        }
    }

    [Serializable]
    public class LogDateWrapper
    {
        public LogDate[] logs;
    }

    [Serializable]
    public class LogDate
    {
        public string date;
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

    public enum Curriculum
    {
        Hanon,
        Czerny100,
        Beyer,
        None,
    }

    public enum NoteKind
    {
        First,
        Second,
        Fourth,
        Eighth,
        Sixteenth,
    }
}
