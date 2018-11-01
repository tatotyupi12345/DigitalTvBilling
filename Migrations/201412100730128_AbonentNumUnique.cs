namespace GlobalTVBilling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AbonentNumUnique : DbMigration
    {
        public override void Up()
        {
            CreateIndex("book.Cards", "abonent_num", unique: true, name: "IX_CardAbonentNum_Cards");
        }
        
        public override void Down()
        {
            DropIndex("book.Cards", "IX_CardAbonentNum_Cards");
        }
    }
}
