namespace GlobalTVBilling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("book.MessageAbonents", "card_id", c => c.Int(nullable: false));
            AddColumn("book.Service", "is_edit", c => c.Boolean(nullable: false));
            AddColumn("book.AutoMessageTemplates", "is_disposable", c => c.Boolean(nullable: false));
            AddColumn("book.MessageNotSendLogs", "card_id", c => c.Int(nullable: false));
            AddColumn("doc.Orders", "num", c => c.Int(nullable: false));
            AddColumn("doc.Orders", "receivers_count", c => c.Int(nullable: false));
            AddColumn("doc.Orders", "get_date", c => c.DateTime(nullable: false));
            AddColumn("doc.Orders", "status", c => c.Int(nullable: false));
            AddColumn("doc.Orders", "user_id", c => c.Int(nullable: false));
            AddColumn("config.Params", "help", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("config.Params", "help");
            DropColumn("doc.Orders", "user_id");
            DropColumn("doc.Orders", "status");
            DropColumn("doc.Orders", "get_date");
            DropColumn("doc.Orders", "receivers_count");
            DropColumn("doc.Orders", "num");
            DropColumn("book.MessageNotSendLogs", "card_id");
            DropColumn("book.AutoMessageTemplates", "is_disposable");
            DropColumn("book.Service", "is_edit");
            DropColumn("book.MessageAbonents", "card_id");
        }
    }
}
