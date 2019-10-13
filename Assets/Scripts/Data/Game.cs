using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Assets.Scripts.GameLogic;
using JetBrains.Annotations;

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
        }

        public void RequestHint()
        {
            Hint hint = Solver.Solver.Hint(GameBoard);
            if (hint.IsSameHintAs(PreviousHint))
            {   // same hint requested again. Show text
                throw new NotImplementedException("Cannot Display Text Hint on GUI yet");

            } else { 
                PlayerStats.NumHintsRequested++;
            }

            foreach (BoardCell cell in hint.CellsToHighlight)
            {
                GameBoard.Highlight(cell);
            }

            PreviousHint = hint;
        }
    }
}
