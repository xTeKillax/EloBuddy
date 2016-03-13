using EloBuddy;
using EloBuddy.SDK;
using AkaliShadow.Utilities;

// Using the config like this makes your life easier, trust me
using Settings = AkaliShadow.Config.Modes.Combo;
using System;

namespace AkaliShadow.Modes
{
    public sealed class Combo : ModeBase
    {
        private static int lastRattempt;

        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on combo mode
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo);
        }

        public override void Execute()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);

            if (target == null)
                return;

            double eDamage = Player.Instance.GetSpellDamage(target, SpellSlot.E);

            Combat.CastItems(target);

            if (Combat.HasBuff(target, "AkaliMota"))
            {
                if(Player.Instance.Distance(target) <= Player.Instance.GetAutoAttackRange())
                {
                    Orbwalker.ForcedTarget = target;
                }
                else
                {
                    //Jump with R if dist > AARange and have enough energy for R+E
                    if (Player.Instance.Distance(target) <= R.Range && Combat.HasEnergyFor(false, false, false, true) && R.IsReady() && Config.Modes.Combo.UseR && (lastRattempt + Config.Modes.Combo.Rdelay) <= Environment.TickCount)
                    {
                        R.Cast(target);
                        lastRattempt = Environment.TickCount;
                    }
                }
            }
            else
            {
                //Mark Q on enemy if not marked
                if (Q.IsReady() && Player.Instance.Distance(target) <= Q.Range && !Combat.HasBuff(target, "AkaliMota") && Config.Modes.Combo.UseQ)
                {
                    Q.Cast(target);
                    Program.QInAir = true;
                }

                //Jump with R if dist > E.Range and have enough energy for R+E
                if (Player.Instance.Distance(target) <= R.Range
                    && ((Player.Instance.Distance(target) > E.Range && (Combat.HasEnergyFor(false, false, true, true) || Combat.HasBuff(target, "AkaliMota")))
                    || (Player.Instance.GetSpellDamage(target, SpellSlot.R) + Player.Instance.GetAutoAttackDamage(target, true)) >= target.Health)
                    && R.IsReady() && Config.Modes.Combo.UseR && (lastRattempt + Config.Modes.Combo.Rdelay) <= Environment.TickCount)
                {
                    R.Cast(target);
                    lastRattempt = Environment.TickCount;
                }
            }
        }
    }
}
