using System.ComponentModel;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Assets.Scripts.Data
{
    public class Game : SingletonBehaviour<Game>, INotifyPropertyChanged
    {
        private Board gameBoard;

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
                (uint) NextBombCount, true);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
