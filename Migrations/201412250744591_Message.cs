namespace GlobalTVBilling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Message : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "book.MessageNotSendLogs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        message_type = c.Int(nullable: false),
                        abonent_id = c.Int(nullable: false),
                        message_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Customers", t => t.abonent_id)
                .ForeignKey("book.Messages", t => t.message_id)
                .Index(t => t.abonent_id, name: "IX_AbonentId_MessageAbonents")
                .Index(t => t.message_id, name: "IX_MessageId_MessageNotSendLogs");
            
            AddColumn("book.Messages", "is_active", c => c.Boolean(nullable: false));
            AlterColumn("book.Messages", "message_type", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("book.MessageNotSendLogs", "message_id", "book.Messages");
            DropForeignKey("book.MessageNotSendLogs", "abonent_id", "book.Customers");
            DropIndex("book.MessageNotSendLogs", "IX_MessageId_MessageNotSendLogs");
            DropIndex("book.MessageNotSendLogs", "IX_AbonentId_MessageAbonents");
            AlterColumn("book.Messages", "message_type", c => c.Int(nullable: false));
            DropColumn("book.Messages", "is_active");
            DropTable("book.MessageNotSendLogs");
        }
    }
}
