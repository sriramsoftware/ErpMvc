namespace ErpMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregandoOrdenAClasificacion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Clasificacions", "Orden", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Clasificacions", "Orden");
        }
    }
}
