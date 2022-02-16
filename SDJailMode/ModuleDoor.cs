using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Framework.Utilities;
using SDG.NetTransport;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SDJailMode
{
    public class ModuleDoor
    {
        public void onGesture(UnturnedPlayer player, UnturnedPlayerEvents.PlayerGesture gesture)
        {
            if (player.GetComponent<PlayerComponent>().team != Team.Police) return;
            try
            {
                Transform raycast = GetBarricadeTransform(player.Player, out BarricadeData data, out BarricadeDrop drop);
                if (raycast == null) return;
                if (!Plugin.Instance.Configuration.Instance.Doors.Exists(x => x == raycast.position)) return;
                if (gesture == UnturnedPlayerEvents.PlayerGesture.PunchLeft && raycast != null)
                    if (((InteractableDoor)drop.interactable) != null || ((InteractableDoorHinge)drop.interactable) != null) ExecuteOpen(player);
                if (gesture == UnturnedPlayerEvents.PlayerGesture.PunchRight && raycast != null)
                    if (((InteractableDoor)drop.interactable) != null || ((InteractableDoorHinge)drop.interactable) != null) ExecuteOpen(player);
            }
            catch { }
        }

        private void ExecuteOpen(UnturnedPlayer player)
        {
            Transform raycast = GetBarricadeTransform(player.Player, out BarricadeData data, out BarricadeDrop drop);
            if (raycast == null) return;
            if (!Plugin.Instance.Configuration.Instance.Doors.Exists(x => x == raycast.position)) return;
            var raycastPos = raycast.position;
            OpenDoor(player);
        }

        public static void OpenDoor(UnturnedPlayer player)
        {
            Transform raycast = GetBarricadeTransform(player.Player, out BarricadeData data, out BarricadeDrop drop);
            bool OpenNeed = !((InteractableDoor)drop.interactable).isOpen;
            BarricadeManager.ServerSetDoorOpen((InteractableDoor)drop.interactable, OpenNeed);
            ClientStaticMethod<bool>
            SendDoor = ClientStaticMethod<bool>.Get(new ClientStaticMethod<bool>.ReceiveDelegate(((InteractableDoor)drop.interactable).ReceiveOpen));
            SendDoor.Invoke(ENetReliability.Reliable, Provider.EnumerateClients(), OpenNeed);
        }

        public static void ChangeDoors(bool isOpen)
        {
            if (Plugin.Instance.Configuration.Instance.Doors.Count > 0) foreach (Vector3 points in Plugin.Instance.Configuration.Instance.Doors)
                {
                    List<Transform> result = new List<Transform>();
                    BarricadeManager.getBarricadesInRadius(points, 0.1f, result);
                    if (result.Count == 0) continue;
                    Transform raycast = GetBarricadeTransform(result[0], out BarricadeData data, out BarricadeDrop drop);
                    BarricadeManager.ServerSetDoorOpen((InteractableDoor)drop.interactable, isOpen);
                    ClientStaticMethod<bool>
                    SendDoor = ClientStaticMethod<bool>.Get(new ClientStaticMethod<bool>.ReceiveDelegate(((InteractableDoor)drop.interactable).ReceiveOpen));
                    SendDoor.Invoke(ENetReliability.Reliable, Provider.EnumerateClients(), isOpen);
                }
        }

        public static Transform GetBarricadeTransform(Player player, out BarricadeData barricadeData, out BarricadeDrop drop)
        {
            barricadeData = null;
            drop = null;
            RaycastHit hit;
            if (PhysicsUtility.raycast(new Ray(player.look.aim.position, player.look.aim.forward), out hit, 25, RayMasks.BARRICADE_INTERACT))
            {
                Transform transform = hit.transform;
                InteractableDoorHinge doorHinge = hit.transform.GetComponent<InteractableDoorHinge>();
                if (doorHinge != null)
                {
                    transform = doorHinge.door.transform;
                }

                drop = BarricadeManager.FindBarricadeByRootTransform(transform);
                if (BarricadeManager.tryGetRegion(transform, out _, out _, out _, out BarricadeRegion region))
                {
                    barricadeData = drop.GetServersideData();
                    return drop.model;
                }
            }

            return null;
        }

        public static Transform GetBarricadeTransform(Transform transform, out BarricadeData barricadeData, out BarricadeDrop drop)
        {
            barricadeData = null;
            drop = null;
            InteractableDoorHinge doorHinge = transform.GetComponent<InteractableDoorHinge>();
            if (doorHinge != null)
            {
                transform = doorHinge.door.transform;
            }

            drop = BarricadeManager.FindBarricadeByRootTransform(transform);
            if (BarricadeManager.tryGetRegion(transform, out _, out _, out _, out BarricadeRegion region))
            {
                barricadeData = drop.GetServersideData();
                return drop.model;
            }

            return transform;
        }
    }
}
