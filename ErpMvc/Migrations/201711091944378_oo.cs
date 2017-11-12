namespace ErpMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class oo : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SalidaPorMermas", "Cantidad", c => c.Decimal(nullable: false, precision: 18, scale: 5));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SalidaPorMermas", "Cantidad", c => c.Decimal(nullable: false, precision: 15, scale: 5));
        }
    }
}
