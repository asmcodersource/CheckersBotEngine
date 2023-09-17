using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersBotEngine
{
    public static class RandomizeTesting
    {
        static GameField GameField { get; set; }

        public static void Run(GameField gameField)
        {
            // Print starting game field state
            GameField = gameField;
            Console.WriteLine(GameField);

            // Simulate game by random steps
            ActionsExecutor actionsExecutor = new ActionsExecutor(gameField);
            for( int i = 0; i < 500; i++ )
            {
                var pos = FindChecker(i % 2 == 0);
                var actions = ActionsGenerator.GetCheckerActions(pos, gameField);
                if( actions.Count() == 0)
                {
                    i--;
                    continue;
                }

                Random random = new Random();
                var actionId =random.Next(0, actions.Count());
                actionsExecutor.ExecuteAction(actions[actionId]);
                Console.WriteLine("________________________________________________________");
                Console.ReadKey(true);
                Console.WriteLine(GameField);
            }
        }

        public static FieldPosition FindChecker( bool findWhite )
        {
            Random random = new Random();
            Checker checker = Checker.None;
            FieldPosition position;
            do
            {
                int x = random.Next(0, 8);
                int y = random.Next(0, 8);
                position = new FieldPosition(x, y);
                checker = GameField.GetCheckerAtPosition(position);
            } while (checker == Checker.None || checker.isWhite() != findWhite);
            return position;
        }
    }
}
