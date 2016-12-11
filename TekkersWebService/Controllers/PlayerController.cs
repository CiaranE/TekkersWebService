using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using Microsoft.Azure.Mobile.Server;
using TekkersService.DataObjects;
using System.Collections.Generic;
using System.Web.Routing;
using System;
using TekkersWebService.DataObjects;
using TekkersWebService.Models;

namespace TekkersWebService.Controllers
{
    public class PlayerController : TableController<Player>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            TekkersContext context = new TekkersContext();
            DomainManager = new EntityDomainManager<Player>(context, Request, enableSoftDelete: true);
        }

        // GET tables/Player
        public List<Player> GetAllPlayer()
        {
            TekkersContext conn = new TekkersContext();
            List<Player> playerList = conn.Players.Where(p => p.Deleted == false).ToList();

            //IF THE PLAYER HAS BEEN ADDED WITHOUT BEING ASSIGNED A TEAM OR THE TEAM HAS BEEN REMOVED SET PLAYER AS "NO TEAM" OR "UNASSIGNED"
            Team team = new Team();
            team.TeamName = "Unassigned";
            foreach (var p in playerList)
            {
                if (p.PlayersTeam == null)
                {
                    p.PlayersTeam = team;
                }
            }
            return playerList;
        }

        // GET tables/Player/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Player> GetPlayer(string id)
        {
            return Lookup(id);
        }


        // GET tables/Player/name
        [Route("tables/Player/GetPlayerByName/{name}")]
        public List<Player> GetPlayerByName(string name)
        {
            TekkersContext conn = new TekkersContext();
            List<Player> playersWithName = conn.Players.Where(p => (p.LastName.Contains(name) || p.FirstName.Contains(name)) && p.Deleted == false).ToList();
            return playersWithName;
        }


        [Route("tables/player/GetPlayersByAge/{age}")]
        public ICollection<Player> GetPlayersByAge(int age)
        {
            TekkersContext conn = new TekkersContext();
            ICollection<Player> allPlayers = conn.Players.ToList();
            foreach (var p in allPlayers)
            {
                p.AgeGroup = DateTime.Now.Year - p.DateOfBirth.Year;
            }
            ICollection<Player> listOfPlayersByAge = conn.Players.Where(t => t.AgeGroup == age).ToList();
            return listOfPlayersByAge;
        }

        [HttpPut]
        [Route("tables/Player/{id}")]
        // PATCH tables/Player/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public async Task PutPlayer(string id, Player p)
        {
            //return UpdateAsync(id, p);
            using (var conn = new TekkersContext())
            {
                Player thePlayer = conn.Players.Single(x => x.Id == p.Id);
                Team theTeam = conn.Teams.Single(t => t.Id == p.PlayersTeam.Id);
                thePlayer.PlayersTeam = null;
                thePlayer.FirstName = p.FirstName;
                thePlayer.LastName = p.LastName;
                thePlayer.ParentFName = p.ParentFName;
                thePlayer.ParentLName = p.ParentLName;
                thePlayer.Email = p.Email;
                thePlayer.DateOfBirth = p.DateOfBirth;
                thePlayer.AgeGroup = p.DateOfBirth.Year;
                thePlayer.PhoneNum = p.PhoneNum;
                thePlayer.PlayersTeam = theTeam;
                try
                {
                    await conn.SaveChangesAsync();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = String.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }
            }
        }

        // POST tables/Player
        public async Task PostPlayer(Player item)
        {
            //Player current = await InsertAsync(item);
            //return CreatedAtRoute("Tables", new { id = current.Id }, current);
            using (var context = new TekkersContext())
            {
                Team theTeam = context.Teams.Single(t => t.Id == item.PlayersTeam.Id);
                item.PlayersTeam = null;
                item.PlayersTeam = theTeam;
                //context.Players.Add(item);
                theTeam.TeamPlayers.Add(item);
                try
                {
                    await context.SaveChangesAsync();
                }
                catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
                {
                    Exception raise = dbEx;
                    foreach (var validationErrors in dbEx.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            string message = String.Format("{0}:{1}",
                                validationErrors.Entry.Entity.ToString(),
                                validationError.ErrorMessage);
                            raise = new InvalidOperationException(message, raise);
                        }
                    }
                    throw raise;
                }

            }
        }

        [HttpDelete]
        [Route("tables/Player/DeletePlayer/{id}")]
        // DELETE tables/Player/DeletePlayer/48D68C86-6EA6-4C25-AA33-223FC9A27959

        public async Task DeletePlayer(string id)
        {
            var conn = new TekkersContext();
            Player player = conn.Players.SingleOrDefault(p => p.Id == id);
            //Team playersteam = new Team();
            //Team playersteam = conn.Teams.Single(t => t.TeamPlayers.Contains(player));

            if (player != null)
            {
                //REMOVE THE PLAYER FROM THE TEAM
                Team playersteam = player.PlayersTeam;
                List<Player> theTeamsPlayers = playersteam.TeamPlayers.ToList();
                theTeamsPlayers.Remove(player);
                playersteam.TeamPlayers = theTeamsPlayers; 
                await conn.SaveChangesAsync();

                //DELETE THE PLAYERS ASSESSMENTS AND TESTS
                if (player.PlayerAssessments.Count > 0 || player.PlayerAssessments != null)
                {
                    List<Assessment> assessments = new List<Assessment>();
                    assessments = conn.Assessments.Where(p => p.Player.Id == id).ToList();
                    foreach (var a in assessments)
                    {
                        if (a.Tests.Count > 0)
                        {
                            List<Test> tests = new List<Test>();
                            tests = conn.Tests.Where(t => t.AssessmentTest.Id == a.Id).ToList();
                            foreach (var t in tests)
                            {
                                conn.Tests.Remove(t);
                                await conn.SaveChangesAsync();
                            }
                            conn.Assessments.Remove(a);
                            await conn.SaveChangesAsync();
                        }
                    }
                }
                //DELETE THE PLAYER
                await DeleteAsync(id);
                return;
            }
            else
            {
                return;
            }
        }

        //GET tables/Player/GetPlayersOnTeamAsync/48D68C86-6EA6-4C25-AA33-223FC9A27959
        [Route("tables/Player/GetPlayersOnTeam/{id}")]
        public List<Player> GetPlayersOnTeam(string id)
        {
            var conn = new TekkersContext();
            var team = conn.Teams.Single(t => t.Id == id);
            var playersOnTeam = team.TeamPlayers.ToList();
            return playersOnTeam;
        }

        [Route("tables/Player/GetPlayerByParent/{name}")]
        public List<Player> GetPlayerByParent(string name)
        {
            var conn = new TekkersContext();
            var parents = conn.Players.Where(p => p.ParentFName == name).ToList();
            return parents;
        }
    }
}
