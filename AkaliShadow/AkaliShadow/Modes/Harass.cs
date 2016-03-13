using AkaliShadow.Utilities;
using EloBuddy;
using EloBuddy.SDK;
// Using the config like this makes your life easier, trust me
using Settings = AkaliShadow.Config.Modes.Harass;

namespace AkaliShadow.Modes
{
    public sealed class Harass : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on harass mode
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass);
        }

        public override void Execute()
        {
            AIHeroClient target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target != null)
            {
                if (Q.IsReady() && Player.Instance.Distance(target) <= Q.Range && Config.Modes.Harass.UseQ)
                {
                    Q.Cast(target);
                    Program.QInAir = true;
                }

                //If we can kill with E then we use, but if we can't and the target have the mark, we won't use E
                //to let akali proc the mark with AA. Shitty nerf, FU Rito.
                if (E.IsReady() && Player.Instance.Distance(target) <= E.Range
                    && (!Combat.HasBuff(target, "AkaliMota") || Player.Instance.GetSpellDamage(target, SpellSlot.E) >= target.Health) && Config.Modes.Harass.UseE)
                {
                    E.Cast();
                }
            }
        }
    }
}
