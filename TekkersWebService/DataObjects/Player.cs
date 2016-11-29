using Microsoft.Azure.Mobile.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TekkersService.DataObjects;

namespace TekkersWebService.DataObjects
{
    public class Player : EntityData
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public string ParentFName { get; set; }

        public string ParentLName { get; set; }

        public string Email { get; set; }

        public string PhoneNum { get; set; }

        public int AgeGroup { get; set; }

        [JsonIgnore]
        public virtual ICollection<Assessment> PlayerAssessments { get; set; }

        [JsonIgnore]
        public virtual ICollection<Test> PlayerTests { get; set; }


        public virtual Team PlayersTeam { get; set; }

    }
}