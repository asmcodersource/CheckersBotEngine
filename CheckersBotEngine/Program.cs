// See https://aka.ms/new-console-template for more information
using CheckersBotEngine;
using CheckersEngine;
using CheckersEngine.BotCore;
using CheckersEngine.Controller;
using CheckersEngine.GameEngine;


while (true)
{
    AbstractController whiteController = new BotController(true, 4);
    AbstractController blackController = new BotController(false, 1);
    Game game = new Game(blackController, whiteController);
    game.InitializeGame();
    GameState state = GameState.WaitForNextStep;
    while (state == GameState.WaitForNextStep)
    {
        Console.WriteLine(game.ActionsExecutor.GameField);
        state = await game.MakeStep();
    }

    Console.WriteLine($"Game state = {state}");
    Console.ReadKey(true);
}