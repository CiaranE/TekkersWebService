using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Tables;
using TekkersService.DataObjects;
using System.Collections.Generic;
using TekkersWebService.DataObjects;

namespace TekkersWebService.Models
{
    public class TekkersContext : DbContext
    {
        // You can add custom code to this file. Changes will not be overwritten.
        // 
        // If you want Entity Framework to alter your database
        // automatically whenever you change your model schema, please use data migrations.
        // For more information refer to the documentation:
        // http://msdn.microsoft.com/en-us/data/jj591621.aspx

        private const string connectionStringName = "Name=MS_TableConnectionString";

        public TekkersContext() : base(connectionStringName)
        {
            //Database.SetInitializer<TekkersContext>(new CreateDatabaseIfNotExists<TekkersContext>());

            // Database.SetInitializer<TekkersContext>(new DropCreateDatabaseIfModelChanges<TekkersContext>());
            //Database.SetInitializer<TekkersContext>(new DropCreateDatabaseAlways<TekkersContext>());
            //Database.SetInitializer<TekkersContext>(new SchoolDBInitializer());

        }

        public DbSet<Player> Players { get; set; }

        public DbSet<Assessment> Assessments { get; set; }

        public DbSet<Test> Tests { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add(
                new AttributeToColumnAnnotationConvention<TableColumnAttribute, string>(
                    "ServiceTableColumn", (property, attributes) => attributes.Single().ColumnType.ToString()));
        }

        public System.Data.Entity.DbSet<TekkersService.DataObjects.Team> Teams { get; set; }
    }
}
