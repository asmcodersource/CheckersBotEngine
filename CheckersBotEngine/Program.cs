// See https://aka.ms/new-console-template for more information
using CheckersBotEngine;

GameField gameField = new GameField();
gameField.InitializeField();

var fieldPosition = new FieldPosition(4, 4);
gameField.SetCheckerAtPosition(fieldPosition, Checker.BlackQueen);

Console.WriteLine(gameField);

var control = new FieldPosition(4, 4);
var steps = control.GetAllPossibleSteps();
Console.WriteLine("All steps: ");
foreach (var step in steps)
    Console.WriteLine(step);

var actions = ActionsGenerator.GetCheckerActions(control, gameField);
Console.WriteLine($"\nAll actions for checker {control}: ");
foreach (var action in actions)
    Console.WriteLine($"{action}\n");
