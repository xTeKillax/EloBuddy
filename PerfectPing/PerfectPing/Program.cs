using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using SharpDX;

namespace PerfectPing
{
    public class Program
    {
        //int autoDisableTime      = 1500;  // time in seconds until automatic disable (when lane phase ends)
        //int heroVisibleTimeout   = 2000;  // mimimum time in ms that a hero has to be invisible to be pinged
        //int heroPingTimeout      = 10000; // minimum time in ms until the same hero can be pinged again
        //int pingTimeout          = 3000;  // minimum time in ms between separate pings
        static bool autoDisabled = false;
        static int heroChaseRange = 600;   // how far a champ has to be from opponents to be considered roaming
        static int heroVisibleThreshold = 1000;  // minimum time in ms a hero has to be visible again to get pinged
        private static bool InJungle = false;
        private static Dictionary<int, int> herosVisible = new Dictionary<int, int>();
        private static Dictionary<int, int> herosPinged = new Dictionary<int, int>();
        private static Dictionary<int, int> plannedPings = new Dictionary<int, int>();
        private static int lastPing = 0;

        private static Menu Config;

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Game_OnGameLoad;
        }

        private static void Game_OnGameLoad(EventArgs args)
        {

            lastPing = Environment.TickCount;

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != ObjectManager.Player.Team))
            {
                herosVisible[enemy.NetworkId] = Environment.TickCount;
                herosPinged[enemy.NetworkId] = Environment.TickCount;
                plannedPings[enemy.NetworkId] = 0;
            }

            Config = MainMenu.AddMenu("Perfect Ping", "Perfect Ping");
            Config.Add("Enabled", new CheckBox("Enabled", true));
            //Config.Add("localPing", new CheckBox("Local ping", true));
            Config.Add("autoDisableTime", new Slider("Auto disable after (minutes)", 25, 10, 60));
            Config.Add("heroVisibleTimeout", new Slider("Time visible before detect (ms)", 1500, 0, 2000));
            Config.Add("heroPingTimeout", new Slider("Time before pinging hero again (ms)", 10000, 0, 10000));
            Config.Add("pingTimeout", new Slider("Time between seperate ping (ms)", 3000, 0, 6000));
            Config.Add("pingType", new EloBuddy.SDK.Menu.Values.Slider("Type of ping to use (Normal, Fallback, Danger)", 1, 1, 3));


            MapPosition.Initialize();
            Game.OnUpdate += OnUpdate;

            Chat.Print("<font color = \"#6B9FE3\">Perfect Ping</font><font color = \"#E3AF6B\"> by BestAkaliAfrica</font>. You like ? Buy me a coffee :p");
        }

        private static void OnUpdate(EventArgs args)
        {
            if (isChecked(Config, "Enabled") && !autoDisabled)
            {
                if (Game.Time > (getSliderValue(Config, "autoDisableTime") * 60))
                {
                    autoDisabled = true;
                    Chat.Print("Perfect ping disabled automatically !");
                }

                foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != ObjectManager.Player.Team))
                {
                    int i = enemy.NetworkId;
                    if (plannedPings[i] != 0)
                    {
                        herosVisible[i] = Environment.TickCount;

                        if (enemy.IsVisible)
                        {
                            if ((Environment.TickCount - plannedPings[i]) >= heroVisibleThreshold)
                            {
                                switch (getSliderValue(Config, "pingType"))
                                {
                                    case 1:
                                        TacticalMap.SendPing(PingCategory.Normal, ObjectManager.GetUnitByNetworkId(Convert.ToUInt32(i)).Position);
                                        break;
                                    case 2:
                                        TacticalMap.SendPing(PingCategory.Fallback, ObjectManager.GetUnitByNetworkId(Convert.ToUInt32(i)));
                                        break;
                                    case 3:
                                        TacticalMap.SendPing(PingCategory.Danger, ObjectManager.GetUnitByNetworkId(Convert.ToUInt32(i)));
                                        break;

                                }

                                lastPing = Environment.TickCount;
                                herosPinged[i] = lastPing;
                                purgePlannedPings();
                            }
                        }
                        else
                            plannedPings[i] = 0;
                    }
                    else
                    {
                        if (enemy.IsVisible)
                        {
                            if ((Environment.TickCount - herosVisible[i]) >= getSliderValue(Config, "heroVisibleTimeout") &&
                                (Environment.TickCount - herosPinged[i]) >= getSliderValue(Config, "heroPingTimeout") &&
                                (Environment.TickCount - lastPing) >= getSliderValue(Config, "pingTimeout") &&
                                ((MapPosition.inInnerJungle(enemy) || MapPosition.inInnerRiver(enemy))) && ObjectManager.Player.Distance(enemy) >= heroChaseRange)
                            {
                                plannedPings[i] = Environment.TickCount;
                            }

                            herosVisible[i] = Environment.TickCount;
                        }
                    }
                }
            }
        }

        private static void purgePlannedPings()
        {
            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != ObjectManager.Player.Team))
            {
                plannedPings[enemy.NetworkId] = 0;
            }
        }

        public static bool isChecked(Menu obj, String value)
        {
            return obj[value].Cast<CheckBox>().CurrentValue;
        }

        public static int getSliderValue(Menu obj, String value)
        {
            return obj[value].Cast<Slider>().CurrentValue;
        }
    }
}
