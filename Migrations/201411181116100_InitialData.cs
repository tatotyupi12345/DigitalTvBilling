namespace GlobalTVBilling.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialData : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "book.Attachments",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(maxLength: 100),
                        original_name = c.String(maxLength: 35),
                        customer_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Customers", t => t.customer_id)
                .Index(t => t.customer_id);
            
            CreateTable(
                "book.Customers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        tdate = c.DateTime(nullable: false),
                        name = c.String(nullable: false, maxLength: 100),
                        lastname = c.String(nullable: false, maxLength: 100),
                        code = c.String(nullable: false, maxLength: 11, unicode: false),
                        address = c.String(nullable: false, maxLength: 255),
                        type = c.Int(nullable: false),
                        juridical_type = c.Int(nullable: false),
                        budget_days = c.Int(nullable: false),
                        city = c.String(nullable: false),
                        village = c.String(),
                        region = c.String(nullable: false),
                        phone1 = c.String(nullable: false, maxLength: 50, unicode: false),
                        phone2 = c.String(maxLength: 50, unicode: false),
                        desc = c.String(),
                        united_pay = c.Boolean(nullable: false),
                        discount = c.Double(nullable: false),
                        doc_num = c.String(nullable: false),
                        security_code = c.String(nullable: false, maxLength: 32, unicode: false),
                        user_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Users", t => t.user_id)
                .Index(t => t.user_id);
            
            CreateTable(
                "book.Cards",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        tdate = c.DateTime(nullable: false),
                        abonent_num = c.String(nullable: false, maxLength: 255, unicode: false),
                        card_num = c.String(nullable: false, maxLength: 255, unicode: false),
                        address = c.String(nullable: false, maxLength: 255),
                        customer_id = c.Int(nullable: false),
                        status = c.Int(nullable: false),
                        close_date = c.DateTime(nullable: false),
                        finish_date = c.DateTime(nullable: false),
                        pause_date = c.DateTime(nullable: false),
                        pause_days = c.Int(nullable: false),
                        user_id = c.Int(nullable: false),
                        tower_id = c.Int(nullable: false),
                        receiver_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Customers", t => t.customer_id)
                .ForeignKey("book.Users", t => t.user_id)
                .ForeignKey("book.Receivers", t => t.receiver_id)
                .ForeignKey("book.Towers", t => t.tower_id)
                .Index(t => t.customer_id)
                .Index(t => t.user_id)
                .Index(t => t.tower_id)
                .Index(t => t.receiver_id);
            
            CreateTable(
                "doc.CardCharges",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        card_id = c.Int(nullable: false),
                        tdate = c.DateTime(nullable: false),
                        amount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Cards", t => t.card_id)
                .Index(t => t.card_id);
            
            CreateTable(
                "doc.CardLogs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        card_id = c.Int(nullable: false),
                        close_tdate = c.DateTime(nullable: false),
                        status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Cards", t => t.card_id)
                .Index(t => t.card_id);
            
            CreateTable(
                "doc.Payments",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        tdate = c.DateTime(nullable: false),
                        card_id = c.Int(nullable: false),
                        pay_type_id = c.Int(nullable: false),
                        amount = c.Decimal(nullable: false, precision: 18, scale: 4),
                        user_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Cards", t => t.card_id)
                .ForeignKey("book.PayTypes", t => t.pay_type_id)
                .ForeignKey("book.Users", t => t.user_id)
                .Index(t => t.card_id)
                .Index(t => t.pay_type_id)
                .Index(t => t.user_id);
            
            CreateTable(
                "book.PayTypes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "book.Users",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        login = c.String(nullable: false, maxLength: 50),
                        password = c.String(maxLength: 32, unicode: false),
                        name = c.String(nullable: false, maxLength: 100),
                        type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.UserTypes", t => t.type)
                .Index(t => t.type);
            
            CreateTable(
                "book.Channels",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 255),
                        user_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Users", t => t.user_id)
                .Index(t => t.user_id);
            
            CreateTable(
                "book.PackageChannels",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        package_id = c.Int(nullable: false),
                        channel_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Channels", t => t.channel_id)
                .ForeignKey("book.Packages", t => t.package_id)
                .Index(t => t.package_id)
                .Index(t => t.channel_id);
            
            CreateTable(
                "book.Packages",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false, maxLength: 255),
                        price = c.Double(nullable: false),
                        min_price = c.Double(nullable: false),
                        jurid_price = c.Double(nullable: false),
                        cas_id = c.Int(nullable: false),
                        user_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Users", t => t.user_id)
                .Index(t => t.user_id);
            
            CreateTable(
                "doc.SubscriptionPackages",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        subscription_id = c.Int(nullable: false),
                        package_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Packages", t => t.package_id)
                .ForeignKey("doc.Subscribes", t => t.subscription_id)
                .Index(t => t.subscription_id)
                .Index(t => t.package_id);
            
            CreateTable(
                "doc.Subscribes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        tdate = c.DateTime(nullable: false),
                        card_id = c.Int(nullable: false),
                        amount = c.Double(nullable: false),
                        status = c.Boolean(nullable: false),
                        user_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Cards", t => t.card_id)
                .ForeignKey("book.Users", t => t.user_id)
                .Index(t => t.card_id)
                .Index(t => t.user_id);
            
            CreateTable(
                "config.Logging",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        tdate = c.DateTime(nullable: false),
                        user_id = c.Int(nullable: false),
                        doc = c.String(maxLength: 255),
                        doc_item = c.String(maxLength: 255),
                        changes = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Users", t => t.user_id)
                .Index(t => t.user_id);
            
            CreateTable(
                "book.Messages",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        abonent_id = c.Int(nullable: false),
                        tdate = c.DateTime(nullable: false),
                        message = c.String(nullable: false),
                        is_send = c.Boolean(nullable: false),
                        user_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Customers", t => t.abonent_id)
                .ForeignKey("book.Users", t => t.user_id)
                .Index(t => t.abonent_id)
                .Index(t => t.user_id);
            
            CreateTable(
                "book.UserTypes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "book.UserPermissions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        tag = c.String(),
                        sign = c.Boolean(nullable: false),
                        type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.UserTypes", t => t.type)
                .Index(t => t.type);
            
            CreateTable(
                "book.Receivers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "book.Towers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false),
                        range = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "book.MessageTemplates",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false),
                        desc = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "config.Params",
                c => new
                    {
                        name = c.String(nullable: false, maxLength: 128),
                        value = c.String(nullable: false),
                        desc = c.String(),
                    })
                .PrimaryKey(t => t.name);

            Sql("INSERT INTO book.UserTypes (name) VALUES(N'ადმინისტრატორი')");
            Sql("INSERT INTO book.Users (type,name,login,password) VALUES(1,N'Admin',N'nika','b59c67bf196a4758191e42f76670ceba')");

            Sql("INSERT INTO book.PayTypes (name) VALUES(N'ნაღდი')");
            Sql("INSERT INTO book.PayTypes (name) VALUES(N'TBC Pay')");

            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'FreeDays',N'5',N'უფასო დღეები')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'AbonentEditTime',N'15',N'აბონენტის რედაქტირება(წთ.)')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'CardPauseDays',N'60',N'ბარათის პაუზის დღები')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'CardPauseAmount',N'5',N'ბარათის პაუზის თანხა')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'CardCharge',N'0:2',N'ბარათის დარიცხვა')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'CloseCardAmount',N'5',N'გათიშული ბარათის ჯარიმა')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'CASAddress',N'10.190.180.150:8000',N'CAS მისამართი')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'PackageDiscount',N'0',N'პაკეტების ფასდაკლება')");
            
        }
        
        public override void Down()
        {
            DropForeignKey("book.Cards", "tower_id", "book.Towers");
            DropForeignKey("book.Cards", "receiver_id", "book.Receivers");
            DropForeignKey("book.Users", "type", "book.UserTypes");
            DropForeignKey("book.UserPermissions", "type", "book.UserTypes");
            DropForeignKey("doc.Payments", "user_id", "book.Users");
            DropForeignKey("book.Messages", "user_id", "book.Users");
            DropForeignKey("book.Messages", "abonent_id", "book.Customers");
            DropForeignKey("config.Logging", "user_id", "book.Users");
            DropForeignKey("book.Customers", "user_id", "book.Users");
            DropForeignKey("book.Channels", "user_id", "book.Users");
            DropForeignKey("book.Packages", "user_id", "book.Users");
            DropForeignKey("doc.Subscribes", "user_id", "book.Users");
            DropForeignKey("doc.SubscriptionPackages", "subscription_id", "doc.Subscribes");
            DropForeignKey("doc.Subscribes", "card_id", "book.Cards");
            DropForeignKey("doc.SubscriptionPackages", "package_id", "book.Packages");
            DropForeignKey("book.PackageChannels", "package_id", "book.Packages");
            DropForeignKey("book.PackageChannels", "channel_id", "book.Channels");
            DropForeignKey("book.Cards", "user_id", "book.Users");
            DropForeignKey("doc.Payments", "pay_type_id", "book.PayTypes");
            DropForeignKey("doc.Payments", "card_id", "book.Cards");
            DropForeignKey("book.Cards", "customer_id", "book.Customers");
            DropForeignKey("doc.CardLogs", "card_id", "book.Cards");
            DropForeignKey("doc.CardCharges", "card_id", "book.Cards");
            DropForeignKey("book.Attachments", "customer_id", "book.Customers");
            DropIndex("book.UserPermissions", new[] { "type" });
            DropIndex("book.Messages", new[] { "user_id" });
            DropIndex("book.Messages", new[] { "abonent_id" });
            DropIndex("config.Logging", new[] { "user_id" });
            DropIndex("doc.Subscribes", new[] { "user_id" });
            DropIndex("doc.Subscribes", new[] { "card_id" });
            DropIndex("doc.SubscriptionPackages", new[] { "package_id" });
            DropIndex("doc.SubscriptionPackages", new[] { "subscription_id" });
            DropIndex("book.Packages", new[] { "user_id" });
            DropIndex("book.PackageChannels", new[] { "channel_id" });
            DropIndex("book.PackageChannels", new[] { "package_id" });
            DropIndex("book.Channels", new[] { "user_id" });
            DropIndex("book.Users", new[] { "type" });
            DropIndex("doc.Payments", new[] { "user_id" });
            DropIndex("doc.Payments", new[] { "pay_type_id" });
            DropIndex("doc.Payments", new[] { "card_id" });
            DropIndex("doc.CardLogs", new[] { "card_id" });
            DropIndex("doc.CardCharges", new[] { "card_id" });
            DropIndex("book.Cards", new[] { "receiver_id" });
            DropIndex("book.Cards", new[] { "tower_id" });
            DropIndex("book.Cards", new[] { "user_id" });
            DropIndex("book.Cards", new[] { "customer_id" });
            DropIndex("book.Customers", new[] { "user_id" });
            DropIndex("book.Attachments", new[] { "customer_id" });
            DropTable("config.Params");
            DropTable("book.MessageTemplates");
            DropTable("book.Towers");
            DropTable("book.Receivers");
            DropTable("book.UserPermissions");
            DropTable("book.UserTypes");
            DropTable("book.Messages");
            DropTable("config.Logging");
            DropTable("doc.Subscribes");
            DropTable("doc.SubscriptionPackages");
            DropTable("book.Packages");
            DropTable("book.PackageChannels");
            DropTable("book.Channels");
            DropTable("book.Users");
            DropTable("book.PayTypes");
            DropTable("doc.Payments");
            DropTable("doc.CardLogs");
            DropTable("doc.CardCharges");
            DropTable("book.Cards");
            DropTable("book.Customers");
            DropTable("book.Attachments");
        }
    }
}
