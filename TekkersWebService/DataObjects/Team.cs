using Microsoft.Azure.Mobile.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TekkersWebService.DataObjects;

namespace TekkersService.DataObjects
{
    public class Team : EntityData
    {
        public string TeamName { get; set; }

        public int TeamAgeGroup { get; set; }

        [JsonIgnore]
        public virtual ICollection<Player> TeamPlayers { get; set; }

        public virtual ICollection<Assessment> TeamAssessments { get; set; }
    }
}