using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CheckersBotEngine
{
    public class GameField
    {
        public Checker[,]? CheckersField { get; protected set; }

        /// <summary>
        /// Initialize this game field, by creating chekers on game-start position.
        /// Previous game field state will be losted
        /// </summary>
        public void InitializeField()
        {
            // Create game field object
            CheckersField = new Checker[8, 8];

            // Place black chekers
            for (int y = 5; y < 8; y++)
                for (int x = 0; x < 8; x++)
                    if ((y + x) % 2 == 0)
                        CheckersField[y, x] = Checker.White;

            // Place While chekers
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 8; x++)
                    if ((y + x) % 2 == 0)
                        CheckersField[y, x] = Checker.Black;
        }

        /// <summary>
        /// Make string representation of game field.
        /// Useful for debugging 
        /// </summary>
        public override String ToString()
        {
            if( CheckersField is null )
                return "Game field is null";

            var checkersSymbols = new Dictionary<Checker, string>()
            {
                { Checker.None, "  " },
                { Checker.White, "WC" },
                { Checker.Black, "BC" },
                { Checker.WhiteQueen, "WK" },
                { Checker.BlackQueen, "BK" },
            };

            StringBuilder builder = new StringBuilder();
            for (int y = 0; y < 8; y++)
            {
                for (int x = 0; x < 8; x++)
                {
                    builder.Append(checkersSymbols[CheckersField[y, x]]);
                    builder.Append(' ');
                }
                builder.Append('\n');
            }

            return builder.ToString();
        }

        public Checker GetCheckerAtPosition( int x, int y, bool inverted = false ) {
            if (CheckersField is null)
                throw new NullReferenceException("Game field is null");
            var fieldPosition = new FieldPosition(x, y);
            if (fieldPosition.isInsideGameField() is not true)
                throw new ArgumentOutOfRangeException("Position is outside of game field");
            y = inverted ? 7 - y : y;
            return CheckersField[y, x];
        }

        public Checker GetCheckerAtPosition(FieldPosition position, bool inverted = false)
        {
            return GetCheckerAtPosition(position.X, position.Y, inverted);
        }

        public List<FieldPosition> GetCheckersBetweenPositions(FieldPosition pos1, FieldPosition pos2)
        {
            var results = new List<FieldPosition>();
            if (pos1.IsStepPossible(pos2) == false)
                return results;
            var dx = pos2.X - pos1.X > 0 ? 1 : -1;
            var dy = pos2.Y - pos1.Y > 0 ? 1 : -1;
            var checkPosition = pos1;
            do
            {
                checkPosition = new FieldPosition(checkPosition.X + dx, checkPosition.Y + dy);
                var checker = GetCheckerAtPosition(checkPosition);
                if (checker != Checker.None)
                    results.Add(checkPosition);
            } while (checkPosition != pos2);
            return results;
        }

        public void SetCheckerAtPosition(int x, int y, Checker checker)
        {
            SetCheckerAtPosition(new FieldPosition(x, y), checker);
        }

        public void SetCheckerAtPosition(FieldPosition fieldPosition, Checker checker)
        {
            if (CheckersField is null)
                throw new NullReferenceException("Game field is null");
            if (fieldPosition.isInsideGameField() is not true)
                throw new ArgumentOutOfRangeException("Position is outside of game field");
            CheckersField[fieldPosition.Y, fieldPosition.X] = checker;
        }
    }
}

