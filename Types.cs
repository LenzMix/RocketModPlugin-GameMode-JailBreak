using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SDJailMode
{
    public enum JailMode
    {
        NotReady,
        LobbyStart,
        Pregame,
        Game,
        Finish
    }

    public enum EndReason
    {
        Time,
        CT,
        T,
        NoPlayers
    }

    public enum Team
    {
        Death,
        Police,
        Prisoner
    }
}
