using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersBotEngine
{
    public record FieldPosition
    {
        int X { get; set; }
        int Y { get; set; }

        public FieldPosition(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
        }

        /// <summary>
        /// Return true if FieldPostion values is inside game field.
        /// </summary>
        public bool isInsideGameField()
        {
            if (X < 0 || Y < 0)
                return false;
            if (X > 8 || Y > 8)
                return false;
            return true;
        }

        /// <summary>
        /// Return all posible steps on game field from current position.
        /// It doesn't check checker type, or game field status. 
        /// </summary>
        public List<FieldPosition> GetAllPossibleSteps()
        {
            var steps = new List<FieldPosition>();
            for (int y = 0; y < 3; y++)
                for (int x = 0; x < 8; x++)
                    if ((y + x) % 2 != 0 && IsStepPossible(y, x))
                        steps.Add(new FieldPosition(y, x));
            return steps;

            bool IsStepPossible(int x, int y)
            {
                var fieldPosition = new FieldPosition(x, y);
                if (fieldPosition.isInsideGameField() == false)
                    return false;
                return true;
            }
        }
    }
}
