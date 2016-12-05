using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using System.Collections.Generic;
using TekkersWebService.DataObjects;
using TekkersWebService.Models;

namespace TekkersWebService.Controllers
{
    public class TestController : TableController<Test>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            TekkersContext context = new TekkersContext();
            DomainManager = new EntityDomainManager<Test>(context, Request, enableSoftDelete: true);
        }

        // GET tables/Test
        public IQueryable<Test> GetAllTest()
        {
            return Query();
        }

        // GET tables/Test/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Test> GetTest(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Test/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Test> PatchTest(string id, Delta<Test> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Test
        public async Task<IHttpActionResult> PostTest(Test item)
        {
            Test current = await InsertAsync(item);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        [HttpDelete]
        [Route("tables/Test/{id}")]
        // DELETE tables/Test/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteTest(string id)
        {
            return DeleteAsync(id);
        }

        // GET tables/Test/AssessmentID
        [Route("tables/Test/GetTestsByAssessment/{assessid}")]
        public ICollection<Test> GetTestsByAssessment(string assessid)
        {
            TekkersContext conn = new TekkersContext();
            Assessment theAssessment = conn.Assessments.Single(a => a.Id.Equals(assessid));
            Player thePlayer = theAssessment.Player;
            ICollection<Test> assessmentTests = conn.Tests.Where(t => t.AssessmentTest.Id.Equals(assessid) && t.PlayerTest.Id == thePlayer.Id).ToList();
            return assessmentTests;
        }

        [HttpPut]
        [Route("tables/Test/{testid}/{score}")]
        public async Task PutAsync(string testid, int score)
        {
            TekkersContext conn = new TekkersContext();
            var theTest = conn.Tests.Single(t => t.Id == testid);
            theTest.TestScore = score;
            await conn.SaveChangesAsync();
        }


        [Route("tables/test/GetAllTestsForPlayerAsync/{playerid}")]
        public ICollection<Test> GetAllTestsForPlayerAsync(string playerid)
        {
            TekkersContext conn = new TekkersContext();
            ICollection<Test> listOfTests = conn.Tests.Where(t => t.PlayerTest.Id == playerid).ToList();
            return listOfTests;
        }

        [Route("tables/test/GetAllTestsForTeamAsync/{teamid}")]
        public ICollection<Test> GetAllTestsForTeamAsync(string teamid)
        {
            TekkersContext conn = new TekkersContext();
            IEnumerable<Test> teamsTests = new List<Test>();
            teamsTests = from test in conn.Tests
                         join player in conn.Players
                         on test.PlayerTest.Id equals player.Id
                         join team in conn.Teams
                         on player.PlayersTeam.Id equals team.Id
                         where team.Id == teamid
                         select test;
            return teamsTests.ToList();
        }
    }
}
