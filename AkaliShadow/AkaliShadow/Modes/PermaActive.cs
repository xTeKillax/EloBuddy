using EloBuddy;
using EloBuddy.SDK;
namespace AkaliShadow.Modes
{
    public sealed class PermaActive : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            // Since this is permaactive mode, always execute the loop
            return true;
        }

        public override void Execute()
        {
            if(Config.Modes.Harass.HarassActiveT)
            {
                var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);

                if (target == null)
                    return;

                if (Q.IsReady() && Q.IsInRange(target) && Config.Modes.Harass.UseQ)
                {
                    Q.Cast(target);
                    Program.QInAir = true;
                }

                if (E.IsReady() && E.IsInRange(target) && Config.Modes.Harass.UseE)
                    E.Cast();
            }
        }
    }
}
