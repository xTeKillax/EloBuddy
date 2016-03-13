using System;
using System.Collections.Generic;
using AkaliShadow.Modes;
using EloBuddy;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Utils;
using EloBuddy.SDK;

namespace AkaliShadow
{
    public static class ModeManager
    {
        private static List<ModeBase> Modes { get; set; }
        private static int[] abilitySequence = new int[] { 1, 2, 1, 3, 1, 4, 1, 3, 1, 3, 4, 3, 3, 2, 2, 4, 2, 2 };

        static ModeManager()
        {
            // Initialize properties
            Modes = new List<ModeBase>();

            // Load all modes manually since we are in a sandbox which does not allow reflection
            // Order matter here! You would want something like PermaActive being called first
            Modes.AddRange(new ModeBase[]
            {
                new PermaActive(),
                new Combo(),
                new Harass(),
                new LaneClear(),
                new JungleClear(),
                new LastHit(),
                new Flee()
            });

            // Listen to events we need
            Game.OnTick += OnTick;
        }

        public static void Initialize()
        {
            // Let the static initializer do the job, this way we avoid multiple init calls aswell
        }

        private static void OnTick(EventArgs args)
        {
            // Execute all modes
            Modes.ForEach(mode =>
            {
                try
                {
                    // Precheck if the mode should be executed
                    if (mode.ShouldBeExecuted())
                    {
                        // Execute the mode
                        mode.Execute();
                    }
                }
                catch (Exception e)
                {
                    // Please enable the debug window to see and solve the exceptions that might occur!
                    Logger.Log(LogLevel.Error, "Error executing mode '{0}'\n{1}", mode.GetType().Name, e);
                }
            });

            if(Config.Misc.AutoLevelUp)
            {
                //Level up credits to dakotasblack
                int qL = Player.Instance.Spellbook.GetSpell(SpellSlot.Q).Level;
                int wL = Player.Instance.Spellbook.GetSpell(SpellSlot.W).Level;
                int eL = Player.Instance.Spellbook.GetSpell(SpellSlot.E).Level;
                int rL = Player.Instance.Spellbook.GetSpell(SpellSlot.R).Level;
                if (qL + wL + eL + rL < Player.Instance.Level)
                {
                    int[] level = new int[] { 0, 0, 0, 0 };
                    for (int i = 0; i < Player.Instance.Level; i++)
                    {
                        level[abilitySequence[i] - 1] = level[abilitySequence[i] - 1] + 1;
                    }
                    if (qL < level[0]) Player.Instance.Spellbook.LevelSpell(SpellSlot.Q);
                    if (wL < level[1]) Player.Instance.Spellbook.LevelSpell(SpellSlot.W);
                    if (eL < level[2]) Player.Instance.Spellbook.LevelSpell(SpellSlot.E);
                    if (rL < level[3]) Player.Instance.Spellbook.LevelSpell(SpellSlot.R);

                }
            }
        }
    }
}
