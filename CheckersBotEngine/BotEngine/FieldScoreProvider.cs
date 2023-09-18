using CheckersEngine.GameEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine.BotEngine
{

    public record FieldSimulationResult
    {
        public CheckerAction FirstCheckerAction { get; set; } = null;
        public long Score { get; set; }

        public static int CompareResults(FieldSimulationResult r1, FieldSimulationResult r2)
        {
             return (int)Math.Clamp((r2.Score - r1.Score), int.MinValue, int.MaxValue);
        }
    }

    public class FieldScoreProvider
    {
        public ActionsExecutor ActionsExecutor { get; protected set; }
        public List<FieldSimulationResult> Results { get; protected set; }
        public int SimulationSteeps { get; protected set; }
        private int beginBlackCount = 0;
        private int beginWhiteCount = 0;
        private bool isWhitePlayer;

        public FieldScoreProvider(ActionsExecutor actionsExecutor, bool isWhitePlayer, int simulationStepsCount = 100 ) {
            ActionsExecutor = actionsExecutor;
            beginBlackCount = ActionsExecutor.BlackCheckersCount;
            beginWhiteCount = ActionsExecutor.WhiteCheckersCount;
            Results = new List<FieldSimulationResult>();
            SimulationSteeps = simulationStepsCount;
            this.isWhitePlayer = isWhitePlayer;
        }

        public async Task GetPositionScore( bool isWhiteTurn, int step = 0 ) 
        {
            List<Task> tasks = new List<Task>();

            for( int i = 0; i < 64; i++) {
                int x = i % 8;
                int y = i / 8;
                var position = new FieldPosition(x, y);
                var gameField = ActionsExecutor.GameField;
                var checker = gameField.GetCheckerAtPosition(position);
                if (checker == Checker.None || checker.isWhite() != isWhiteTurn)
                    continue;
                var actions = ActionsGenerator.GetCheckerActions(position, gameField);
                if (actions.Count == 0)
                    continue;

                Task longRunningTask = Task.Run(() => StartSimulationScore(actions, (ActionsExecutor)ActionsExecutor.Clone()));
                tasks.Add(longRunningTask);
            }
            await Task.WhenAll(tasks);
        }

        protected async Task StartSimulationScore(List<CheckerAction> actions, ActionsExecutor actionsExecutor)
        {
            foreach (var action in actions)
            {
                var simulationScore = new FieldSimulationResult();
                simulationScore.FirstCheckerAction = action;
                actionsExecutor.ExecuteAction(action);
                Results.Add(simulationScore);
                SimulateScoreBody(Results.Count-1, !isWhitePlayer, 1, actionsExecutor);
                actionsExecutor.CancelLastAction();
            }
        }

        protected void SimulateScoreBody(int scoreIndex, bool isWhiteTurn, int step, ActionsExecutor actionsExecutor )
        {
            if (step == SimulationSteeps)
            {
                StoreResult(scoreIndex, actionsExecutor);
                return;
            }

            bool thereIsNoStep = true;
            for (int i = 0; i < 64; i++)
            {
                int x = i % 8;
                int y = i / 8;
                var position = new FieldPosition(x, y);
                var gameField = actionsExecutor.GameField;
                var checker = gameField.GetCheckerAtPosition(position);
                if (checker == Checker.None || checker.isWhite() != isWhiteTurn)
                    continue;
                var actions = ActionsGenerator.GetCheckerActions(position, gameField);
                if (actions.Count == 0)
                    continue;
                foreach (var action in actions)
                {
                    thereIsNoStep = false;
                    actionsExecutor.ExecuteAction(action);
                    SimulateScoreBody(scoreIndex, !isWhiteTurn, step + 1, actionsExecutor);
                    actionsExecutor.CancelLastAction();
                }
            }

            if( thereIsNoStep )
                StoreResult(scoreIndex, actionsExecutor);
        }

        protected void StoreResult(int scoreIndex, ActionsExecutor actionsExecutor)
        {
            var removedWhite = beginWhiteCount - actionsExecutor.WhiteCheckersCount;
            var removedBlack = beginBlackCount - actionsExecutor.BlackCheckersCount;
            var score = isWhitePlayer ? removedBlack - removedWhite : removedWhite - removedBlack;
            Results[scoreIndex].Score += score;
            return;
        }
    }
}
