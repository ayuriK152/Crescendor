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



    public enum Scene
    {
        Unknown,
        ActualModScene,
        PracticeModScene,
        SongSelectionScene,
        ResultScene,
    }

    public enum UIEvent
    {
        Click,
        Drag,
    }


}
