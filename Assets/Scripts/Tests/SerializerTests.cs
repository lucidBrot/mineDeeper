using System.Collections;
using Assets.Scripts.Data;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class SerializerTests
    {
        [Test]
        public void testSerializationAndDeserialization()
        {   
            // set up board
            Board board = new Board(5,5,4);
            for (int i = 0; i < 4; i++)
            {
                board.SetBombState(i, 0, 0, true);
            }

            board.SetBombState(0,0,0, false);
            board.SetBombState(2,2,2, false);
            board[0, 1, 2].Highlighted = true;

            // serialize Board
            string json = GameSerializer.serialize(new GameRepresentation(board, new PlayerStats()));
            int dbg = 0;
            
            // deserialize again and check if the values are still the same
            GameRepresentation grep = GameSerializer.deserialize(json);

            Assert.AreEqual(grep.board[0, 1, 2].PosX, 0);
            Assert.AreEqual(grep.board[0, 1, 2].PosY, 1);
            Assert.AreEqual(grep.board[0, 1, 2].PosZ, 2);
            Assert.AreEqual(grep.board[0, 0, 0].IsBomb, false, "huh...");
            Assert.AreEqual(grep.board[1, 0, 0].IsBomb, true, "hmm...");
            Assert.AreEqual(grep.board[2, 2, 2].IsBomb, false, "heh.");
            Assert.AreEqual(grep.board[0, 1, 2].Highlighted, true, "huh...");
        }
        
    }
}