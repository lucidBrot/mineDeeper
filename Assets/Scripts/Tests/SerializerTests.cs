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
        public void serializeSomeBoard()
        {   
            // set up board
            Board board = new Board(5,5,4);
            for (int i = 0; i < 4; i++)
            {
                board.SetBombState(i, 0, 0, true);
            }

            board.SetBombState(0,0,0, false);
            board.SetBombState(2,2,2, false);

            // serialize Board
            string json_board = GameSerializer.serialize(new GameRepresentation(board, new PlayerStats()));
            int dbg = 0;
        }
        
    }
}