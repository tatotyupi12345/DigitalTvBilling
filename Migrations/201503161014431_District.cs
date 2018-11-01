namespace GlobalTVBilling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class District : DbMigration
    {
        public override void Up()
        {
            AddColumn("book.Customers", "district", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("book.Customers", "district");
        }
    }
}
