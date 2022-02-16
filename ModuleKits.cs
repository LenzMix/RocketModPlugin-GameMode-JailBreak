using fr34kyn01535.Uconomy;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDJailMode
{
    public static class ModuleKits
    {
        public static bool GetKit(UnturnedPlayer player, string name)
        {
            if (!Plugin.Instance.Configuration.Instance.Kits.Exists(x => x.name == name))
            {
                UnturnedChat.Say(player, Plugin.Instance.Translate("KitsNotExists"));
                return false;
            }
            Kit kit = Plugin.Instance.Configuration.Instance.Kits.Find(x => x.name == name);
            if (!player.HasPermission(kit.permission))
            {
                UnturnedChat.Say(player, Plugin.Instance.Translate("KitsNotPerm"));
                return false;
            }
            if (Uconomy.Instance.Database.GetBalance(player.CSteamID.m_SteamID.ToString()) < kit.Cost)
            {
                UnturnedChat.Say(player, Plugin.Instance.Translate("KitsNotMoney"));
                return false;
            }
            if ((player.GetComponent<PlayerComponent>().team == Team.Police && !kit.isCT) || (player.GetComponent<PlayerComponent>().team == Team.Prisoner && kit.isCT))
            {                                                                                                                  
                UnturnedChat.Say(player, Plugin.Instance.Translate("KitsNoTeam"));
                return false;
            }
            foreach (ushort id in kit.items) player.GiveItem(id, 1);
            UnturnedChat.Say(player, Plugin.Instance.Translate("KitsGet", kit.name));
            Uconomy.Instance.Database.IncreaseBalance(player.CSteamID.m_SteamID.ToString(), -kit.Cost);
            return true;
        }

        public static void GenerateList(UnturnedPlayer player)
        {
            List<Kit> kits = Plugin.Instance.Configuration.Instance.Kits;
            kits.RemoveAll(x => x.isGet);
            for (int i = 0; i < kits.Count; i++)
            {
                EffectManager.sendUIEffectText(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.CSteamID), true, "kitname_" + i.ToString(), kits[i].name);
                EffectManager.sendUIEffectText(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.CSteamID), true, "kitcost_" + i.ToString(), "Цена: " + kits[i].Cost.ToString() + "$");
                if (player.HasPermission(kits[i].permission)) EffectManager.sendUIEffectText(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.CSteamID), true, "kitavailable_" + i.ToString(), "<Color=Red>Недоступно</Color>");
                else EffectManager.sendUIEffectText(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.CSteamID), true, "kitavailable_" + i.ToString(), "<Color=Green>Доступно</Color>");
                if (kits[i].isCT) EffectManager.sendUIEffectText(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.CSteamID), true, "kitteam_" + i.ToString(), "Команда: Надзиратель");
                else EffectManager.sendUIEffectText(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.CSteamID), true, "kitteam_" + i.ToString(), "Команда: Заключённый");
                EffectManager.sendUIEffectVisibility(Plugin.Instance.Configuration.Instance.UIKey, Provider.findTransportConnection(player.CSteamID), true, "kit_" + i.ToString(), true);
            }
        }
    }
}
