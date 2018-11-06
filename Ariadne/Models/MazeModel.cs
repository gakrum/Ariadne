using Ariadne.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Ariadne.Models
{
    public class MazeModel
    {
        private int _yUBound;
        private int _xUBound;

        private char[,] _sourceMap = null;
        private string _inputMazeString = null;

        private char STARTCHAR = 'A';
        private char STOPCHAR = 'B';
        private char OPENCHAR = '.';
        private char WALLCHAR = '#';
        private char SOLUTIONCHAR = '@';

        public MazeModel(string mazeString)
        {
            if (!mazeString.Contains("A") || !mazeString.Contains("B"))
            {
                throw new ArgumentException("No start or end position found!");
            }

            _inputMazeString = mazeString;

            this._sourceMap = ParseMazeString(mazeString);

            StartPosition = FindStartOrStop(CharType.StartChar);
            EndPosition = FindStartOrStop(CharType.StopChar);
        }

        public SolutionViewModel SolveMaze()
        {
            var shortestPath = this.FindMazePath();

            string result = this.CreateMazeSolutionString(shortestPath);

            return new SolutionViewModel()
            {
                steps = shortestPath.GetUpperBound(0) + 1,
                solution = result
            };
        }

        private Coordinate FindStartOrStop(CharType charType)
        {
            var coordinate = new Coordinate();

            char compareChar = STARTCHAR;
            switch (charType)
            {
                case CharType.StartChar:
                    break;
                case CharType.StopChar:
                    compareChar = STOPCHAR;
                    break;
            }

            for (int x = 0; x <= _xUBound; x++)
            {
                for (int y = 0; y <= _yUBound; y++)
                {
                    if (this._sourceMap[x, y] == compareChar)
                    {
                        coordinate.x = x;
                        coordinate.y = y;

                        return coordinate;
                    }
                }
            }

            coordinate.x = -1;
            coordinate.y = -1;

            return coordinate;
        }

        private Coordinate StartPosition { get; set; }

        private Coordinate EndPosition { get; set; }

        private bool IsEndPosition(Coordinate coordinate)
        {
            return (EndPosition.x == coordinate.x) && (EndPosition.y == coordinate.y);
        }

        private bool isStartPosition(Coordinate coordinate)
        {
            return (StartPosition.x == coordinate.x) && (StartPosition.y == coordinate.y);
        }

        private bool PeekMove(Coordinate coordinate)
        {
            if ((coordinate.x < 0 || coordinate.x >= this._xUBound) || coordinate.y < 0 || coordinate.y >= this._yUBound)
            {
                return false;
            }

            char coordinateVal = this._sourceMap[coordinate.x, coordinate.y];

            return (coordinateVal == this.OPENCHAR) || (coordinateVal == STOPCHAR) || (coordinateVal == STARTCHAR);
        }

        private char[,] ParseMazeString(string mazeString)
        {
            var splitArray = mazeString.Split(Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            _yUBound = splitArray.GetUpperBound(0);                             // array bound equals y boundary
            if (_yUBound < 0)
                throw new ArgumentException();

            _xUBound = splitArray[0].Length - 1;                                // get char length of string of x boundary
            char[,] mazeArray = new char[_xUBound + 1, _yUBound + 1];

            for (int y = 0; y <= _yUBound; y++)
            {
                var rowArray = splitArray[y].ToCharArray();
                for (int x = 0; x <= _xUBound; x++)
                {
                    mazeArray[x, y] = rowArray[x];
                }
            }

            return mazeArray;
        }

        private Coordinate[] GetCoordinateOptions(Coordinate coordinate)
        {
            return new Coordinate[] {
                                         new Coordinate() { x = coordinate.x, y = coordinate.y + 1 },
                                         new Coordinate() { x = coordinate.x - 1, y =  coordinate.y},
                                         new Coordinate() { x = coordinate.x, y =  coordinate.y - 1},
                                         new Coordinate() { x = coordinate.x + 1, y =  coordinate.y}
                                     };
        }

        private Coordinate MarkCoordinateSolved(Coordinate coordinate)
        {
            if (!(this.StartPosition.x == coordinate.x && this.StartPosition.y == coordinate.y))
            {
                this._sourceMap[coordinate.x, coordinate.y] = SOLUTIONCHAR;
            }

            return new Coordinate();
        }

        private string BuildOutputSolution(char[,] solvedMatrix)
        {
            StringBuilder sbSolution = new StringBuilder();

            for (int y = 0; y <= _yUBound; y++)
            {
                for (int x = 0; x <= _xUBound; x++)
                {
                    sbSolution.Append(solvedMatrix[x, y]);
                }

                sbSolution.Append(Environment.NewLine);
            }

            return sbSolution.ToString();
        }

        private string CreateMazeSolutionString(Coordinate[] shortestPath)
        {
            if (shortestPath == null || shortestPath.GetUpperBound(0) == 0)
            {
                throw new ArgumentException("No solution found!");
            };

            for (int i = 0; i <= shortestPath.GetUpperBound(0); i++)
            {
                if (!this.IsEndPosition(shortestPath[i]))
                {
                    this.MarkCoordinateSolved(shortestPath[i]);
                }
            }

            return BuildOutputSolution(this._sourceMap);
        }

        private Coordinate[] FindMazePath()
        {
            // solution modeled after maze algorithm at https://stackoverflow.com/questions/30551194/find-shortest-path-in-a-maze-with-recursive-algorithm

            var levelArray = this.MapMazeToStepArray();
            var moveStack = new Stack<Coordinate>();

            moveStack.Push(this.StartPosition);

            while (moveStack.Count > 0)
            {
                var coordinate = moveStack.Pop();
                if (this.IsEndPosition(coordinate))
                {
                    break;
                }

                var levelValue = levelArray[coordinate.x, coordinate.y];
                var possibleMoves = this.GetCoordinateOptions(coordinate);

                for (int i = 0; i <= possibleMoves.GetUpperBound(0); i++)
                {
                    var potentialMove = possibleMoves[i];

                    if (this.PeekMove(potentialMove))
                    {
                        if (levelArray[potentialMove.x, potentialMove.y] == 0)
                        {
                            moveStack.Push(potentialMove);
                            levelArray[potentialMove.x, potentialMove.y] = levelValue + 1;
                        }
                    }
                }
            }

            if (levelArray[this.EndPosition.x, this.EndPosition.y] == 0)
            {
                // Unable to solve this maze
                return null;
            }

            //Get the shortest path
            var pathStack = new Stack<Coordinate>();
            var pathCoordinate = this.EndPosition;

            while (!(this.isStartPosition(pathCoordinate)))
            {
                pathStack.Push(pathCoordinate);
                var levelValue = levelArray[pathCoordinate.x, pathCoordinate.y];
                var moveOptions = this.GetCoordinateOptions(pathCoordinate);

                for (int p = 0; p <= moveOptions.GetUpperBound(0); p++)
                {
                    if (this.PeekMove(moveOptions[p]))
                    {
                        if (levelArray[moveOptions[p].x, moveOptions[p].y] == levelValue - 1)
                        {
                            pathCoordinate = moveOptions[p];
                            break;
                        }
                    }
                }
            }

            return pathStack.ToArray();
        }

        /// <summary>
        /// Create steps array to 
        /// </summary>
        /// <returns></returns>
        private int[,] MapMazeToStepArray()
        {
            // fetch a 'walled-up' maze
            var result = this.InitBaseStepsArray();

            for (int x = 0; x <= _xUBound; x++)
            {
                for (int y = 0; y <= _yUBound; y++)
                {
                    // check source _matrix maze for open areas and the stop/exit
                    if (this._sourceMap[x, y] == OPENCHAR || 
                        this._sourceMap[x, y] == STOPCHAR)
                    {
                        result[x, y] = 0;
                    }
                    else
                    {
                        result[x, y] = -1;
                    }
                }
            }

            result[StartPosition.x, StartPosition.y] = 1;                   // set starting position directly

            return result;
        }

        /// <summary>
        /// Create an identically-sized maze and initialize to all coordinates to wall chars int value
        /// </summary>
        /// <returns></returns>
        private int[,] InitBaseStepsArray()
        {
            int[,] newMaze = new int[_xUBound + 1, _yUBound + 1];

            for (int x = 0; x <= _xUBound; x++)
            {
                for (int y = 0; y <= _yUBound; y++)
                {
                    newMaze[x, y] = this.WALLCHAR;
                }
            }

            return newMaze;
        }
    }
}
