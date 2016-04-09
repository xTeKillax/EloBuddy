using JaKCass.Utilities;
using EloBuddy;
using EloBuddy.SDK;
using System.Collections.Generic;
using System.Linq;

namespace JaKCass.Modes
{
    public sealed class JungleClear : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on jungleclear mode
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.JungleClear);
        }

        public override void Execute()
        {
            //var useQi = Config.Modes.Farm.UseQ;
            //var useEi = Config.Modes.Farm.UseE;
            //var useQ = (useQi == 1 || useQi == 2);
            //var useE = (useEi == 1 || useEi == 2);

            //foreach (Obj_AI_Base minion in EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition, Q.Range))
            //{
            //    if (useQ && Q.IsReady())
            //    {
            //        Q.Cast(minion);
            //        if (Combat.HasBuff(minion, "AkaliMota") && Player.Instance.GetAutoAttackRange() >= Player.Instance.Distance(minion))
            //            Orbwalker.ForcedTarget = minion;
            //    }

            //    if (useE && E.IsReady())
            //        if (Player.Instance.Distance(minion) <= E.Range)
            //            if (EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition, E.Range).Count() >= Config.Modes.Farm.hitCounter)
            //                E.Cast();
            //}
        }
    }
}
