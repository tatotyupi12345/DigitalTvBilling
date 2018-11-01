using DigitalTVBilling.Models;
using DigitalTVBilling.Utils;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace DigitalTVBilling.Jobs
{
    public class JobSheduler
    {

        //public static Quartz.Collection.ISet<ITrigger> triggers_set { get; set; }
        //public static ITrigger[] triggers;
        private static IScheduler scheduler = StdSchedulerFactory.GetDefaultScheduler();


        //private static IJobDetail charge_card_job = JobBuilder.Create<CardsChargeJob>().StoreDurably()
        //                                                        .WithIdentity("ChargeJob", "group1")
        //                                                        .Build();

        //public static void resCheduleTrigger(string triggerkey, int CardID, int t_hour, int t_minute)
        //{
        //    if (triggerkey != null)
        //    {
        //        ITrigger old_trigger = triggers_set.Where(x => x.Key.Name == triggerkey).FirstOrDefault();
        //        if (old_trigger != null)
        //        {
        //            if (scheduler.CheckExists(old_trigger.Key))
        //            {
        //                ITrigger trigger = old_trigger.GetTriggerBuilder()
        //                     .WithIdentity(triggerkey, "group1").UsingJobData("CardID", CardID)
        //                     .StartNow()
        //                     .WithSimpleSchedule(x => x
        //                         .WithIntervalInMinutes(60)
        //                         .RepeatForever())
        //                     .Build();

        //                scheduler.RescheduleJob(old_trigger.Key, trigger);
        //            }
        //        }
        //        else
        //        {
        //            triggers_set.Add(TriggerBuilder.Create()
        //                                .WithIdentity(triggerkey, "group1").UsingJobData("CardID", CardID)
        //                                    .StartNow()
        //                                        .WithSimpleSchedule(x => x
        //                                        .WithIntervalInMinutes(60)
        //                                        .RepeatForever()).ForJob(charge_card_job)
        //                                    .Build());

        //            ITrigger trigger = triggers_set.Where(x => x.Key.Name == triggerkey).FirstOrDefault();
        //            scheduler.ScheduleJob(trigger);
        //        }
        //    }
        //}

        public static void Start()
        {
            //scheduler.AddJob(charge_card_job, false);


            //List<int> cards;

            //using (DataContext _db = new DataContext())
            //{
            //    cards = _db.Cards.Select(x => x.Id).ToList();
            //    //cards = _db.Cards.Where(c => c.CardStatus != CardStatus.Canceled).ToList();

            //    triggers_set = new Quartz.Collection.HashSet<ITrigger>();
            //    triggers = new ITrigger[cards.Count];

            //    for (int i = 0; i < cards.Count; i++)
            //    {
            //        ITrigger trigger = TriggerBuilder.Create()
            //             .WithIdentity("trigger_" + cards[i].ToString(), "group1").UsingJobData("CardID", cards[i])
            //                .WithSimpleSchedule(x => x
            //                .WithIntervalInMinutes(120)
            //                .RepeatForever().WithMisfireHandlingInstructionIgnoreMisfires()).ForJob(charge_card_job).StartNow()
            //             .Build();

            //        //if(cards[i] == 353403)
            //        //{
            //        //    scheduler.ScheduleJob(trigger);
            //        //}
            //        //else
            //        scheduler.ScheduleJob(trigger);
            //        //triggers[i] = trigger;
            //        //triggers_set.Add(trigger);
            //    }
            //}

            //if(triggers_set.Count > 0)
            //{
            //    foreach(ITrigger trig in triggers_set.ToList())
            //    {
            //        scheduler.ScheduleJob(trig);
            //    }
            //}

            //scheduler.ScheduleJob(charge_card_job, triggers_set, false);

            IJobDetail report_disabling_job = JobBuilder.Create<ReportUserAboutDisablingJob>()
               .WithIdentity("ReportDeisablingJob", "group1")
               .Build();

            IJobDetail report_on_disable_job = JobBuilder.Create<ReportUserOnDisableJob>()
                .WithIdentity("ReportOnDisableJob", "group2")
                .Build();

            IJobDetail charge_card_job = JobBuilder.Create<ChargeCardJob>()
                .WithIdentity("ChargeJob", "group3")
                .Build();

            IJobDetail reset_pause_job = JobBuilder.Create<ResetCardsPauseJob>()
                .WithIdentity("ChargeJob", "group3")
                .Build();

            IJobDetail disable_card_job = JobBuilder.Create<DisableCardsJob>()
                .WithIdentity("DisableJob", "group4")
                .Build();

            IJobDetail rent_late_message_job = JobBuilder.Create<ReporOnRentLateJob>()
                .WithIdentity("MessageJob", "group5")
                .Build();

            IJobDetail invoicejuridical_send_job = JobBuilder.Create<JuridicalInvoce>()
                .WithIdentity("JuridicalInvoce", "group6")
                .Build();

            IJobDetail free_instalation = JobBuilder.Create<ReporFreeInstalation>()
              .WithIdentity("ReporFreeInstalation", "group10")
              .Build();

            IJobDetail change_orde_damage_cancellation = JobBuilder.Create<ChangeOrderDamageCancellation>()
            .WithIdentity("ChangeOrderDamageCancellation", "group11")
            .Build();

            IJobDetail promo_change_pack = JobBuilder.Create<PromoChangePack_8>()
            .WithIdentity("PromoChangePack_8", "group12")
            .Build();
            //IJobDetail message_job = JobBuilder.Create<MessageJob>()
            //    .WithIdentity("MessageJob", "group2")
            //    .Build();

            //IJobDetail staffs_emails_job = JobBuilder.Create<StaffsEmailsJob>()
            //    .WithIdentity("StaffsEmails", "group3")
            //    .Build();

            //IJobDetail invoice_send_job = JobBuilder.Create<InvoiceSendJob>()
            //    .WithIdentity("InvoiceSendJob", "group4")
            //    .Build();

            //IJobDetail auto_subscrib_job = JobBuilder.Create<AutoSubscribJob>()
            //    .WithIdentity("AutoSubscrib", "group6")
            //    .Build();

            /*IJobDetail post_charge_job = JobBuilder.Create<PostChargeJob>()
                .WithIdentity("PostChargeJob", "group6")
                .Build();*/

            IJobDetail report_disabling_share8_job = JobBuilder.Create<ReportUserAboutDisablingShare8Job>()
               .WithIdentity("ReportDeisablingShare8Job", "group7")
               .Build();

            IJobDetail report_disable_share8_job = JobBuilder.Create<ReportUserOnDisableShare8Job>()
               .WithIdentity("ReportDeisableShare8Job", "group8")
               .Build();

            //IJobDetail rent_sms_job = JobBuilder.Create<RentSMSJob>()
            //    .WithIdentity("RentSMSJob", "group9")
            //    .Build();

            int charge_min = 0, charge_hour = 0;
            int message_min = 0, message_hour = 0;
            using (DataContext _db = new DataContext())
            {
                var _params = _db.Params.Where(p => p.Name == "CardCharge" || p.Name == "MessageTime").ToList();
                string[] charge_val = _params.Find(c => c.Name == "CardCharge").Value.Split(':');
                charge_hour = int.Parse(charge_val[0]);
                charge_min = int.Parse(charge_val[1]);

                string[] message_val = _params.Find(c => c.Name == "MessageTime").Value.Split(':');
                message_hour = int.Parse(message_val[0]);
                message_min = int.Parse(message_val[1]);
            }


            if (scheduler.CheckExists(change_orde_damage_cancellation.Key))
            {
                scheduler.RescheduleJob(new TriggerKey("change"), TriggerBuilder.Create().WithIdentity("change")
                    .WithDailyTimeIntervalSchedule
                      (s =>
                         s.WithIntervalInHours(24)
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(charge_hour, 10)) //saati da cuti parametrebidan
                      )
                    .Build());


                //.WithSimpleSchedule(x => x
                //                        .WithIntervalInMinutes(5)
                //                        .RepeatForever()).StartNow()
                //                        .Build());
            }
            else
            {
                scheduler.ScheduleJob(change_orde_damage_cancellation, TriggerBuilder.Create().WithIdentity("change")
                    .WithDailyTimeIntervalSchedule
                      (s =>
                         s.WithIntervalInHours(24)
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(charge_hour, 10)) //saati da cuti parametrebidan
                      )
                    .Build());

                //.WithSimpleSchedule(x => x
                //.WithIntervalInMinutes(5)
                //.RepeatForever()).StartNow()
                //.Build());
            }

            if (scheduler.CheckExists(free_instalation.Key))
            {
                scheduler.RescheduleJob(new TriggerKey("report_free"), TriggerBuilder.Create().WithIdentity("report_free")
                    .WithDailyTimeIntervalSchedule
                      (s =>
                         s.WithIntervalInHours(24)
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(message_hour, message_min)) //saati da cuti parametrebidan
                      )
                    .Build());


                //.WithSimpleSchedule(x => x
                //                        .WithIntervalInMinutes(5)
                //                        .RepeatForever()).StartNow()
                //                        .Build());
            }
            else
            {
                scheduler.ScheduleJob(free_instalation, TriggerBuilder.Create().WithIdentity("report_free")
                    .WithDailyTimeIntervalSchedule
                      (s =>
                         s.WithIntervalInHours(24)
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(message_hour, message_min)) //saati da cuti parametrebidan
                      )
                    .Build());

                //.WithSimpleSchedule(x => x
                //.WithIntervalInMinutes(5)
                //.RepeatForever()).StartNow()
                //.Build());
            }



            if (scheduler.CheckExists(report_disabling_job.Key))
            {
                scheduler.RescheduleJob(new TriggerKey("report_disabling_trigger"), TriggerBuilder.Create().WithIdentity("report_disabling_trigger")
                    .WithDailyTimeIntervalSchedule
                      (s =>
                         s.WithIntervalInHours(24)
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(message_hour, message_min)) //saati da cuti parametrebidan
                      )
                    .Build());


                //.WithSimpleSchedule(x => x   
                //                    .WithIntervalInHours(1)
                //                    .RepeatForever())
                //                    .Build());
            }
            else
            {
                scheduler.ScheduleJob(report_disabling_job, TriggerBuilder.Create().WithIdentity("report_disabling_trigger")
                    .WithDailyTimeIntervalSchedule
                          (s =>
                             s.WithIntervalInHours(24)
                            .OnEveryDay()
                            .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(message_hour, message_min)) //saati da cuti parametrebidan
                          )
                        .Build());

                //.WithSimpleSchedule(x => x
                //                    .WithIntervalInHours(1)
                //                    .RepeatForever())
                //                    .Build());




            }
            if (scheduler.CheckExists(promo_change_pack.Key))
            {
                scheduler.RescheduleJob(new TriggerKey("promo_change_pack_8"), TriggerBuilder.Create().WithIdentity("promo_change_pack_8")
                    .WithDailyTimeIntervalSchedule
                      (s =>
                         s.WithIntervalInHours(24)
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(message_hour, message_min)) //saati da cuti parametrebidan
                      )
                    .Build());
            }
            else
            {
                scheduler.ScheduleJob(promo_change_pack, TriggerBuilder.Create().WithIdentity("promo_change_pack_8")
                    .WithDailyTimeIntervalSchedule
                          (s =>
                             s.WithIntervalInHours(24)
                            .OnEveryDay()
                            .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(message_hour, message_min)) //saati da cuti parametrebidan
                          )
                        .Build());

            }

            if (scheduler.CheckExists(report_disable_share8_job.Key))
            {
                scheduler.RescheduleJob(new TriggerKey("report_disable_share8_trigger"), TriggerBuilder.Create().WithIdentity("report_disable_share8_trigger")
                    .WithDailyTimeIntervalSchedule
                      (s =>
                         s.WithIntervalInHours(24)
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(message_hour, message_min)) //saati da cuti parametrebidan
                      )
                    .Build());
            }
            else
            {
                scheduler.ScheduleJob(report_disable_share8_job, TriggerBuilder.Create().WithIdentity("report_disable_share8_trigger")
                    .WithDailyTimeIntervalSchedule
                          (s =>
                             s.WithIntervalInHours(24)
                            .OnEveryDay()
                            .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(message_hour, message_min)) //saati da cuti parametrebidan
                          )
                        .Build());

            }


            if (scheduler.CheckExists(report_disabling_share8_job.Key))
            {
                scheduler.RescheduleJob(new TriggerKey("report_disabling_share8_trigger"), TriggerBuilder.Create().WithIdentity("report_disabling_share8_trigger")
                    .WithDailyTimeIntervalSchedule
                      (s =>
                         s.WithIntervalInHours(24)
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(message_hour, message_min)) //saati da cuti parametrebidan
                      )
                    .Build());


                //.WithSimpleSchedule(x => x   
                //                    .WithIntervalInHours(1)
                //                    .RepeatForever())
                //                    .Build());
            }
            else
            {
                scheduler.ScheduleJob(report_disabling_share8_job, TriggerBuilder.Create().WithIdentity("report_disabling_share8_trigger")
                    .WithDailyTimeIntervalSchedule
                          (s =>
                             s.WithIntervalInHours(24)
                            .OnEveryDay()
                            .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(message_hour, message_min)) //saati da cuti parametrebidan
                          )
                        .Build());

                //.WithSimpleSchedule(x => x
                //                    .WithIntervalInHours(1)
                //                    .RepeatForever())
                //                    .Build());




            }


            if (scheduler.CheckExists(report_on_disable_job.Key))
            {
                scheduler.RescheduleJob(new TriggerKey("report_on_disable_trigger"), TriggerBuilder.Create().WithIdentity("report_on_disable_trigger")
                    .WithDailyTimeIntervalSchedule
                      (s =>
                         s.WithIntervalInHours(24)
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(message_hour, message_min)) //saati da cuti parametrebidan
                      )
                    .Build());
            }
            else
            {
                scheduler.ScheduleJob(report_on_disable_job, TriggerBuilder.Create().WithIdentity("report_on_disable_trigger")
                        .WithDailyTimeIntervalSchedule
                          (s =>
                             s.WithIntervalInHours(24)
                            .OnEveryDay()
                            .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(message_hour, message_min)) //saati da cuti parametrebidan
                          )
                        .Build());
            }

            /////// rent sms 
            //if (scheduler.CheckExists(rent_sms_job.Key))
            //{
            //    scheduler.RescheduleJob(new TriggerKey("rent_trigger"), TriggerBuilder.Create().WithIdentity("rent_trigger")
            //        .WithDailyTimeIntervalSchedule
            //          (s =>
            //             s.WithIntervalInHours(24)
            //            .OnEveryDay()
            //            .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(message_hour, message_min)) //saati da cuti parametrebidan
            //          )
            //        .Build());
            //}
            //else
            //{
            //    scheduler.ScheduleJob(rent_sms_job, TriggerBuilder.Create().WithIdentity("rent_trigger")
            //            .WithDailyTimeIntervalSchedule
            //              (s =>
            //                 s.WithIntervalInHours(24)
            //                .OnEveryDay()
            //                .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(message_hour, message_min)) //saati da cuti parametrebidan
            //              )
            //            .Build());
            //}
            //if (scheduler.CheckExists(report_on_disable_job.Key))
            //{
            //    scheduler.RescheduleJob(new TriggerKey("report_on_disable_trigger"), TriggerBuilder.Create().WithIdentity("report_on_disable_trigger")
            //        .WithSimpleSchedule(x => x
            //                            .WithIntervalInSeconds(75)
            //                            .RepeatForever())
            //                            .Build());
            //}
            //else
            //{
            //    scheduler.ScheduleJob(report_on_disable_job, TriggerBuilder.Create().WithIdentity("report_on_disable_trigger")
            //            .WithSimpleSchedule(x => x
            //                                .WithIntervalInSeconds(75)
            //                                .RepeatForever())
            //                                .Build());
            //}

            if (scheduler.CheckExists(charge_card_job.Key))
            {
                scheduler.RescheduleJob(new TriggerKey("charge_trigger"), TriggerBuilder.Create().WithIdentity("charge_trigger")
                    .WithDailyTimeIntervalSchedule
                      (s =>
                         s.WithIntervalInHours(24)
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(charge_hour, charge_min)) //saati da cuti parametrebidan
                      )
                    .Build());


                //.WithSimpleSchedule(x => x
                //                        .WithIntervalInMinutes(5)
                //                        .RepeatForever()).StartNow()
                //                        .Build());
            }
            else
            {
                scheduler.ScheduleJob(charge_card_job, TriggerBuilder.Create().WithIdentity("charge_trigger")
                    .WithDailyTimeIntervalSchedule
                      (s =>
                         s.WithIntervalInHours(24)
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(charge_hour, charge_min)) //saati da cuti parametrebidan
                      )
                    .Build());

                //.WithSimpleSchedule(x => x
                //.WithIntervalInMinutes(5)
                //.RepeatForever()).StartNow()
                //.Build());
            }

            if (scheduler.CheckExists(reset_pause_job.Key))
            {
                scheduler.RescheduleJob(new TriggerKey("pause_reset_trigger"), TriggerBuilder.Create().WithIdentity("pause_reset_trigger")
                    .WithDailyTimeIntervalSchedule
                      (s =>
                         s.WithIntervalInHours(24)
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(charge_hour, charge_min)) //saati da cuti parametrebidan
                      )
                    .Build());
            }
            else
            {
                scheduler.ScheduleJob(reset_pause_job, TriggerBuilder.Create().WithIdentity("pause_reset_trigger")
                    .WithDailyTimeIntervalSchedule
                      (s =>
                         s.WithIntervalInHours(24)
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(charge_hour, charge_min)) //saati da cuti parametrebidan
                      )
                    .Build());
            }

            if (scheduler.CheckExists(rent_late_message_job.Key))
            {
                scheduler.RescheduleJob(new TriggerKey("report_on_rent_late_trigger"), TriggerBuilder.Create().WithIdentity("report_on_rent_late_trigger")
                    .WithDailyTimeIntervalSchedule
                      (s =>
                         s.WithIntervalInHours(24)
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(message_hour, message_min)) //saati da cuti parametrebidan
                      )
                    .Build());


                //.WithSimpleSchedule(x => x
                //                        .WithIntervalInMinutes(5)
                //                        .RepeatForever()).StartNow()
                //                        .Build());
            }
            else
            {
                scheduler.ScheduleJob(rent_late_message_job, TriggerBuilder.Create().WithIdentity("report_on_rent_late_trigger")
                    .WithDailyTimeIntervalSchedule
                      (s =>
                         s.WithIntervalInHours(24)
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(message_hour, message_min)) //saati da cuti parametrebidan
                      )
                    .Build());

                //.WithSimpleSchedule(x => x
                //.WithIntervalInMinutes(5)
                //.RepeatForever()).StartNow()
                //.Build());
            }

            if (scheduler.CheckExists(disable_card_job.Key))
            {
                scheduler.RescheduleJob(new TriggerKey("disable_trigger"), TriggerBuilder.Create().WithIdentity("disable_trigger")
                                    .WithDailyTimeIntervalSchedule
                                      (s =>
                                         s.WithIntervalInHours(24)
                                        .OnEveryDay()
                                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 1)) //saati da cuti parametrebidan
                                      )
                                    .Build());


                //.WithSimpleSchedule(x => x
                //                        .WithIntervalInMinutes(5)
                //                        .RepeatForever()).StartNow()
                //                        .Build());
            }
            else
            {
                scheduler.ScheduleJob(disable_card_job, TriggerBuilder.Create().WithIdentity("disable_trigger")
                    .WithDailyTimeIntervalSchedule
                      (s =>
                         s.WithIntervalInHours(24)
                        .OnEveryDay()
                        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 1)) //saati da cuti parametrebidan
                      )
                    .Build());


                //.WithSimpleSchedule(x => x
                //.WithIntervalInMinutes(5)
                //.RepeatForever()).StartNow()
                //.Build());
            }

            //if (scheduler.CheckExists(message_job.Key))
            //{
            //    scheduler.RescheduleJob(new TriggerKey("message_trigger"), TriggerBuilder.Create().WithIdentity("message_trigger")
            //    .WithDailyTimeIntervalSchedule
            //      (s =>
            //         s.WithIntervalInHours(24)
            //        .OnEveryDay()
            //        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(message_hour, message_min)) //saati da cuti parametrebidan
            //      )
            //    .Build());
            //}
            //else
            //{
            //    scheduler.ScheduleJob(message_job, TriggerBuilder.Create().WithIdentity("message_trigger")
            //    .WithDailyTimeIntervalSchedule
            //      (s =>
            //         s.WithIntervalInHours(24)
            //        .OnEveryDay()
            //        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(message_hour, message_min)) //saati da cuti parametrebidan
            //      )
            //    .Build());
            //}

            //if (scheduler.CheckExists(staffs_emails_job.Key))
            //{
            //    scheduler.RescheduleJob(new TriggerKey("staffs_email_trigger"), TriggerBuilder.Create().WithIdentity("staffs_email_trigger")
            //    .WithDailyTimeIntervalSchedule
            //      (s =>
            //         s.WithIntervalInHours(24)
            //        .OnEveryDay()
            //        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(10, 0)) //saati da cuti parametrebidan
            //      )
            //    .Build());
            //}
            //else
            //{
            //    scheduler.ScheduleJob(staffs_emails_job, TriggerBuilder.Create().WithIdentity("staffs_email_trigger")
            //    .WithDailyTimeIntervalSchedule
            //      (s =>
            //         s.WithIntervalInHours(24)
            //        .OnEveryDay()
            //        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(10, 0)) //saati da cuti parametrebidan
            //      )
            //    .Build());
            //}


            //if (scheduler.CheckExists(auto_subscrib_job.Key))
            //{
            //    scheduler.RescheduleJob(new TriggerKey("auto_subscrib_trigger"), TriggerBuilder.Create().WithIdentity("auto_subscrib_trigger")
            //    .WithDailyTimeIntervalSchedule
            //      (s =>
            //         s.WithIntervalInHours(24)
            //        .OnEveryDay()
            //        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 5)) //saati da cuti parametrebidan
            //      )
            //    .Build());
            //}
            //else
            //    scheduler.ScheduleJob(auto_subscrib_job, TriggerBuilder.Create().WithIdentity("auto_subscrib_trigger")
            //    .WithDailyTimeIntervalSchedule
            //      (s =>
            //         s.WithIntervalInHours(24)
            //        .OnEveryDay()
            //        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 5)) //saati da cuti parametrebidan
            //      )
            //    .Build());

            //if (scheduler.CheckExists(invoice_send_job.Key))
            //{
            //    scheduler.RescheduleJob(new TriggerKey("invoice_send_trigger"), TriggerBuilder.Create().WithIdentity("invoice_send_trigger")
            //    .WithDailyTimeIntervalSchedule
            //      (s =>
            //         s.WithIntervalInHours(24)
            //        .OnEveryDay()
            //        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(13, 0)) //saati da cuti parametrebidan
            //      )
            //    .Build());
            //}
            //else
            //{
            //    scheduler.ScheduleJob(invoice_send_job, TriggerBuilder.Create().WithIdentity("invoice_send_trigger")
            //    .WithDailyTimeIntervalSchedule
            //      (s =>
            //         s.WithIntervalInHours(24)
            //        .OnEveryDay()
            //        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(13, 0)) //saati da cuti parametrebidan
            //      )
            //    .Build());
            //}


            // invoisi
            if (scheduler.CheckExists(invoicejuridical_send_job.Key))
            {
                scheduler.RescheduleJob(new TriggerKey("invoice_send_trigger"), TriggerBuilder.Create().WithIdentity("invoice_send_trigger")
                .WithIdentity("Run Infinitely every 2nd day of the month", "Monthly_Day_2")
                //.StartNow()
                .WithSchedule(CronScheduleBuilder.MonthlyOnDayAndHourAndMinute((DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), 10, 01))
                .Build());
            }
            else
            {
                scheduler.ScheduleJob(invoicejuridical_send_job, TriggerBuilder.Create().WithIdentity("invoice_send_trigger")
                 .WithIdentity("Run Infinitely every 2nd day of the month", "Monthly_Day_2")
                //.StartNow()
                .WithSchedule(CronScheduleBuilder.MonthlyOnDayAndHourAndMinute((DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)), 10, 01))
                .Build());
            }
            //if (scheduler.CheckExists(invoicejuridical_send_job.Key))
            //{
            //    scheduler.RescheduleJob(new TriggerKey("invoice_send_trigger"), TriggerBuilder.Create().WithIdentity("invoice_send_trigger")
            //    .WithIdentity("Run Infinitely every 2nd day of the month", "Monthly_Day_2")
            //    .StartNow()
            //    .WithSimpleSchedule(x => x
            //        .WithIntervalInMinutes(4)
            //        .RepeatForever())
            //    .Build());
            //}
            //else
            //{
            //    scheduler.ScheduleJob(invoicejuridical_send_job, TriggerBuilder.Create().WithIdentity("invoice_send_trigger")
            //     .WithIdentity("Run Infinitely every 2nd day of the month", "Monthly_Day_2")
            //    .StartNow()
            //    .WithSimpleSchedule(x => x
            //        .WithIntervalInMinutes(4)
            //        .RepeatForever())
            //    .Build());
            //}
            //if (scheduler.CheckExists(invoicejuridical_send_job.Key))
            //{
            //    scheduler.RescheduleJob(new TriggerKey("invoice_send_trigger"), TriggerBuilder.Create().WithIdentity("invoice_send_trigger")
            //    .WithDailyTimeIntervalSchedule
            //      (s =>
            //         s.WithIntervalInMinutes(3)
            //        .OnEveryDay()
            //        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(13, 45)) //saati da cuti parametrebidan
            //      )
            //    .Build());
            //}
            //else
            //{
            //    scheduler.ScheduleJob(invoicejuridical_send_job, TriggerBuilder.Create().WithIdentity("invoice_send_trigger")
            //    .WithDailyTimeIntervalSchedule
            //      (s =>
            //         s.WithIntervalInMinutes(59)
            //        .OnEveryDay()
            //        .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(13, 45)) //saati da cuti parametrebidan
            //      )
            //    .Build());
            //}
            //         scheduler.ScheduleJob(invoicejuridical_send_job, TriggerBuilder.Create().WithIdentity("invoice_send_trigger")
            //.WithDailyTimeIntervalSchedule
            //  (s =>
            //     s.WithIntervalInMinutes(30)
            //    .OnEveryDay()
            //    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(21, 31)) //saati da cuti parametrebidan
            //  )
            //.Build());
            //     }
            /*if (scheduler.CheckExists(post_charge_job.Key))
            {
                scheduler.RescheduleJob(new TriggerKey("post_charge_trigger"), TriggerBuilder.Create().WithIdentity("post_charge_trigger")
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 15)) //saati da cuti parametrebidan
                  )
                .Build());
            }
            else
            {
                scheduler.ScheduleJob(post_charge_job, TriggerBuilder.Create().WithIdentity("post_charge_trigger")
                .WithDailyTimeIntervalSchedule
                  (s =>
                     s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 15)) //saati da cuti parametrebidan
                  )
                .Build());
            }*/

            scheduler.Start();
        }

        //public static void RemoveTrigger(string trigger_key)
        //{
        //    ITrigger old_trigger = triggers_set.Where(x => x.Key.Name == trigger_key).FirstOrDefault();
        //    if (old_trigger != null)
        //    {
        //        scheduler.UnscheduleJob(old_trigger.Key);
        //        triggers_set.Remove(old_trigger);
        //    }

        //}

        //public static void refresh()
        //{
        //    scheduler.ScheduleJob(charge_card_job, triggers_set, true);
        //}

        //public static void getScheduleDetails()
        //{
        //    //ISchedulerFactory schedFact = new StdSchedulerFactory();
        //    //foreach (IScheduler scheduler in schedFact.AllSchedulers)
        //    //{
        //    //    var scheduler1 = scheduler;
        //    //    //var JGN = scheduler1.jobna

        //    //    foreach (var jobDetail in from jobGroupName in scheduler1.GetJobGroupNames()
        //    //                              from jobName in scheduler1.GetJobKeys(jobGroupName)
        //    //                              select scheduler1.GetJobDetail(jobName, jobGroupName))
        //    //    {
        //    //        //Get props about job from jobDetail
        //    //    }

        //    //    foreach (var triggerDetail in from triggerGroupName in scheduler1.TriggerGroupNames
        //    //                                  from triggerName in scheduler1.GetTriggerNames(triggerGroupName)
        //    //                                  select scheduler1.GetTrigger(triggerName, triggerGroupName))
        //    //    {
        //    //        //Get props about trigger from triggerDetail
        //    //    }
        //    //}
        //}
    }
}