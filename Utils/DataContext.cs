using DigitalTVBilling.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Utils
{
    public class DataContext: DbContext
    {
        public DataContext() : base("DataConnect")
        {
            this.Configuration.LazyLoadingEnabled = false;
            this.Configuration.ValidateOnSaveEnabled = false;
            this.Configuration.AutoDetectChangesEnabled = false;
            this.Database.CommandTimeout = 1200;
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Channel> Channels { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<PayType> PayTypes { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Subscribtion> Subscribtions { get; set; }
        public DbSet<SubscriptionPackage> SubscriptionPackages { get; set; }
        public DbSet<PackageChannel> PackageChannels { get; set; }
        public DbSet<CardCharge> CardCharges { get; set; }
        public DbSet<Attachment> Attachments { get; set; }
        public DbSet<CardLog> CardLogs { get; set; }
        public DbSet<Param> Params { get; set; }
        public DbSet<Logging> Loggings { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<MessageAbonent> MessageAbonents { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<UserPermission> UserPermissions { get; set; }
        public DbSet<MessageTemplate> MessageTemplates { get; set; }
        public DbSet<Receiver> Receivers { get; set; }
        public DbSet<ReturnedCard> ReturnedCards { get; set; }
        public DbSet<ReturnedCardAttachment> ReturnedCardAttachments { get; set; }
        public DbSet<BortHistory> BortHistorys { get; set; }
        public DbSet<Tower> Towers { get; set; }
        public DbSet<LoggingItem> LoggingItems { get; set; }
        public DbSet<ChargeCrushLog> ChargeCrushLogs { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<CardService> CardServices { get; set; }
        public DbSet<PayTransaction> PayTransactions { get; set; }
        public DbSet<PayTransactionCard> PayTransactionCard { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<CustomerChat> CustomersChat { get; set; }
        public DbSet<MessageNotSendLog> MessageNotSendLogs { get; set; }
        public DbSet<CardDamage> CardDamages { get; set; }
        public DbSet<AutoMessageTemplate> AutoMessageTemplates { get; set; }
        public DbSet<Reason> Reasons { get; set; }
        public DbSet<OrderReason> OrderReasons { get; set; }
        public DbSet<DamageReason> DamageReasons { get; set; }
        public DbSet<CanceledPayment> CanceledPayments { get; set; }
        public DbSet<OfficeCard> OfficeCards { get; set; }
        public DbSet<AutoSubscribChangeCard> AutoSubscribChangeCards { get; set; }
        public DbSet<st_Customers> st_Customers { get; set; }
        public DbSet<InvoceLogging> InvoiceLoggings { get; set; }
        public DbSet<Damage> Damages { get; set; }
        public DbSet<OperatorGroupUser> OperatorGroupUsers { get; set; }
        public DbSet<TempCasCard> TempCasCards { get; set; }
        public DbSet<Cancellation> Cancellations { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<SellerObject> Seller { get; set; }
        public DbSet<ReceiverSubscribtion> receiver_subscribes { get; set; }
        public DbSet<SellAttachment> SellAttachments { get; set; }
        public DbSet<ReceiverAttachment> ReceiverAttachments { get; set; }
        public DbSet<CustomerSellAttachments> CustomerSellAttachments { get; set; }
        public DbSet<RecordAudioFile> RecordAudioFiles { get; set; }
        public DbSet<SellAttachmentsLogging> SellAttachmentsLoggings { get; set; }
        public DbSet<MessageLogging> MessageLoggings { get; set; }
        public DbSet<JuridicalStatus> JuridicalStatus { get; set; }
        public DbSet<JuridicalLogging> JuridicalLoggings { get; set; }
        public DbSet<JuridicalInfoStatus> JuridicalInfoStatus { get; set; }
        public DbSet<ReRegistering> ReRegisterings { get; set; }
        public DbSet<MiniSMS> MiniSMS { get; set; }
        public DbSet<JuridicalDocsInfo> JuridicalDocsInfo { get; set; }
        public DbSet<PromoCahngePack> PromoCahngePacks { get; set; }
        public DbSet<OrderReserveAnswer> OrderReserveAnswers { get; set; }
        public DbSet<DamageReserveAnswer> DamageReserveAnswers { get; set; }
        public DbSet<StatusLogging> StatusLoggings { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            modelBuilder.Entity<CardCharge>().Property(p => p.Amount).HasPrecision(18, 4);
            modelBuilder.Entity<CanceledPayment>().Property(p => p.Amount).HasPrecision(18, 4);
            modelBuilder.Entity<Payment>().Property(p => p.Amount).HasPrecision(18, 4);
            modelBuilder.Entity<CardService>().Property(p => p.Amount).HasPrecision(18, 4);
            modelBuilder.Entity<PayTransaction>().Property(p => p.Amount).HasPrecision(18, 4);

            modelBuilder.Entity<Order>()
                    .HasRequired(m => m.UserUser)
                    .WithMany(t => t.UserOrders)
                    .HasForeignKey(m => m.UserId)
                    .WillCascadeOnDelete(false);

            modelBuilder.Entity<Order>()
                        .HasRequired(m => m.UserGroup)
                        .WithMany(t => t.GroupOrders)
                        .HasForeignKey(m => m.UserGroupId)
                        .WillCascadeOnDelete(false);
        }

        public System.Data.Entity.DbSet<DigitalTVBilling.ListModels.AbonentRecord> AbonentRecords { get; set; }

        public System.Data.Entity.DbSet<DigitalTVBilling.Models.AbonentList> AbonentLists { get; set; }
    }
}