namespace GlobalTVBilling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DamageGetDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("doc.CardDamages", "get_date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("doc.CardDamages", "get_date");
        }
    }
}
