namespace ErpMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AgregandoObservacionesMovProdYSeleccionCV : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SeleccionCompras",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CompraId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Compras", t => t.CompraId, cascadeDelete: true)
                .Index(t => t.CompraId);
            
            CreateTable(
                "dbo.SeleccionVentas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VentaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Ventas", t => t.VentaId, cascadeDelete: true)
                .Index(t => t.VentaId);
            
            AddColumn("dbo.MovimientoDeProductoes", "Observaciones", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SeleccionVentas", "VentaId", "dbo.Ventas");
            DropForeignKey("dbo.SeleccionCompras", "CompraId", "dbo.Compras");
            DropIndex("dbo.SeleccionVentas", new[] { "VentaId" });
            DropIndex("dbo.SeleccionCompras", new[] { "CompraId" });
            DropColumn("dbo.MovimientoDeProductoes", "Observaciones");
            DropTable("dbo.SeleccionVentas");
            DropTable("dbo.SeleccionCompras");
        }
    }
}
