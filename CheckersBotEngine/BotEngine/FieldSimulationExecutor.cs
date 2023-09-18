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

    public class FieldSimulationExecutor
    {
        public ActionsExecutor ActionsExecutor { get; protected set; }
        public List<FieldSimulationResult> Results { get; protected set; }
        public int SimulationSteeps { get; protected set; }
        protected GameField gameField { get; set; }
        private int beginBlackCount = 0;
        private int beginWhiteCount = 0;
        private bool isWhitePlayer;

        public FieldSimulationExecutor(ActionsExecutor actionsExecutor, bool isWhitePlayer, int simulationStepsCount = 100 ) {
            ActionsExecutor = actionsExecutor;
            beginBlackCount = ActionsExecutor.BlackCheckersCount;
            beginWhiteCount = ActionsExecutor.WhiteCheckersCount;
            Results = new List<FieldSimulationResult>();
            SimulationSteeps = simulationStepsCount;
            this.isWhitePlayer = isWhitePlayer;
            gameField = actionsExecutor.GameField;
        }

        public void GetPositionScore( bool isWhiteTurn, int step = 0 ) 
        {
            for( int i = 0; i < 64; i++) {
                int x = i % 8;
                int y = i / 8;
                var position = new FieldPosition(x, y);
                var checker = gameField.GetCheckerAtPosition(position);
                if (checker == Checker.None || checker.isWhite() != isWhiteTurn)
                    continue;
                var actions = ActionsGenerator.GetCheckerActions(position, gameField);
                if (actions.Count == 0)
                    continue;
                StartSimulationScore(actions);
            }
        }

        protected void StartSimulationScore(List<CheckerAction> actions)
        {
            foreach (var action in actions)
            {
                var simulationScore = new FieldSimulationResult();
                simulationScore.FirstCheckerAction = action;
                ActionsExecutor.ExecuteAction(action);
                Results.Add(simulationScore);
                SimulateScoreBody(Results.Count-1, !isWhitePlayer, 1);
                ActionsExecutor.CancelLastAction();
            }
        }

        protected void SimulateScoreBody(int scoreIndex, bool isWhiteTurn, int step)
        {
            if (step == SimulationSteeps)
            {
                StoreResult(scoreIndex);
                return;
            }

            bool thereIsNoStep = true;
            for (int i = 0; i < 64; i++)
            {
                int x = i % 8;
                int y = i / 8;
                var position = new FieldPosition(x, y);
                var checker = gameField.GetCheckerAtPosition(position);
                if (checker == Checker.None || checker.isWhite() != isWhiteTurn)
                    continue;
                var actions = ActionsGenerator.GetCheckerActions(position, gameField);
                if (actions.Count == 0)
                    continue;
                foreach (var action in actions)
                {
                    thereIsNoStep = false;
                    ActionsExecutor.ExecuteAction(action);
                    SimulateScoreBody(scoreIndex, !isWhiteTurn, step + 1);
                    ActionsExecutor.CancelLastAction();
                }
            }

            if( thereIsNoStep )
                StoreResult(scoreIndex);
        }

        protected void StoreResult(int scoreIndex)
        {
            var removedWhite = beginWhiteCount - ActionsExecutor.WhiteCheckersCount;
            var removedBlack = beginBlackCount - ActionsExecutor.BlackCheckersCount;
            var score = isWhitePlayer ? removedBlack - removedWhite : removedWhite - removedBlack;
            Results[scoreIndex].Score += score;
            return;
        }
    }
}
