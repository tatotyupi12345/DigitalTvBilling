namespace GlobalTVBilling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Payments : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "doc.CardLogs", name: "IX_card_id", newName: "IX_CardId_CardLogs");
            CreateTable(
                "doc.PayTransactionCards",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        transaction_id = c.Int(nullable: false),
                        card_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Cards", t => t.card_id)
                .ForeignKey("doc.PayTransactions", t => t.transaction_id)
                .Index(t => t.transaction_id, name: "IX_PayTransactionId_TransactionCards")
                .Index(t => t.card_id, name: "IX_CardId_TransactionCards");
            
            CreateTable(
                "doc.PayTransactions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        transaction_id = c.Int(nullable: false),
                        amount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        tdate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .Index(t => t.transaction_id, unique: true, name: "IX_TransactionId_PayTransactions");
            
            AddColumn("doc.Payments", "pay_transaction", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropForeignKey("doc.PayTransactionCards", "transaction_id", "doc.PayTransactions");
            DropForeignKey("doc.PayTransactionCards", "card_id", "book.Cards");
            DropIndex("doc.PayTransactions", "IX_TransactionId_PayTransactions");
            DropIndex("doc.PayTransactionCards", "IX_CardId_TransactionCards");
            DropIndex("doc.PayTransactionCards", "IX_PayTransactionId_TransactionCards");
            DropColumn("doc.Payments", "pay_transaction");
            DropTable("doc.PayTransactions");
            DropTable("doc.PayTransactionCards");
            RenameIndex(table: "doc.CardLogs", name: "IX_CardId_CardLogs", newName: "IX_card_id");
        }
    }
}
