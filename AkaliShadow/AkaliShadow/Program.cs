using System;
using EloBuddy;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Rendering;
using SharpDX;
using System.Collections.Generic;
using EloBuddy.SDK;
using AkaliShadow.Utilities;
using System.Drawing;

namespace AkaliShadow
{
    public static class Program
    {
        #region "Variables"
        public static bool QInAir = false;

        private static bool _drawWspots;
        private const int SpotMagnetRadius = 125;

        private static bool _wCountdown;
        private static int _wTick;
        private static Text _wCountdownText;


        private static ColorBGRA RColor = new ColorBGRA(0, 0, 0, 100);
        private static ColorBGRA EColor = new ColorBGRA(63, 212, 242, 100);
        private static ColorBGRA QColor = new ColorBGRA(242, 188, 63, 100);
        private static ColorBGRA spotColor = new ColorBGRA(150, 150, 242, 255);

        public const string ChampName = "Akali";
        #endregion

        public static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            if (Player.Instance.ChampionName != ChampName)
                return;

            _wCountdownText = new Text(String.Empty, new Font("Lucida Sans Unicode", 7.7F, FontStyle.Bold));
            _wCountdownText.Color = System.Drawing.Color.YellowGreen;

            Config.Initialize();
            SpellManager.Initialize();
            ModeManager.Initialize();

            Map.InitializeEvadeSpots();

            Drawing.OnDraw += OnDraw;
            GameObject.OnCreate += GameObject_OnCreate;
            Gapcloser.OnGapcloser += AntiGapcloser_OnEnemyGapcloser;
            Obj_AI_Base.OnProcessSpellCast += OnProcessSpellCast;
            Game.OnWndProc += OnWndProc;
            Orbwalker.OnPostAttack += OnPostAttack;

            Chat.Print("<font color = \"#6B9FE3\">Akali Shadow</font><font color = \"#E3AF6B\"> by BestAkaliAfrica</font>. You like ? Buy me a coffee :p");
        }

        private static void AntiGapcloser_OnEnemyGapcloser(AIHeroClient sender, Gapcloser.GapcloserEventArgs gapcloser)
        {
            //0=targeted 1=skillshot 2=both 4=no
            var gapcloserType = Config.Misc.antiGapCloser;

            if (gapcloserType == 4)
                return;

            if (!SpellManager.W.IsReady())
                return;

            if ((gapcloser.Type == Gapcloser.GapcloserType.Targeted && gapcloserType == 0 || gapcloserType == 2) || ((gapcloser.Type == Gapcloser.GapcloserType.Skillshot && gapcloserType == 1) || gapcloserType == 2 && (Player.Instance.Position.Distance(gapcloser.End)) <= 600))
                SpellManager.W.Cast(Player.Instance);
        }

        private static void OnPostAttack(AttackableUnit target, EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                if (SpellManager.E.IsReady() && Config.Modes.Combo.UseE && target.Type == GameObjectType.AIHeroClient && target.IsValidTarget(SpellManager.E.Range))
                    SpellManager.E.Cast();
        }

        private static void OnProcessSpellCast(Obj_AI_Base sender, GameObjectProcessSpellCastEventArgs args)
        {
            if (Config.Drawings.wCountdown)
            {
                if (sender.Type == GameObjectType.AIHeroClient && sender.NetworkId == Player.Instance.NetworkId)
                {
                    if (args.SData.Name.Equals("AkaliSmokeBomb"))
                    {
                        _wCountdown = true;
                        _wTick = Environment.TickCount;
                    }
                }
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            //Detect whenever our Q land on someone
            if (sender.Name.Contains("akali_markOftheAssasin_marker_tar.troy") && !sender.IsEnemy )
                QInAir = false;
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


            if (_drawWspots)
            {
                foreach (Vector3 safeSpot in Map._wardSpots)
                    //if(Drawing.OnScreen(Drawing.WorldToScreen(safeSpot)))
                    Circle.Draw(spotColor, SpotMagnetRadius, safeSpot);
            }

            if(_wCountdown)
            {
                int remainingTime = 8 - ((Environment.TickCount - _wTick) / 1000);
                if (remainingTime > 0)
                {
                    Vector2 drawPos = Drawing.WorldToScreen(Player.Instance.Position);
                    _wCountdownText.Position = Drawing.WorldToScreen(Player.Instance.Position);
                    _wCountdownText.TextValue = remainingTime.ToString();
                    _wCountdownText.Draw();
                }
                else
                    _wCountdown = false;
            }
        }

        private static void OnWndProc(WndEventArgs args)
        {
            //a key is pressed
            if (args.Msg == (uint)WindowMessages.KeyDown)
            {
                //is the key for wSpotActive ?
                if (Config.Misc.wSpotActive)
                {
                    _drawWspots = true;
                }


                //if (args.WParam == 0x60) //numpad 0
                //{
                //    string line = "_WardSpots.Add(new Vector3(" + Game.CursorPos.X + "f, " + Game.CursorPos.Y + "f, " + Game.CursorPos.Z + "f));";
                //    debug_output.WriteLine(line + "\r\n");
                //    Game.PrintChat(line);
                //}
                //if (args.WParam == 0x61) //numpad 1
                //    debug_output.Close();
            }
            else if (args.Msg == (uint)WindowMessages.LeftButtonDown && _drawWspots)
            {
                _drawWspots = false;

                foreach (Vector3 safeSpot in Map._wardSpots)
                    if (safeSpot.Distance(Game.CursorPos) <= SpotMagnetRadius)
                        SpellManager.W.Cast(safeSpot);
            }
            else if ((args.Msg == (uint)WindowMessages.LeftButtonUp || args.Msg == (uint)WindowMessages.RightButtonDown) && _drawWspots)
                _drawWspots = false;
        }
    }
}
