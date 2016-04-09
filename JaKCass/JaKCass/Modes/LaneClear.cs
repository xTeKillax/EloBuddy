using JaKCass.Utilities;
using EloBuddy;
using EloBuddy.SDK;
using System.Collections.Generic;
using System.Linq;

namespace JaKCass.Modes
{
    public sealed class LaneClear : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on laneclear mode
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear);
        }

        public override void Execute()
        {
            var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition, E.Range).OrderBy(a => a.Health);
            var minion = minions.FirstOrDefault();

            if (minion == null || !minion.IsValidTarget()) 
                return;

            if(Config.Modes.Farm.UseQ)
                if(Q.IsReady() && Q.IsInRange(minion) && !Utilities.Combat.isPoisoned(minion))
                    Q.Cast(minion);

            if (Config.Modes.Farm.UseE)
                if (E.IsReady() && E.IsInRange(minion) && Utilities.Combat.isPoisoned(minion))
                    if (Config.Modes.Farm.UseEfinish)
                        if (minion.Health <= Player.Instance.GetSpellDamage(minion, SpellSlot.E))
                            E.Cast(minion);
                        else
                            Orbwalker.ForcedTarget = minion;
                    else
                        E.Cast(minion);
        }
    }
}