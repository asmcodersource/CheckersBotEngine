using CheckersEngine.BotCore;
using CheckersEngine.Controller;
using CheckersEngine.GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine
{
    public class BotTesting
    {
        protected ActionsExecutor actionsExecutor { get; set; }
        public GameField GameField { get; protected set; }
        public AbstractController WhiteBot { get; protected set; }
        public AbstractController BlackBot { get; protected set; }
        public static ScoreStorage ScoreStorage { get; protected set; }

        static BotTesting()
        {
            ScoreStorage = new ScoreStorage();
            ScoreStorage.LoadFromDatabase();
        }

        public BotTesting(GameField gameField ) { 
            GameField = gameField;
            WhiteBot = new BotController(gameField, true, 5);
            BlackBot = new PlayerController(gameField, false);
            actionsExecutor = new ActionsExecutor(gameField, ScoreStorage);
        }

        public async Task<int> Run()
        {
            actionsExecutor.RecountCheckersCount();
            bool isWhiteTurn = true;
            int i = 0;
            while ( i < 64 )
            {
                CheckerAction? action = isWhiteTurn ? await WhiteBot.GetAction(actionsExecutor) : await BlackBot.GetAction(actionsExecutor);
                if (action == null)
                    break;
                actionsExecutor.ExecuteAction(action);
                Console.WriteLine(GameField);
                isWhiteTurn = !isWhiteTurn;
                i++;
            }
            if (this.actionsExecutor.BlackCheckersCount > this.actionsExecutor.WhiteCheckersCount)
                return -1;
            else if (this.actionsExecutor.BlackCheckersCount < this.actionsExecutor.WhiteCheckersCount)
                return +1;
            return 0;
        }
    }
}
