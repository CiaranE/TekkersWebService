using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using TekkersService.DataObjects;
using TekkersWebService.Models;

namespace TekkersWebService.Controllers
{
    public class TeamController : TableController<Team>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            TekkersContext context = new TekkersContext();
            DomainManager = new EntityDomainManager<Team>(context, Request);
        }

        // GET tables/Team
        public IQueryable<Team> GetAllTeam()
        {
            return Query();
        }

        // GET tables/Team/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Team> GetTeam(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Team/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Team> PatchTeam(string id, Delta<Team> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Team
        public async Task<IHttpActionResult> PostTeam(Team team)
        {
            Team current = await InsertAsync(team);
            return CreatedAtRoute("Tables", new { id = current.Id }, current);
        }

        // DELETE tables/Team/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task DeleteTeam(string id)
        {
            var conn = new TekkersContext();
            var team = conn.Teams.Single(t => t.Id == id);
            var playersOnTeam = team.TeamPlayers.ToList();        //conn.Players.Where(p => p.PlayersTeam.Id == id);
            foreach (var p in playersOnTeam)
            {
                p.PlayersTeam = null;
            }
            conn.Teams.Remove(team);
            await conn.SaveChangesAsync();
            return;
        }
    }
}
