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
        public int removedWhite { get; set; } = 0;
        public int removedBlack { get; set; } = 0;

        public static int CompareResults(FieldSimulationResult r1, FieldSimulationResult r2)
        {
            var result = (r1.removedBlack - r1.removedWhite) - (r2.removedBlack - r2.removedWhite);
            return result == 0 ? 0 : result > 0 ? 1 : - 1;
        }
    }

    public class FieldSimulationExecutor
    {
        public ActionsExecutor ActionsExecutor { get; set; }
        public List<FieldSimulationResult> Results { get; protected set; }
        public int SimulationSteeps { get; protected set; }

        public FieldSimulationExecutor(GameField gameField, int simulationStepsCount = 100 ) {
            ActionsExecutor = new ActionsExecutor(gameField);
            Results  = new List<FieldSimulationResult>();
            SimulationSteeps = simulationStepsCount;
        }

        public void Simulate(GameField gameField, FieldSimulationResult simulationScore, bool isWhiteTurn, int step = 0 ) 
        {
            if (step >= SimulationSteeps)
            {
                simulationScore.removedWhite = 12 - ActionsExecutor.WhiteCheckersCount;
                simulationScore.removedBlack = 12 - ActionsExecutor.BlackCheckersCount;
                Results.Add(simulationScore);
                return;
            }

            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    if ((x + y) % 2 != 0)
                        continue;
                    var position = new FieldPosition(x, y);
                    var checker = gameField.GetCheckerAtPosition(position);
                    if (checker == Checker.None || checker.isWhite() != isWhiteTurn)
                        continue;
                    var actions = ActionsGenerator.GetCheckerActions(position, gameField);
                    if (actions.Count == 0)
                        continue;
                    var temp = simulationScore;
                    foreach (var action in actions)
                    {
                        if (simulationScore == null)
                        {   
                            simulationScore = new FieldSimulationResult();
                            simulationScore.FirstCheckerAction = action;
                        }
                        ActionsExecutor.ExecuteAction(action);
                        Simulate(gameField, simulationScore, !isWhiteTurn, step + 1);
                        ActionsExecutor.CancelLastAction();
                    }
                    simulationScore = temp;
                }
            }
        }
    }
}
