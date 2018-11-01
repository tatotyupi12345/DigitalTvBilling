namespace GlobalTVBilling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _3_18122014 : DbMigration
    {
        public override void Up()
        {
            AddColumn("book.Cards", "mode", c => c.Int(nullable: false, defaultValueSql: "0"));
        }
        
        public override void Down()
        {
            DropColumn("book.Cards", "mode");
        }
    }
}
