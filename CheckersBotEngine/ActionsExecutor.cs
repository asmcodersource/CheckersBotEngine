using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersBotEngine
{
    public class ActionsExecutor
    {
        public GameField GameField { get; protected set; }
        public List<CheckerAction> ActionsHistory { get; protected set; }


        public ActionsExecutor(GameField gameField)
        {
            GameField = gameField;
            ActionsHistory = new List<CheckerAction>();
        } 
        
        public void ExecuteAction( CheckerAction action )
        {
            if (action is WrongAction)
                return;

            var checker = GameField.GetCheckerAtPosition(action.FieldStartPosition);
            GameField.SetCheckerAtPosition(action.FieldEndPosition, checker);
            GameField.SetCheckerAtPosition(action.FieldStartPosition, Checker.None);

            if (action is CheckerBeatAction)
                GameField.SetCheckerAtPosition(((CheckerBeatAction)action).CheckerRemovePosition, Checker.None);
            ActionsHistory.Add(action);
        }

        public void CancelLastAction()
        {
            if (ActionsHistory.Count == 0)
                throw new Exception("Action list is empty");

            var action = ActionsHistory.Last();
            var checker = GameField.GetCheckerAtPosition(action.FieldEndPosition);
            GameField.SetCheckerAtPosition(action.FieldStartPosition, checker);
            GameField.SetCheckerAtPosition(action.FieldEndPosition, Checker.None);

            if (action is CheckerBeatAction) {
                var beatAction = (CheckerBeatAction)action;
                GameField.SetCheckerAtPosition(beatAction.CheckerRemovePosition, beatAction.RemoveCheckerType);
            }
            ActionsHistory.RemoveAt(ActionsHistory.Count - 1);
        }
    }
}
