using CheckersEngine.GameEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersEngine.Controller
{
    public abstract class AbstractController
    {
        public abstract Task<CheckerAction?> GetAction(ActionsExecutor actionsExecutor);
    }
}
