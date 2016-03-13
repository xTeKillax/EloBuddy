using AkaliShadow.Utilities;
using EloBuddy;
using EloBuddy.SDK;
using System.Collections.Generic;
using System.Linq;

namespace AkaliShadow.Modes
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
            var useQi = Config.Modes.Farm.UseQ;
            var useEi = Config.Modes.Farm.UseE;
            var useQ = (useQi == 1 || useQi == 2);
            var useE = (useEi == 1 || useEi == 2);

            var minions = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition, Q.Range).OrderBy(a => a.Health);
            var minion = minions.FirstOrDefault();

            if (minion == null || !minion.IsValidTarget()) 
                return;

            if (Combat.HasBuff(minion, "AkaliMota"))
            {
                if(Player.Instance.GetAutoAttackRange() >= Player.Instance.Distance(minion))
                    Orbwalker.ForcedTarget = minion;
            }
            else
            {
                if (useQ && Q.IsReady())
                    Q.Cast(minion);

                if (useE && E.IsReady())
                    if (Player.Instance.Distance(minion) <= E.Range)
                        if (minions.Where(m => m.Distance(Player.Instance.Position) <= E.Range).Count() >= Config.Modes.Farm.hitCounter)
                            E.Cast();
            }
        }
    }
}
