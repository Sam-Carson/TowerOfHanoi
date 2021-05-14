using static System.Console;
using System.Collections.Generic;

namespace ClassLibrary
{
    public class Towers
    {
        public  int NumberOfDiscs { get; set; }
        public  int NumberOfMoves { get; set; }
        public  int MinimumPossibleMoves { get; set; }
        public  bool IsComplete { get; set; }

        public Stack<int> poleOne = new Stack<int>();
        public Stack<int> poleTwo = new Stack<int>();
        public Stack<int> poleThree = new Stack<int>();
        public Stack<int>[] poleArray = new Stack<int>[3];


        public Towers(int numberOfDiscs)
        {
            NumberOfDiscs = numberOfDiscs;

            for (int i = numberOfDiscs; i > 0; i--)
            {
                poleOne.Push(i);
            }
            
            poleArray[0] = (poleOne);
            poleArray[1] = (poleTwo);
            poleArray[2] = (poleThree);
        }


        public MoveRecord Move(int pFrom, int pTo)
        {
            int movedDisk;

            if (pFrom < 1 || pFrom > 3 || pTo < 1 || pTo > 3) // validates input for poles
            {
                throw new InvalidMoveException("Invalid Tower Number.");
            }
            else if (pFrom == pTo) // prevents from choosing same pole to move from --> to
            {
                throw new InvalidMoveException("Move Cancelled");
            }
            else if (poleArray[pFrom - 1].Count == 0) // Checks if pull is empty
            {
                throw new InvalidMoveException($"Tower {pFrom} is empty.");
            }
            else if (!(poleArray[pTo - 1].Count == 0) && poleArray[pFrom - 1].Peek() > poleArray[pTo - 1].Peek())
            {
                throw new InvalidMoveException($"Top disc of tower {pFrom} is larger than top disc on tower {pTo}.");
            }
            else
            {
                movedDisk = poleArray[pFrom - 1].Pop();
                poleArray[pTo - 1].Push(movedDisk);
                NumberOfMoves++;
                MoveRecord recordedMove = new MoveRecord(NumberOfMoves, movedDisk, pFrom, pTo);

                if (poleArray[2].Count == NumberOfDiscs)
                {
                    IsComplete = true;
                    WriteLine("Winner");
                    WriteLine($"Total number of moves {NumberOfMoves}.");
                }
                return recordedMove;
            }
        }


        public int[][] ToArray()
        {
            int[][] jaggedArray = new int[3][];

            jaggedArray[0] = poleOne.ToArray();
            jaggedArray[1] = poleTwo.ToArray();
            jaggedArray[2] = poleThree.ToArray();
            return jaggedArray;
        }
    }
}

