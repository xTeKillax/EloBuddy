using JaKCass.Utilities;
using EloBuddy.SDK;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using System.Collections.Generic;
using System.Linq;


// ReSharper disable InconsistentNaming
// ReSharper disable MemberHidesStaticFromOuterClass
namespace JaKCass
{
    // I can't really help you with my layout of a good config class
    // since everyone does it the way they like it most, go checkout my
    // config classes I make on my GitHub if you wanna take over the
    // complex way that I use
    public static class Config
    {
        private const string MenuName = "JaKCass";

        private static Menu Menu;

        static Config()
        {
            // Initialize the menu
            Menu = MainMenu.AddMenu(MenuName, MenuName.ToLower());
            Menu.AddLabel("JaKCass - by BestAkaliAfrica");
            Menu.AddLabel("Say NO! to shitty-logic addons!");

            // Initialize the modes
            Modes.Initialize();
            Drawings.Initialize();
            Misc.Initialize();
        }

        public static void Initialize()
        {
        }






        #region "ModesMenu"
        public static class Modes
        {
            static Modes()
            {
                // Initialize all modes
                // Combo
                Combo.Initialize();

                Skills.Initialize();

                // Harass
                Harass.Initialize();

                // Farm
                Farm.Initialize();
            }

            public static void Initialize()
            {
            }

            public static class Combo
            {
                private static Menu ComboMenu;

                public static bool UseQ
                {
                    get { return ComboMenu["comboUseQ"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseW
                {
                    get { return ComboMenu["comboUseW"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseE
                {
                    get { return ComboMenu["comboUseE"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseR
                {
                    get { return ComboMenu["comboUseR"].Cast<CheckBox>().CurrentValue; }
                }

                static Combo()
                {
                    // Initialize the menu
                    ComboMenu = Config.Menu.AddSubMenu("Combo");
                    ComboMenu.Add("comboUseQ", new CheckBox("Use Q"));
                    ComboMenu.Add("comboUseW", new CheckBox("Use W"));
                    ComboMenu.Add("comboUseE", new CheckBox("Use E"));
                    ComboMenu.Add("comboUseR", new CheckBox("Use R"));
                }

                public static void Initialize()
                {
                }
            }

            public static class Harass
            {
                private static Menu HarassMenu;

                public static bool UseQ
                {
                    get { return HarassMenu["harassUseQ"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseE
                {
                    get { return HarassMenu["harassUseE"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool HarassActiveT
                {
                    get { return HarassMenu["HarassActiveT"].Cast<KeyBind>().CurrentValue; }
                }

                static Harass()
                {
                    // Initialize the menu
                    HarassMenu = Config.Menu.AddSubMenu("Harass");
                    HarassMenu.Add("harassUseQ", new CheckBox("Use Q", false));
                    HarassMenu.Add("harassUseE", new CheckBox("Use E"));
                    HarassMenu.Add("HarassActiveT", new KeyBind("Harass toggle", false, KeyBind.BindTypes.PressToggle, 'Y'));
                }

                public static void Initialize()
                {
                }
            }

            public static class Farm
            {
                private static Menu FarmMenu;

                public static bool UseQ
                {
                    get { return FarmMenu["farmUseQ"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseE
                {
                    get { return FarmMenu["farmUseE"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseEfinish
                {
                    get { return FarmMenu["farmUseEfinish"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseQlh
                {
                    get { return FarmMenu["farmUseQlasthit"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseElh
                {
                    get { return FarmMenu["farmUseElasthit"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseElhfinish
                {
                    get { return FarmMenu["farmUseElasthitfinish"].Cast<CheckBox>().CurrentValue; }
                }


                static Farm()
                {
                    FarmMenu = Config.Menu.AddSubMenu("Farm");
                    FarmMenu.AddGroupLabel("Clear");
                    FarmMenu.Add("farmUseQ", new CheckBox("Use Q"));
                    FarmMenu.Add("farmUseE", new CheckBox("Use E"));
                    FarmMenu.Add("farmUseEfinish", new CheckBox("Use E only to kill"));

                    FarmMenu.AddGroupLabel("Lasthit");
                    FarmMenu.Add("farmUseQlasthit", new CheckBox("Use Q"));
                    FarmMenu.Add("farmUseElasthit", new CheckBox("Use E"));
                    FarmMenu.Add("farmUseElasthitfinish", new CheckBox("Use E only to kill"));
                }

                public static void Initialize()
                {
                }
            }
        }
        #endregion



        #region "Skills"

        public static class Skills
        {
            private static Menu SkillsMenu;

            public static bool SafeW
            {
                get { return SkillsMenu["SafeW"].Cast<CheckBox>().CurrentValue; }
            }
            public static bool initDash
            {
                get { return SkillsMenu["initDash"].Cast<CheckBox>().CurrentValue; }
            }
            public static bool eKS
            {
                get { return SkillsMenu["eKS"].Cast<CheckBox>().CurrentValue; }
            }
            public static bool FastE
            {
                get { return SkillsMenu["FastE"].Cast<CheckBox>().CurrentValue; }
            }
            public static bool AssistedUlt
            {
                get { return SkillsMenu["AssistedUlt"].Cast<KeyBind>().CurrentValue; }
            }
            public static bool AutoTurretUlt
            {
                get { return SkillsMenu["AutoTurretUlt"].Cast<CheckBox>().CurrentValue; }
            }
            public static int rEnemiesSBTW
            {
                get { return SkillsMenu["rEnemiesSBTW"].Cast<Slider>().CurrentValue; }
            }
            public static int rEnemiesAUTO
            {
                get { return SkillsMenu["rEnemiesAUTO"].Cast<Slider>().CurrentValue; }
            }
            public static bool NinjaInteruption
            {
                get { return SkillsMenu["NinjaInteruption"].Cast<CheckBox>().CurrentValue; }
            }
            public static bool legitE
            {
                get { return SkillsMenu["legitE"].Cast<CheckBox>().CurrentValue; }
            }
            public static int eDelay
            {
                get { return SkillsMenu["eDelay"].Cast<Slider>().CurrentValue; }
            }
            public static bool legitQ
            {
                get { return SkillsMenu["legitQ"].Cast<CheckBox>().CurrentValue; }
            }
            public static int qDelay
            {
                get { return SkillsMenu["qDelay"].Cast<Slider>().CurrentValue; }
            }

            static Skills()
            {
                SkillsMenu = Config.Menu.AddSubMenu("Skills settings");

                SkillsMenu.AddGroupLabel("Q Skill");
                SkillsMenu.Add("legitQ", new CheckBox("humanize Q"));
                SkillsMenu.Add("qDelay", new Slider("Q max delay", 200, 120, 800));

                SkillsMenu.AddSeparator();

                SkillsMenu.AddGroupLabel("W Skill");
                SkillsMenu.Add("SafeW", new CheckBox("Use W only if Q fail"));
                SkillsMenu.Add("initDash", new CheckBox("Use W on champ with dash"));

                SkillsMenu.AddSeparator();

                SkillsMenu.AddGroupLabel("E Skill");
                SkillsMenu.Add("eKS", new CheckBox("Kill steal with E"));
                SkillsMenu.Add("FastE", new CheckBox("Cast E before Q hit"));
                SkillsMenu.Add("legitE", new CheckBox("humanize E"));
                SkillsMenu.Add("eDelay", new Slider("E max delay", 460, 0, 1000));

                SkillsMenu.AddSeparator();

                SkillsMenu.AddGroupLabel("R Skill");
                SkillsMenu.Add("AssistedUlt", new KeyBind("Use assisted ultimate (disable R smartcast !)", false, KeyBind.BindTypes.HoldActive, 'R'));
                SkillsMenu.Add("AutoTurretUlt", new CheckBox("Auto ultimate under turret"));
                SkillsMenu.Add("NinjaInteruption", new CheckBox("Interupt skills with R"));

                SkillsMenu.AddLabel("R trigger settings");
                SkillsMenu.Add("rEnemiesSBTW", new Slider("R total weight (for combo mode)", 2, 1, 5));
                SkillsMenu.Add("rEnemiesAUTO", new Slider("R total weight (for automatic R)", 2, 1, 5));
            }

            public static void Initialize()
            {
            }
        }
        #endregion



        #region "DrawingMenu"
        public static class Drawings
        {
            private static Menu DrawingsMenu;

            public static bool Qrange
            {
                get { return DrawingsMenu["Qrange"].Cast<CheckBox>().CurrentValue; }
            }
            public static bool Wrange
            {
                get { return DrawingsMenu["Wrange"].Cast<CheckBox>().CurrentValue; }
            }
            public static bool Erange
            {
                get { return DrawingsMenu["Erange"].Cast<CheckBox>().CurrentValue; }
            }
            public static bool Rrange
            {
                get { return DrawingsMenu["Rrange"].Cast<CheckBox>().CurrentValue; }
            }

            static Drawings()
            {
                // Initialize the menu
                DrawingsMenu = Config.Menu.AddSubMenu("Drawing");

                DrawingsMenu.Add("Qrange", new CheckBox("Q Range", true));     //Skyblue 255
                DrawingsMenu.Add("Erange", new CheckBox("E Range", true));     //Black 255
                DrawingsMenu.Add("Rrange", new CheckBox("R Range", true));     //Black 255

                /*CheckBox fullComboDamageItem = DrawingsMenu.Add("FullComboDraw", new CheckBox("Draw fullCombo damage", true));


                DamageIndicator.DamageToUnit = Combat.GetComboDamage;
                DamageIndicator.Enabled = FullComboDraw;

                fullComboDamageItem.OnValueChange += delegate(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs ValChangeArgs)
                {
                    DamageIndicator.Enabled = ValChangeArgs.NewValue;
                };*/
            }

            public static void Initialize()
            {
            }
        }
        #endregion







        #region "MiscMenu"
        public static class Misc
        {
            private static Menu MiscMenu;

            public static bool AutoLevelUp
            {
                get { return MiscMenu["AutoLevelUp"].Cast<CheckBox>().CurrentValue; }
            }
            public static bool UseAA
            {
                get { return MiscMenu["UseAA"].Cast<CheckBox>().CurrentValue; }
            }

            static Misc()
            {
                // Initialize the menu
                MiscMenu = Config.Menu.AddSubMenu("Misc");

                MiscMenu.Add("UseAA", new CheckBox("Use AA"));
                MiscMenu.Add("AutoLevelUp", new CheckBox("Auto lvel up", false));
            }

            public static void Initialize()
            {
            }
        }
        #endregion
    }
}
