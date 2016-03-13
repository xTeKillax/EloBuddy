using AkaliShadow.Utilities;
using EloBuddy;
using EloBuddy.SDK;
using System.Collections.Generic;
using System.Linq;

namespace AkaliShadow.Modes
{
    public sealed class LastHit : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on lasthit mode
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit);
        }

        public override void Execute()
        {
            var useQi = Config.Modes.Farm.UseQ;
            var useEi = Config.Modes.Farm.UseE;
            var useQ = (useQi == 0 || useQi == 2);
            var useE = (useEi == 0 || useEi == 2);

            foreach (Obj_AI_Base minion in EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Player.Instance.ServerPosition, Q.Range))
            {
                if (useQ && Q.IsReady())
                {
                    //Q kill him or Q+Proc kill him.
                    if (minion.Health <= Player.Instance.GetSpellDamage(minion, SpellSlot.Q) || (minion.Health <= (Player.Instance.GetSpellDamage(minion, SpellSlot.Q) + Player.Instance.GetSpellDamage(minion, SpellSlot.Q, DamageLibrary.SpellStages.Detonation)) && (minion.Health > Player.Instance.GetSpellDamage(minion, SpellSlot.Q)) && Player.Instance.Distance(minion) <= Player.Instance.GetAutoAttackRange()))
                    {
                        Q.Cast(minion);
                    }

                    if (Combat.HasBuff(minion, "AkaliMota") && Player.Instance.GetAutoAttackRange() >= Player.Instance.Distance(minion))
                        Orbwalker.ForcedTarget = minion;
                }

                if (useE && E.IsReady())
                    if (Player.Instance.Distance(minion) <= E.Range)
                        if (minion.Health <= Player.Instance.GetSpellDamage(minion, SpellSlot.E))
                            E.Cast();
            }
        }
    }
}
