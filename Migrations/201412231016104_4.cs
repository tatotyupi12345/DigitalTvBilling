namespace GlobalTVBilling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _4 : DbMigration
    {
        public override void Up()
        {
            DropIndex("doc.PayTransactions", "IX_TransactionId_PayTransactions");
            CreateTable(
                "doc.CustomersChat",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        tdate = c.DateTime(nullable: false),
                        message = c.String(),
                        customer_id = c.Int(nullable: false),
                        user_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Customers", t => t.customer_id)
                .ForeignKey("book.Users", t => t.user_id)
                .Index(t => t.customer_id, name: "IX_CustomerId_CustomersChat")
                .Index(t => t.user_id, name: "IX_UserId_CustomersChat");
            
            CreateTable(
                "doc.Orders",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        code = c.String(maxLength: 11),
                        name = c.String(),
                        data = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            AddColumn("doc.Payments", "file_attach", c => c.String());
            AddColumn("config.ChargeCrushLogs", "card_num", c => c.String());
            AddColumn("config.ChargeCrushLogs", "type", c => c.Int(nullable: false));
            AlterColumn("doc.PayTransactions", "transaction_id", c => c.Long(nullable: false));
            CreateIndex("doc.PayTransactions", "transaction_id", unique: true, name: "IX_TransactionId_PayTransactions");
        }
        
        public override void Down()
        {
            DropForeignKey("doc.CustomersChat", "user_id", "book.Users");
            DropForeignKey("doc.CustomersChat", "customer_id", "book.Customers");
            DropIndex("doc.CustomersChat", "IX_UserId_CustomersChat");
            DropIndex("doc.CustomersChat", "IX_CustomerId_CustomersChat");
            DropIndex("doc.PayTransactions", "IX_TransactionId_PayTransactions");
            AlterColumn("doc.PayTransactions", "transaction_id", c => c.Int(nullable: false));
            DropColumn("config.ChargeCrushLogs", "type");
            DropColumn("config.ChargeCrushLogs", "card_num");
            DropColumn("doc.Payments", "file_attach");
            DropTable("doc.Orders");
            DropTable("doc.CustomersChat");
            CreateIndex("doc.PayTransactions", "transaction_id", unique: true, name: "IX_TransactionId_PayTransactions");
        }
    }
}
