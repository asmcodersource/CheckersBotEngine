using CheckersEngine.BotEngine;
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
        public BotController WhiteBot { get; protected set; }
        public BotController BlackBot { get; protected set; }

        public BotTesting(GameField gameField ) { 
            GameField = gameField;
            WhiteBot = new BotController(gameField, true, 5);
            BlackBot = new BotController(gameField, false, 5);
            actionsExecutor = new ActionsExecutor(gameField);
        }

        public void Run()
        {
            bool isWhiteTurn = true;
            while (true)
            {
                CheckerAction action = isWhiteTurn ? WhiteBot.GetAction() : BlackBot.GetAction();
                if (action == null)
                {
                    Console.WriteLine("No actions left!");
                    break;
                }
                actionsExecutor.ExecuteAction(action);
                Console.WriteLine("______________________________________");
                //Console.ReadKey(true);
                Console.WriteLine(GameField);
                isWhiteTurn = !isWhiteTurn;
            }
        }
    }
}
