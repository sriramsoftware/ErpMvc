namespace ErpMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Cambios_En_Elaboracion : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DetalleSalidaAlmacens", "ValeId", "dbo.ValeSalidaDeAlmacens");
            AddColumn("dbo.Elaboracions", "CostoPlanificado", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Elaboracions", "IndiceEsperado", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Elaboracions", "PrecioDeVenta", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.ValeSalidaDeAlmacens", "AlmacenId", c => c.Int(nullable: false));
            CreateIndex("dbo.ValeSalidaDeAlmacens", "AlmacenId");
            AddForeignKey("dbo.ValeSalidaDeAlmacens", "AlmacenId", "dbo.alm_almacenes", "Id");
            AddForeignKey("dbo.DetalleSalidaAlmacens", "ValeId", "dbo.ValeSalidaDeAlmacens", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DetalleSalidaAlmacens", "ValeId", "dbo.ValeSalidaDeAlmacens");
            DropForeignKey("dbo.ValeSalidaDeAlmacens", "AlmacenId", "dbo.alm_almacenes");
            DropIndex("dbo.ValeSalidaDeAlmacens", new[] { "AlmacenId" });
            DropColumn("dbo.ValeSalidaDeAlmacens", "AlmacenId");
            DropColumn("dbo.Elaboracions", "PrecioDeVenta");
            DropColumn("dbo.Elaboracions", "IndiceEsperado");
            DropColumn("dbo.Elaboracions", "CostoPlanificado");
            AddForeignKey("dbo.DetalleSalidaAlmacens", "ValeId", "dbo.ValeSalidaDeAlmacens", "Id", cascadeDelete: true);
        }
    }
}
