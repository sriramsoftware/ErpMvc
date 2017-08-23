namespace ErpMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CambiosDeComanda : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.rst_agregados_de_comandas",
                c => new
                    {
                        DetalleDeComandaId = c.Int(nullable: false),
                        AgregadoId = c.Int(nullable: false),
                        Cantidad = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.DetalleDeComandaId, t.AgregadoId })
                .ForeignKey("dbo.cv_agregados", t => t.AgregadoId, cascadeDelete: true)
                .ForeignKey("dbo.rst_detalles_de_comandas", t => t.DetalleDeComandaId, cascadeDelete: true)
                .Index(t => t.DetalleDeComandaId)
                .Index(t => t.AgregadoId);
            
            CreateTable(
                "dbo.rst_detalles_de_comandas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ComandaId = c.Int(nullable: false),
                        ElaboracionId = c.Int(nullable: false),
                        Cantidad = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.rst_comandas", t => t.ComandaId, cascadeDelete: true)
                .ForeignKey("dbo.Elaboracions", t => t.ElaboracionId, cascadeDelete: true)
                .Index(t => t.ComandaId)
                .Index(t => t.ElaboracionId);
            
            CreateTable(
                "dbo.rst_comandas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        VentaId = c.Int(),
                        DiaContableId = c.Int(nullable: false),
                        Fecha = c.DateTime(nullable: false, precision: 0),
                        VendedorId = c.Int(nullable: false),
                        PuntoDeVentaId = c.Int(nullable: false),
                        CantidadPersonas = c.Int(nullable: false),
                        EstadoDeVenta = c.Int(nullable: false),
                        UsuarioId = c.String(maxLength: 128, storeType: "nvarchar"),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.contb_dia_contable", t => t.DiaContableId, cascadeDelete: true)
                .ForeignKey("dbo.PuntoDeVentas", t => t.PuntoDeVentaId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetUsers", t => t.UsuarioId)
                .ForeignKey("dbo.ident_personas", t => t.VendedorId, cascadeDelete: true)
                .ForeignKey("dbo.Ventas", t => t.VentaId)
                .Index(t => t.VentaId)
                .Index(t => t.DiaContableId)
                .Index(t => t.VendedorId)
                .Index(t => t.PuntoDeVentaId)
                .Index(t => t.UsuarioId);
            
            CreateTable(
                "dbo.rst_ordenes_comanda",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Numero = c.Int(nullable: false),
                        Comensal = c.Int(nullable: false),
                        ComandaId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.rst_comandas", t => t.ComandaId)
                .Index(t => t.ComandaId);
            
            CreateTable(
                "dbo.rst_orden_por_detalle",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DetalleDeComandaId = c.Int(nullable: false),
                        OrdenId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.rst_detalles_de_comandas", t => t.DetalleDeComandaId, cascadeDelete: true)
                .ForeignKey("dbo.rst_ordenes_comanda", t => t.OrdenId, cascadeDelete: true)
                .Index(t => t.DetalleDeComandaId)
                .Index(t => t.OrdenId);
            
            CreateTable(
                "dbo.rst_anotaciones",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Abreviatura = c.String(unicode: false),
                        Descripcion = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.rst_anotaciones_orden_detalles",
                c => new
                    {
                        OrdenPorDetalle_Id = c.Int(nullable: false),
                        Anotacion_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.OrdenPorDetalle_Id, t.Anotacion_Id })
                .ForeignKey("dbo.rst_orden_por_detalle", t => t.OrdenPorDetalle_Id, cascadeDelete: true)
                .ForeignKey("dbo.rst_anotaciones", t => t.Anotacion_Id, cascadeDelete: true)
                .Index(t => t.OrdenPorDetalle_Id)
                .Index(t => t.Anotacion_Id);
            
            //AddColumn("dbo.Ventas", "CantidadPersonas", c => c.Int(nullable: false));
            //AddColumn("dbo.ProductoConcretoes", "ProporcionDeMerma", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            //AddColumn("dbo.Compras", "TieneComprobante", c => c.Boolean(nullable: false));
            //AddColumn("dbo.MovimientoDeProductoes", "Costo", c => c.Decimal(nullable: false, precision: 15, scale: 12));
            //AddColumn("dbo.SalidaPorMermas", "Costo", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            //AlterColumn("dbo.ExistenciaAlmacens", "ExistenciaEnAlmacen", c => c.Decimal(nullable: false, precision: 15, scale: 5));
            //AlterColumn("dbo.ExistenciaCentroDeCostoes", "Cantidad", c => c.Decimal(nullable: false, precision: 15, scale: 5));
            //AlterColumn("dbo.SalidaPorMermas", "Cantidad", c => c.Decimal(nullable: false, precision: 15, scale: 5));
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.rst_detalles_de_comandas", "ElaboracionId", "dbo.Elaboracions");
            DropForeignKey("dbo.rst_comandas", "VentaId", "dbo.Ventas");
            DropForeignKey("dbo.rst_comandas", "VendedorId", "dbo.ident_personas");
            DropForeignKey("dbo.rst_comandas", "UsuarioId", "dbo.AspNetUsers");
            DropForeignKey("dbo.rst_comandas", "PuntoDeVentaId", "dbo.PuntoDeVentas");
            DropForeignKey("dbo.rst_comandas", "DiaContableId", "dbo.contb_dia_contable");
            DropForeignKey("dbo.rst_detalles_de_comandas", "ComandaId", "dbo.rst_comandas");
            DropForeignKey("dbo.rst_ordenes_comanda", "ComandaId", "dbo.rst_comandas");
            DropForeignKey("dbo.rst_orden_por_detalle", "OrdenId", "dbo.rst_ordenes_comanda");
            DropForeignKey("dbo.rst_orden_por_detalle", "DetalleDeComandaId", "dbo.rst_detalles_de_comandas");
            DropForeignKey("dbo.rst_anotaciones_orden_detalles", "Anotacion_Id", "dbo.rst_anotaciones");
            DropForeignKey("dbo.rst_anotaciones_orden_detalles", "OrdenPorDetalle_Id", "dbo.rst_orden_por_detalle");
            DropForeignKey("dbo.rst_agregados_de_comandas", "DetalleDeComandaId", "dbo.rst_detalles_de_comandas");
            DropForeignKey("dbo.rst_agregados_de_comandas", "AgregadoId", "dbo.cv_agregados");
            DropIndex("dbo.rst_anotaciones_orden_detalles", new[] { "Anotacion_Id" });
            DropIndex("dbo.rst_anotaciones_orden_detalles", new[] { "OrdenPorDetalle_Id" });
            DropIndex("dbo.rst_orden_por_detalle", new[] { "OrdenId" });
            DropIndex("dbo.rst_orden_por_detalle", new[] { "DetalleDeComandaId" });
            DropIndex("dbo.rst_ordenes_comanda", new[] { "ComandaId" });
            DropIndex("dbo.rst_comandas", new[] { "UsuarioId" });
            DropIndex("dbo.rst_comandas", new[] { "PuntoDeVentaId" });
            DropIndex("dbo.rst_comandas", new[] { "VendedorId" });
            DropIndex("dbo.rst_comandas", new[] { "DiaContableId" });
            DropIndex("dbo.rst_comandas", new[] { "VentaId" });
            DropIndex("dbo.rst_detalles_de_comandas", new[] { "ElaboracionId" });
            DropIndex("dbo.rst_detalles_de_comandas", new[] { "ComandaId" });
            DropIndex("dbo.rst_agregados_de_comandas", new[] { "AgregadoId" });
            DropIndex("dbo.rst_agregados_de_comandas", new[] { "DetalleDeComandaId" });
            //AlterColumn("dbo.SalidaPorMermas", "Cantidad", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            //AlterColumn("dbo.ExistenciaCentroDeCostoes", "Cantidad", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            //AlterColumn("dbo.ExistenciaAlmacens", "ExistenciaEnAlmacen", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            //DropColumn("dbo.SalidaPorMermas", "Costo");
            //DropColumn("dbo.MovimientoDeProductoes", "Costo");
            //DropColumn("dbo.Compras", "TieneComprobante");
            //DropColumn("dbo.ProductoConcretoes", "ProporcionDeMerma");
            //DropColumn("dbo.Ventas", "CantidadPersonas");
            DropTable("dbo.rst_anotaciones_orden_detalles");
            DropTable("dbo.rst_anotaciones");
            DropTable("dbo.rst_orden_por_detalle");
            DropTable("dbo.rst_ordenes_comanda");
            DropTable("dbo.rst_comandas");
            DropTable("dbo.rst_detalles_de_comandas");
            DropTable("dbo.rst_agregados_de_comandas");
        }
    }
}
