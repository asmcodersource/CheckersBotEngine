using CheckersEngine.Controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine.GameEngine
{
    public enum GameState
    {
        WaitForNextStep,
        NoMoreStepsLeft,
        WrongActionProvided,
        WrontActionMustBeat,
    }

    public class Game
    {
        public AbstractController BlackController { get; protected set; }
        public AbstractController WhiteController { get; protected set; }
        public ActionsExecutor ActionsExecutor { get; protected set; }
        public bool IsWhiteTurn { get; protected set; }
        protected GameField gameField { get; set; }

        public Game(AbstractController blackController, AbstractController whiteController ) { 
            BlackController = blackController;
            WhiteController = whiteController;
            
        }

        public void InitializeGame()
        {
            IsWhiteTurn = true;
            gameField = new GameField();
            gameField.InitializeField();
            ActionsExecutor = new ActionsExecutor(gameField);
            ActionsExecutor.RecountCheckersCount();
        }

        public async Task<GameState> MakeStep()
        {
            var controller = IsWhiteTurn ? WhiteController : BlackController;
            var (isHaveSteps, isHaveBeatSteps) = controller.IsControllerHavePossibleStep(gameField);
            if (isHaveSteps == false)
                return GameState.NoMoreStepsLeft;
            var action = await controller.GetAction(ActionsExecutor);
            if( isHaveBeatSteps )
            {
                // Beating steps must be executed first
                if (action is CheckerMoveAction)
                    return GameState.WrontActionMustBeat;
            }
            if( action == null )
                throw new NullReferenceException("Game controller actions is null");
            if ( action.VerifyAction(gameField) == false )
                return GameState.WrongActionProvided;
            if (ActionsExecutor.ExecuteAction(action))
                IsWhiteTurn = !IsWhiteTurn;
            return GameState.WaitForNextStep;
        }
    }
}
