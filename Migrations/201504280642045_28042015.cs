namespace GlobalTVBilling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _28042015 : DbMigration
    {
        public override void Up()
        {
            AddColumn("book.Cards", "closed_is_pen", c => c.Boolean(nullable: false));
            AddColumn("book.UserPermissions", "group", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("book.UserPermissions", "group");
            DropColumn("book.Cards", "closed_is_pen");
        }
    }
}
