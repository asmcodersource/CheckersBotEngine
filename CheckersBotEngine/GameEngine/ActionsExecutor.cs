using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine.GameEngine
{
    public class ActionsExecutor
    {
        public int WhiteCheckersCount { get; protected set; } = 12;
        public int BlackCheckersCount { get; protected set; } = 12;
        public GameField GameField { get; protected set; }
        public List<CheckerAction> ActionsHistory { get; protected set; }


        public ActionsExecutor(GameField gameField)
        {
            GameField = gameField;
            ActionsHistory = new List<CheckerAction>();
        }

        public void ExecuteAction(CheckerAction action)
        {
            if (action is WrongAction)
                return;

            var checker = GameField.GetCheckerAtPosition(action.FieldStartPosition);
            GameField.SetCheckerAtPosition(action.FieldEndPosition, checker);
            GameField.SetCheckerAtPosition(action.FieldStartPosition, Checker.None);
            if (action.BecameQueen)
            {
                var queenType = checker.isWhite() ? Checker.WhiteQueen : Checker.BlackQueen;
                GameField.SetCheckerAtPosition(action.FieldEndPosition, queenType);
            }

            if (action is CheckerBeatAction)
            {
                GameField.SetCheckerAtPosition(((CheckerBeatAction)action).CheckerRemovePosition, Checker.None);
                if (checker.isWhite())
                    BlackCheckersCount--;
                else
                    WhiteCheckersCount--;
            }
            ActionsHistory.Add(action);
        }

        public void CancelLastAction()
        {
            if (ActionsHistory.Count == 0)
                throw new Exception("Action list is empty");

            var action = ActionsHistory.Last();
            var checker = GameField.GetCheckerAtPosition(action.FieldEndPosition);

            if (action.BecameQueen)
            {
                var checkerType = checker.isWhite() ? Checker.White : Checker.Black;
                checker = checkerType;
            }

            GameField.SetCheckerAtPosition(action.FieldStartPosition, checker);
            GameField.SetCheckerAtPosition(action.FieldEndPosition, Checker.None);

            if (action is CheckerBeatAction)
            {
                var beatAction = (CheckerBeatAction)action;
                GameField.SetCheckerAtPosition(beatAction.CheckerRemovePosition, beatAction.RemoveCheckerType);
                if (checker.isWhite())
                    BlackCheckersCount++;
                else
                    WhiteCheckersCount++;
            }
            ActionsHistory.RemoveAt(ActionsHistory.Count - 1);
        }
    }
}
