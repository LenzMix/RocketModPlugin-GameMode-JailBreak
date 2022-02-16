using fr34kyn01535.Uconomy;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = Rocket.Core.Logging.Logger;

namespace SDJailMode
{
    public class Events
    {
        public IEnumerator StartGameRutine()
        {
            Plugin.mode = JailMode.LobbyStart;
            int timer = Plugin.Instance.Configuration.Instance.timerForStart;
            foreach (SteamPlayer player in Provider.clients)
            {
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.playerID.steamID), true, "bar_info", true);
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.playerID.steamID), true, "bar_score", false);
            }
            while (timer >= 0)
            {
                foreach (SteamPlayer player in Provider.clients) EffectManager.sendUIEffectText(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.playerID.steamID), true, "bar_info_text", Plugin.Instance.Translate("bar_start", timer));
                timer--;
                yield return new WaitForSeconds(1f);
            }
            if (Provider.clients.Count < Plugin.Instance.Configuration.Instance.minPlayers)
            {
                Plugin.timer = null;
                if (Provider.clients.Count > 0) foreach (SteamPlayer player in Provider.clients) UnturnedChat.Say(UnturnedPlayer.FromSteamPlayer(player), Plugin.Instance.Translate("NoPlayers", (Plugin.Instance.Configuration.Instance.minPlayers - Provider.clients.Count).ToString()), Color.yellow);
                Plugin.mode = JailMode.NotReady;
                Plugin.timer = null;
                yield break;
                //yield return null;
            }
            Plugin.timer = Plugin.Instance.StartCoroutine(StartPreGameRutine());
            yield break;
        }

        public IEnumerator StartPreGameRutine()
        {
            Plugin.mode = JailMode.Pregame;
            //Logger.Log("1");
            int timer = Plugin.Instance.Configuration.Instance.timerForPreGame;
            int CTcount = Provider.clients.Count / Plugin.Instance.Configuration.Instance.multiple + 1;
            //Logger.Log("2 + " + CTcount.ToString());
            while (CTcount > 0 && Plugin.PriorityCT.Count > 0)
            {
                //Logger.Log("3");
                if (Plugin.Instance.Configuration.Instance.BlockedPlayersCT.Contains(Plugin.PriorityCT[0]))
                {
                    Plugin.PriorityCT.RemoveAt(0);
                    continue;
                }
                //Logger.Log("4");
                Player player = UnturnedPlayer.FromCSteamID(new CSteamID(Plugin.PriorityCT[0])).Player;
                //Logger.Log("5");
                player.GetComponent<PlayerComponent>().team = Team.Police;
                //Logger.Log("6");
                int randSpawn = new System.Random().Next(0, Plugin.Instance.Configuration.Instance.CTspawn.Count);
                //Logger.Log("7 + " + randSpawn.ToString());
                UnturnedPlayer.FromPlayer(player).Teleport(Plugin.Instance.Configuration.Instance.CTspawn[randSpawn], 0);
                //Logger.Log("8");
                UnturnedChat.Say(UnturnedPlayer.FromPlayer(player), Plugin.Instance.Translate("YouCT"), Color.yellow);
                //Logger.Log("9");
                player.movement.sendPluginSpeedMultiplier(0);
                //Logger.Log("10");
                //ModuleKits.GetKit(UnturnedPlayer.FromPlayer(player), Plugin.Instance.Configuration.Instance.StandartKitCT);
                //Logger.Log("11");
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.channel.owner.playerID.steamID), true, "menu", false);
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.channel.owner.playerID.steamID), true, "main", false);
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.channel.owner.playerID.steamID), true, "kits", true);
                EffectManager.sendUIEffectText(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.channel.owner.playerID.steamID), true, "kits_team", Plugin.Instance.Translate("YouCT"));
                //Logger.Log("13");
                player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);
                //Logger.Log("14");
                Plugin.PriorityCT.RemoveAt(0);
                //Logger.Log("15");
                CTcount--;
                //Logger.Log("16");
            }
            List<ulong> ps = new List<ulong>();
            //Logger.Log("17");
            foreach (SteamPlayer player in Provider.clients) ps.Add(player.playerID.steamID.m_SteamID);
            //Logger.Log("18");
            ps.RemoveAll(x => UnturnedPlayer.FromCSteamID(new CSteamID(x)).Player.life.isDead == true || Plugin.Instance.Configuration.Instance.BlockedPlayersCT.Contains(x) || UnturnedPlayer.FromCSteamID(new CSteamID(x)).GetComponent<PlayerComponent>().team != Team.Death);
            //Logger.Log("19 + " + ps.Count);
            if (ps.Count > 0)for (int i = 0; i < CTcount; i++)
            {
                //Logger.Log("20");
                int randPlayer = new System.Random().Next(0, ps.Count);
                //Logger.Log("21 + " + randPlayer);
                UnturnedPlayer.FromCSteamID(new CSteamID(ps[randPlayer])).GetComponent<PlayerComponent>().team = Team.Police;
                //Logger.Log("22");
                int randSpawn = new System.Random().Next(0, Plugin.Instance.Configuration.Instance.CTspawn.Count);
                //Logger.Log("23 + " + randSpawn);
                UnturnedPlayer.FromCSteamID(new CSteamID(ps[randPlayer])).Teleport(Plugin.Instance.Configuration.Instance.CTspawn[randSpawn], 0);
                //Logger.Log("24");
                UnturnedChat.Say(UnturnedPlayer.FromCSteamID(new CSteamID(ps[randPlayer])), Plugin.Instance.Translate("YouCT"), Color.yellow);
                //Logger.Log("25");
                UnturnedPlayer.FromCSteamID(new CSteamID(ps[randPlayer])).Player.movement.sendPluginSpeedMultiplier(0);
                //Logger.Log("26");
                //ModuleKits.GetKit(UnturnedPlayer.FromCSteamID(new CSteamID(ps[randPlayer])), Plugin.Instance.Configuration.Instance.StandartKitCT);
                //Logger.Log("27");
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(new CSteamID(ps[randPlayer])), true, "menu", false);
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(new CSteamID(ps[randPlayer])), true, "main", false);
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(new CSteamID(ps[randPlayer])), true, "kits", true);
                EffectManager.sendUIEffectText(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(new CSteamID(ps[randPlayer])), true, "kits_team", Plugin.Instance.Translate("YouCT"));
                //Logger.Log("28");
                UnturnedPlayer.FromCSteamID(new CSteamID(ps[randPlayer])).Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);
                //Logger.Log("29");
                ps.RemoveAt(randPlayer);
                //Logger.Log("30");
            }
            ps = new List<ulong>();
            foreach (SteamPlayer player in Provider.clients) ps.Add(player.playerID.steamID.m_SteamID);
            ps.RemoveAll(x => UnturnedPlayer.FromCSteamID(new CSteamID(x)).Player.life.isDead == true || UnturnedPlayer.FromCSteamID(new CSteamID(x)).GetComponent<PlayerComponent>().team != Team.Death);
            //Logger.Log("31 + " + ps.Count);
            if (ps.Count > 0) foreach (ulong player in ps)
                {
                    //Logger.Log("32");
                    UnturnedPlayer.FromCSteamID(new CSteamID(player)).GetComponent<PlayerComponent>().team = Team.Prisoner;
                    //Logger.Log("33");
                    int randSpawn = new System.Random().Next(0, Plugin.Instance.Configuration.Instance.Tspawn.Count);
                    //Logger.Log("34 + " + randSpawn);
                    UnturnedPlayer.FromCSteamID(new CSteamID(player)).Teleport(Plugin.Instance.Configuration.Instance.Tspawn[randSpawn], 0);
                    Logger.Log("35");
                    UnturnedChat.Say(UnturnedPlayer.FromCSteamID(new CSteamID(player)), Plugin.Instance.Translate("YouT"), Color.yellow);
                    Logger.Log("36");
                    //ModuleKits.GetKit(UnturnedPlayer.FromCSteamID(new CSteamID(player)), Plugin.Instance.Configuration.Instance.StandartKitT);
                    //Logger.Log("37");
                    UnturnedPlayer.FromCSteamID(new CSteamID(player)).Player.movement.sendPluginSpeedMultiplier(0);
                    //Logger.Log("38");
                    EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(new CSteamID(player)), true, "menu", false);
                    EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(new CSteamID(player)), true, "main", false);
                    EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(new CSteamID(player)), true, "kits", true);
                    UnturnedPlayer.FromCSteamID(new CSteamID(player)).Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);
                    EffectManager.sendUIEffectText(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(new CSteamID(player)), true, "kits_team", Plugin.Instance.Translate("YouT"));
                    //Logger.Log("39");
                }
            foreach (SteamPlayer player in Provider.clients)
            {
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.playerID.steamID), true, "bar_info", true);
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.playerID.steamID), true, "bar_score", false);
            }
            while (timer > 0)
            {
                timer--;
                if (Provider.clients.Count > 0) foreach (SteamPlayer player in Provider.clients) EffectManager.sendUIEffectText(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.playerID.steamID), true, "bar_info_text", Plugin.Instance.Translate("bar_prepare", timer));
                yield return new WaitForSeconds(1f);
            }
            ModuleItemSpawn.SpawnItems();
            Plugin.timer = Plugin.Instance.StartCoroutine(StartPlayRutine());
            yield break;
        }

        public IEnumerator StartPlayRutine()
        {
            Plugin.mode = JailMode.Game;
            int timer = Plugin.Instance.Configuration.Instance.timerForGame;
            Plugin.Instance.JBOnStartFunc();
            Plugin.Instance.CheckTeams();
            if (Provider.clients.Count > 0) foreach (SteamPlayer player in Provider.clients)
                {
                    player.player.movement.sendPluginSpeedMultiplier(1);
                    EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.playerID.steamID), true, "kits", false);
                    player.player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                }
            Plugin.Instance.CheckFinishGame();
            foreach (SteamPlayer player in Provider.clients)
            {
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.playerID.steamID), true, "bar_info", false);
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.playerID.steamID), true, "bar_score", true);
            }
            while (timer >= 0)
            {
                timer--;
                    if (Provider.clients.Count > 0) foreach (SteamPlayer player in Provider.clients)
                    {
                        //UnturnedChat.Say(UnturnedPlayer.FromSteamPlayer(player), Plugin.Instance.Translate("timer", (timer).ToString()), Color.yellow);
                        EffectManager.sendUIEffectText(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.playerID.steamID), true, "bar_score_text", Plugin.Instance.Translate("bar_timer", (timer / 60).ToString("00"), (timer % 60).ToString("00")));
                    }
                yield return new WaitForSeconds(1f);
            }
            Plugin.timer = Plugin.Instance.StartCoroutine(StopPlayRutine(EndReason.Time));
            yield break;
        }

        public IEnumerator StopPlayRutine(EndReason reason)
        {
            Plugin.mode = JailMode.Finish;
            int timer = Plugin.Instance.Configuration.Instance.timerForEnd;
            foreach (SteamPlayer player in Provider.clients)
            {
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.playerID.steamID), true, "bar_info", true);
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.playerID.steamID), true, "bar_score", false);
            }
            Plugin.Instance.JBOnEndFunc(reason);
            if (reason == EndReason.Time)
            {
                UnturnedChat.Say(Plugin.Instance.Translate("NoTime"));
                if (Provider.clients.Count > 0) foreach (SteamPlayer player in Provider.clients)
                {
                    if (player.player.GetComponent<PlayerComponent>().team == Team.Death)
                    {
                        Uconomy.Instance.Database.IncreaseBalance(player.playerID.steamID.m_SteamID.ToString(), Plugin.Instance.Configuration.Instance.MoneyNoTime_D);
                        UnturnedChat.Say(player.playerID.steamID, Plugin.Instance.Translate("NoTimeDeath", Plugin.Instance.Configuration.Instance.MoneyNoTime_D.ToString()), Color.yellow);
                    }
                    if (player.player.GetComponent<PlayerComponent>().team == Team.Police)
                    {
                        Uconomy.Instance.Database.IncreaseBalance(player.playerID.steamID.m_SteamID.ToString(), Plugin.Instance.Configuration.Instance.MoneyNoTime_CT);
                        UnturnedChat.Say(player.playerID.steamID, Plugin.Instance.Translate("NoTimePolice", Plugin.Instance.Configuration.Instance.MoneyNoTime_CT.ToString()), Color.yellow);
                    }
                    if (player.player.GetComponent<PlayerComponent>().team == Team.Prisoner)
                    {
                        Uconomy.Instance.Database.IncreaseBalance(player.playerID.steamID.m_SteamID.ToString(), Plugin.Instance.Configuration.Instance.MoneyNoTime_T);
                        UnturnedChat.Say(player.playerID.steamID, Plugin.Instance.Translate("NoTimePrisoner", Plugin.Instance.Configuration.Instance.MoneyNoTime_T.ToString()), Color.yellow);
                    }
                    EffectManager.sendUIEffectText(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.playerID.steamID), true, "bar_info_text", Plugin.Instance.Translate("bar_endtime", timer));
                }
            }
            else if (reason == EndReason.CT)
            {
                UnturnedChat.Say(Plugin.Instance.Translate("CTWin"));
                if (Provider.clients.Count > 0) foreach (SteamPlayer player in Provider.clients)
                {
                    if (player.player.GetComponent<PlayerComponent>().team == Team.Death)
                    {
                        Uconomy.Instance.Database.IncreaseBalance(player.playerID.steamID.m_SteamID.ToString(), Plugin.Instance.Configuration.Instance.MoneyCTWin_D);
                        UnturnedChat.Say(player.playerID.steamID, Plugin.Instance.Translate("CTWinDeath", Plugin.Instance.Configuration.Instance.MoneyCTWin_D.ToString()), Color.yellow);
                    }
                    if (player.player.GetComponent<PlayerComponent>().team == Team.Police)
                    {
                        Uconomy.Instance.Database.IncreaseBalance(player.playerID.steamID.m_SteamID.ToString(), Plugin.Instance.Configuration.Instance.MoneyCTWin_CT);
                        UnturnedChat.Say(player.playerID.steamID, Plugin.Instance.Translate("CTWinPolice", Plugin.Instance.Configuration.Instance.MoneyCTWin_CT.ToString()), Color.yellow);
                    }
                    if (player.player.GetComponent<PlayerComponent>().team == Team.Prisoner)
                    {
                        Uconomy.Instance.Database.IncreaseBalance(player.playerID.steamID.m_SteamID.ToString(), Plugin.Instance.Configuration.Instance.MoneyCTWin_T);
                        UnturnedChat.Say(player.playerID.steamID, Plugin.Instance.Translate("CTWinPrisoner", Plugin.Instance.Configuration.Instance.MoneyCTWin_T.ToString()), Color.yellow);
                    }
                    EffectManager.sendUIEffectText(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.playerID.steamID), true, "bar_info_text", Plugin.Instance.Translate("bar_endct", timer));
                }
            }
            else if (reason == EndReason.T)
            {
                UnturnedChat.Say(Plugin.Instance.Translate("TWin"));
                if (Provider.clients.Count > 0) foreach (SteamPlayer player in Provider.clients)
                {
                    if (player.player.GetComponent<PlayerComponent>().team == Team.Death)
                    {
                        Uconomy.Instance.Database.IncreaseBalance(player.playerID.steamID.m_SteamID.ToString(), Plugin.Instance.Configuration.Instance.MoneyTWin_D);
                        UnturnedChat.Say(player.playerID.steamID, Plugin.Instance.Translate("TWinDeath", Plugin.Instance.Configuration.Instance.MoneyTWin_D.ToString()), Color.yellow);
                    }
                    if (player.player.GetComponent<PlayerComponent>().team == Team.Police)
                    {
                        Uconomy.Instance.Database.IncreaseBalance(player.playerID.steamID.m_SteamID.ToString(), Plugin.Instance.Configuration.Instance.MoneyTWin_CT);
                        UnturnedChat.Say(player.playerID.steamID, Plugin.Instance.Translate("TWinPolice", Plugin.Instance.Configuration.Instance.MoneyTWin_CT.ToString()), Color.yellow);
                    }
                    if (player.player.GetComponent<PlayerComponent>().team == Team.Prisoner)
                    {
                        Uconomy.Instance.Database.IncreaseBalance(player.playerID.steamID.m_SteamID.ToString(), Plugin.Instance.Configuration.Instance.MoneyTWin_T);
                        UnturnedChat.Say(player.playerID.steamID, Plugin.Instance.Translate("TWinPrisoner", Plugin.Instance.Configuration.Instance.MoneyTWin_T.ToString()), Color.yellow);
                    }
                    EffectManager.sendUIEffectText(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.playerID.steamID), true, "bar_info_text", Plugin.Instance.Translate("bar_endt", timer));
                }
            }
            while (timer > 0)
            {
                timer--;
                yield return new WaitForSeconds(1f);
            }
            ModuleDoor.ChangeDoors(false);
            if (Provider.clients.Count > 0) foreach (SteamPlayer player in Provider.clients)
                {
                    if (player.player.GetComponent<PlayerComponent>().team == Team.Death) continue;
                    player.player.GetComponent<PlayerComponent>().team = Team.Death;
                    UnturnedPlayer.FromSteamPlayer(player).Teleport(Plugin.Instance.Configuration.Instance.Lobby, 0);
                    InventoryHelper.ClearItems(UnturnedPlayer.FromSteamPlayer(player));
                }
            ItemManager.askClearAllItems();
            Plugin.timer = Plugin.Instance.StartCoroutine(StartGameRutine());
            yield break;
        }
    }
}
