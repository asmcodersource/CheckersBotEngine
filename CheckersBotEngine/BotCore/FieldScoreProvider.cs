using CheckersEngine.GameEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CheckersEngine.BotCore
{

    public record FieldScoreResult
    {
        public CheckerAction FirstCheckerAction { get; set; } = null;
        public long WorstWaste { get; set; } = 0;
        public long BestWin { get; set; } = 0;
        public long Score { get; set; }

        public FieldScoreResult()
        {

        }

        public static int CompareResults(FieldScoreResult r1, FieldScoreResult r2)
        {
             return -(int)Math.Clamp((r1.Score - r2.Score), int.MinValue, int.MaxValue);
        }

        public String SerializeToJson()
        {
            var jsonString = JsonSerializer.Serialize(this);
            return jsonString;
        }

        public static FieldScoreResult DeserializeFromJson(string jsonString)
        {
            var options = new JsonSerializerOptions
            {
                IncludeFields = true,
                WriteIndented = true,
            };
            object? obj = JsonSerializer.Deserialize<FieldScoreResult>(jsonString, options);
            return (FieldScoreResult)obj;
        }
    }

    public class FieldScoreProvider
    {
        public List<FieldScoreResult> Results { get; protected set; }
        protected int simulationSteeps { get; set; }
        protected Game game { get; set; }

        private int beginBlackCount = 0;
        private int beginWhiteCount = 0;
        private bool isWhiteControllerTurn;

        public FieldScoreProvider(Game game, int simulationStepsCount ) {
            this.game = game;
            Results = new List<FieldScoreResult>();
            simulationSteeps = simulationStepsCount;
            isWhiteControllerTurn = this.game.IsWhiteTurn;
            this.game.ActionsExecutor.RecountCheckersCount();
            beginBlackCount = this.game.ActionsExecutor.BlackCheckersCount;
            beginWhiteCount = this.game.ActionsExecutor.WhiteCheckersCount;
        }

        public async Task GetPositionScore( bool isWhiteTurn, int step = 0 ) 
        {
            List<Task> tasks = new List<Task>();
            for( int i = 0; i < 64; i++)
            {
                int x = i % 8;
                int y = i / 8;

                var checkerPosition = new FieldPosition(x, y);
                var checkerType = game.GameField.GetCheckerAtPosition(x, y);
                if (checkerType == Checker.None || checkerType.isWhite() != isWhiteTurn)
                    continue;

                //var result = game.ScoreStorage.GetResult(game.GameField.GetGameStateIdentify(), isWhiteTurn);
                var task = Task.Run(()=>StartSimulationScore(checkerPosition));
                tasks.Add(task);
            }
            if (tasks.Count == 0)
                throw new Exception("Field score provider unknown error");
            await Task.WhenAll(tasks);
        }

        protected async Task StartSimulationScore(FieldPosition position)
        {
            var gameCopy = (Game)game.Clone();
            var fakeController = new FakeController(isWhiteControllerTurn);
            var actions = ActionsGenerator.GetCheckerActions(position, gameCopy.GameField);
            foreach(var action in actions)
            {
                fakeController.ActionToComplete = action;
                gameCopy.ChangeController(fakeController, isWhiteControllerTurn);
                var gameState = await gameCopy.MakeStep();
                if (gameState != GameState.WaitForNextStep)
                    continue;
                var fieldScoreResult = new FieldScoreResult();
                fieldScoreResult.FirstCheckerAction = action;
                int resultIndex = 0;
                lock (Results) {
                    Results.Add(fieldScoreResult);
                    resultIndex = Results.Count - 1;
                }
                await SimulateScoreBody(gameCopy, resultIndex, 0);
                gameCopy.ActionsExecutor.CancelLastAction();
            }
        }

        protected async Task SimulateScoreBody(Game game, int resultIndex, int step )
        {
            if (step >= simulationSteeps)
                return;
            for (int i = 0; i < 64; i++)
            {
                int x = i % 8;
                int y = i / 8;

                var checkerPosition = new FieldPosition(x, y);
                var checkerType = game.GameField.GetCheckerAtPosition(x, y);
                if (checkerType == Checker.None || checkerType.isWhite() != game.IsWhiteTurn)
                    continue;
                var fakeController = new FakeController(game.IsWhiteTurn);
                var actions = ActionsGenerator.GetCheckerActions(checkerPosition, game.GameField);
                foreach (var action in actions)
                {
                    fakeController.ActionToComplete = action;
                    game.ChangeController(fakeController, game.IsWhiteTurn);
                    var gameState = await game.MakeStep();
                    if (gameState != GameState.WaitForNextStep)
                        continue;
                    var fieldScoreResult = new FieldScoreResult();
                    fieldScoreResult.FirstCheckerAction = action;
                    await SimulateScoreBody(game, resultIndex, step + 1);
                    game.ActionsExecutor.CancelLastAction();
                }
            }
            StoreScore(resultIndex, game, step + 1);
        }

        protected void StoreScore(int resultIndex, Game game, int step )
        {
            var removedWhite = beginWhiteCount - game.ActionsExecutor.WhiteCheckersCount;
            var removedBlack = beginBlackCount - game.ActionsExecutor.BlackCheckersCount;
            int waste = isWhiteControllerTurn ? removedWhite : removedBlack;
            int win = isWhiteControllerTurn ? removedBlack : removedWhite;
            Results[resultIndex].WorstWaste = Math.Max(waste, Results[resultIndex].WorstWaste);
            Results[resultIndex].BestWin = Math.Max(win, Results[resultIndex].BestWin);
            Results[resultIndex].Score = Results[resultIndex].BestWin - Results[resultIndex].WorstWaste;
        }
    }
}
