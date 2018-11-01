namespace GlobalTVBilling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _10 : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "doc.AutoSubscribChangeCards", name: "IX_CardId_CardDamages", newName: "IX_CardId_AutoSubscribChangeCards");
            RenameIndex(table: "doc.AutoSubscribChangeCards", name: "IX_UserId_CardDamages", newName: "IX_UserId_AutoSubscribChangeCards");
            AddColumn("book.Customers", "is_facktura", c => c.Boolean(nullable: false));
            AddColumn("doc.AutoSubscribChangeCards", "send_date", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("doc.AutoSubscribChangeCards", "send_date");
            DropColumn("book.Customers", "is_facktura");
            RenameIndex(table: "doc.AutoSubscribChangeCards", name: "IX_UserId_AutoSubscribChangeCards", newName: "IX_UserId_CardDamages");
            RenameIndex(table: "doc.AutoSubscribChangeCards", name: "IX_CardId_AutoSubscribChangeCards", newName: "IX_CardId_CardDamages");
        }
    }
}
