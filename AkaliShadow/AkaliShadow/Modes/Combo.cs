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
        private static int randomRdelay = (new System.Random()).Next(100, 1500);

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

            double eDamage = Player.Instance.GetSpellDamage(target, SpellSlot.E);

            Combat.CastItems(target);

            //If marked we will do everything to proc it
            if (Combat.HasBuff(target, "AkaliMota"))
            {
                if(Player.Instance.Distance(target) <= Player.Instance.GetAutoAttackRange())
                {
                    Orbwalker.ForcedTarget = target;
                }
                else
                {
                    //if E he's not in AA range but in E range and E can kill him
                    if (E.IsReady() && E.IsInRange(target) && (Player.Instance.GetSpellDamage(target, SpellSlot.E) >= target.Health) && Config.Modes.Combo.UseE)
                        E.Cast();

                    //Jump with R to gapclose
                    if (R.IsInRange(target) && R.IsReady() && Config.Modes.Combo.UseR && (lastRattempt + Config.Modes.Combo.Rdelay + (Config.Modes.Combo.Rrnd ? 0 : randomRdelay)) <= Environment.TickCount)
                    {
                        R.Cast(target);
                        lastRattempt = Environment.TickCount;
                        if (Config.Modes.Combo.Rrnd) randomRdelay = (new System.Random()).Next(100, 1500);
                    }
                }
            }
            else
            {
                //Mark Q on enemy if not marked
                if (Q.IsReady() && Q.IsInRange(target) && Config.Modes.Combo.UseQ)
                {
                    Q.Cast(target);
                    Program.QInAir = true;
                }

                //If Q in air, we have time to E before we force AA, so lets do it for a faster combo only if we have enough energy to jump
                if (E.IsReady() && E.IsInRange(target) && (Player.Instance.GetSpellDamage(target, SpellSlot.E) >= target.Health) && Config.Modes.Combo.UseE)
                    E.Cast();

                //Jump with R if dist > E.Range and have enough energy for R+E
                //Jump with R if can kill him with R+AA
                if (R.IsInRange(target) &&
                    ((!(E.IsInRange(target)) && Combat.HasEnergyFor(false, false, true, true)) || 
                    (Player.Instance.GetSpellDamage(target, SpellSlot.R) + Player.Instance.GetAutoAttackDamage(target, true)) >= target.Health) &&
                    R.IsReady() && Config.Modes.Combo.UseR && (lastRattempt + Config.Modes.Combo.Rdelay + (Config.Modes.Combo.Rrnd ? 0 : randomRdelay)) <= Environment.TickCount)
                {
                    R.Cast(target);
                    lastRattempt = Environment.TickCount;
                    if (Config.Modes.Combo.Rrnd) randomRdelay = (new System.Random()).Next(100, 1500);
                }
            }
        }
    }
}
