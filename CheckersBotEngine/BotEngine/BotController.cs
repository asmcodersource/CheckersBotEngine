using CheckersEngine.GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine.BotEngine
{
    public class BotController
    {
        public GameField gameField { get; protected set; }
        public bool IsWhite { get; protected set; }
        public int Complexity { get; protected set; }
        public double RandomPart { get; protected set; } = 0.15;

        public BotController(GameField gameField, bool isWhite, int complexity = 5 ) 
        {
            this.gameField = gameField;    
            this.IsWhite = isWhite;
            this.Complexity = complexity;
        }

        public CheckerAction? GetAction()
        {
            FieldSimulationExecutor simulator = new FieldSimulationExecutor(gameField, Complexity);
            simulator.Simulate(gameField, null, IsWhite);
            var results = simulator.Results;
            var bestResult = GetBestResult(results);
            return bestResult == null ? null : bestResult.FirstCheckerAction;
        }

        private FieldSimulationResult? GetBestResult(List<FieldSimulationResult> results )
        {
            var array = results.ToArray();
            if( IsWhite )
                Array.Sort(array, FieldSimulationResult.CompareResultsForWhite);
            else
                Array.Sort(array, FieldSimulationResult.CompareResultsForBlack);

            if (array.Length == 0)
                return null;

            Random random = new Random(Environment.TickCount);
            return array[^(1+(int)(random.Next(array.Length) * RandomPart))];
        }

        private int ComputeResultScore(FieldSimulationResult result)
        {
            if (IsWhite)
                return result.removedBlack - result.removedWhite;
            else
                return result.removedWhite - result.removedBlack;
        }
    }
}
