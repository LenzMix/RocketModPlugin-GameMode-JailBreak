using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDJailMode
{
    class ModuleVoice
    {
        internal static void OnVoice(PlayerVoice speaker, bool wantsToUseWalkieTalkie, ref bool shouldAllow, ref bool shouldBroadcastOverRadio, ref PlayerVoice.RelayVoiceCullingHandler cullingHandler)
        {
            //if (!speaker.player) return;
            shouldBroadcastOverRadio = true;
            if (speaker.player.GetComponent<PlayerComponent>().team == Team.Death)
            {
                cullingHandler = (voice, listener) => Voice(speaker, listener);
            }
            else
            {
                cullingHandler = (voice, listener) => VoiceOther(speaker, listener);
            }
        }

        public static bool Voice(PlayerVoice speaker, PlayerVoice listener)
        {
            if (UnturnedPlayer.FromPlayer(listener.player).GetComponent<PlayerComponent>().team != Team.Death) return false;
            return true;
        }

        public static bool VoiceOther(PlayerVoice speaker, PlayerVoice listener)
        {
            return true;
        }
    }
}
