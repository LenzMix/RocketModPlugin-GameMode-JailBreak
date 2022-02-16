using Rocket.API;
using System.Collections.Generic;
using UnityEngine;

namespace SDJailMode
{
    public class Config : IDefaultable, IRocketPluginConfiguration
    {
        public void LoadDefaults()
        {
            this.multiple = 4;
            this.minPlayers = 3;
            this.timerForStart = 10;
            this.timerForPreGame = 10;
            this.timerForGame = 600;
            this.timerForEnd = 10;
            this.CTspawn = new List<Vector3>();
            this.Tspawn = new List<Vector3>();
            this.VoiceMicrophone = new List<Vector3>();
            this.Doors = new List<Vector3>();
            this.Lobby = new Vector3();
            this.MicrophoneRadius = 5f;
            this.MoneyTWin_T = 10;
            this.MoneyTWin_CT = 10;
            this.MoneyTWin_D = 10;
            this.MoneyCTWin_T = 10;
            this.MoneyCTWin_CT = 10;
            this.MoneyCTWin_D = 10;
            this.MoneyNoTime_T = 10;
            this.MoneyNoTime_CT = 10;
            this.MoneyNoTime_D = 10;
            this.MoneyCTkillT = 10;
            this.MoneyTkillCT = 10;
            this.Kits = new List<Kit>();
            this.StandartKitCT = "";
            this.StandartKitT = "";
            this.BlockedPlayersCT = new List<ulong>();
            this.ItemPoints = new List<ItemPoint>();
            this.UIID = 23111;
            this.UIKey = 23111;
            this.VKLink = "https://";
            this.DiscordLink = "https://";
            this.ShopLink = "https://";
            this.WelcomeText = "Я не пидор\nЯ НАТУРАЛ";
        }

        public int multiple;
        public int minPlayers;
        public int timerForStart;
        public int timerForPreGame;
        public int timerForGame;
        public int timerForEnd;
        public string StandartKitCT;
        public string StandartKitT;
        public string DiscordLink;
        public string VKLink;
        public string ShopLink;
        public string WelcomeText;
        public decimal MoneyTWin_T;
        public decimal MoneyTWin_CT;
        public decimal MoneyTWin_D;
        public decimal MoneyCTWin_T;
        public decimal MoneyCTWin_CT;
        public decimal MoneyCTWin_D;
        public decimal MoneyNoTime_T;
        public decimal MoneyNoTime_CT;
        public decimal MoneyNoTime_D;
        public decimal MoneyCTkillT;
        public decimal MoneyTkillCT;
        public float MicrophoneRadius;
        public List<Vector3> CTspawn;
        public List<Vector3> Tspawn;
        public List<Vector3> Doors;
        public List<Vector3> VoiceMicrophone;
        public Vector3 Lobby;
        public List<Kit> Kits;
        public List<ulong> BlockedPlayersCT;
        public List<ItemPoint> ItemPoints;
        public ushort UIID;
        public short UIKey;

    }

    public class Kit
    {
        public string name;
        public bool isGet;
        public bool isCT;
        public decimal Cost;
        public string permission;
        public List<ushort> items;
    }

    public class ItemPoint
    {
        public ushort id;
        public Vector3 point;
    }
}