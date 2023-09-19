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
        public int Complexity { get; protected set; }
        public double RandomPart { get; protected set; }

        public BotController( bool isWhite, int complexity = 5, double randomPart = 0.15 ) : base(isWhite)
        {
            Complexity = complexity;
            RandomPart = randomPart;
        }

        public override async Task<CheckerAction?> GetAction(ActionsExecutor actionsExecutor)
        {
            FieldScoreProvider simulator = new FieldScoreProvider(actionsExecutor, IsWhiteController, Complexity);
            await simulator.GetPositionScore(IsWhiteController);
            var results = simulator.Results;
            var bestResult = GetBestResult(results, actionsExecutor);
            var gameFieldState = actionsExecutor.GameField.GetGameStateIdentify();
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
            var gameFieldState = actionsExecutor.GameField.GetGameStateIdentify();
            if (bestResult != null)
                actionsExecutor.ScoreStorage.StoreResult(gameFieldState, bestResult, IsWhiteController, Complexity);
            return array[(int)(random.Next(array.Length) * RandomPart)];
        }
    }
}