// See https://aka.ms/new-console-template for more information
using CheckersBotEngine;

GameField gameField = new GameField();
gameField.InitializeField();

RandomizeTesting.Run(gameField);