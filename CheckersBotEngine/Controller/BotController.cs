using CheckersEngine.BotCore;
using CheckersEngine.GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine.Controller
{
    public class BotController : AbstractController
    {
        public GameField gameField { get; protected set; }
        public bool IsWhitePlayer { get; protected set; }
        public int Complexity { get; protected set; }
        public double RandomPart { get; protected set; } = 0.1;

        public BotController(GameField gameField, bool isWhite, int complexity = 5)
        {
            this.gameField = gameField;
            IsWhitePlayer = isWhite;
            Complexity = complexity;
        }

        public override async Task<CheckerAction?> GetAction(ActionsExecutor actionsExecutor)
        {
            FieldScoreProvider simulator = new FieldScoreProvider(actionsExecutor, IsWhitePlayer, Complexity);
            await simulator.GetPositionScore(IsWhitePlayer);
            var results = simulator.Results;
            var bestResult = GetBestResult(results, actionsExecutor);
            var gameFieldState = gameField.GetGameStateIdentify();
            return bestResult == null ? null : bestResult.FirstCheckerAction;
        }

        private FieldScoreResult? GetBestResult(List<FieldScoreResult> results, ActionsExecutor actionsExecutor)
        {
            var array = results.ToArray();
            Array.Sort(array, FieldScoreResult.CompareResults);
            if (array.Length == 0)
                return null;

            Random random = new Random(Environment.TickCount);
            var bestResult = array[0];
            var gameFieldState = gameField.GetGameStateIdentify();
            if (bestResult != null)
                actionsExecutor.ScoreStorage.StoreResult(gameFieldState, bestResult, IsWhitePlayer, Complexity);
            return array[(int)(random.Next(array.Length) * RandomPart)];
        }
    }
}