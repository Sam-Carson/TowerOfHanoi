using System;
using System.Collections.Generic;
using System.Text;

namespace ClassLibrary
{
    public class MoveRecord
    {
        public int MoveNumber { get; set; }
        public int Disc { get; set; }
        public int From { get; set; }
        public int To { get; set; }

        public MoveRecord(int moveNumber, int disc, int from, int to)
        {
            MoveNumber = moveNumber;
            Disc = disc;
            From = from;
            To = to;
        }
    }
}
