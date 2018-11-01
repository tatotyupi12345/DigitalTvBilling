namespace GlobalTVBilling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _8 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("doc.Orders", "user_id", "book.Users");
            CreateTable(
                "doc.DamageReasons",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        damage_id = c.Int(nullable: false),
                        reason_id = c.Int(nullable: false),
                        text = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("doc.CardDamages", t => t.damage_id)
                .ForeignKey("book.Reasons", t => t.reason_id)
                .Index(t => t.damage_id, name: "IX_DamageId_DamageReasons")
                .Index(t => t.reason_id, name: "IX_ReasonId_OrderDamageReasons");
            
            CreateTable(
                "book.Reasons",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        text = c.String(nullable: false),
                        type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "doc.OrderReasons",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        order_id = c.Int(nullable: false),
                        reason_id = c.Int(nullable: false),
                        text = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("doc.Orders", t => t.order_id)
                .ForeignKey("book.Reasons", t => t.reason_id)
                .Index(t => t.order_id, name: "IX_OrderId_OrderReasons")
                .Index(t => t.reason_id, name: "IX_ReasonId_OrderDamageReasons");
            
            AddColumn("doc.CardDamages", "status", c => c.Int(nullable: false));
            AddColumn("doc.CardDamages", "cgd", c => c.String());
            AddColumn("doc.CardDamages", "changer_user", c => c.String());
            AddColumn("doc.CardDamages", "approve_user", c => c.String());
            AddColumn("doc.CardDamages", "change_date", c => c.DateTime(nullable: false));
            AddColumn("doc.CardDamages", "user_id", c => c.Int(nullable: false));
            AddColumn("doc.CardDamages", "user_group_id", c => c.Int(nullable: false));
            AddColumn("doc.Orders", "tdate", c => c.DateTime(nullable: false));
            AddColumn("doc.Orders", "address", c => c.String());
            AddColumn("doc.Orders", "card_address", c => c.String());
            AddColumn("doc.Orders", "approve_user", c => c.String());
            AddColumn("doc.Orders", "is_approved", c => c.Boolean(nullable: false));
            AddColumn("doc.Orders", "change_date", c => c.DateTime(nullable: false));
            AddColumn("doc.Orders", "montage_status", c => c.Boolean(nullable: false));
            AddColumn("doc.Orders", "changer_user", c => c.String());
            AddColumn("doc.Orders", "user_group_id", c => c.Int(nullable: false));
            CreateIndex("doc.CardDamages", "user_id", name: "IX_UserId_CardDamages");
            CreateIndex("doc.CardDamages", "user_group_id", name: "IX_UserGroupId_CardDamages");
            CreateIndex("doc.Orders", "user_group_id", name: "IX_UserGroupId_CardDamages");
            AddForeignKey("doc.CardDamages", "user_id", "book.Users", "id");
            AddForeignKey("doc.CardDamages", "user_group_id", "book.Users", "id");
            AddForeignKey("doc.Orders", "user_group_id", "book.Users", "id");
            DropColumn("doc.CardDamages", "is_active");

            Sql("INSERT INTO book.Reasons (text, type) VALUES(N'სხვა',-1)");
        }
        
        public override void Down()
        {
            AddColumn("doc.CardDamages", "is_active", c => c.Boolean(nullable: false));
            DropForeignKey("doc.Orders", "user_group_id", "book.Users");
            DropForeignKey("doc.CardDamages", "user_group_id", "book.Users");
            DropForeignKey("doc.CardDamages", "user_id", "book.Users");
            DropForeignKey("doc.OrderReasons", "reason_id", "book.Reasons");
            DropForeignKey("doc.OrderReasons", "order_id", "doc.Orders");
            DropForeignKey("doc.DamageReasons", "reason_id", "book.Reasons");
            DropForeignKey("doc.DamageReasons", "damage_id", "doc.CardDamages");
            DropIndex("doc.OrderReasons", "IX_ReasonId_OrderDamageReasons");
            DropIndex("doc.OrderReasons", "IX_OrderId_OrderReasons");
            DropIndex("doc.Orders", "IX_UserGroupId_CardDamages");
            DropIndex("doc.DamageReasons", "IX_ReasonId_OrderDamageReasons");
            DropIndex("doc.DamageReasons", "IX_DamageId_DamageReasons");
            DropIndex("doc.CardDamages", "IX_UserGroupId_CardDamages");
            DropIndex("doc.CardDamages", "IX_UserId_CardDamages");
            DropColumn("doc.Orders", "user_group_id");
            DropColumn("doc.Orders", "changer_user");
            DropColumn("doc.Orders", "montage_status");
            DropColumn("doc.Orders", "change_date");
            DropColumn("doc.Orders", "is_approved");
            DropColumn("doc.Orders", "approve_user");
            DropColumn("doc.Orders", "card_address");
            DropColumn("doc.Orders", "address");
            DropColumn("doc.Orders", "tdate");
            DropColumn("doc.CardDamages", "user_group_id");
            DropColumn("doc.CardDamages", "user_id");
            DropColumn("doc.CardDamages", "change_date");
            DropColumn("doc.CardDamages", "approve_user");
            DropColumn("doc.CardDamages", "changer_user");
            DropColumn("doc.CardDamages", "cgd");
            DropColumn("doc.CardDamages", "status");
            DropTable("doc.OrderReasons");
            DropTable("book.Reasons");
            DropTable("doc.DamageReasons");
            AddForeignKey("doc.Orders", "user_id", "book.Users", "id");
        }
    }
}
