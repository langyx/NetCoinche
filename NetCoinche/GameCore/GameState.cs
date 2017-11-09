using System;

namespace NetCoinche
{
    public enum GameState
    {
        Init,
        Waitting,
        Bet,
        BetTraitement,
        Pli,
        Ended
    }

    public static class GameStateExtension
    {
        public static string ToString(this GameState state)
        {
            switch (state)
            {
                case GameState.Init:
                    return "Waitting for players...";
                case GameState.Waitting:
                    return "Having break.";
                case GameState.Bet:
                    return "Bet round.";
                case GameState.BetTraitement:
                    return "Result of bet round.";
                case GameState.Pli:
                    return "Playing!";
                case GameState.Ended:
                    return "Game Ended !";
                default:
                    return "Idle.";
            }
        }
    }
}