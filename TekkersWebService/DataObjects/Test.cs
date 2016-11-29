using Microsoft.Azure.Mobile.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace TekkersWebService.DataObjects
{
    public class Test : EntityData
    {
        public string TestName { get; set; }

        public string TestDescription { get; set; }

        public DateTime TestDate { get; set; }

        public int TestScore { get; set; }

        [JsonIgnore]
        public virtual Assessment AssessmentTest { get; set; }

        public virtual Player PlayerTest { get; set; }
    }
}