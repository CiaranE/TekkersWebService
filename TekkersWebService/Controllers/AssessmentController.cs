using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.OData;
using Microsoft.Azure.Mobile.Server;
using TekkersWebService.Models;
using TekkersWebService.DataObjects;
using System;
using System.Collections.Generic;

namespace TekkersWebService.Controllers
{
    public class AssessmentController : TableController<Assessment>
    {
        protected override void Initialize(HttpControllerContext controllerContext)
        {
            base.Initialize(controllerContext);
            TekkersContext context = new TekkersContext();
            DomainManager = new EntityDomainManager<Assessment>(context, Request, enableSoftDelete: true);
        }

        // GET tables/Assessment
        public IQueryable<Assessment> GetAllAssessment()
        {
            return Query();
        }

        // GET tables/Assessment/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public SingleResult<Assessment> GetAssessment(string id)
        {
            return Lookup(id);
        }

        // PATCH tables/Assessment/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task<Assessment> PatchAssessment(string id, Delta<Assessment> patch)
        {
            return UpdateAsync(id, patch);
        }

        // POST tables/Assessment
        [HttpPost]
        public async Task PostAssessment(Assessment item)
        {
            using (var context = new TekkersContext())
            {
                Player thePlayer = context.Players.Single(p => p.Id == item.Player.Id);
                item.Player = null;
                item.Player = thePlayer;
                foreach (Test t in item.Tests)
                {
                    t.PlayerTest = thePlayer;
                }
                thePlayer.PlayerAssessments.Add(item);
                await context.SaveChangesAsync();
            }
        }

        // DELETE tables/Assessment/48D68C86-6EA6-4C25-AA33-223FC9A27959
        public Task DeleteAssessment(string id)
        {
            return DeleteAsync(id);
        }

        //GET tables/Assessment/PostAssessmentScore/48D68C86-6EA6-4C25-AA33-223FC9A27959/12
        [Route("tables/Assessment/PutAssessmentScore/{id}/{score}")]
        public async Task PutAssessmentScore(string id, int score)
        {
            var conn = new TekkersContext();
            var assessment = conn.Assessments.Single(ass => ass.Id == id);
            assessment.AssessmentScore = score;
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
            return;
        }

        [Route("tables/Assessment/GetMostRecentAssessmentForPlayer/{id}")]
        public Assessment GetMostRecentAssessmentForPlayer(string id)
        {
            var conn = new TekkersContext();
            Assessment pa = new Assessment();
            //var assessments = conn.Assessments.Any(a => a.Player.Id == id);
            var assessment = conn.Assessments.Where(a => a.Player.Id == id && a.Deleted == false)
                                              .OrderByDescending(x => x.AssessmentDate)
                                              .Take(1)
                                              .ToList();
            if (assessment.Count == 1)
            {
                pa = assessment[0];
            }
            else if(assessment.Count == 0)
            {
                pa = null;
            }
            return pa;
        }

        [Route("tables/Assessment/GetTopAssessmentsByAge/{agegroup}")]
        public List<Assessment> GetTopAssessmentsByAge(int agegroup)
        {
            var conn = new TekkersContext();
            var assessments = conn.Assessments.Where(a => a.Player.AgeGroup == agegroup && a.Deleted==false)
                                              .OrderByDescending(x => x.AssessmentScore)
                                              .Take(5)
                                              .ToList();
            return assessments;
        }

        [Route("tables/Assessment/GetAllAssessmentsForPlayer/{playerId}")]
        public List<Assessment> GetAllAssessmentsForPlayer(string playerId)
        {
            var conn = new TekkersContext();
            List<Assessment> theAssessments = new List<Assessment>();
            var assessments = conn.Assessments.Where(a => a.Player.Id == playerId && a.Deleted == false)
                                              .OrderBy(x => x.AssessmentDate)
                                              .ToList();
            if (assessments.Count > 0)
            {
                theAssessments = assessments;
            }
            else if(assessments.Count == 0)
            {
                theAssessments = null;
            }
            return theAssessments;
        }
    }
}

