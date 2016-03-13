using AkaliShadow.Utilities;
using EloBuddy;
using EloBuddy.SDK;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AkaliShadow.Modes
{
    public sealed class Flee : ModeBase
    {
        public override bool ShouldBeExecuted()
        {
            // Only execute this mode when the orbwalker is on flee mode
            return Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Flee);
        }

        public override void Execute()
        {
            Vector2 escape_pos = Player.Instance.Position.Extend(Game.CursorPos, R.Range);

            EloBuddy.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            var creepNear = EntityManager.MinionsAndMonsters.GetLaneMinions(EntityManager.UnitTeam.Enemy, Game.CursorPos, R.Range + 50);
            var jungleNear = EntityManager.MinionsAndMonsters.GetJungleMonsters(Game.CursorPos, 200);
            if (creepNear.Count() < 1 && jungleNear.Count() < 1)
            {
                if (!Map.IsWall(escape_pos) && Map.IsWallBetween(Player.Instance.Position, escape_pos.To3D()) && Player.Instance.Distance(Game.CursorPos) < R.Range)
                    if (W.IsReady())
                        W.Cast(Player.Instance.Position.Extend(Game.CursorPos, W.Range).To3D());
            }
            else
            {
                R.Cast(creepNear.Count() < 1 ? jungleNear.FirstOrDefault() : creepNear.FirstOrDefault());
            }
        }
    }
}
