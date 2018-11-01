namespace GlobalTVBilling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Invoice : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "book.Invoices",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        num = c.String(),
                        tdate = c.DateTime(nullable: false),
                        file_name = c.String(),
                        abonent_nums = c.String(),
                        customer_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Customers", t => t.customer_id)
                .Index(t => t.customer_id, name: "IX_CustomerId_Customers");
            
            AddColumn("book.Cards", "auto_invoice", c => c.Boolean(nullable: false));
            AddColumn("book.Customers", "email", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("book.Invoices", "customer_id", "book.Customers");
            DropIndex("book.Invoices", "IX_CustomerId_Customers");
            DropColumn("book.Customers", "email");
            DropColumn("book.Cards", "auto_invoice");
            DropTable("book.Invoices");
        }
    }
}
