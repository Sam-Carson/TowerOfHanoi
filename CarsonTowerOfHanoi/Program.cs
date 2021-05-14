using System;
using System.Collections.Generic;
using System.Threading;
using ClassLibrary;
using static System.Console;

namespace CarsonTowerOfHanoi
{
    public class Program
    {
        static void Main(string[] args)
        {
            bool playAgain;
            do
            {
                int numDiscs = 0;
                int from = 0;
                int to = 0;
                int howToPlay;
                bool validInput;
                bool endGame;
                bool askDisplayCtrlZ = false;
                bool askDisplayCtrlY = false;

                Queue<MoveRecord> recordedMovesQ = new Queue<MoveRecord>();
                Stack<MoveRecord> undoStack = new Stack<MoveRecord>();
                Stack<MoveRecord> redoStack = new Stack<MoveRecord>();

                Clear();
                do
                {
                    try
                    {
                        numDiscs = GetGameDiscs();
                        Clear();
                    }
                    catch (InvalidHeightException e)
                    {
                        WriteLine(e.Message);
                    }
                } while (numDiscs == 0);

                // Creates towers 
                // sets MinimumPossibleMoves property
                // Displays towers & converts myTowers to array
                // Asks how to play the game
                Towers myTowers = new Towers(numDiscs);
                myTowers.MinimumPossibleMoves = MinimumMoves(numDiscs);
                Update(myTowers);
                howToPlay = HowToPlay();

                if (howToPlay == 2) // Auto-solve play
                {
                    WriteLine("\nPress a key and watch closely!");
                    ReadLine();
                    AutoPlay(numDiscs,1, 3, 2, myTowers, recordedMovesQ);
                }
                else if (howToPlay == 3) //Step-by-step
                {
                    WriteLine("\nPress a key to see the first move!");
                    ReadLine();
                    StepByStep(1, 3, 2, myTowers, recordedMovesQ);
                }
                else if (howToPlay == 1)// Manual play
                {
                    do
                    {
                        do
                        {
                            validInput = false;
                            endGame = false;// resets validInput
                            if (myTowers.NumberOfMoves > 0) askDisplayCtrlZ = true;
                            if (redoStack.Count > 0)
                            {
                                askDisplayCtrlY = true;
                            }

                            try // Gets 'from' pole
                            {
                                from = MoveFrom(askDisplayCtrlZ, askDisplayCtrlY, undoStack, redoStack);
                                if (from == -3 && undoStack.Count == 0)
                                {
                                    WriteLine("\nCan't undo!");
                                    validInput = false;
                                }
                                else if (from == -2 && redoStack.Count == 0)
                                {
                                    WriteLine("\nCan't redo!");
                                    validInput = false;
                                }
                                else validInput = true;
                            }
                            catch (InvalidMoveException e)
                            {
                                WriteLine(e.Message);
                            }
                        } while (validInput == false);

                        if (from < 0)
                        {
                            switch (from)
                            {
                                case -1: // quit
                                    endGame = true;
                                    break;
                                case -2: // redo
                                    Redo(undoStack, redoStack, myTowers, recordedMovesQ);
                                    break;
                                case -3: // undo
                                    Undo(undoStack, redoStack, myTowers, recordedMovesQ);
                                    break;
                            }
                        }
                        else
                        {
                            do
                            {
                                try // gets 'to' pole
                                {
                                    to = MoveTo();
                                    validInput = true;
                                }
                                catch (InvalidMoveException e)
                                {
                                    WriteLine(e.Message);
                                    validInput = false;
                                }
                            } while (validInput == false);
                        }
                        if (!endGame) // if the user did not quit
                        {
                            // Makes move an returns recorded move
                            // Pushes recordedMove to undoStack
                            // Adds recordedMove to recordedMovesQ
                            // Clears redoStack becuase user made a regular move
                            // Displays towers
                            // converts to array
                            // Checks if game is complete
                            // Ends game if game is complete

                            if (from > 0) // if the user made a regular move
                            {
                                MoveRecord recordedMove = myTowers.Move(from, to);
                                undoStack.Push(recordedMove);
                                redoStack.Clear();
                                recordedMovesQ.Enqueue(recordedMove);
                            }

                            Update(myTowers);
                            GameComplete(myTowers, from, to, undoStack, redoStack);
                            if (myTowers.IsComplete) endGame = true;
                        }
                    } while (!endGame);
                }
                // list turns
                DisplayMoves(recordedMovesQ);
                playAgain = PlayAgain();

            } while (playAgain);

        } // end Main

        public static int MoveFrom(bool askCtrlZ, bool askCtrlY, Stack<MoveRecord> pUndoStack, Stack<MoveRecord> pRedoStack)
        {
            // if the user has undone or redone the maximum number of turns, the below writeline updates
            if (pUndoStack.Count == 0) askCtrlZ = false;
            if (pRedoStack.Count == 0) askCtrlY = false;

            string ctrlOptionsY = askCtrlY == true ? " 'Ctrl+y' to redo," : "";
            string ctrlOptionsZ = askCtrlZ == true ? ", 'Ctrl+z' to undo," : "";


            Write($"\nEnter 'from' tower number{ctrlOptionsZ}{ctrlOptionsY} or 'x' to quit: ");
            ConsoleKeyInfo validFromInput = ReadKey();
            if (validFromInput.Key == ConsoleKey.X) // user inputs x
            {
                return -1;
            }
            else if (validFromInput.Modifiers == ConsoleModifiers.Control && validFromInput.Key == ConsoleKey.Y) // user inputs ctrl+y to redo
            {
                return -2;
            }
            else if (validFromInput.Modifiers == ConsoleModifiers.Control && validFromInput.Key == ConsoleKey.Z) // user inputs ctrl+z to undo
            {
                return -3;
            }
            else
            {
                int.TryParse(validFromInput.KeyChar.ToString(), out int validFromInt);
                if (validFromInt == 0 || validFromInt < 0 || validFromInt > 3) throw new InvalidMoveException("Invalid tower.");
                else return validFromInt;
            }
        }

        public static int MoveTo()
        {
            int validToInput;

            Write("\nEnter 'to' tower number: ");
            int.TryParse(ReadKey().KeyChar.ToString().ToUpper(), out validToInput);
            if (validToInput == 0 || validToInput < 0 || validToInput > 3)
            {
                throw new InvalidMoveException("Invalid tower.");
            }
            return validToInput;
        }

        public static int GetGameDiscs()
        {
            int validInt;

            Write("How many discs would you like? (5 is default, 9 is max): ");
            int.TryParse(ReadLine(), out validInt);
            if (validInt == 0 || validInt < 0 || validInt > 9)
            {
                WriteLine("Number of discs defaulting to 5. Press any key to continue.");
                ReadLine();
                return 5;
                //throw new InvalidHeightException();
            }
            return validInt;
        }


        public static int MinimumMoves(int numDiscs)
        {
            int minMoves = (int)Math.Pow(2, numDiscs) - 1;

            return minMoves;
        }

        public static bool PlayAgain()
        {
            WriteLine("\nWould you like to play again?('y' for yes! or any key to quit): ");
            string playAgainInput = ReadKey().KeyChar.ToString().ToUpper();
            if (playAgainInput == "Y") return true;
            else return false;
        }

        
        public static void GameComplete(Towers pMyTowers, int pFrom, int pTo, Stack<MoveRecord> pUndoStack, Stack<MoveRecord> pRedoStack)
        {
            if (pMyTowers.IsComplete)
            {
                WriteLine($"\nCongratulations, you completed the puzzle in {pMyTowers.NumberOfMoves} moves.");
                if (pMyTowers.MinimumPossibleMoves == pMyTowers.NumberOfMoves) WriteLine($"\nThat's the fewest number of moves possible. I ANOINT YOU THE RULER OF HANOI!");
                else
                {
                    WriteLine($"\nYou completed the puzzle in {pMyTowers.NumberOfMoves} moves but the fewest possible is {pMyTowers.MinimumPossibleMoves}");
                    WriteLine("\nLet's give it another shot. What do you say?");
                }
            }
            else if (pFrom == -2) // redo
            {
                MoveRecord redoMoveDetails = pUndoStack.Peek();
                WriteLine($"\nMove {pMyTowers.NumberOfMoves} complete by redo of move {pMyTowers.NumberOfMoves - 1}. Disc {redoMoveDetails.Disc} restored to tower {redoMoveDetails.To} from tower {redoMoveDetails.From}.");
            }
            else if (pFrom == -3) // undo
            {
                MoveRecord undoMoveDetails = pRedoStack.Peek();
                WriteLine($"\nMove {pMyTowers.NumberOfMoves} complete by undo of move {undoMoveDetails.MoveNumber}. Disc {undoMoveDetails.Disc} restored to tower {undoMoveDetails.From} from tower {undoMoveDetails.To}.");
            }
            else
            {
                WriteLine($"\nMove {pMyTowers.NumberOfMoves} complete. Successfully moved disc {pUndoStack.Peek().Disc} from tower {pFrom} to tower {pTo}.");
            }
        }

        public static void Update(Towers pMyTowers)
        {
            TowerUtilities.DisplayTowers(pMyTowers);
            pMyTowers.ToArray();
        }

        public static void DisplayMoves(Queue<MoveRecord> pRecordedMoves)
        {
            Write("\nWould you like to see a list of moves? ('y' for yes): ");
            string recordedMovesInput = ReadKey().KeyChar.ToString().ToUpper();
            WriteLine();

            if (recordedMovesInput == "Y")
            {
                foreach (MoveRecord item in pRecordedMoves)
                {
                    WriteLine($"\t{item.MoveNumber}: You moved disc {item.Disc} from tower {item.From} to tower {item.To}");
                }
            }
        }

        public static MoveRecord Undo(Stack<MoveRecord> pUndoStack, Stack<MoveRecord> pRedoStack, Towers pMyTowers, Queue<MoveRecord> pRecordedMovesQ)
        {
            MoveRecord undoMoveRecord = pUndoStack.Pop();
            pRedoStack.Push(undoMoveRecord);
            MoveRecord postUndoMove = pMyTowers.Move(undoMoveRecord.To, undoMoveRecord.From);
            pRecordedMovesQ.Enqueue(postUndoMove);
            return postUndoMove;
        }

        public static MoveRecord Redo(Stack<MoveRecord> pUndoStack, Stack<MoveRecord> pRedoStack, Towers pMyTowers, Queue<MoveRecord> pRecordedMovesQ)
        {
            MoveRecord redoMoveRecord = pRedoStack.Pop();
            pUndoStack.Push(redoMoveRecord);
            MoveRecord postRedoMove = pMyTowers.Move(redoMoveRecord.From, redoMoveRecord.To);
            pRecordedMovesQ.Enqueue(postRedoMove);

            return postRedoMove;
        }

        public static int HowToPlay()
        {
            string hTPInput;
            bool validHTPInput;

            WriteLine("Options: ");
            WriteLine("- M - Solve the puzzle manually");
            WriteLine("- A - Auto-solve");
            WriteLine("- S - Auto-solve step by step");

            do
            {
                Write("\nChoose an approach: ");
                hTPInput = ReadKey().KeyChar.ToString().ToUpper();

                switch (hTPInput)
                {
                    case "M":
                        return 1;
                    case "A":
                        return 2;
                    case "S":
                        return 3;
                    default:
                        validHTPInput = false;
                        break;
                }
            } while (!validHTPInput);
            return 0; // all code paths must return a value....
        }

        public static void AutoPlay(int n, int source, int destination, int aux, Towers pMyTowers, Queue<MoveRecord> moveQueue)
        {
            if (n > 0)
            {
                AutoPlay(n - 1, source, aux, destination, pMyTowers, moveQueue);
                Thread.Sleep(250);
                MoveRecord recordedMove = pMyTowers.Move(source, destination);
                moveQueue.Enqueue(recordedMove);
                Update(pMyTowers);
                WriteLine($"Move {moveQueue.Count} complete. Successfully moved disc {recordedMove.Disc} from tower {recordedMove.From} to tower {recordedMove.To}.");
                AutoPlay(n - 1, aux, destination, source, pMyTowers, moveQueue);
            }
        }

        public static void StepByStep(int source, int destination, int aux, Towers pMyTowers, Queue<MoveRecord> moveQueue)
        {
            string exitSTS = null;
            bool evenNumDiscs;
            bool exitIteration = false;
            if (pMyTowers.NumberOfDiscs % 2 == 0) evenNumDiscs = true;
            else evenNumDiscs = false;

            do
            {
                for (int i = 1; i < pMyTowers.MinimumPossibleMoves + 1; i++)
                {
                    if (evenNumDiscs)
                    {
                        if (i % 3 == 1) //source aux
                        {
                            if (pMyTowers.NumberOfMoves == 0)
                            {
                                MoveRecord recordedMove = pMyTowers.Move(source, aux);
                                exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                            }
                            else if (pMyTowers.poleTwo.Count == 0)
                            {
                                MoveRecord recordedMove = pMyTowers.Move(source, aux);
                                exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                            }
                            else if (pMyTowers.poleOne.Count == 0)
                            {
                                MoveRecord recordedMove = pMyTowers.Move(aux, source);
                                exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                            }
                            else if (pMyTowers.poleTwo.Count != 0 && pMyTowers.poleOne.Count != 0)
                            {
                                if (pMyTowers.poleOne.Peek() > pMyTowers.poleTwo.Peek())
                                {
                                    MoveRecord recordedMove = pMyTowers.Move(aux, source);
                                    exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                                }
                                else
                                {
                                    MoveRecord recordedMove = pMyTowers.Move(source, aux);
                                    exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                                }
                            }
                            if (exitSTS == "X" || pMyTowers.IsComplete)
                            {
                                exitIteration = true;
                                break;
                            }
                        }
                        else if (i % 3 == 2) // source destination
                        {
                            if (pMyTowers.poleOne.Count == 0)
                            {
                                MoveRecord recordedMove = pMyTowers.Move(destination, source);
                                exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                            }
                            else if (pMyTowers.poleThree.Count == 0)
                            {
                                MoveRecord recordedMove = pMyTowers.Move(source, destination);
                                exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                            }
                            else if (pMyTowers.poleOne.Count != 0 && pMyTowers.poleThree.Count != 0)
                            {
                                if (pMyTowers.poleOne.Peek() > pMyTowers.poleThree.Peek())
                                {
                                    MoveRecord recordedMove = pMyTowers.Move(destination, source);
                                    exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                                }
                                else
                                {
                                    MoveRecord recordedMove = pMyTowers.Move(source, destination);
                                    exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                                }
                            }
                            if (exitSTS == "X" || pMyTowers.IsComplete)
                            {
                                exitIteration = true;
                                break;
                            }
                        }
                        else if (i % 3 == 0) //aux destination
                        {
                            if (pMyTowers.poleTwo.Count == 0)
                            {
                                MoveRecord recordedMove = pMyTowers.Move(destination, aux);
                                exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                            }
                            else if (pMyTowers.poleThree.Count == 0)
                            {
                                MoveRecord recordedMove = pMyTowers.Move(aux, destination);
                                exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                            }
                            else if (pMyTowers.poleTwo.Count != 0 && pMyTowers.poleThree.Count != 0)
                            {
                                if (pMyTowers.poleTwo.Peek() > pMyTowers.poleThree.Peek())
                                {
                                    MoveRecord recordedMove = pMyTowers.Move(destination, aux);
                                    exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                                }
                                else
                                {
                                    MoveRecord recordedMove = pMyTowers.Move(aux, destination);
                                    exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                                }
                            }
                            if (exitSTS == "X" || pMyTowers.IsComplete)
                            {
                                exitIteration = true;
                                break;
                            }
                        }
                    }
                    else
                    {

                        if (i % 3 == 1) //source destination
                        {
                            if (pMyTowers.NumberOfMoves == 0)
                            {
                                MoveRecord recordedMove = pMyTowers.Move(source, destination);
                                exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                            }
                            else if (pMyTowers.poleThree.Count == 0)
                            {
                                MoveRecord recordedMove = pMyTowers.Move(source, destination);
                                exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                            }
                            else if (pMyTowers.poleOne.Count == 0)
                            {
                                MoveRecord recordedMove = pMyTowers.Move(destination, source);
                                exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                            }
                            else if (pMyTowers.poleThree.Count != 0 && pMyTowers.poleOne.Count != 0)
                            {
                                if (pMyTowers.poleOne.Peek() > pMyTowers.poleThree.Peek())
                                {
                                    MoveRecord recordedMove = pMyTowers.Move(destination, source);
                                    exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                                }
                                else
                                {
                                    MoveRecord recordedMove = pMyTowers.Move(source, destination);
                                    exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                                }
                            }
                            if (exitSTS == "X" || pMyTowers.IsComplete)
                            {
                                exitIteration = true;
                                break;
                            }
                        }
                        else if (i % 3 == 2) // source aux
                        {
                            if (pMyTowers.poleOne.Count == 0)
                            {
                                MoveRecord recordedMove = pMyTowers.Move(aux, source);
                                exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                            }
                            else if (pMyTowers.poleTwo.Count == 0)
                            {
                                MoveRecord recordedMove = pMyTowers.Move(source, aux);
                                exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                            }
                            else if (pMyTowers.poleOne.Count != 0 && pMyTowers.poleTwo.Count != 0)
                            {
                                if (pMyTowers.poleOne.Peek() > pMyTowers.poleTwo.Peek())
                                {
                                    MoveRecord recordedMove = pMyTowers.Move(aux, source);
                                    exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                                }
                                else
                                {
                                    MoveRecord recordedMove = pMyTowers.Move(source, aux);
                                    exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                                }
                            }
                            if (exitSTS == "X" || pMyTowers.IsComplete)
                            {
                                exitIteration = true;
                                break;
                            }
                        }
                        else if (i % 3 == 0) //aux destination
                        {
                            if (pMyTowers.poleTwo.Count == 0)
                            {
                                MoveRecord recordedMove = pMyTowers.Move(destination, aux);
                                exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                            }
                            else if (pMyTowers.poleThree.Count == 0)
                            {
                                MoveRecord recordedMove = pMyTowers.Move(aux, destination);
                                exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                            }
                            else if (pMyTowers.poleTwo.Count != 0 && pMyTowers.poleThree.Count != 0)
                            {
                                if (pMyTowers.poleTwo.Peek() > pMyTowers.poleThree.Peek())
                                {
                                    MoveRecord recordedMove = pMyTowers.Move(destination, aux);
                                    exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                                }
                                else
                                {
                                    MoveRecord recordedMove = pMyTowers.Move(aux, destination);
                                    exitSTS = SBSUpdate(recordedMove, moveQueue, pMyTowers);
                                }
                            }
                            if (exitSTS == "X" || pMyTowers.IsComplete)
                            {
                                exitIteration = true;
                                break;
                            }
                        }
                    }
                }
            } while (!exitIteration);
        }

        public static string SBSUpdate(MoveRecord pRecordedMove, Queue<MoveRecord> moveQueue, Towers pMyTowers)
        {
            moveQueue.Enqueue(pRecordedMove);
            Update(pMyTowers);
            IterativeMoveMessage(pRecordedMove);
            if (pMyTowers.IsComplete)
            {
                WriteLine($"\nStep-through completed. Number of moves: {pRecordedMove.MoveNumber}");
                return "X";
            }
            WriteLine("\nPress any key to see the next move or 'X' to exit: ");
            return ReadKey().KeyChar.ToString().ToUpper();
        }

        public static void IterativeMoveMessage(MoveRecord pRecordedMove)
        {
            WriteLine($"Move {pRecordedMove.MoveNumber} complete. Successfully moved disc from pole {pRecordedMove.From} to pole {pRecordedMove.To}.");
        }
    }
}
