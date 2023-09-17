using CheckersEngine.GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine.BotEngine
{

    public record FieldSimulationResult
    {
        public CheckerAction FirstCheckerAction { get; set; } = null;
        public int Score { get; set; }

        public static int CompareResults(FieldSimulationResult r1, FieldSimulationResult r2)
        {
            return r1.Score - r2.Score;
        }
    }

    public class FieldSimulationExecutor
    {
        public ActionsExecutor ActionsExecutor { get; set; }
        public List<FieldSimulationResult> Results { get; protected set; }
        public int SimulationSteeps { get; protected set; }
        private int beginBlackCount = 0;
        private int beginWhiteCount = 0;
        private bool isWhitePlayer;

        public FieldSimulationExecutor(GameField gameField, bool isWhitePlayer, int simulationStepsCount = 100 ) {
            ActionsExecutor = new ActionsExecutor(gameField);
            Results  = new List<FieldSimulationResult>();
            SimulationSteeps = simulationStepsCount;
            beginBlackCount = ActionsExecutor.BlackCheckersCount;
            beginWhiteCount = ActionsExecutor.WhiteCheckersCount;
            this.isWhitePlayer = isWhitePlayer;
        }

        public void Simulate(GameField gameField, FieldSimulationResult simulationScore, bool isWhiteTurn, int step = 0 ) 
        {
            if (step >= SimulationSteeps)
            {
                StoreResult(simulationScore);
                return;
            }
            var temp = simulationScore;
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {

                    var position = new FieldPosition(x, y);
                    var checker = gameField.GetCheckerAtPosition(position);
                    if (checker == Checker.None || checker.isWhite() != isWhiteTurn)
                        continue;
                    var actions = ActionsGenerator.GetCheckerActions(position, gameField);
                    if (actions.Count == 0)
                        continue;
                    foreach (var action in actions)
                    {
                        simulationScore = temp;
                        if (simulationScore == null)
                        {   
                            simulationScore = new FieldSimulationResult();
                            simulationScore.FirstCheckerAction = action;
                        }
                        ActionsExecutor.ExecuteAction(action);
                        Simulate(gameField, simulationScore, !isWhiteTurn, step + 1);
                        ActionsExecutor.CancelLastAction();
                    }
                }
            }
            if( step != 0 )
                StoreResult(simulationScore);
        }

        protected void StoreResult(FieldSimulationResult simulationScore)
        {
            var removedWhite = beginWhiteCount - ActionsExecutor.WhiteCheckersCount;
            var removedBlack = beginBlackCount - ActionsExecutor.BlackCheckersCount;
            simulationScore.Score = isWhitePlayer ? removedBlack - removedWhite : removedWhite - removedBlack;
            Results.Add(simulationScore);
            return;
        }
    }
}
