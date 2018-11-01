namespace GlobalTVBilling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _9 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "doc.AutoSubscribChangeCards",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        card_id = c.Int(nullable: false),
                        cas_ids = c.String(),
                        package_ids = c.String(),
                        package_names = c.String(),
                        amount = c.Double(nullable: false),
                        user_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Cards", t => t.card_id)
                .ForeignKey("book.Users", t => t.user_id)
                .Index(t => t.card_id, name: "IX_CardId_CardDamages")
                .Index(t => t.user_id, name: "IX_UserId_CardDamages");
            
            CreateTable(
                "doc.CanceledPayments",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        tdate = c.DateTime(nullable: false),
                        card_id = c.Int(nullable: false),
                        amount = c.Decimal(nullable: false, precision: 18, scale: 4),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Cards", t => t.card_id)
                .Index(t => t.card_id, name: "IX_CardId_Payments");
            
            CreateTable(
                "book.OfficeCards",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        tdate = c.DateTime(nullable: false),
                        card_num = c.String(nullable: false, maxLength: 255, unicode: false),
                        address = c.String(nullable: false, maxLength: 255),
                        name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.id);
            
            AddColumn("book.Users", "hard_autorize", c => c.Boolean(nullable: false));
            AddColumn("book.AutoMessageTemplates", "name", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("doc.CanceledPayments", "card_id", "book.Cards");
            DropForeignKey("doc.AutoSubscribChangeCards", "user_id", "book.Users");
            DropForeignKey("doc.AutoSubscribChangeCards", "card_id", "book.Cards");
            DropIndex("doc.CanceledPayments", "IX_CardId_Payments");
            DropIndex("doc.AutoSubscribChangeCards", "IX_UserId_CardDamages");
            DropIndex("doc.AutoSubscribChangeCards", "IX_CardId_CardDamages");
            DropColumn("book.AutoMessageTemplates", "name");
            DropColumn("book.Users", "hard_autorize");
            DropTable("book.OfficeCards");
            DropTable("doc.CanceledPayments");
            DropTable("doc.AutoSubscribChangeCards");
        }
    }
}
