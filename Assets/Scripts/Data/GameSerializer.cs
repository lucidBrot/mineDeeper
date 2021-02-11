using System;
using UnityEngine;

namespace Assets.Scripts.Data
{
    public class GameSerializer
    {
        public static string serialize(Game game)
        {
            GameRepresentation grep = new GameRepresentation(game.GameBoard, game.PlayerStats);
            return serialize(grep);
        }
        public static string serialize(GameRepresentation grep)
        {
            string json_game = JsonUtility.ToJson(grep);
            return json_game;
        }

        public static GameRepresentation deserialize (string json_game)
        {
            GameRepresentation game = JsonUtility.FromJson<GameRepresentation>(json_game);
            
            // allow the game board to reconstruct some information
            game.board.OnSerializeFinished();

            return game;
        }
    }

    [Serializable]
    public class GameRepresentation
    {
        [SerializeField]
        public string version = "v1";
        [SerializeField]
        public Board board;
        [SerializeField]
        public PlayerStats playerStats;

        public GameRepresentation(Board board, PlayerStats playerStats)
        {
            this.board = board;
            this.playerStats = playerStats;
        }
    }
}