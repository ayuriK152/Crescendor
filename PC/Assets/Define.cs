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

        public Notes(int keyNum, int startTime, int endTime)
        {
            this.keyNum = keyNum;
            this.startTime = startTime;
            this.endTime = endTime;
            this.deltaTime = endTime - startTime;
        }
    }
}
