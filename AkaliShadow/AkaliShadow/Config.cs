using AkaliShadow.Utilities;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

// ReSharper disable InconsistentNaming
// ReSharper disable MemberHidesStaticFromOuterClass
namespace AkaliShadow
{
    // I can't really help you with my layout of a good config class
    // since everyone does it the way they like it most, go checkout my
    // config classes I make on my GitHub if you wanna take over the
    // complex way that I use
    public static class Config
    {
        private const string MenuName = "Akali Shadow";

        private static Menu Menu;

        static Config()
        {
            // Initialize the menu
            Menu = MainMenu.AddMenu(MenuName, MenuName.ToLower());
            Menu.AddLabel("Akali Shadow - by BestAkaliAfrica");
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
                public static bool UseE
                {
                    get { return ComboMenu["comboUseE"].Cast<CheckBox>().CurrentValue; }
                }
                public static bool UseR
                {
                    get { return ComboMenu["comboUseR"].Cast<CheckBox>().CurrentValue; }
                }
                public static int Rdelay
                {
                    get { return ComboMenu["Rdelay"].Cast<Slider>().CurrentValue; }
                }
                public static bool Rrnd
                {
                    get { return ComboMenu["Rrnd"].Cast<CheckBox>().CurrentValue; }
                }

                static Combo()
                {
                    // Initialize the menu
                    ComboMenu = Config.Menu.AddSubMenu("Combo");
                    ComboMenu.Add("comboUseQ", new CheckBox("Use Q"));
                    ComboMenu.Add("comboUseE", new CheckBox("Use E"));
                    ComboMenu.Add("comboUseR", new CheckBox("Use R"));
                    ComboMenu.Add("Rdelay", new Slider("Delay between R", 1000, 0, 2000));
                    ComboMenu.Add("Rrnd", new CheckBox("Add random R delay to existing delay"));
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

                public static int UseQ
                {
                    get { return FarmMenu["farmUseQ"].Cast<Slider>().CurrentValue; }
                }
                public static int UseE
                {
                    get { return FarmMenu["farmUseE"].Cast<Slider>().CurrentValue; }
                }
                public static int hitCounter
                {
                    get { return FarmMenu["hitCounter"].Cast<Slider>().CurrentValue; }
                }

                static Farm()
                {
                    // Initialize the menu
                    FarmMenu = Config.Menu.AddSubMenu("Farm");
                    FarmMenu.Add("farmUseQ", new Slider("Use Q : [Freeze/LaneClear/Both/Never]", 2, 0, 3));
                    FarmMenu.Add("farmUseE", new Slider("Use E : [Freeze/LaneClear/Both/Never]", 1, 0, 3));
                    FarmMenu.Add("hitCounter", new Slider("Use E if will hit min", 3, 1, 6));
                }

                public static void Initialize()
                {
                }
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
            public static bool wCountdown
            {
                get { return DrawingsMenu["wCountdown"].Cast<CheckBox>().CurrentValue; }
            }
            public static bool FullComboDraw
            {
                get { return DrawingsMenu["FullComboDraw"].Cast<CheckBox>().CurrentValue; }
            }

            /*public static SharpDX.Color RColor
                {
                    get { return _rColor.GetSharpColor(); }
                }*/

            static Drawings()
            {
                // Initialize the menu
                DrawingsMenu = Config.Menu.AddSubMenu("Drawing");

                DrawingsMenu.Add("Qrange", new CheckBox("Q Range", true));     //Skyblue 255
                DrawingsMenu.Add("Erange", new CheckBox("E Range", false));    //LimeGreen 150
                DrawingsMenu.Add("Rrange", new CheckBox("R Range", true));     //Black 255
                DrawingsMenu.Add("wCountdown", new CheckBox("Draw W countdown", true));
                CheckBox fullComboDamageItem = DrawingsMenu.Add("FullComboDraw", new CheckBox("Draw fullCombo damage", true));


                DamageIndicator.DamageToUnit = Combat.GetComboDamage;
                DamageIndicator.Enabled = FullComboDraw;

                fullComboDamageItem.OnValueChange += delegate(ValueBase<bool> sender, ValueBase<bool>.ValueChangeArgs ValChangeArgs)
                {
                    DamageIndicator.Enabled = ValChangeArgs.NewValue;
                };

                /*
                  healthColor = new ColorConfig(DrawMenu, "healthColorConfig", Color.Orange, "Color Damage Indicator:");
                    _qColor = new ColorConfig(DrawMenu, "qColorConfig", Color.Blue, "Color Q:");
                    _wColor = new ColorConfig(DrawMenu, "wColorConfig", Color.Red, "Color W:");
                    _eColor = new ColorConfig(DrawMenu, "eColorConfig", Color.DeepPink, "Color E:");
                    _rColor = new ColorConfig(DrawMenu, "rColorConfig", Color.Yellow, "Color R:");
                 */
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

            public static bool wSpotActive
            {
                get { return MiscMenu["wSpotActive"].Cast<KeyBind>().CurrentValue; }
            }
            public static int antiGapCloser
            {
                get { return MiscMenu["antiGapCloser"].Cast<Slider>().CurrentValue; }
            }
            public static bool AutoLevelUp
            {
                get { return MiscMenu["AutoLevelUp"].Cast<CheckBox>().CurrentValue; }
            }

            static Misc()
            {
                // Initialize the menu
                MiscMenu = Config.Menu.AddSubMenu("Misc");

                MiscMenu.Add("wSpotActive", new KeyBind("W perfect spot (press once and left click)", false, KeyBind.BindTypes.HoldActive, 'W'));
                MiscMenu.Add("antiGapCloser", new Slider("Use W on gapcloser [Targeted Only/Skillshot Only/Both/Off]", 2, 0, 3));
                MiscMenu.AddSeparator();
                MiscMenu.AddLabel("Press flee key to use intelligent W+R jump");
                MiscMenu.AddSeparator();
                MiscMenu.Add("AutoLevelUp", new CheckBox("Auto lvel up", false));
            }

            public static void Initialize()
            {
            }
        }
        #endregion
    }
}
