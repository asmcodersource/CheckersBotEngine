﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersBotEngine
{
    public record CheckerAction
    {
        public FieldPosition FieldStartPosition { get; set; }
        public FieldPosition FieldEndPosition { get; set; }
        public bool BecameQueen { get; set; } = false;

        public CheckerAction(FieldPosition start, FieldPosition end )
        {
            FieldStartPosition = start;
            FieldEndPosition = end;
        }

        public virtual bool VerifyAction( GameField gameField )
        {
            throw new NotImplementedException();
        }
    }

    public record CheckerBeatAction: CheckerAction
    {
        public FieldPosition CheckerRemovePosition { get; set; }
        public Checker RemoveCheckerType { get; set; }
        public CheckerBeatAction(FieldPosition start, FieldPosition end) : base(start, end) { }
    }

    public record CheckerMoveAction : CheckerAction
    {
        public CheckerMoveAction(FieldPosition start, FieldPosition end) : base(start, end) { }
    }

    public record WrongAction: CheckerAction
    {
        public WrongAction(FieldPosition start, FieldPosition end) : base(start, end) { }
    }

    public record MoveChecker : CheckerMoveAction
    {
        public MoveChecker(FieldPosition start, FieldPosition end): base(start, end) { }

        public override bool VerifyAction( GameField gameField )
        {
            if( FieldStartPosition.IsCloseStep(FieldEndPosition) == false )
                return false;
            if (gameField.GetCheckerAtPosition(FieldEndPosition) != Checker.None)
                return false;
            var checker = gameField.GetCheckerAtPosition(FieldStartPosition);
            if (FieldPosition.IsDirectionRight(FieldStartPosition, FieldEndPosition, checker) == false)
                return false;
            if (FieldEndPosition.IsBecameQueenPosition(checker.isWhite()))
                BecameQueen = true;
            return true;
        }
    }


    public record MoveQueen : CheckerMoveAction
    {
        public MoveQueen(FieldPosition start, FieldPosition end) : base(start, end) { }

        public override bool VerifyAction(GameField gameField)
        {
            var checkersOnLine = gameField.GetCheckersBetweenPositions(FieldStartPosition, FieldEndPosition);
            return checkersOnLine.Count == 0;
        }
    }

    public record BeatByChecker : CheckerBeatAction
    {
        public BeatByChecker(FieldPosition start, FieldPosition end) : base(start, end) { }

        public override bool VerifyAction(GameField gameField)
        {
            var dx = FieldEndPosition.X - FieldStartPosition.X;
            var dy = FieldEndPosition.Y - FieldStartPosition.Y;
            if( Math.Abs(dy) != 2 && Math.Abs(dx) != 2 )
                return false;
            dx = dx > 0 ? 1 : -1;
            dy = dy > 0 ? 1 : -1;
            CheckerRemovePosition = new FieldPosition(FieldStartPosition.X + dx, FieldStartPosition.Y + dy);
            var removeChecker = gameField.GetCheckerAtPosition(CheckerRemovePosition);
            RemoveCheckerType = removeChecker;
            var beatingChecker = gameField.GetCheckerAtPosition(FieldStartPosition);
            if (removeChecker.isWhite() == beatingChecker.isWhite() || removeChecker == Checker.None )
                return false;
            if( gameField.GetCheckerAtPosition(FieldEndPosition) != Checker.None )
                return false;
            if (FieldPosition.IsDirectionRight(FieldStartPosition, FieldEndPosition, beatingChecker) == false)
                return false;
            if (FieldEndPosition.IsBecameQueenPosition(beatingChecker.isWhite()))
                BecameQueen = true;
            return true;
        }
    }


    public record BeatByQueen : CheckerBeatAction
    {
        public BeatByQueen(FieldPosition start, FieldPosition end) : base(start, end) { }

        public override bool VerifyAction(GameField gameField)
        {
            var checkersOnLine = gameField.GetCheckersBetweenPositions(FieldStartPosition, FieldEndPosition);
            if( checkersOnLine.Count != 1 )
                return false;
            if( gameField.GetCheckerAtPosition(FieldEndPosition) != Checker.None )
                return false;
            var beatenPos = checkersOnLine.First();
            var beatenChecker = gameField.GetCheckerAtPosition(beatenPos);
            var beatingChecker = gameField.GetCheckerAtPosition(FieldStartPosition);
            CheckerRemovePosition = beatenPos;
            RemoveCheckerType = beatenChecker;
            return beatenChecker.isWhite() != beatingChecker.isWhite();
        }
    }
}
