using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Define
{
    public class Notes
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
    }



    public enum Scene
    {
        Unknown,
        ActualModScene,
        PracticeMoveScene,
        SongSelectionScene,
        ResultScene,
    }

    public enum UIEvent
    {
        Click,
        Drag,
    }


}
