using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClassLibrary;
using System;

namespace UnitTests
{
    [TestClass]
    public class Towers_Should
    {
        [TestMethod]
        public void ConstructTower_WhenNumberOfDisksIsDeclared()
        {
            //arrange
            int numberOfDiscs = 5;

            //act
            Towers testTower = new Towers(numberOfDiscs);

            //assert
            Assert.AreEqual(testTower.NumberOfDiscs, numberOfDiscs);
        }

        [TestMethod]
        public void ConstructTowerWithAllDisksOnLeftPole_WhenNumberOfDisksIsDeclared()
        {
            //arrange
            int numberOfDiscs = 5;
            int pole1Count;

            //act
            Towers testTower = new Towers(numberOfDiscs);
            pole1Count = testTower.poleOne.Count;
           
            //assert
            Assert.AreEqual(numberOfDiscs, pole1Count);
        }
    }

    [TestClass]
    public class Move_Should
    {
        [TestMethod]
        [ExpectedException(typeof(InvalidMoveException),
                "\tInvalid tower number.")]
        public void ThrowException_WhenInvalidMoveMade()
        {
            //arrange
            int numberOfDiscs = 5;
            int from = 1;
            int to = 1;
            Towers testTower = new Towers(numberOfDiscs);
            
            //act
            testTower.Move(from, to);

            //assert
            Assert.Fail();
        }

        [TestMethod]
        public void ReturnMoveRecord_WhenValidMoveMade()
        {
            //arrange
            int numberOfDiscs = 5;
            int to = 3;
            int from = 1;
            Towers testTower = new Towers(numberOfDiscs);
            MoveRecord testRecord = new MoveRecord(1, 1, 1, 3);

            //act
            MoveRecord tRecordedMove = testTower.Move(from, to);

            //assert
            Assert.ReferenceEquals(testRecord, tRecordedMove);
        }
    }

    [TestClass]
    public class ToArray_Should
    {
        [TestMethod]
        public void ReturnJaggedArray_WhenCalled()
        {
            //arrange
            int numberOfDiscs = 5;
            int[][] testJaggedArray = new int[3][];
            Towers testTower = new Towers(numberOfDiscs);

            //act
            testTower.ToArray();

            //assert
            Assert.ReferenceEquals(testJaggedArray, testTower.ToArray());
        }

        [TestMethod]
        public void CreateJaggedArrayWithThreeElementsInFirstDimension_WhenCalled()
        {
            //arrange
            int numberOfDiscs = 5;
            int numberOfPoles = 3;
            int[][] testJaggedArray = new int[3][];
            Towers testTower = new Towers(numberOfDiscs);

            //act
            testTower.ToArray();

            //assert
            Assert.AreEqual(testTower.poleArray.Length, numberOfPoles);

        }
    }
}
