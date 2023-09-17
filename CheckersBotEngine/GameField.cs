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
    public enum Checker {
        None,
        White, 
        Black,
        WhiteQueen,
        BlackQueen
    };

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

        public Checker GetCheckerAtPosition( int x, int y ) {
            if (CheckersField is null)
                throw new NullReferenceException("Game field is null");
            return CheckersField[y, x];
        }
    }
}

