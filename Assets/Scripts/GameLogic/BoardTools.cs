using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Assets.Scripts.Data;
using UnityEngine;

namespace Assets.Scripts.GameLogic
{
    public static class BoardTools
    {
        public static void Reveal(this Board board, BoardCell cell)
        {
            cell.State = CellState.Revealed;

            if (cell.IsNude && !cell.IsBomb)
            {
                board.ForEachNeighbor(cell, (c) =>
                {
                    if (c.State != CellState.Default)
                    {
                        return;
                    }

                    board.Reveal(c);
                });
            }
        }

        public static Revelation RevealSlow(this Board board, BoardCell cell)
        {
            return new Revelation(board, cell, 0.03f);
        }

        public static void ForEachNeighbor(this Board board, BoardCell cell, Action<BoardCell> action)
        {
            var posX = cell.PosX;
            var posY = cell.PosY;
            var posZ = cell.PosZ;

            var maxX = board.Width - 1;
            var maxY = board.Height - 1;
            var maxZ = board.Depth - 1;

            for (var x = -1; x <= 1; x++)
            {
                var indX = x + posX;
                if (indX < 0 || indX > maxX)
                {
                    continue;
                }

                for (var y = -1; y <= 1; y++)
                {
                    var indY = y + posY;
                    if (indY < 0 || indY > maxY)
                    {
                        continue;
                    }

                    for (var z = -1; z <= 1; z++)
                    {
                        var indZ = z + posZ;
                        if (indZ < 0 || indZ > maxZ)
                        {
                            continue;
                        }

                        if (x == 0 && y == 0 && z == 0)
                        {
                            continue;
                        }

                        action(board[indX, indY, indZ]);
                    }
                }
            }
        }

        public static IEnumerable<BoardCell> NeighborsOf(this Board board, BoardCell cell)
        {
            var posX = cell.PosX;
            var posY = cell.PosY;
            var posZ = cell.PosZ;

            var maxX = board.Width - 1;
            var maxY = board.Height - 1;
            var maxZ = board.Depth - 1;

            for (var x = -1; x <= 1; x++)
            {
                var indX = x + posX;
                if (indX < 0 || indX > maxX)
                {
                    continue;
                }

                for (var y = -1; y <= 1; y++)
                {
                    var indY = y + posY;
                    if (indY < 0 || indY > maxY)
                    {
                        continue;
                    }

                    for (var z = -1; z <= 1; z++)
                    {
                        var indZ = z + posZ;
                        if (indZ < 0 || indZ > maxZ)
                        {
                            continue;
                        }

                        if (x == 0 && y == 0 && z == 0)
                        {
                            continue;
                        }

                        yield return board[indX, indY, indZ];
                    }
                }
            }
        }

        public static int CountNeighbors(this Board board, BoardCell cell, Func<BoardCell, bool> predicate)
        {
            var count = 0;
            board.ForEachNeighbor(cell, c =>
            {
                if (predicate(c))
                {
                    count++;
                }
            });

            return count;
        }
    }
}