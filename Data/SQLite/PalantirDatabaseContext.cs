﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Palantir_Rebirth.Data.SQLite
{
    public class PalantirDatabaseContext : DbContext
    {
        private string path;
        public PalantirDatabaseContext(string path) {
            this.path = path;
        }

        public DbSet<MemberEntity> Members { get; set; }
        public DbSet<PalantirRegularEntity> Palantiri { get; set; }
        public DbSet<PalantirNightlyEntity> PalantiriNightly { get; set; }
        public DbSet<ReportEntity> Reports { get; set; }
        public DbSet<LobbyEntity> Lobbies { get; set; }
        public DbSet<GuildLobbiesEntity> GuildLobbies { get; set; }
        public DbSet<GuildSettingsEntity> GuildSettings { get; set; }
        public DbSet<StatusEntity> Status { get; set; }
        public DbSet<OnlineSpritesEntity> OnlineSprites { get; set; }
        public DbSet<SpritesEntity> Sprites { get; set; }
        public DbSet<SceneEntity> Scenes { get; set; }
        public DbSet<DropEntity> Drop { get; set; }
        public DbSet<PastDropEntity> PastDrops { get; set; }
        public DbSet<BubbleTraceEntity> BubbleTraces { get; set; }
        public DbSet<EventEntity> Events { get; set; }
        public DbSet<EventDropEntity> EventDrops { get; set; }
        public DbSet<EventCreditEntity> EventCredits { get; set; }
        public DbSet<TypoThemeEntity> Themes { get; set; }
        public DbSet<BoostEntity> DropBoosts { get; set; }
        public DbSet<WebhookEntity> Webhooks { get; set; }
        public DbSet<BoostSplitEntity> BoostSplits { get; set; }
        public DbSet<SplitCreditEntity> SplitCredits { get; set; }
        public DbSet<OnlineItemsEntity> OnlineItems { get; set; }
        public DbSet<SpriteProfileEntity> SpriteProfiles { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=" + path);
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ReportEntity>()
                .HasKey(e => new { e.LobbyID, e.ObserveToken, e.Report });
            modelBuilder.Entity<EventCreditEntity>()
                .HasKey(e => new { e.Login, e.EventDropID });
            modelBuilder.Entity<WebhookEntity>()
                .HasKey(e => new { e.ServerID, e.Name });
            modelBuilder.Entity<SplitCreditEntity>()
                .HasKey(e => new { e.Login, e.Split });
            modelBuilder.Entity<PastDropEntity>()
                 .HasKey(e => new { e.DropID, e.CaughtLobbyPlayerID });
            modelBuilder.Entity<OnlineItemsEntity>()
                 .HasKey(e => new { e.ItemType, e.Slot, e.ItemID, e.LobbyKey, e.LobbyPlayerID });
            modelBuilder.Entity<SpriteProfileEntity>()
                 .HasKey(e => new { e.Login, e.Name });
        }

    }


    public class MemberEntity
    {
        [Key]
        public string Login { get; set; }
        public string Member { get; set; }
        public string Sprites { get; set; }
        public int Bubbles { get; set; }
        public int Drops { get; set; }
        public int Flag { get; set; }
        public string? Emoji { get; set; }
        public string? Patronize { get; set; }
        public string? Customcard { get; set; }
        public string? Scenes { get; set; }
        public string Streamcode { get; set; }
        public string? RainbowSprites { get; set; }
    }
    public abstract class PalantirEntity
    {
        [Key]
        public string Token { get; set; }
        public string Palantir { get; set; }
    }
    public class PalantirRegularEntity : PalantirEntity
    {
    }
    public class PalantirNightlyEntity : PalantirEntity
    {
    }
    public class SpritesEntity
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public int Cost { get; set; }
        public bool Special { get; set; }
        public bool Rainbow { get; set; }
        public int EventDropID { get; set; }
        public string? Artist { get; set; }
    }
    public class SceneEntity
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string URL { get; set; }
        public string Color { get; set; }
        public string GuessedColor { get; set; }
        public string Artist { get; set; }
        public int EventID { get; set; }
        public bool Exclusive { get; set; }
    }
    public class ReportEntity
    {
        public string LobbyID { get; set; }
        public string ObserveToken { get; set; }
        public string Report { get; set; }
        public string Date { get; set; }
    }
    public class LobbyEntity
    {
        [Key]
        public string LobbyID { get; set; }
        public string Lobby { get; set; }
    }
    public class GuildLobbiesEntity
    {
        [Key]
        public string GuildID { get; set; }
        public string Lobbies { get; set; }
    }
    public class GuildSettingsEntity
    {
        [Key]
        public string GuildID { get; set; }
        public string Settings { get; set; }
    }
    public class StatusEntity
    {
        [Key]
        public string SessionID { get; set; }
        public string Status { get; set; }
        public string Date { get; set; }
    }

    public class OnlineSpritesEntity
    {
        [Key]
        public string ID { get; set; }
        public string LobbyKey { get; set; }
        public string LobbyPlayerID { get; set; }
        public string Sprite { get; set; }
        public string Date { get; set; }
        public int Slot { get; set; }
    }

    public class OnlineItemsEntity
    {
        public string ItemType { get; set; }
        public int Slot { get; set; }
        public int ItemID { get; set; }
        public string LobbyKey { get; set; }
        public string LobbyPlayerID { get; set; }
        public long Date { get; set; }
    }

    public class DropEntity
    {
        [Key]
        public string DropID { get; set; }
        public string CaughtLobbyPlayerID { get; set; }
        public string CaughtLobbyKey { get; set; }
        public string ValidFrom { get; set; }
        public int EventDropID { get; set; }
        public int LeagueWeight { get; set; }
    }
    public class PastDropEntity
    {
        public string DropID { get; set; }
        public string CaughtLobbyPlayerID { get; set; }
        public string CaughtLobbyKey { get; set; }
        public string ValidFrom { get; set; }
        public int EventDropID { get; set; }
        public int LeagueWeight { get; set; }
    }

    public class BubbleTraceEntity
    {
        [Key]
        public int ID { get; set; }
        public string Date { get; set; }
        public string Login { get; set; }
        public int Bubbles { get; set; }
    }
    public class BoostEntity
    {
        [Key]
        public int Login { get; set; }
        public double StartUTCS { get; set; }
        public int DurationS { get; set; }
        public double Factor { get; set; }
        public int CooldownBonusS { get; set; }
    }

    public class EventEntity
    {
        [Key]
        public int EventID { get; set; }
        public string EventName { get; set; }
        public int DayLength { get; set; }
        public string ValidFrom { get; set; }
        public string Description { get; set; }
    }

    public class EventDropEntity
    {
        [Key]
        public int EventDropID { get; set; }
        public int EventID { get; set; }
        public string URL { get; set; }
        public string Name { get; set; }
    }

    public class EventCreditEntity
    {
        public string Login { get; set; }
        public int EventDropID { get; set; }
        public int Credit { get; set; }
    }
    public class TypoThemeEntity
    {
        [Key]
        public string Ticket { get; set; }
        public string Theme { get; set; }
        public string? ThumbnailLanding { get; set; }
        public string? ThumbnailGame { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
    }

    public class WebhookEntity
    {
        public string ServerID { get; set; }
        public string Name { get; set; }
        public string WebhookURL { get; set; }
    }

    public class BoostSplitEntity
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public int Value { get; set; }
    }

    public class SplitCreditEntity
    {
        public int Login { get; set; }
        public int Split { get; set; }
        public string RewardDate { get; set; }
        public string Comment { get; set; }
        public int ValueOverride { get; set; }
    }

    public class SpriteProfileEntity
    {
        public string Login { get; set; }
        public string Name { get; set; }
        public string Combo { get; set; }
        public string RainbowSprites { get; set; }
        public string Scene { get; set; }
    }
}
