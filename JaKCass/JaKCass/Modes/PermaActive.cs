using EloBuddy;
using EloBuddy.SDK;
using JaKCass.Utilities;
using System;
using System.Linq;


namespace JaKCass.Modes
{
    public sealed class PermaActive : ModeBase
    {
        private static int autoRtick = 0;
        public override bool ShouldBeExecuted()
        {
            // Since this is permaactive mode, always execute the loop
            return true;
        }

        public override void Execute()
        {
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

            if (target == null)
                return;

            if (Config.Skills.AssistedUlt)
                if (R.IsReady())
                    if (EntityManager.Heroes.Enemies.Count(t => t.IsValidTarget(R.Range) && t.IsFacing(Player.Instance)) != 0)
                        R.Cast(target);

            if (Environment.TickCount - autoRtick >= 200)
            {
                autoRtick = Environment.TickCount;

                if (Config.Skills.AutoTurretUlt)
                    if (target.IsUnderEnemyturret() && R.IsReady() && R.IsInRange(target) && target.IsFacing(Player.Instance))
                        R.Cast(target);

                if (R.IsReady())
                {
                    var facing = EntityManager.Heroes.Enemies.Where(t => t.IsValidTarget(R.Range) && t.IsFacing(Player.Instance));
                    if (Config.Skills.rEnemiesAUTO <= facing.Count())
                        R.Cast(facing.FirstOrDefault());
                }

                if (Config.Skills.eKS)
                    if (E.IsReady() && E.IsInRange(target) && target.Health <= Player.Instance.GetSpellDamage(target, SpellSlot.E))
                        E.Cast(target);
            }
        }
    }
}
