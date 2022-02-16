using fr34kyn01535.Uconomy;
using Rocket.API;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
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
    public class Plugin : RocketPlugin<Config>
    {
        public static Plugin Instance;
        public static JailMode mode = JailMode.NotReady;
        public static Coroutine timer = null;
        public static Events events = new Events();
        public static ModuleDoor doors = new ModuleDoor();
        public static InventoryHelper inventoryhelper = new InventoryHelper();
        public static List<ulong> PriorityCT = new List<ulong>();

        protected override void Load()
        {
            Instance = this;
            Logger.Log("------------------------------------------------------------", System.ConsoleColor.Blue);
            Logger.Log("|                                                          |", System.ConsoleColor.Blue);
            Logger.Log("|                       Order Plugins                      |", System.ConsoleColor.Blue);
            Logger.Log("|                SodaDevs: SD JailBreak Mode               |", System.ConsoleColor.Blue);
            Logger.Log("|                                                          |", System.ConsoleColor.Blue);
            Logger.Log("------------------------------------------------------------", System.ConsoleColor.Blue);
            Logger.Log("Version: " + Assembly.GetName().Version, System.ConsoleColor.Blue);
            U.Events.OnPlayerConnected += OnConnect;
            U.Events.OnPlayerDisconnected += OnDisconnect;
            UnturnedPlayerEvents.OnPlayerDeath += OnDeath;
            UnturnedPlayerEvents.OnPlayerUpdatePosition += OnPosition;
            UnturnedPlayerEvents.OnPlayerUpdateGesture += doors.onGesture;
            PlayerAnimator.OnLeanChanged_Global += OnLean;
            EffectManager.onEffectButtonClicked += OnButton;
            DamageTool.onPlayerAllowedToDamagePlayer += OnDamage;
        }

        private void OnDamage(Player instigator, Player victim, ref bool isAllowed)
        {
            if (victim.GetComponent<PlayerComponent>().team == instigator.GetComponent<PlayerComponent>().team && instigator.GetComponent<PlayerComponent>().team != Team.Prisoner) isAllowed = false;
        }

        public void CheckTeams()
        {
            int t = 0;
            int ct = 0;
            foreach (SteamPlayer player in Provider.clients)
            {
                if (player.player.GetComponent<PlayerComponent>().team == Team.Police) ct++;
                if (player.player.GetComponent<PlayerComponent>().team == Team.Prisoner) t++;
            }
            foreach (SteamPlayer player in Provider.clients)
            {
                EffectManager.sendUIEffectText(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.playerID.steamID), true, "bar_score_t", t.ToString());
                EffectManager.sendUIEffectText(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.playerID.steamID), true, "bar_score_ct", ct.ToString());
            }
                
        }

        public event JailBreakOnDeath JBOnDeath;
        public delegate void JailBreakOnDeath(UnturnedPlayer player, Team team, CSteamID killer);

        public event JailBreakEndGame JBOnEnd;
        public delegate void JailBreakEndGame(EndReason reason);
        internal void JBOnEndFunc(EndReason reason)
        {
            JBOnEnd(reason);
        }

        public event JailBreakStartGame JBOnStart;
        public delegate void JailBreakStartGame();
        internal void JBOnStartFunc()
        {
            JBOnStart();
        }

        private void OnLean(PlayerAnimator obj)
        {
            if (obj.player.GetComponent<PlayerComponent>().team != Team.Death) return;
            EffectManager.sendUIEffectVisibility(Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(obj.player.channel.owner.playerID.steamID), true, "menu", true);
            obj.player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);
        }

        private void OnButton(Player player, string buttonName)
        {
            // Menu Kit Pregame
            if (mode == JailMode.Pregame)
            {
                if (buttonName == "kits_close")
                {
                    EffectManager.sendUIEffectVisibility(Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.channel.owner.playerID.steamID), true, "kits", false);
                    player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                    return;
                }
                if (buttonName.Contains("kit_"))
                {
                    List<Kit> kits = Instance.Configuration.Instance.Kits;
                    kits.RemoveAll(x => x.isGet == false);
                    int count = 0;
                    for (int i = 4; i < buttonName.Length; i++)
                        count = count * 10 + Convert.ToInt32(buttonName[i].ToString());
                    if (kits.Count <= count)
                    {
                        return;
                    }
                    Kit kit = kits[count];
                    if (ModuleKits.GetKit(UnturnedPlayer.FromPlayer(player), kit.name))
                    {
                        EffectManager.sendUIEffectVisibility(Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.channel.owner.playerID.steamID), true, "kits", false);
                        player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                    }
                    return;
                }
            }
            
            // Menu
            if (buttonName == "menu_close")
            {
                EffectManager.sendUIEffectVisibility(Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.channel.owner.playerID.steamID), true, "menu", false);
                player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                return;
            }
            if (buttonName == "menu_vk")
            { 
                player.sendBrowserRequest("Группа ВКонтакте", Instance.Configuration.Instance.VKLink);
                EffectManager.sendUIEffectVisibility(Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.channel.owner.playerID.steamID), true, "menu", false);
                player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                return;
            }
            if (buttonName == "menu_discord")
            {
                player.sendBrowserRequest("Группа Discord", Instance.Configuration.Instance.DiscordLink);
                EffectManager.sendUIEffectVisibility(Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.channel.owner.playerID.steamID), true, "menu", false);
                player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                return;
            }
            if (buttonName == "menu_shop")
            {
                player.sendBrowserRequest("Автоматический магазин", Instance.Configuration.Instance.ShopLink);
                EffectManager.sendUIEffectVisibility(Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.channel.owner.playerID.steamID), true, "menu", false);
                player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                return;
            }
            if (buttonName == "menu_ctwant")
            {
                if (Instance.Configuration.Instance.BlockedPlayersCT.Contains(player.channel.owner.playerID.steamID.m_SteamID))
                {
                    UnturnedChat.Say(UnturnedPlayer.FromPlayer(player), Translate("CTWant_Blocked"), Color.yellow);
                    EffectManager.sendUIEffectVisibility(Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.channel.owner.playerID.steamID), true, "menu", false);
                    player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                    return;
                }
                if (PriorityCT.Contains(player.channel.owner.playerID.steamID.m_SteamID))
                {
                    UnturnedChat.Say(UnturnedPlayer.FromPlayer(player), Translate("CTWant_Already"), Color.yellow);
                    EffectManager.sendUIEffectVisibility(Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.channel.owner.playerID.steamID), true, "menu", false);
                    player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                    return;
                }
                UnturnedChat.Say(UnturnedPlayer.FromPlayer(player), Translate("CTWant_Success"), Color.yellow);
                PriorityCT.Add(player.channel.owner.playerID.steamID.m_SteamID);
                EffectManager.sendUIEffectVisibility(Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.channel.owner.playerID.steamID), true, "menu", false);
                player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                return;
            }
            
            // Main Info
            if (buttonName == "main_close")
            {
                EffectManager.sendUIEffectVisibility(Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.channel.owner.playerID.steamID), true, "main", false);
                player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                return;
            }
        }

        private void OnPosition(UnturnedPlayer player, Vector3 position)
        {
            if (Instance.Configuration.Instance.VoiceMicrophone.Exists(x => Vector3.Distance(x, player.Position) < Instance.Configuration.Instance.MicrophoneRadius) && !player.GetComponent<PlayerComponent>().isVoice)
            {
                player.GetComponent<PlayerComponent>().isVoice = true;
                UnturnedChat.Say(player, Translate("Voice_ON"), Color.yellow);
            }
            if (!Instance.Configuration.Instance.VoiceMicrophone.Exists(x => Vector3.Distance(x, player.Position) < Instance.Configuration.Instance.MicrophoneRadius) && player.GetComponent<PlayerComponent>().isVoice)
            {
                player.GetComponent<PlayerComponent>().isVoice = false;
                UnturnedChat.Say(player, Translate("Voice_OFF"), Color.yellow);
            }
        }

        private void OnDisconnect(UnturnedPlayer player)
        {
            PriorityCT.RemoveAll(x => x == player.CSteamID.m_SteamID);
            if (mode == JailMode.Game) CheckTeams();
            if (mode == JailMode.Game) CheckFinishGame();
        }

        private void OnDeath(UnturnedPlayer player, EDeathCause cause, ELimb limb, CSteamID murderer)
        {
            if (mode == JailMode.Game)
            {
                JBOnDeath(player, player.GetComponent<PlayerComponent>().team, murderer);
                try
                {
                    if (player.GetComponent<PlayerComponent>().team != Team.Death && murderer != null && player.GetComponent<PlayerComponent>().team != UnturnedPlayer.FromCSteamID(murderer).GetComponent<PlayerComponent>().team)
                    {
                        if (UnturnedPlayer.FromCSteamID(murderer).GetComponent<PlayerComponent>().team == Team.Police) Uconomy.Instance.Database.IncreaseBalance(player.CSteamID.m_SteamID.ToString(), Instance.Configuration.Instance.MoneyCTkillT);
                        if (UnturnedPlayer.FromCSteamID(murderer).GetComponent<PlayerComponent>().team == Team.Prisoner) Uconomy.Instance.Database.IncreaseBalance(player.CSteamID.m_SteamID.ToString(), Instance.Configuration.Instance.MoneyTkillCT);
                    }
                }
                catch { }
                player.GetComponent<PlayerComponent>().team = Team.Death;
                CheckTeams();
                CheckFinishGame();
            }
        }

        public void CheckFinishGame()
        {
            if (mode != JailMode.Game) return;
            //if (Provider.clients.Count <= 0) return;
            StopCoroutine(timer);
            if (!Provider.clients.Exists(x => x.player.GetComponent<PlayerComponent>().team == Team.Police)) timer = StartCoroutine(events.StopPlayRutine(EndReason.T));
            else if (!Provider.clients.Exists(x => x.player.GetComponent<PlayerComponent>().team == Team.Prisoner)) timer = StartCoroutine(events.StopPlayRutine(EndReason.CT));
            else timer = StartCoroutine(events.StopPlayRutine(EndReason.NoPlayers));
        }

        private void OnConnect(UnturnedPlayer player)
        {
            player.GetComponent<PlayerComponent>().team = Team.Death;
            player.Teleport(Instance.Configuration.Instance.Lobby, 0);
            if (mode == JailMode.NotReady)
                CheckStart();
            EffectManager.sendUIEffect(Instance.Configuration.Instance.UIID, Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.CSteamID), true, Instance.Configuration.Instance.WelcomeText);
            player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);
            ModuleKits.GenerateList(player);
            if (mode == JailMode.Finish)
            {
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.CSteamID), true, "bar_info", true);
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.CSteamID), true, "bar_score", false);
            }
            else
            if (mode == JailMode.Game)
            {
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.CSteamID), true, "bar_info", false);
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.CSteamID), true, "bar_score", true);

            }
            else
            if (mode == JailMode.LobbyStart)
            {
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.CSteamID), true, "bar_info", true);
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.CSteamID), true, "bar_score", false);
            }
            else
            if (mode == JailMode.NotReady)
            {
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.CSteamID), true, "bar_info", true);
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.CSteamID), true, "bar_score", false);
            }
            else
            if (mode == JailMode.Pregame)
            {
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.CSteamID), true, "bar_info", true);
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.CSteamID), true, "bar_score", false);
            }
        }

        public void CheckStart()
        {
            if (Provider.clients.Count < Instance.Configuration.Instance.minPlayers)
                foreach (SteamPlayer player in Provider.clients) UnturnedChat.Say(UnturnedPlayer.FromSteamPlayer(player), Translate("WaitPlayers", (Instance.Configuration.Instance.minPlayers - Provider.clients.Count).ToString()), Color.yellow);
            else
                StartGame();

        }

        private void StartGame()
        {
            if (Provider.clients.Count < Instance.Configuration.Instance.minPlayers) return;
            if (Provider.clients.Count > 0) foreach (SteamPlayer player in Provider.clients) UnturnedChat.Say(UnturnedPlayer.FromSteamPlayer(player), Translate("StartGame"), Color.yellow);
            StopCoroutine(timer);
            timer = StartCoroutine(events.StartGameRutine());
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList{
                    {"StartGame", "На сервер зашло достаточно игроков! Игра начинается"},
                    {"WaitPlayers", "Для запуска сервера требуется ещё {0} игроков"},
                    {"YouCT", "Вы играете за Надзирателя!"},
                    {"YouT", "Вы играете за Заключённый!"},
                    {"WaitPlayers", "Для запуска сервера требуется ещё {0} игроков"},
                    {"Voice_ON", "Вы стоите у микрофона! Ваш голос слышат все игроки"},
                    {"Voice_OFF", "Вы отошли от микрофона!"},
                    {"CTWant_Blocked", "Вам запрещено играть за Надзирателей"},
                    {"CTWant_Already", "Вы уже находитесь в очереди на игру за Надзирателей"},
                    {"CTWant_Success", "Вы добавлены в очередь на игру за Надзирателей"},
                    {"bar_start", "Игра начнётся через {0} секунд"},
                    {"bar_prepare", "Подготовка {0} секунд"},
                    {"bar_timer", "{0}:{1}"},
                    {"bar_start", "Игра начнётся через {0} секунд"},
                    {"bar_endtime", "Игра закончилась - Время вышло"},
                    {"bar_endct", "Игра закончилась - Надзиратели победили"},
                    {"bar_endt", "Игра закончилась - Заключённые победили"},
                    {"NoTimeDeath", "Время закончилось. Вы получили: {0}$"},
                    {"NoTimeCT", "Время закончилось. Вы получили: {0}$"},
                    {"NoTimeT", "Время закончилось. Вы получили: {0}$"},
                    {"NoTime", "Время закончилось"},
                    {"CTWinDeath", "Надзиратели победили. Вы получили: {0}$"},
                    {"CTWinCT", "Надзиратели победили. Вы получили: {0}$"},
                    {"CTWinT", "Надзиратели победили. Вы получили: {0}$"},
                    {"TWinDeath", "Заключённые победили. Вы получили: {0}$"},
                    {"TWinCT", "Заключённые победили. Вы получили: {0}$"},
                    {"TWinT", "Заключённые победили. Вы получили: {0}$"},
                    {"KitsNoTeam", "Данный набор недоступен за данную команду"},
                    {"KitsNotMoney", "У вас недостаточно денежных средств для данного набора"},
                    {"KitsNotPerm", "Данный набор недоступен вам"},
                    {"KitsNotExists", "Данный набор несуществует"},
                    {"KitsGet", "Вы получили набор '{0}'"},
                };
            }
        }
    }

    public class PlayerComponent : UnturnedPlayerComponent
    {
        protected override void Load()
        {
            this.team = Team.Death;
            this.isVoice = false;
        }

        public Team team;
        public bool isVoice;
    }
}
