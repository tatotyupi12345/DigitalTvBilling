namespace GlobalTVBilling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class preFinish : DbMigration
    {
        public override void Up()
        {
            AddColumn("book.Cards", "city", c => c.String(nullable: false));
            AddColumn("book.Cards", "village", c => c.String());
            AddColumn("book.Cards", "region", c => c.String());
            AddColumn("doc.CardLogs", "card_status", c => c.Int(nullable: false, defaultValueSql: "0"));
        }
        
        public override void Down()
        {
            DropColumn("doc.CardLogs", "card_status");
            DropColumn("book.Cards", "region");
            DropColumn("book.Cards", "village");
            DropColumn("book.Cards", "city");
        }
    }
}
