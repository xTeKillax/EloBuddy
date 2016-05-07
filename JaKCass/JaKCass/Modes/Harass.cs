using JaKCass.Utilities;
using EloBuddy;
using EloBuddy.SDK;
// Using the config like this makes your life easier, trust me
using Settings = JaKCass.Config.Modes.Harass;
using System;

namespace JaKCass.Modes
{
    public sealed class Harass : ModeBase
    {
        private static int lastECast = 0;
        private static Random rnd;

        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on harass mode
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
        }

        public override void Execute()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target == null)
                return;


            if (Config.Modes.Harass.UseQ)
            {
                rnd = new Random();
                if (Q.IsReady() && Q.IsInRange(target) && !Utilities.Combat.isPoisoned(target) && (((Program.lastQcastTime + rnd.Next(0, Config.Skills.qDelay)) <= Environment.TickCount) || !Config.Skills.legitQ))
                {
                    var QPred = Q.GetPrediction(target);
                    if (QPred.HitChancePercent >= 85)
                        Q.Cast(QPred.CastPosition);
                }
            }

            if (Config.Modes.Harass.UseE)
            {
                rnd = new Random();
                if ((Utilities.Combat.isPoisoned(target) || (Config.Skills.FastE && Program.QisCasting) || (target.Health <= Player.Instance.GetSpellDamage(target, SpellSlot.E))) && (((lastECast + rnd.Next(0, Config.Skills.eDelay)) <= Environment.TickCount) || !Config.Skills.legitE))
                {
                    E.Cast(target);
                    lastECast = Environment.TickCount;
                }
            }
        }
    }
}
