using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Assets.Scripts.GameLogic;
using JetBrains.Annotations;
using Unity.Collections.LowLevel.Unsafe;
using Unity_Tools.Components;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class Game : SingletonBehaviour<Game>, INotifyPropertyChanged
    {
        private Board gameBoard;
        private Hint PreviousHint;
        private PlayerStats playerStats;

        public Board GameBoard
        {
            get => gameBoard;
            set
            {
                if (Equals(value, gameBoard)) return;
                gameBoard = value;
                OnPropertyChanged();
            }
        }

        public PlayerStats PlayerStats
        {
            get => playerStats;
            set
            {
                if (Equals(value, playerStats)) return;
                playerStats = value;
                OnPropertyChanged();
            }
        }

        public int NextBoardWidth { get; set; }

        public int NextBoardHeight { get; set; }

        public int NextBoardDepth { get; set; }

        public int NextBombCount { get; set; }

        public Game()
        {
            NextBoardWidth = 10;
            NextBoardHeight = 10;
            NextBoardDepth = 10;
            NextBombCount = 50;
        }

        public void StartNewGame()
        {
            var generator = new Generator.Generator();
            GameBoard = generator.Generate((uint) NextBoardWidth, (uint) NextBoardHeight, (uint) NextBoardDepth,
                (uint) NextBombCount);
            PlayerStats = new PlayerStats();
        }

        public void StartDebugFlagsGame()
        {
            Board board = new Board(5, 3, 1);
            board.SetBombState(2, 1, 0, true);
            board.SetBombState(4, 1, 0, true);

            board[1, 0, 0].State = CellState.Revealed;
            board[1, 1, 0].State = CellState.Suspect; // wrong flag
            board[2, 1, 0].State = CellState.Suspect; // good flag

            GameBoard = board;
            playerStats = new PlayerStats();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Reveal(BoardCell cell)
        {
            GameBoard.Reveal(cell);
            if (cell.IsBomb)
            {
                PlayerStats.NumBombsExploded++;
            }

            TestWhetherHintStillValid();
        }

        public Revelation RevealSlow(BoardCell cell)
        {
            var revelation = GameBoard.RevealSlow(cell);

            revelation.FollowWith(() =>
            {
                if (cell.IsBomb)
                {
                    PlayerStats.NumBombsExploded++;
                }

                TestWhetherHintStillValid();
            });

            return revelation;
        }

        private void TestWhetherHintStillValid()
        {
            if (PreviousHint != null)
            {
                Hint nextHint = Solver.Solver.Hint(GameBoard);
                if (!nextHint.IsEquivalentTo(PreviousHint))
                {
                    UILayer.Instance.HintText = null;
                    PreviousHint?.CellsToHighlight.ForEach(c => c.Highlighted = false);
                }
            }
        }

        public void RequestHint()
        {
            Hint hint = Solver.Solver.Hint(GameBoard);
            if (hint.IsEquivalentTo(PreviousHint))
            {   // same hint requested again. Show text
                UILayer.Instance.HintText = hint.Text;

            } else { 
                PlayerStats.NumHintsRequested++;
                RedrawHintHighlights(hint);
            }

            PreviousHint = hint;
        }

        public void RedrawHintHighlights([CanBeNull] Hint hint = null)
        {
            if (hint == null)
            {
                hint = PreviousHint;
            }

            if (hint == null)
            {
                // nothing to highlight
                return;
            }

            foreach (BoardCell cell in hint.CellsToHighlight)
            {
                GameBoard.Highlight(cell);
            }
        }

        public void ToggleMarking(BoardCell cell)
        {
            var oldState = cell.State;
            cell.ToggleMarking();

            if (oldState != CellState.Suspect && cell.State == CellState.Suspect)
            {
                GameBoard.FlagCount++;
            }
            else if (oldState == CellState.Suspect && cell.State != CellState.Suspect)
            {
                GameBoard.FlagCount--;
            }

            if (oldState != CellState.Unknown && cell.State == CellState.Unknown)
            {
                GameBoard.UnknownCount++;
            }
            else if (oldState == CellState.Unknown && cell.State != CellState.Unknown)
            {
                GameBoard.UnknownCount--;
            }

            CheckIfGameFinished();
            TestWhetherHintStillValid();
        }

        public void CheckIfGameFinished()
        {
            var flags = GameBoard.Where(c => c.State == CellState.Suspect);
            if (flags.Count() == GameBoard.BombCount - GameBoard.Count(b => b.IsBomb && b.State == CellState.Revealed)
                && GameBoard.All(c => c.State==CellState.Revealed || (c.State==CellState.Suspect && c.IsBomb)))
            {
                // Game Won!
                FinishGame();
            }
        }

        private void FinishGame()
        {
            UILayer.Instance.HintText = "Game Finished!";
            foreach (var boardCell in GameBoard)
            {
                boardCell.Highlighted = true;
            }
        }
    }
}
