using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SDJailMode
{
    class CommandAddTSpawn : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "addTspawn";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string>
                {
                    "addTspawn"
                };

        public List<string> Permissions => new List<string>
                {
                    "addTspawn"
                };


        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer UP = (UnturnedPlayer)caller;
            Plugin.Instance.Configuration.Instance.Tspawn.Add(UP.Position);
            Plugin.Instance.Configuration.Save();
            UnturnedChat.Say(UP, "Позиция Т спавна добавлена");
        }
    }
    class CommandAddCTSpawn : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "addCTspawn";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string>
                {
                    "addCTspawn"
                };

        public List<string> Permissions => new List<string>
                {
                    "addCTspawn"
                };


        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer UP = (UnturnedPlayer)caller;
            Plugin.Instance.Configuration.Instance.CTspawn.Add(UP.Position);
            Plugin.Instance.Configuration.Save();
            UnturnedChat.Say(UP, "Позиция CТ спавна добавлена");
        }
    }
    class CommandAddLobby : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "addLobby";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string>
                {
                    "addLobby"
                };

        public List<string> Permissions => new List<string>
                {
                    "addLobby"
                };


        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer UP = (UnturnedPlayer)caller;
            Plugin.Instance.Configuration.Instance.Lobby = UP.Position;
            Plugin.Instance.Configuration.Save();
            UnturnedChat.Say(UP, "Позиция Lobby установлена");
        }
    }
    class CommandAddItem : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "addItem";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string>
                {
                    "addItem"
                };

        public List<string> Permissions => new List<string>
                {
                    "addItem"
                };


        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer UP = (UnturnedPlayer)caller;
            if (command.Length < 1)
            {
                UnturnedChat.Say(UP, "Введи ID предмета");
                return;
            }
            ushort id = 0;
            try
            {
                id = Convert.ToUInt16(command[0]);
            }
            catch
            {
                return;
            }
            Plugin.Instance.Configuration.Instance.ItemPoints.Add(new ItemPoint
            {
                id = id,
                point = UP.Position
            });
            Plugin.Instance.Configuration.Save();
            UnturnedChat.Say(UP, "Позиция предмета установлена");
        }
    }
    class CommandAddDoor : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "addDoor";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string>
                {
                    "addDoor"
                };

        public List<string> Permissions => new List<string>
                {
                    "addDoor"
                };


        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer UP = (UnturnedPlayer)caller;
            Transform raycast = ModuleDoor.GetBarricadeTransform(UP.Player, out BarricadeData data, out BarricadeDrop drop);
            if (raycast == null)
            {
                UnturnedChat.Say(UP, "Смотрите на дверь");
                return;
            }
            if (Plugin.Instance.Configuration.Instance.Doors.Contains(data.point))
            {
                UnturnedChat.Say(UP, "Дверь уже добавлена");
                return;
            }
            Plugin.Instance.Configuration.Instance.Doors.Add(data.point);
            Plugin.Instance.Configuration.Save();
            UnturnedChat.Say(UP, "Дверь добавлена");
        }
    }
    class CommandAddKit : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "addKit";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string>
                {
                    "addKit"
                };

        public List<string> Permissions => new List<string>
                {
                    "addKit"
                };


        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer UP = (UnturnedPlayer)caller;
            if (command.Length < 1)
            {
                UnturnedChat.Say(UP, "Введи название набора");
                return;
            }
            if (Plugin.Instance.Configuration.Instance.Kits.Exists(x => x.name.ToLower() == command[0].ToLower()))
            {
                UnturnedChat.Say(UP, "Набор с таким названием уже существует");
                return;
            }
            List<ushort> items = new List<ushort>();

            for (byte page = 0; page < 7; page++)
            {
                if (UP.Inventory.items[page].getItemCount() > 0)
                    foreach (ItemJar item in UP.Inventory.items[page].items)
                    {
                        if (item != null)
                        {
                            items.Add(item.item.id);
                        }
                    }
            }
            Plugin.Instance.Configuration.Instance.Kits.Add(new Kit
            {
                Cost = 0,
                isCT = false,
                isGet = false,
                items = items,
                name = command[0],
                permission = "kit." + command[0],

            });
            Plugin.Instance.Configuration.Save();
            UnturnedChat.Say(UP, "Набор добавлен. Для настройки требуется посидеть в CONFIG'е");
        }
    }
    class CommandDoorOpen : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "dooropen";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string>
                {
                    "do"
                };

        public List<string> Permissions => new List<string>
                {
                    "dooropen"
                };


        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer UP = (UnturnedPlayer)caller;
            if (UP.GetComponent<PlayerComponent>().team != Team.Police)
            {
                UnturnedChat.Say(UP, "Вы не можете управлять дверьми!", Color.yellow);
                return;
            }
            ModuleDoor.ChangeDoors(true);
            UnturnedChat.Say(UP, "Все двери открыты", Color.yellow);
        }
    }
    class CommandDoorClose : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;

        public string Name => "doorclose";

        public string Help => "";

        public string Syntax => "";

        public List<string> Aliases => new List<string>
                {
                    "dc"
                };

        public List<string> Permissions => new List<string>
                {
                    "doorclose"
                };


        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer UP = (UnturnedPlayer)caller;
            if (UP.GetComponent<PlayerComponent>().team != Team.Police)
            {
                UnturnedChat.Say(UP, "Вы не можете управлять дверьми!", Color.yellow);
                return;
            }
            ModuleDoor.ChangeDoors(true);
            UnturnedChat.Say(UP, "Все двери закрыты", Color.yellow);
        }
    }
}
