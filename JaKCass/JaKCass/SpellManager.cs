using EloBuddy;
using EloBuddy.SDK;

namespace JaKCass
{
    public static class SpellManager
    {
        // You will need to edit the types of spells you have for each champ as they
        // don't have the same type for each champ, for example Xerath Q is chargeable,
        // right now it's  set to Active.
        public static Spell.Skillshot Q { get; private set; }
        public static Spell.Skillshot W { get; private set; }
        public static Spell.Targeted E { get; private set; }
        public static Spell.Skillshot R { get; private set; }

        static SpellManager()
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 850, EloBuddy.SDK.Enumerations.SkillShotType.Circular, 400, null, 130);
            W = new Spell.Skillshot(SpellSlot.W, 850, EloBuddy.SDK.Enumerations.SkillShotType.Circular, 250, 2500, 125);
            E = new Spell.Targeted(SpellSlot.E, 700){ CastDelay = 125 };
            R = new Spell.Skillshot(SpellSlot.R, 825, EloBuddy.SDK.Enumerations.SkillShotType.Cone, 500, null, 80);
        }

        public static void Initialize()
        {
            // Let the static initializer do the job, this way we avoid multiple init calls aswell
        }
    }
}