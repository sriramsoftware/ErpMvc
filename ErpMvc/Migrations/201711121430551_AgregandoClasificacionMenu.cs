namespace ErpMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregandoClasificacionMenu : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Clasificacions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nombre = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Elaboracions", "ClasificacionId", c => c.Int());
            CreateIndex("dbo.Elaboracions", "ClasificacionId");
            AddForeignKey("dbo.Elaboracions", "ClasificacionId", "dbo.Clasificacions", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Elaboracions", "ClasificacionId", "dbo.Clasificacions");
            DropIndex("dbo.Elaboracions", new[] { "ClasificacionId" });
            DropColumn("dbo.Elaboracions", "ClasificacionId");
            DropTable("dbo.Clasificacions");
        }
    }
}
