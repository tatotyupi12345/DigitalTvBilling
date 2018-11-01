namespace GlobalTVBilling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2_08122014 : DbMigration
    {
        public override void Up()
        {
            AddColumn("book.Cards", "doc_num", c => c.String(nullable: false));
            AddColumn("book.Cards", "discount", c => c.Double(nullable: false));
            AddColumn("book.Cards", "group", c => c.Int(nullable: false));
            AddColumn("book.Customers", "is_budget", c => c.Boolean(nullable: false));
            DropColumn("book.Customers", "united_pay");
            DropColumn("book.Customers", "discount");
            DropColumn("book.Customers", "doc_num");
        }
        
        public override void Down()
        {
            AddColumn("book.Customers", "doc_num", c => c.String(nullable: false));
            AddColumn("book.Customers", "discount", c => c.Double(nullable: false));
            AddColumn("book.Customers", "united_pay", c => c.Boolean(nullable: false));
            DropColumn("book.Customers", "is_budget");
            DropColumn("book.Cards", "group");
            DropColumn("book.Cards", "discount");
            DropColumn("book.Cards", "doc_num");
        }
    }
}
