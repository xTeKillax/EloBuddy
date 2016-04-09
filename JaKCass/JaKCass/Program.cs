using System;
using EloBuddy;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;
using SharpDX;
using System.Collections.Generic;
using EloBuddy.SDK;
using JaKCass.Utilities;
using System.Drawing;
using System.Linq;

namespace JaKCass
{
    public static class Program
    {
        #region "Variables"
        private static ColorBGRA RColor = new ColorBGRA(0, 0, 0, 100);
        private static ColorBGRA EColor = new ColorBGRA(63, 212, 242, 100);
        private static ColorBGRA QColor = new ColorBGRA(242, 188, 63, 100);
        private static ColorBGRA spotColor = new ColorBGRA(63, 63, 242, 100);
        public static List<String> DashingChamps = new List<string>()
	    {
	        "Aatrox","Gragas","Graves","Irelia","Jax","Khazix","Leblanc","LeeSin","MonkeyKing","Pantheon","Renekton","Shen","Sejuani","Tristana","Tryndamere","XinZhao","Fizz", "Ezreal"
	    };

        public static bool QisCasting = false;
        public static int lastQcastTime = 0;

        public static bool ShouldDisableAA = true;

        public const string ChampName = "Cassiopeia";
        #endregion

        public static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != ChampName)
                return;

            Config.Initialize();
            SpellManager.Initialize();
            ModeManager.Initialize();

            Drawing.OnDraw += OnDraw;
            //Gapcloser.OnGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Interrupter.OnInterruptableSpell += OnInterruptableSpell;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpell;

            Chat.Print("<font color = \"#6B9FE3\">JaKCass</font><font color = \"#E3AF6B\"> by BestAkaliAfrica</font>. You like ? Buy me a coffee :p");
        }

        private static void OnProcessSpell(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;

            if(args.SData.Name == "CassiopeiaNoxiousBlast")
            {
                lastQcastTime = Environment.TickCount;
		        QisCasting = true;
            }
        }

        private static void OnInterruptableSpell(Obj_AI_Base enemy, Interrupter.InterruptableSpellEventArgs e)
        {
            if(Config.Skills.NinjaInteruption)
                if (e.Sender.IsEnemy && SpellManager.R.IsInRange(e.Sender) && e.Sender.IsFacing(Player.Instance) && e.DangerLevel == EloBuddy.SDK.Enumerations.DangerLevel.High)
                {
                    Random rnd = new Random();
                    Core.DelayAction(() => SpellManager.R.Cast(e.Sender), rnd.Next(0, 367));
                }
        }

        private static void OnDraw(EventArgs args)
        {
            if (Player.Instance.IsDead || Shop.IsOpen) 
                return;

            if (Config.Drawings.Qrange)
                Circle.Draw(QColor, SpellManager.Q.Range, Player.Instance.Position);

            if (Config.Drawings.Erange)
                Circle.Draw(EColor, SpellManager.E.Range, Player.Instance.Position);

            if (Config.Drawings.Rrange)
                Circle.Draw(RColor, SpellManager.R.Range, Player.Instance.Position);
        }
    }
}
