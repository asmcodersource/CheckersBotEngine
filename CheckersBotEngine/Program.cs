// See https://aka.ms/new-console-template for more information
using CheckersBotEngine;
using CheckersEngine;
using CheckersEngine.BotEngine;
using CheckersEngine.GameEngine;

while (true)
{
    GameField gameField = new GameField();
    gameField.InitializeField();
    Console.WriteLine(gameField);

    BotTesting botTesting = new BotTesting(gameField);
    botTesting.Run();

    Console.ReadKey(true);
}