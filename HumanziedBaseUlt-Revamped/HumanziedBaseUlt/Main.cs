using System;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using Font = SharpDX.Direct3D9.Font;

namespace HumanziedBaseUlt
{
    class Main : Events
    {
        private readonly AIHeroClient me = ObjectManager.Player;
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private Menu config;
        private Font Text;
        static float BarX = Drawing.Width * 0.425f;
        float BarY = Drawing.Height * 0.80f;
        static int BarWidth = (int)(Drawing.Width - 2 * BarX);
        int BarHeight = 6;
        int SeperatorHeight = 5;
        static float Scale = (float)BarWidth / 8000;


        public Main()
        {
            config = MainMenu.AddMenu("HumanizedBaseUlts", "humanizedBaseUlts");
            config.Add("on", new CheckBox("Enabled"));
            config.Add("min20", new CheckBox("20 min passed"));
            config.Add("minDelay", new Slider("Minimum ultimate delay", 1000, 0, 2500));
            config.AddLabel("The time to let the enemy regenerate health in base");

            config.AddSeparator(20);
            config.Add("fountainReg", new Slider("Fountain regeneration speed", 89, 85, 92));
            config.Add("fountainRegMin20", new Slider("Fountain regeneration speed after minute 20", 366, 350, 370));

            config.AddSeparator();
            config.AddLabel("[Draven]");
            config.Add("dravenCastBackBool", new CheckBox("Enable 'Draven Cast Back'"));
            config.Add("dravenCastBackDelay", new Slider("Cast Back X ms earlier", 400, 0, 500));

            config.AddSeparator();
            config.Add("PanicKey", new KeyBind("PanicKey", false, KeyBind.BindTypes.HoldActive, 32));
            config.Add("drawRecalls", new CheckBox("Draw recalls"));

            Listing.potionMenu = config.AddSubMenu("Potions", "potionsMenuasrfsdg");
            Listing.potionMenu.AddLabel("[Regeneration Speed in HP/Sec.]");
            Listing.potionMenu.Add("healPotionRegVal", new Slider("Heal Potion / Cookie", 10, 5, 20));
            Listing.potionMenu.Add("crystalFlaskRegVal", new Slider("Crystal Flask", 10, 5, 20));
            Listing.potionMenu.Add("crystalFlaskJungleRegVal", new Slider("Crystal Flask Jungle", 9, 5, 20));
            Listing.potionMenu.Add("darkCrystalFlaskVal", new Slider("Dark Crystal Flask", 16, 5, 20));

            Listing.snipeMenu = config.AddSubMenu("Enemy Recall Snipe", "snipeultimatesae3re");
            Listing.snipeMenu.AddLabel("[No premade feature currently]");
            Listing.snipeMenu.Add("snipeEnabled", new CheckBox("Enabled"));
            AddStringList(Listing.snipeMenu, "minSnipeHitChance", "Minimum Snipe HitChance", 
                new []{ "Impossible", "Low", "Above Average", "Very High"}, 2);

            Listing.allyconfig = config.AddSubMenu("Premades");
            foreach (var ally in EntityManager.Heroes.Allies)
            {
                if (Listing.spellDataList.Any(x => x.championName == ally.ChampionName))
                    Listing.allyconfig.Add(ally.ChampionName + "/Premade", new CheckBox(ally.ChampionName, ally.IsMe));
            }


            Listing.disableMenu = config.AddSubMenu("Disable");
            Listing.disableMenu.AddLabel("Disable BaseUlt for :");
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                Listing.disableMenu.Add(enemy.ChampionName, new CheckBox(enemy.ChampionName, false));
            }

            foreach (var enemy in EntityManager.Heroes.Enemies)
                Listing.visibleEnemies.Add(enemy);

            Text = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Calibri", Height = 13, Width = 6, OutputPrecision = FontPrecision.Default, Quality = FontQuality.Default });

            Drawing.OnPreReset += Drawing_OnPreReset;
            Drawing.OnPostReset += Drawing_OnPostReset;
            Drawing.OnDraw += Drawing_OnDraw;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomain_DomainUnload;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomain_DomainUnload;
            Game.OnUpdate += GameOnOnUpdate;
            Teleport.OnTeleport += TeleportOnOnTeleport;
            OnEnemyInvisible += OnOnEnemyInvisible;
            OnEnemyVisible += OnOnEnemyVisible;
        }

        private void AddStringList(Menu m, string uniqueId, string displayName, string[] values, int defaultValue)
        {
            var mode = m.Add(uniqueId, new Slider(displayName, defaultValue, 0, values.Length - 1));
            mode.DisplayName = displayName + ": " + values[mode.CurrentValue];
            mode.OnValueChange += delegate (ValueBase<int> sender, ValueBase<int>.ValueChangeArgs args)
            {
                sender.DisplayName = displayName + ": " + values[args.NewValue];
            };
        }

        private void TeleportOnOnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            if (sender == null || !sender.IsValid())
                return;

            var invisEnemiesEntry = Listing.invisEnemiesList.FirstOrDefault(x => x.sender == sender);

            switch (args.Status)
            {
                case TeleportStatus.Start:

                    if (invisEnemiesEntry == null)
                        return;

                    if (Listing.teleportingEnemies.All(x => x.Sender != sender))
                    {
                        Listing.teleportingEnemies.Add(new Listing.PortingEnemy
                        {
                            Sender = (AIHeroClient) sender,
                            Duration = args.Duration,
                            StartTick = args.Start,
                            Locked = false
                        });
                    }
                    break;
                case TeleportStatus.Abort:
                    var teleportingEnemiesEntry = Listing.teleportingEnemies.FirstOrDefault(x => x.Sender.Equals(sender));


                    if (teleportingEnemiesEntry == null)
                        return;
                    Listing.teleportingEnemies.Remove(teleportingEnemiesEntry);
                    break;

                case TeleportStatus.Finish:
                    var teleportingEnemiesEntry2 = Listing.teleportingEnemies.FirstOrDefault(x => x.Sender.Equals(sender));

                    if (teleportingEnemiesEntry2 == null)
                        return;

                    Core.DelayAction(() => Listing.teleportingEnemies.Remove(teleportingEnemiesEntry2), 5000);
                    break;
            }
        }
              
        /*enemy appear*/
        private void OnOnEnemyVisible(AIHeroClient sender)
        {
            var invisEntry = Listing.invisEnemiesList.Where(x => x.sender.NetworkId == sender.NetworkId).FirstOrDefault();

            if (invisEntry == null)
                return;

            Listing.Regeneration.enemyBuffs.Remove(sender);

            Listing.visibleEnemies.Add(sender);

            Listing.invisEnemiesList.Remove(invisEntry);

            var sinpeEntry = Listing.Pathing.enemySnipeProcs.Where(x => x.target == sender).FirstOrDefault();

            if (sinpeEntry != null)
              sinpeEntry.CancelProcess();

            Listing.Pathing.enemySnipeProcs.Remove(sinpeEntry);
        }
        
        /*enemy disappear*/
        private void OnOnEnemyInvisible(InvisibleEventArgs args)
        {
            if (Listing.Regeneration.HasPotionActive(args.sender))
                Listing.Regeneration.enemyBuffs.Add(args.sender, Listing.Regeneration.GetPotionBuff(args.sender));

            Listing.visibleEnemies.Remove(args.sender);

            Listing.invisEnemiesList.Add(args);

            if (Listing.snipeMenu["snipeEnabled"].Cast<CheckBox>().CurrentValue)
                Listing.Pathing.enemySnipeProcs.Add(new SnipePrediction(args));
        }

        private void GameOnOnUpdate(EventArgs args)
        {
            config.Get<CheckBox>("min20").CurrentValue = Game.Time > 1225f;

            Listing.Regeneration.UpdateEnemyNormalRegenartions();
            UpdateEnemyVisibility();
            Listing.Pathing.UpdateEnemyPaths();
            CheckRecallingEnemies();
        }

        private void CheckRecallingEnemies()
        {
            if (!config.Get<CheckBox>("on").CurrentValue)
                return;

            foreach (Listing.PortingEnemy enemyInst in Listing.teleportingEnemies.OrderBy(x => x.Sender.Health - Damage.GetBaseUltSpellDamage(x.Sender, me)))
            {
                var enemy = enemyInst.Sender;
                InvisibleEventArgs invisEntry = Listing.invisEnemiesList.FirstOrDefault(x => x.sender.Equals(enemy));

                if (invisEntry == null)
                    continue;

                if (Listing.disableMenu.Get<CheckBox>(enemy.ChampionName).CurrentValue)
                    continue;

                int recallEndTime = enemyInst.StartTick + enemyInst.Duration;
                float timeLeft = recallEndTime - Core.GameTickCount;
                float travelTime = Algorithm.GetUltTravelTime(me);

                float regedHealthRecallFinished = Algorithm.SimulateHealthRegen(enemy, invisEntry.StartTime, recallEndTime);
                float totalEnemyHp = enemy.Health + regedHealthRecallFinished;
                float fountainReg = GetFountainReg(enemy);

                float aioDmg = Damage.GetAioDmg(enemy, timeLeft);

                if (aioDmg > totalEnemyHp)
                {
                    /*contains own enemy hp reg during fly delay*/
                    float realDelayTime = Algorithm.SimulateRealDelayTime(enemy, recallEndTime, aioDmg, fountainReg);

                    if (realDelayTime < config.Get<Slider>("minDelay").CurrentValue)
                    {
                        Chat.Print("<font color=\"#0cf006\">Delay too low: " + realDelayTime + "ms</font>");
                        continue;
                    }

                    Messaging.ProcessInfo(realDelayTime, enemy.ChampionName);

                    if (travelTime <= timeLeft)
                    {
                        if (!Algorithm.GetCollision(me.ChampionName).Any())
                        {
                            
                            Vector3 enemyBaseVec =
                                ObjectManager.Get<Obj_SpawnPoint>().First(x => x.IsEnemy).Position;
                            float delay = timeLeft + realDelayTime - travelTime;

                            enemyInst.Locked = true;
                            enemyInst.ShootDelay = delay;

                            if (!config.Get<KeyBind>("PanicKey").CurrentValue)
                            {
                                Core.DelayAction(() =>
                                {
                                    Player.CastSpell(SpellSlot.R, enemyBaseVec);

                                    /*Draven*/
                                    if (config.Get<CheckBox>("dravenCastBackBool").CurrentValue)
                                    {
                                        int castBackReduction = config.Get<Slider>("dravenCastBackDelay").CurrentValue;
                                        float travelTime2 = Algorithm.GetUltTravelTime(me);
                                        if (me.ChampionName == "Draven")
                                            Core.DelayAction(() =>
                                            {
                                                Player.CastSpell(SpellSlot.R);
                                            }, (int)(travelTime2 - castBackReduction));
                                    }
                                    /*Draven*/
                                },
                                (int)Math.Floor(delay));
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// per second
        /// </summary>
        /// <param name="enemy"></param>
        /// <returns></returns>
        private float GetFountainReg(AIHeroClient enemy)
        {
            float regSpeedDefault = config.Get<Slider>("fountainReg").CurrentValue / 10;
            float regSpeedMin20 = config.Get<Slider>("fountainRegMin20").CurrentValue / 10;


            float fountainReg = config.Get<CheckBox>("min20").CurrentValue ? enemy.MaxHealth / 100 * regSpeedMin20 : 
                                    enemy.MaxHealth / 100 * regSpeedDefault;

            return fountainReg;
        }

        private void UpdateEnemyVisibility()
        {
            foreach (var enemy in EntityManager.Heroes.Enemies)
            {
                if (enemy.IsHPBarRendered && !Listing.visibleEnemies.Contains(enemy))
                {
                    FireOnEnemyVisible(enemy);
                }

                if (!enemy.IsHPBarRendered && Listing.visibleEnemies.Contains(enemy))
                {
                    FireOnEnemyInvisible(new InvisibleEventArgs
                    {
                        StartTime = Core.GameTickCount,
                        sender = enemy,
                        StdHealthRegen = Listing.Regeneration.lastEnemyRegens[enemy],
                        LastRealPath = Listing.Pathing.GetLastEnemyPath(enemy)
                    });
                }
            }
        }




        void Drawing_OnDraw(EventArgs args)
        {
            if (!config.Get<CheckBox>("drawRecalls").CurrentValue || Drawing.Direct3DDevice == null || Drawing.Direct3DDevice.IsDisposed)
                return;

            float fadeout = 1f;
            int count = 0;

            foreach (Listing.PortingEnemy enemyInst in Listing.teleportingEnemies.OrderBy(
                x => x.Sender.Health))
            {
                if (enemyInst.Sender.IsDead)
                    continue;

                int recallEndTime = enemyInst.StartTick + enemyInst.Duration;
                float timeLeft = recallEndTime - Core.GameTickCount;
                float travelTime = Algorithm.GetUltTravelTime(me);

                if (timeLeft < 0)
                    continue;

                if (!enemyInst.Locked)
                {
                    Color color = System.Drawing.Color.White;

                    DrawRect(BarX, BarY, (int)(Scale * (float)timeLeft), BarHeight, 1, System.Drawing.Color.FromArgb((int)(100f * fadeout), System.Drawing.Color.White));
                    DrawRect(BarX + Scale * (float)timeLeft - 1, BarY + 1, 2, BarHeight - 1, 1, System.Drawing.Color.FromArgb((int)(255f * fadeout), color));

                    Text.DrawText(null, enemyInst.Sender.BaseSkinName, (int)BarX + (int)(Scale * (float)timeLeft - (float)(enemyInst.Sender.BaseSkinName.Length * Text.Description.Width) / 2), (int)BarY - SeperatorHeight - Text.Description.Height - 1, new ColorBGRA(color.R, color.G, color.B, (byte)((float)color.A * fadeout)));
                }
                else
                {

                    DrawRect(BarX, BarY, (int)(Scale * (float)timeLeft), BarHeight, 1, System.Drawing.Color.FromArgb(255, System.Drawing.Color.Red));
                    DrawRect(BarX + Scale * (float)timeLeft - 1, BarY + 1, 2, BarHeight - 1, 1, System.Drawing.Color.IndianRed);

                    Text.DrawText(null, enemyInst.Sender.BaseSkinName, (int)BarX + (int)(Scale * (float)timeLeft - (float)(enemyInst.Sender.BaseSkinName.Length * Text.Description.Width) / 2), (int)BarY + SeperatorHeight + Text.Description.Height / 2, new ColorBGRA(255, 92, 92, 255));
                }

                count++;
            }

            /*
             * Show in a red rectangle right next to the normal bar the names of champs which can be killed (when they are not recalling yet)
             * Requires calculating the damages (make more functions!)
             * 
             * var BaseUltableEnemies = EnemyInfo.Where(x =>
                x.Player.IsValid<Obj_AI_Hero>() &&
                !x.RecallInfo.ShouldDraw() &&
                !x.Player.IsDead && //maybe redundant
                x.RecallInfo.GetRecallCountdown() > 0 && x.RecallInfo.LockedTarget).OrderBy(x => x.RecallInfo.GetRecallCountdown());*/

            if (count > 0)
            {
                DrawRect(BarX, BarY, BarWidth, BarHeight, 1, System.Drawing.Color.FromArgb((int)(40f * fadeout), System.Drawing.Color.White));

                DrawRect(BarX - 1, BarY + 1, 1, BarHeight, 1, System.Drawing.Color.FromArgb((int)(255f * fadeout), System.Drawing.Color.White));
                DrawRect(BarX - 1, BarY - 1, BarWidth + 2, 1, 1, System.Drawing.Color.FromArgb((int)(255f * fadeout), System.Drawing.Color.White));
                DrawRect(BarX - 1, BarY + BarHeight, BarWidth + 2, 1, 1, System.Drawing.Color.FromArgb((int)(255f * fadeout), System.Drawing.Color.White));
                DrawRect(BarX + 1 + BarWidth, BarY + 1, 1, BarHeight, 1, System.Drawing.Color.FromArgb((int)(255f * fadeout), System.Drawing.Color.White));
            }
        }

        public void DrawRect(float x, float y, int width, int height, float thickness, System.Drawing.Color color)
        {
            for (int i = 0; i < height; i++)
                Drawing.DrawLine(x, y + i, x + width, y + i, thickness, color);
        }

        void Drawing_OnPostReset(EventArgs args)
        {
            Text.OnResetDevice();
        }

        void Drawing_OnPreReset(EventArgs args)
        {
            Text.OnLostDevice();
        }

        void CurrentDomain_DomainUnload(object sender, EventArgs e)
        {
            Text.Dispose();
        }
    }
}
