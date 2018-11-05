using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Ariadne.Models
{
    public struct Coordinate
    {
        public int x;
        public int y;
    };

    public struct Simple
    {
        public int Position;
        public bool Exists;
        public double LastValue;
    };
}