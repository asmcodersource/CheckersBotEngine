using CheckersEngine.Controller;
using CheckersEngine.GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine.BotCore
{
    internal class FakeController: AbstractController
    {
        public CheckerAction ActionToComplete { get; set; }

        public FakeController(bool isWhite) : base(isWhite)
        {
            IsWhiteController = isWhite;
        }

        public override async Task<CheckerAction?> GetAction(Game game, bool mustBeat)
        {
            return ActionToComplete;
        }
    }
}
