using System;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace BaseUltPlusPlus
{
    public class Program
    {
        public static Menu BaseUltMenu { get; set; }

        public static void Main(string[] args)
        {
            // Wait till the name has fully loaded
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        private static void Loading_OnLoadingComplete(EventArgs args)
        {
            //Menu
            BaseUltMenu = MainMenu.AddMenu("BaseUlt-3x", "BUP");
            BaseUltMenu.Add("baseult", new CheckBox("BaseUlt"));
            BaseUltMenu.Add("showrecalls", new CheckBox("Show Recalls"));
            BaseUltMenu.Add("checkcollision", new CheckBox("Check Collision"));
            BaseUltMenu.AddSeparator();
            BaseUltMenu.Add("timeLimit", new Slider("FOW Time Limit (SEC)", 0, 0, 120));
            BaseUltMenu.AddSeparator();
            BaseUltMenu.Add("nobaseult", new KeyBind("No BaseUlt while", false, KeyBind.BindTypes.HoldActive, 32));
            BaseUltMenu.AddSeparator();
            BaseUltMenu.AddGroupLabel("BaseUlt-3x Targets");
            foreach (var unit in EntityManager.Heroes.Enemies)
            {
                BaseUltMenu.Add("target" + unit.ChampionName,
                    new CheckBox(string.Format("{0} ({1})", unit.ChampionName, unit.Name)));
            }

            BaseUltMenu.AddGroupLabel("BaseUlt-3x - By BestAkaliAfrica");
            BaseUltMenu.AddLabel("Based on Roach_ version of LunarBlue Addon");
            BaseUltMenu.AddLabel("FinnDev, MrOwl, Beaving, DanThePman");

            // Initialize the Addon
            OfficialAddon.Initialize();

            // Listen to the two main events for the Addon
            Game.OnUpdate += args1 => OfficialAddon.Game_OnUpdate();
            Drawing.OnPreReset += args1 => OfficialAddon.Drawing_OnPreReset(args1);
            Drawing.OnPostReset += args1 => OfficialAddon.Drawing_OnPostReset(args1);
            Drawing.OnDraw += args1 => OfficialAddon.Drawing_OnDraw(args1);
            Teleport.OnTeleport += OfficialAddon.Teleport_OnTeleport;
        }
    }
}