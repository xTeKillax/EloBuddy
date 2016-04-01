using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using SharpDX.Direct3D9;
using Color = System.Drawing.Color;
using Font = SharpDX.Direct3D9.Font;
using Rectangle = SharpDX.Rectangle;
using Sprite = EloBuddy.SDK.Rendering.Sprite;

namespace BaseUltPlusPlus
{
    public class OfficialAddon
    {
        private static String[] compatibleChamps = new[] { "Jinx", "Ezreal", "Ashe", "Draven", "Karthus" }; //Ziggs, Xerath, Lux
        private static readonly List<Recall> Recalls = new List<Recall>();
        private static readonly List<BaseUltUnit> BaseUltUnits = new List<BaseUltUnit>();
        private static readonly List<BaseUltSpell> BaseUltSpells = new List<BaseUltSpell>();
        private static readonly AIHeroClient Player = ObjectManager.Player;
        private static SpellDataInst Ultimate;
        
        
        private static Font Text;
        private static float BarX = Drawing.Width * 0.425f;
        private static float BarY = Drawing.Height * 0.80f;
        private static int BarW = (int)(Drawing.Width - 2 * BarX);
        private static float Scale = (float)BarW / 8000;
        private static int BarH = 6;

        public static void Initialize()
        {
            Text = new Font(Drawing.Direct3DDevice, new FontDescription { FaceName = "Calibri", Height = 13, Width = 6, OutputPrecision = FontPrecision.Default, Quality = FontQuality.Default });

            #region Spells

            BaseUltSpells.Add(new BaseUltSpell("Ezreal", SpellSlot.R, 1000, 2000, 160, false));
            BaseUltSpells.Add(new BaseUltSpell("Jinx", SpellSlot.R, 600, 1700, 140, true));
            BaseUltSpells.Add(new BaseUltSpell("Ashe", SpellSlot.R, 250, 1600, 130, true));
            BaseUltSpells.Add(new BaseUltSpell("Draven", SpellSlot.R, 400, 2000, 160, true));
            BaseUltSpells.Add(new BaseUltSpell("Karthus", SpellSlot.R, 3125, 0, 0, false));
            BaseUltSpells.Add(new BaseUltSpell("Ziggs", SpellSlot.Q, 250, 3100, 0, false));
            BaseUltSpells.Add(new BaseUltSpell("Lux", SpellSlot.R, 1375, 0, 0, false));
            BaseUltSpells.Add(new BaseUltSpell("Xerath", SpellSlot.R, 700, 600, 0, false));

            #endregion

            Ultimate = Player.Spellbook.GetSpell(BaseUltSpells.Find(h => h.Name == Player.ChampionName).Slot);
        }
        //compatibleChamps.Any(h => h == Player.ChampionName)
        public static void Game_OnUpdate()
        {
            foreach (var recall in Recalls)
            {
                //Compute
                if (BaseUltUnits.All(h => h.Unit.NetworkId != recall.Unit.NetworkId))
                {
                    var spell = BaseUltSpells.Find(h => h.Name == Player.ChampionName);
                    if (Ultimate.IsReady && Ultimate.Level > 0)
                        BaseUltCalcs(recall);
                }
            }

            foreach (var unit in BaseUltUnits)
            {
                if (unit.Unit.IsVisible)
                    unit.LastSeen = Game.Time;

                var timeLimit = Program.BaseUltMenu["timeLimit"].Cast<Slider>().CurrentValue;

                if (Math.Round(unit.FireTime, 1) <= Core.GameTickCount && ((Game.Time - timeLimit) >= unit.LastSeen))
                {
                    if (Ultimate.IsReady)
                        Player.Spellbook.CastSpell(Ultimate.Slot, GetFountainPos());
                }
            }
        }

        public static void Drawing_OnDraw(EventArgs args)
        {
            if (!Program.BaseUltMenu["showrecalls"].Cast<CheckBox>().CurrentValue && !Recalls.Any())
                return;


            foreach (var recall in Recalls.OrderBy(x => x.Unit.Health))
            {
                double RecallProgress = recall.Started + recall.Duration - Core.GameTickCount;
                bool isBaseUlt = BaseUltUnits.Any(h => h.Unit.NetworkId == recall.Unit.NetworkId);

                Color colorIndicator = isBaseUlt ? Color.Red : Color.White;

                DrawRect(BarX, BarY, (int)(Scale * RecallProgress), BarH, 1, System.Drawing.Color.FromArgb(100, colorIndicator));
                DrawRect(BarX + (int)(Scale * RecallProgress), BarY + 1, 2, BarH - 1, 1, System.Drawing.Color.FromArgb(255, Color.White));

                if (isBaseUlt)
                {
                    var unit = BaseUltUnits.FirstOrDefault(h => h.Unit.NetworkId == recall.Unit.NetworkId);

                    if (unit == null)
                        continue;

                    var barPos = ((recall.Started + recall.Duration) - unit.FireTime);

                    DrawRect(BarX + (int)(Scale * barPos), BarY + 4, 4, BarH - 1, 1, System.Drawing.Color.FromArgb(255, Color.Yellow));
                }

                Text.DrawText(null, recall.Unit.BaseSkinName, (int)BarX + (int)(Scale * RecallProgress - (float)(recall.Unit.BaseSkinName.Length * Text.Description.Width) / 2), (int)BarY - 5 - Text.Description.Height - 1, new ColorBGRA(255, 255, 255, (byte)((float)255)));
            }

            if (Recalls.Any())
            {
                DrawRect(BarX, BarY, BarW, BarH, 1, System.Drawing.Color.FromArgb(40, System.Drawing.Color.White));

                DrawRect(BarX - 1, BarY + 1, 1, BarH, 1, System.Drawing.Color.FromArgb(255, System.Drawing.Color.White));
                DrawRect(BarX - 1, BarY - 1, BarW + 2, 1, 1, System.Drawing.Color.FromArgb(255, System.Drawing.Color.White));
                DrawRect(BarX - 1, BarY + BarH, BarW + 2, 1, 1, System.Drawing.Color.FromArgb(255, System.Drawing.Color.White));
                DrawRect(BarX + 1 + BarW, BarY + 1, 1, BarH, 1, System.Drawing.Color.FromArgb(255, System.Drawing.Color.White));
            }
        }

        public static void DrawRect(float x, float y, int width, int height, float thickness, System.Drawing.Color color)
        {
            for (int i = 0; i < height; i++)
                Drawing.DrawLine(x, y + i, x + width, y + i, thickness, color);
        }

        private static Vector3 GetFountainPos()
        {
            return ObjectManager.Get<Obj_SpawnPoint>().First(x => x.IsEnemy).Position;
        }

        private static double GetRecallPercent(Recall recall)
        {
            var recallDuration = recall.Duration;
            var cd = recall.Started + recallDuration - Game.Time;
            var percent = (cd > 0 && Math.Abs(recallDuration) > float.Epsilon) ? 1f - (cd/recallDuration) : 1f;
            return percent;
        }

        private static float GetBaseUltTravelTime(AIHeroClient source, Vector3? dest = null)
        {
            try
            {
                var targetpos = dest.HasValue ? dest.Value : GetFountainPos();
                float speed = BaseUltSpells.First(x => x.Name == source.ChampionName).Speed;
                float delay = BaseUltSpells.First(x => x.Name == source.ChampionName).Delay;


                float distance = source.ServerPosition.Distance(targetpos);

                float missilespeed = speed;

                if (source.ChampionName.ToLower().Contains("jinx") && distance > 1350)
                {
                    const float accelerationrate = 0.3f; //= (1500f - 1350f) / (2200 - speed), 1 unit = 0.3units/second

                    var acceldifference = distance - 1350f;

                    if (acceldifference > 150f) //it only accelerates 150 units
                        acceldifference = 150f;

                    var difference = distance - 1500f;

                    missilespeed = (1350f * speed + acceldifference * (speed + accelerationrate * acceldifference) +
                        difference * 2200f) / distance;
                }

                return (distance / missilespeed + delay) * 1000;
            }
            catch
            {
                return int.MaxValue;
            }
        }

        private static double GetBaseUltSpellDamage(AIHeroClient target, AIHeroClient source)
        {
            var level = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.R).Level - 1;
            if (source.ChampionName == "Jinx")
            {
                {
                    var damage = new float[] { 250, 350, 450 }[level] +
                                 new float[] { 25, 30, 35 }[level] / 100 * (target.MaxHealth - target.Health) +
                                 1 * ObjectManager.Player.FlatPhysicalDamageMod;
                    return source.CalculateDamageOnUnit(target, DamageType.Physical, damage);
                }
            }
            if (source.ChampionName == "Ezreal")
            {
                {
                    var damage = new float[] { 350, 500, 650 }[level] + 0.9f * ObjectManager.Player.FlatMagicDamageMod +
                                 1 * ObjectManager.Player.FlatPhysicalDamageMod;
                    return source.CalculateDamageOnUnit(target, DamageType.Magical, damage) * 0.7f;
                }
            }
            if (source.ChampionName == "Ashe")
            {
                {
                    var damage = new float[] { 250, 425, 600 }[level] + 1 * ObjectManager.Player.FlatMagicDamageMod;
                    return source.CalculateDamageOnUnit(target, DamageType.Magical, damage);
                }
            }
            if (source.ChampionName == "Draven")
            {
                {
                    var damage = new float[] { 175, 275, 375 }[level] + 1.1f * ObjectManager.Player.FlatPhysicalDamageMod;
                    return source.CalculateDamageOnUnit(target, DamageType.Physical, damage) * 0.7f;
                }
            }

            return 0;
        }

        private static void BaseUltCalcs(Recall recall)
        {
            float recallEndTime = recall.Started + recall.Duration;
            float travelTime = GetBaseUltTravelTime(Player);
            float timeLeft = recallEndTime - Core.GameTickCount;
            float delay = recallEndTime - travelTime;


            var spellDmg = GetBaseUltSpellDamage(recall.Unit, Player);
            bool collision = Program.BaseUltMenu["checkcollision"].Cast<CheckBox>().CurrentValue ? GetCollision(Player.ChampionName).Any() : false;
            var spellData = BaseUltSpells.Find(h => h.Name == Player.ChampionName);

            Chat.Print(String.Format("recallEndTime {0}   delay {1}   travelTime {2}", recallEndTime, delay, travelTime));

            if (travelTime <= recallEndTime && !collision && recall.Unit.Health < spellDmg
                && Program.BaseUltMenu["target" + recall.Unit.ChampionName].Cast<CheckBox>().CurrentValue
                && Program.BaseUltMenu["baseult"].Cast<CheckBox>().CurrentValue
                && !Program.BaseUltMenu["nobaseult"].Cast<KeyBind>().CurrentValue)
            {
                Chat.Print("Added");
                BaseUltUnits.Add(new BaseUltUnit(recall.Unit, delay + Core.GameTickCount));
            }
            else if (BaseUltUnits.Any(h => h.Unit.NetworkId == recall.Unit.NetworkId))
            {
                BaseUltUnits.Remove(BaseUltUnits.Find(h => h.Unit.NetworkId == recall.Unit.NetworkId));
            }
            
        }

        public static void Teleport_OnTeleport(Obj_AI_Base sender, Teleport.TeleportEventArgs args)
        {
            var enemy = EntityManager.Heroes.Enemies.FirstOrDefault(x => x.Equals(sender));

            if(enemy == null || args.Type != TeleportType.Recall)
                return;

            switch (args.Status)
            {
                case TeleportStatus.Start:
                {
                    Recalls.Add(new Recall(enemy, RecallStatus.Active){
                        Started = args.Start,
                        Duration = args.Duration
                    });
                    break;
                }

                case TeleportStatus.Abort:
                {
                    var abortedEnemy = Recalls.FirstOrDefault(x => x.Unit.Equals(enemy));

                    if(abortedEnemy == null)
                        return;

                    Recalls.Remove(abortedEnemy);
                    removeFromBaseUlt(abortedEnemy.Unit);
                    break;
                }

                case TeleportStatus.Finish:
                {
                    var finishedEnemy = Recalls.FirstOrDefault(x => x.Unit.Equals(enemy));

                    if (finishedEnemy == null)
                        return;

                    Recalls.Remove(finishedEnemy);
                    removeFromBaseUlt(finishedEnemy.Unit);
                    break;
                }
            }
        }

        private static void removeFromBaseUlt(AIHeroClient obj)
        {
            var target = BaseUltUnits.FirstOrDefault(x => x.Unit.Equals(obj));
            
            if (target == null)
                return;

            BaseUltUnits.Remove(target);
        }

        private static IEnumerable<Obj_AI_Base> GetCollision(string sourceName)
        {
            if (sourceName == "Ezreal")
                return new List<Obj_AI_Base>();

            var heroEntry = BaseUltSpells.First(x => x.Name == sourceName);
            Vector3 enemyBaseVec = ObjectManager.Get<Obj_SpawnPoint>().First(x => x.IsEnemy).Position;

            return (from unit in EntityManager.Heroes.Enemies.Where(h => ObjectManager.Player.Distance(h) < 2000)
                    let pred =
                        Prediction.Position.PredictLinearMissile(unit, 2000, (int)heroEntry.Radius, (int)heroEntry.Delay,
                            heroEntry.Speed, -1)
                    let endpos = ObjectManager.Player.ServerPosition.Extend(enemyBaseVec, 2000)
                    let projectOn = pred.UnitPosition.To2D().ProjectOn(ObjectManager.Player.ServerPosition.To2D(), endpos)
                    where projectOn.SegmentPoint.Distance(endpos) < (int)heroEntry.Radius + unit.BoundingRadius
                    select unit).Cast<Obj_AI_Base>().ToList();
        }

        public static void Drawing_OnPostReset(EventArgs args)
        {
            Text.OnResetDevice();
        }

        public static void Drawing_OnPreReset(EventArgs args)
        {
            Text.OnLostDevice();
        }
    }

    public class Recall
    {
        public Recall(AIHeroClient unit, RecallStatus status)
        {
            Unit = unit;
            Status = status;
        }

        public AIHeroClient Unit { get; set; }
        public RecallStatus Status { get; set; }
        public float Started { get; set; }
        public float Ended { get; set; }
        public float Duration { get; set; }
    }

    public class BaseUltUnit
    {
        public BaseUltUnit(AIHeroClient unit, float fireTime)
        {
            Unit = unit;
            FireTime = fireTime;
        }

        public AIHeroClient Unit { get; set; }
        public float FireTime { get; set; }
        public float LastSeen { get; set; }
    }

    public class BaseUltSpell
    {
        public BaseUltSpell(string name, SpellSlot slot, float delay, float speed, float radius, bool collision)
        {
            Name = name;
            Slot = slot;
            Delay = delay;
            Speed = speed;
            Radius = radius;
            Collision = collision;
        }

        public string Name { get; set; }
        public SpellSlot Slot { get; set; }
        public float Delay { get; set; }
        public float Speed { get; set; }
        public float Radius { get; set; }
        public bool Collision { get; set; }
    }

    public enum RecallStatus
    {
        Active,
        Inactive,
        Finished,
        Abort
    }
}