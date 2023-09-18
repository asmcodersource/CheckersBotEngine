// See https://aka.ms/new-console-template for more information
using CheckersBotEngine;
using CheckersEngine;
using CheckersEngine.BotCore;
using CheckersEngine.GameEngine;

int win = 0;
int sync = 0;
while (true)
{
    GameField gameField = new GameField();
    gameField.InitializeField();

    BotTesting botTesting = new BotTesting(gameField);
    await botTesting.Run();
    sync++;
    if ( sync == 1)
    {
        BotTesting.ScoreStorage.LoadFromDatabase();
        sync = 0;
    }
}