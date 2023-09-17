// See https://aka.ms/new-console-template for more information
using CheckersBotEngine;
using CheckersEngine;
using CheckersEngine.BotEngine;
using CheckersEngine.GameEngine;

int win = 0;
while (true)
{
    GameField gameField = new GameField();
    gameField.InitializeField();

    BotTesting botTesting = new BotTesting(gameField);
    win += botTesting.Run();
    Console.WriteLine(win);
}