using EloBuddy;
using EloBuddy.SDK;
using JaKCass.Utilities;
using System.Collections.Generic;
using System.Linq;

// Using the config like this makes your life easier, trust me
using Settings = JaKCass.Config.Modes.Combo;
using System;

namespace JaKCass.Modes
{
    public sealed class Combo : ModeBase
    {
        public static int lastECast = 0;
        private static Random rnd;

        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on combo mode
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public override void Execute()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target == null)
                return;

            Combat.CastItems(target);

            if(Config.Skills.initDash)
                if (W.IsReady() && W.IsInRange(target) && Program.DashingChamps.Contains(target.ChampionName))
                    {
                        var WPred = W.GetPrediction(target);
                        if(WPred.HitChancePercent >= 70)
                            W.Cast(WPred.CastPosition);
                    }

            if (Config.Modes.Combo.UseQ)
            {
                rnd = new Random();
                if (Q.IsReady() && Q.IsInRange(target) && !Utilities.Combat.isPoisoned(target) && (((Program.lastQcastTime + rnd.Next(0, Config.Skills.qDelay)) <= Environment.TickCount) || !Config.Skills.legitQ))
                {
                    var QPred = Q.GetPrediction(target);
                    if (QPred.HitChancePercent >= 90)
                        Q.Cast(QPred.CastPosition);
                }
            }

            if(Config.Modes.Combo.UseE)
            {
                rnd = new Random();
                if ((Utilities.Combat.isPoisoned(target) || (Config.Skills.FastE && Program.QisCasting) || (target.Health <= Player.Instance.GetSpellDamage(target, SpellSlot.E))) && (((lastECast + rnd.Next(0, Config.Skills.eDelay)) <= Environment.TickCount) || !Config.Skills.legitE))
                {
                    E.Cast(target);
                    lastECast = Environment.TickCount;
                }
            }

            //If Q not ready and we dont care using W
            //OR we are not casting Q and the target is not poisoned and we keep W only for safeW
            if (Config.Modes.Combo.UseW)
                if((!Q.IsReady() && !Config.Skills.SafeW) || (!Program.QisCasting && !Utilities.Combat.isPoisoned(target) && Config.Skills.SafeW))
                    W.Cast(target);

            if (Config.Modes.Combo.UseR)
                if (!Q.IsReady() && !E.IsReady() && R.IsReady())
                {
                    var facing = EntityManager.Heroes.Enemies.Count(t => t.IsValidTarget(R.Range) && t.IsFacing(Player.Instance));
                    if (Config.Skills.rEnemiesSBTW <= facing)
                        R.Cast(target);
                }
        }
    }
}