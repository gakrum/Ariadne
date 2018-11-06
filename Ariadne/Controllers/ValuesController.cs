using Ariadne.Models;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace Ariadne.Controllers
{
    public class MazeController : ApiController
    {
        [Route("api/solveMaze")]
        [ResponseType(typeof(SolutionViewModel))]
        public async Task<HttpResponseMessage> Post([FromBody]MazePostModel mazeModel)
        {
            if (string.IsNullOrWhiteSpace(mazeModel.mazeString))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            var theseus = new MazeModel(mazeModel.mazeString); 

            var solutionModel = theseus.SolveMaze();
            return Request.CreateResponse(HttpStatusCode.OK, solutionModel);
        }
    }
}
