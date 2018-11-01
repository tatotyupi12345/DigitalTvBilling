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
                        payment_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("doc.Payments", t => t.payment_id)
                .Index(t => t.payment_id);
            
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
                .ForeignKey("book.Users", t => t.user_id)
                .ForeignKey("book.Cards", t => t.card_id)
                .ForeignKey("book.PayTypes", t => t.pay_type_id)
                .Index(t => t.card_id, name: "IX_CardId_Payments")
                .Index(t => t.pay_type_id)
                .Index(t => t.user_id, name: "IX_UserId_Payments");
            
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
                        cas_date = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Users", t => t.user_id)
                .ForeignKey("book.Customers", t => t.customer_id)
                .ForeignKey("book.Receivers", t => t.receiver_id)
                .ForeignKey("book.Towers", t => t.tower_id)
                .Index(t => t.customer_id, name: "IX_CustomerId_Cards")
                .Index(t => t.status, name: "IX_CardStatus_Cards")
                .Index(t => t.user_id, name: "IX_UserId_Cards")
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
                .Index(t => t.card_id, name: "IX_CardId_CardCharges");
            
            CreateTable(
                "doc.CardLogs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        card_id = c.Int(nullable: false),
                        close_tdate = c.DateTime(nullable: false),
                        status = c.Int(nullable: false),
                        user_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Cards", t => t.card_id)
                .ForeignKey("book.Users", t => t.user_id)
                .Index(t => t.card_id)
                .Index(t => t.user_id, name: "IX_UserId_CardLogs");
            
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
                .Index(t => t.user_id, name: "IX_UserId_Channels");
            
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
                .Index(t => t.package_id, name: "IX_PackageId_PackageChannels")
                .Index(t => t.channel_id, name: "IX_ChannelId_PackageChannels");
            
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
                        is_default = c.Boolean(nullable: false),
                        user_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Users", t => t.user_id)
                .Index(t => t.user_id, name: "IX_UserId_Packages");
            
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
                .Index(t => t.subscription_id, name: "IX_SubscriptionId_SubscriptionPackages")
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
                .Index(t => t.card_id, name: "IX_CardId_Subscribes")
                .Index(t => t.user_id);
            
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
                        jurid_finish_date = c.DateTime(),
                        city = c.String(nullable: false),
                        village = c.String(),
                        region = c.String(nullable: false),
                        phone1 = c.String(nullable: false, maxLength: 9, unicode: false),
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
                .Index(t => t.user_id, name: "IX_UserId_Customers");
            
            CreateTable(
                "book.Messages",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        tdate = c.DateTime(nullable: false),
                        message = c.String(nullable: false),
                        message_type = c.Int(nullable: false),
                        user_id = c.Int(nullable: false),
                        Customer_Id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Users", t => t.user_id)
                .ForeignKey("book.Customers", t => t.Customer_Id)
                .Index(t => t.user_id, name: "IX_UserId_Messages")
                .Index(t => t.Customer_Id);
            
            CreateTable(
                "book.MessageAbonents",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        abonent_id = c.Int(nullable: false),
                        message_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Customers", t => t.abonent_id)
                .ForeignKey("book.Messages", t => t.message_id)
                .Index(t => t.abonent_id, name: "IX_AbonentId_MessageAbonents")
                .Index(t => t.message_id, name: "IX_MessageId_MessageAbonents");
            
            CreateTable(
                "config.Logging",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        tdate = c.DateTime(nullable: false),
                        user_id = c.Int(nullable: false),
                        type = c.Int(nullable: false),
                        mode = c.Int(nullable: false),
                        type_value = c.String(),
                        type_id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Users", t => t.user_id)
                .Index(t => t.user_id, name: "IX_UserId_Logging")
                .Index(t => t.type_id, name: "IX_TypeId_Loggings");
            
            CreateTable(
                "config.LoggingItems",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        logging_id = c.Int(nullable: false),
                        column_display = c.String(),
                        old_value = c.String(),
                        new_value = c.String(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("config.Logging", t => t.logging_id)
                .Index(t => t.logging_id, name: "IX_LoggingId_loggingItems");
            
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
                "doc.CardServices",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        card_id = c.Int(nullable: false),
                        service_id = c.Int(nullable: false),
                        pay_type = c.Int(nullable: false),
                        is_active = c.Boolean(nullable: false),
                        tdate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("book.Cards", t => t.card_id)
                .ForeignKey("book.Service", t => t.service_id)
                .Index(t => t.card_id, name: "IX_CardId_Cards")
                .Index(t => t.service_id, name: "IX_ServiceId_Cards");
            
            CreateTable(
                "book.Service",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(nullable: false),
                        amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.id);
            
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
                "book.PayTypes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "config.ChargeCrushLogs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        tdate = c.DateTime(nullable: false),
                        text = c.String(),
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
            Sql("INSERT INTO book.Users (type,name,login,password) VALUES(1,N'სისტემა',N'სისტემა','54b53072540eeeb8f8e9343e71f28176')"); // password system
            Sql("INSERT INTO book.Users (type,name,login,password) VALUES(1,N'Admin',N'nika','b59c67bf196a4758191e42f76670ceba')");

            Sql("INSERT INTO book.PayTypes (name) VALUES(N'გადანაწილება')");
            Sql("INSERT INTO book.PayTypes (name) VALUES(N'ნაღდი')");
            Sql("INSERT INTO book.PayTypes (name) VALUES(N'TBC Pay')");
            Sql("INSERT INTO book.PayTypes (name) VALUES(N'გატანა')");
            Sql("INSERT INTO book.PayTypes (name) VALUES(N'ძვ. ბილინგში გადატანა')");
            Sql("INSERT INTO book.PayTypes (name) VALUES(N'სხვა ბარათზე გადატანა')");
            Sql("INSERT INTO book.PayTypes (name) VALUES(N'გაურკვეველი თანხის გადატანა')");

            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'FreeDays',N'5',N'უფასო დღეები')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'AbonentEditTime',N'15',N'აბონენტის რედაქტირება(წთ.)')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'CardPauseDays',N'60',N'ბარათის პაუზის დღები')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'CardPauseAmount',N'5',N'ბარათის პაუზის თანხა')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'CardCharge',N'0:2',N'ბარათის დარიცხვა')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'CloseCardAmount',N'5',N'გათიშული ბარათის ჯარიმა')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'CloseCardDailyAmount',N'0',N'გათიშული ბარათის დღიური ჯარიმა')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'CloseCardDailyAmountLimit',N'10',N'დღიური ჯარიმის ვადა')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'CASAddress',N'10.190.180.150:8000',N'CAS მისამართი')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'PackageDiscount',N'0',N'პაკეტების ფასდაკლება')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'OSDDuration',N'10',N'OSD-ს ხანგრძლივობა(წ)')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'ClosedCardDays',N'3',N'გათიშული ბარათის დღეები')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'ServiceDays',N'3',N'მომსახურების დღეები')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'JuridLimitMonths',N'3',N'იურიდიულის თვეები')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'SMSUsername',N'',N'SMS ლოგინი')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'SMSPassword',N'',N'SMS პროლი')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'CreditDays',N'5',N'კრედიტის დღეები')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'FTPLogin',N'5',N'FTP ლოგინი')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'FTPPassword',N'5',N'FTP პაროლი')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'FTPHost',N'5',N'FTP მისამართი')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'MessageTime',N'21:30',N'მესიჯის დრო')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'BlockCardDays',N'5',N'ბარათის დაბლოკვის დღეები')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'CreditValidDays',N'10',N'კრედიტის გაცემის დღეები')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'Emails',N'',N'ელ-ფოსტები')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'PacketChangeTime',N'30',N'პაკეტის შეცვლის დრო')");

            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'SystemEmail',N'billing@globaltv.ge',N'სისტემის ელ-ფოსტა')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'SystemEmailPassword',N'@qwerty77',N'სისტემის ელ-ფოსტის პაროლი')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'InvoiceText',N'text',N'ინვოისის აღწერა')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'BudgetAbonentInvoiceDay',N'28',N'საბიუჯეტო ინვოისის დღე')");
            Sql("INSERT INTO config.Params (name,value,[desc]) VALUES(N'InvoiceSendIntervals',N'10;5;3',N'ინვოისის დღეების ინერვალი')");

            Sql(@"CREATE FUNCTION [dbo].[GetCardStatus](@status int)
RETURNS NVARCHAR(20) 
AS 
-- Returns the stock level for the product.
BEGIN
    DECLARE @ret NVARCHAR(20);
    SET @ret = CASE @status WHEN 0 THEN N'აქტიური' 
                            WHEN 5 THEN N'დაბლოკილი'
                            WHEN 4 THEN N'გაუქმებული' 
                            WHEN 1 THEN N'გათიშული'
                            WHEN 3 THEN N'მონტაჟი'
                            WHEN 2 THEN N'დაპაუზებული'
                            ELSE '' END
    RETURN @ret;
END;");


            Sql(@"CREATE FUNCTION [dbo].[GetLogStatusByCardStatus](@card_status int)
RETURNS int 
AS 

BEGIN
    DECLARE @ret int;
    SET @ret = CASE @card_status WHEN 0 THEN 0 
                                 WHEN 5 THEN 6
                                 WHEN 4 THEN 5
                                 WHEN 1 THEN 1
                                 WHEN 3 THEN 2
                                 WHEN 2 THEN 3
                                 ELSE -1 END
    RETURN @ret;
END;");
        }
        
        public override void Down()
        {
            DropForeignKey("doc.Payments", "pay_type_id", "book.PayTypes");
            DropForeignKey("book.Cards", "tower_id", "book.Towers");
            DropForeignKey("book.Cards", "receiver_id", "book.Receivers");
            DropForeignKey("doc.Payments", "card_id", "book.Cards");
            DropForeignKey("doc.CardServices", "service_id", "book.Service");
            DropForeignKey("doc.CardServices", "card_id", "book.Cards");
            DropForeignKey("doc.CardLogs", "user_id", "book.Users");
            DropForeignKey("book.Users", "type", "book.UserTypes");
            DropForeignKey("book.UserPermissions", "type", "book.UserTypes");
            DropForeignKey("doc.Payments", "user_id", "book.Users");
            DropForeignKey("config.Logging", "user_id", "book.Users");
            DropForeignKey("config.LoggingItems", "logging_id", "config.Logging");
            DropForeignKey("book.Customers", "user_id", "book.Users");
            DropForeignKey("book.Messages", "Customer_Id", "book.Customers");
            DropForeignKey("book.Messages", "user_id", "book.Users");
            DropForeignKey("book.MessageAbonents", "message_id", "book.Messages");
            DropForeignKey("book.MessageAbonents", "abonent_id", "book.Customers");
            DropForeignKey("book.Cards", "customer_id", "book.Customers");
            DropForeignKey("book.Channels", "user_id", "book.Users");
            DropForeignKey("book.Packages", "user_id", "book.Users");
            DropForeignKey("doc.Subscribes", "user_id", "book.Users");
            DropForeignKey("doc.SubscriptionPackages", "subscription_id", "doc.Subscribes");
            DropForeignKey("doc.Subscribes", "card_id", "book.Cards");
            DropForeignKey("doc.SubscriptionPackages", "package_id", "book.Packages");
            DropForeignKey("book.PackageChannels", "package_id", "book.Packages");
            DropForeignKey("book.PackageChannels", "channel_id", "book.Channels");
            DropForeignKey("book.Cards", "user_id", "book.Users");
            DropForeignKey("doc.CardLogs", "card_id", "book.Cards");
            DropForeignKey("doc.CardCharges", "card_id", "book.Cards");
            DropForeignKey("book.Attachments", "payment_id", "doc.Payments");
            DropIndex("doc.CardServices", "IX_ServiceId_Cards");
            DropIndex("doc.CardServices", "IX_CardId_Cards");
            DropIndex("book.UserPermissions", new[] { "type" });
            DropIndex("config.LoggingItems", "IX_LoggingId_loggingItems");
            DropIndex("config.Logging", "IX_TypeId_Loggings");
            DropIndex("config.Logging", "IX_UserId_Logging");
            DropIndex("book.MessageAbonents", "IX_MessageId_MessageAbonents");
            DropIndex("book.MessageAbonents", "IX_AbonentId_MessageAbonents");
            DropIndex("book.Messages", new[] { "Customer_Id" });
            DropIndex("book.Messages", "IX_UserId_Messages");
            DropIndex("book.Customers", "IX_UserId_Customers");
            DropIndex("doc.Subscribes", new[] { "user_id" });
            DropIndex("doc.Subscribes", "IX_CardId_Subscribes");
            DropIndex("doc.SubscriptionPackages", new[] { "package_id" });
            DropIndex("doc.SubscriptionPackages", "IX_SubscriptionId_SubscriptionPackages");
            DropIndex("book.Packages", "IX_UserId_Packages");
            DropIndex("book.PackageChannels", "IX_ChannelId_PackageChannels");
            DropIndex("book.PackageChannels", "IX_PackageId_PackageChannels");
            DropIndex("book.Channels", "IX_UserId_Channels");
            DropIndex("book.Users", new[] { "type" });
            DropIndex("doc.CardLogs", "IX_UserId_CardLogs");
            DropIndex("doc.CardLogs", new[] { "card_id" });
            DropIndex("doc.CardCharges", "IX_CardId_CardCharges");
            DropIndex("book.Cards", new[] { "receiver_id" });
            DropIndex("book.Cards", new[] { "tower_id" });
            DropIndex("book.Cards", "IX_UserId_Cards");
            DropIndex("book.Cards", "IX_CardStatus_Cards");
            DropIndex("book.Cards", "IX_CustomerId_Cards");
            DropIndex("doc.Payments", "IX_UserId_Payments");
            DropIndex("doc.Payments", new[] { "pay_type_id" });
            DropIndex("doc.Payments", "IX_CardId_Payments");
            DropIndex("book.Attachments", new[] { "payment_id" });
            DropTable("config.Params");
            DropTable("book.MessageTemplates");
            DropTable("config.ChargeCrushLogs");
            DropTable("book.PayTypes");
            DropTable("book.Towers");
            DropTable("book.Receivers");
            DropTable("book.Service");
            DropTable("doc.CardServices");
            DropTable("book.UserPermissions");
            DropTable("book.UserTypes");
            DropTable("config.LoggingItems");
            DropTable("config.Logging");
            DropTable("book.MessageAbonents");
            DropTable("book.Messages");
            DropTable("book.Customers");
            DropTable("doc.Subscribes");
            DropTable("doc.SubscriptionPackages");
            DropTable("book.Packages");
            DropTable("book.PackageChannels");
            DropTable("book.Channels");
            DropTable("book.Users");
            DropTable("doc.CardLogs");
            DropTable("doc.CardCharges");
            DropTable("book.Cards");
            DropTable("doc.Payments");
            DropTable("book.Attachments");
        }
    }
}
