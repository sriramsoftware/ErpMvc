namespace ErpMvc.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Agregando_Propiedad_EsInventariable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.com_productos", "EsInventariable", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.com_productos", "EsInventariable");
        }
    }
}
