using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AllTameable
{
    public static class DBG
    {
        #region Debug
        public static void cprt(string s)
        {
            global::Console.instance.Print(s);
        }
        public static void InfoTL(string s)
        {
            Player.m_localPlayer.Message(MessageHud.MessageType.TopLeft, s, 0, null);
        }
        public static void InfoCT(string s)
        {
            Player.m_localPlayer.Message(MessageHud.MessageType.Center, s, 0, null);
        }
        public static void blogInfo(object o)
        {
            Plugin.logger.LogInfo(o);
        }
        public static void blogWarning(object o)
        {
            Plugin.logger.LogWarning(o);
        }
        #endregion Debug
    }
}
