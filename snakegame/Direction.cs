using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace snakegame
{
    public class Direction
    {
        public readonly static Direction left = new Direction(0, -1);
        public readonly static Direction right = new Direction(0, 1);
        public readonly static Direction up = new Direction(-1, 0);
        public readonly static Direction down = new Direction(1, 0);

        public int Rowoffest { get; }
        public int Coloffest { get; }

        private Direction(int rowoffest, int coloffest)
        {
        Rowoffest = rowoffest;
        Coloffest = coloffest;
        }

        public Direction Opposite()
        {
            return new Direction(-Rowoffest, -Coloffest);
        }

        public override bool Equals(object obj)
        {
            return obj is Direction direction &&
                   Rowoffest == direction.Rowoffest &&
                   Coloffest == direction.Coloffest;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Rowoffest, Coloffest);
        }

        public static bool operator ==(Direction left, Direction right)
        {
            return EqualityComparer<Direction>.Default.Equals(left, right);
        }

        public static bool operator !=(Direction left, Direction right)
        {
            return !(left == right);
        }
    }
}
