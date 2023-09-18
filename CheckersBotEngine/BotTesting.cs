﻿using CheckersEngine.BotEngine;
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
            WhiteBot = new BotController(gameField, true, 4);
            BlackBot = new BotController(gameField, false, 5);
            actionsExecutor = new ActionsExecutor(gameField);
        }

        public async Task<int> Run()
        {
            actionsExecutor.RecountCheckersCount();
            bool isWhiteTurn = true;
            int i = 0;
            while ( i < 256 )
            {
                CheckerAction? action = isWhiteTurn ? await WhiteBot.GetAction(actionsExecutor) : await BlackBot.GetAction(actionsExecutor);
                if (action == null)
                    break;
                actionsExecutor.ExecuteAction(action);
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
