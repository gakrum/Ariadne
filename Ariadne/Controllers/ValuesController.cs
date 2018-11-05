using Ariadne.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Ariadne.Controllers
{
    public class MazeController : ApiController
    {
        private int _yUBound;
        private int _xUBound;

        public const char STARTCHAR = 'A';
        public const char STOPCHAR = 'B';

        [Route("api/solveMaze")]
        [ResponseType(typeof(SolutionViewModel))]
        public async Task<HttpResponseMessage> Post([FromBody]MazePostModel mazeModel)
        {
            if (string.IsNullOrWhiteSpace(mazeModel.mazeString))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var mazeArray = ParseMazeString(mazeModel.mazeString);
            var start = this.FindStart(mazeArray);
            if (start.x == -1 ||
                start.y == -1)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }





            return Request.CreateResponse(HttpStatusCode.OK,
                new SolutionViewModel()
                {
                    steps = 12,
                    solution = this.BuildOutputSolution(mazeArray)
                });
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

        private string BuildOutputSolution(char[,] solution)
        {
            StringBuilder sbSolution = new StringBuilder();

            for (int y = 0; y <= _yUBound; y++)
            {
                for (int x = 0; x <= _xUBound; x++)
                {
                    sbSolution.Append(solution[x, y]);
                }

                sbSolution.Append(Environment.NewLine);
            }

            return sbSolution.ToString();
        }

        private Coordinate FindStart(char[,] mazeArray)
        {
            Coordinate start;

            for (int y = 0; y <= _yUBound; y++)
            {
                for (int x = 0; x <= _xUBound; x++)
                {
                    if (mazeArray[x, y] == STARTCHAR)
                    {
                        start.x = x;
                        start.y = y;

                        return start;
                    }
                }
            }

            start.x = -1;
            start.y = -1;

            return start;
        }
    }
}
