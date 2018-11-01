namespace GlobalTVBilling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _6 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "doc.CardDamages",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        tdate = c.DateTime(nullable: false),
                        card_id = c.Int(nullable: false),
                        desc = c.String(),
                        is_active = c.Boolean(nullable: false),
                        is_approved = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Cards", t => t.card_id)
                .Index(t => t.card_id, name: "IX_CardId_CardDamages");
            
            CreateTable(
                "book.AutoMessageTemplates",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        message = c.String(),
                        type = c.Int(nullable: false),
                        query = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            AddColumn("book.Users", "phone", c => c.String(nullable: false, maxLength: 9));
            AddColumn("book.Users", "email", c => c.String(nullable: false, maxLength: 100));
            AddColumn("doc.CardServices", "amount", c => c.Decimal(nullable: false, precision: 18, scale: 4));
        }
        
        public override void Down()
        {
            DropForeignKey("doc.CardDamages", "card_id", "book.Cards");
            DropIndex("doc.CardDamages", "IX_CardId_CardDamages");
            DropColumn("doc.CardServices", "amount");
            DropColumn("book.Users", "email");
            DropColumn("book.Users", "phone");
            DropTable("book.AutoMessageTemplates");
            DropTable("doc.CardDamages");
        }
    }
}
