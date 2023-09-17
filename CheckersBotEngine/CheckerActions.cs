using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersBotEngine
{
    abstract class CheckerAction
    {
        FieldPosition fieldStartPosition;
        FieldPosition fieldEndPosition;

        public abstract bool VerifyAction();
    }

    class MoveWhite: CheckerAction
    {
        public override bool VerifyAction()
        {
            throw new NotImplementedException();
        }
    }

    class MoveBlack : CheckerAction
    {
        public override bool VerifyAction()
        {
            throw new NotImplementedException();
        }
    }

    class MoveQueen : CheckerAction
    {
        
        public override bool VerifyAction()
        {
            throw new NotImplementedException();
        }
    }

    class BeatByWhiteChecker : CheckerAction
    {
        FieldPosition checkerRemovePosition;
        public override bool VerifyAction()
        {
            throw new NotImplementedException();
        }
    }

    class BeatByBlackChecker : CheckerAction
    {
        FieldPosition checkerRemovePosition;
        public override bool VerifyAction()
        {
            throw new NotImplementedException();
        }
    }

    class BeatByWhiteQueen : CheckerAction
    {
        FieldPosition checkerRemovePosition;
        public override bool VerifyAction()
        {
            throw new NotImplementedException();
        }
    }

    class BeatByBlackQueen : CheckerAction
    {
        FieldPosition checkerRemovePosition;
        public override bool VerifyAction()
        {
            throw new NotImplementedException();
        }
    }
}
