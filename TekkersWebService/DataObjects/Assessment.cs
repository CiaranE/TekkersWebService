using Microsoft.Azure.Mobile.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TekkersService.DataObjects;

namespace TekkersWebService.DataObjects
{
    public class Assessment : EntityData
    {
        public string AssessmentName { get; set; }

        public DateTime? AssessmentDate { get; set; }

        public int AssessmentScore { get; set; }

        public virtual ICollection<Test> Tests { get; set; }

        public virtual Player Player { get; set; }

    }
}