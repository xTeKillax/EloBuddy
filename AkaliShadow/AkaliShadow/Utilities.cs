using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Rendering;
using System.Drawing;
using SharpDX;

namespace AkaliShadow.Utilities
{
    public static class Combat
    {
        public static bool HasBuff(Obj_AI_Base target, string buffName)
        {
            return target.Buffs.Any(buff => buff.Name == buffName);
        }

        public static void CastItems(AIHeroClient target)
        {
            foreach (var item in Player.Instance.InventoryItems)
            {
                switch (item.Id)
                {
                    case ItemId.Hextech_Gunblade: //HexTech
                        if (Item.CanUseItem((int)ItemId.Hextech_Gunblade)) Item.UseItem((int)ItemId.Hextech_Gunblade, target);
                        break;
                    case ItemId.Bilgewater_Cutlass: //BwC
                        if (Item.CanUseItem((int)ItemId.Bilgewater_Cutlass)) Item.UseItem((int)ItemId.Bilgewater_Cutlass, target);
                        break;
                    case ItemId.Blade_of_the_Ruined_King:
                        if (Item.CanUseItem((int)ItemId.Blade_of_the_Ruined_King)) Item.UseItem((int)ItemId.Blade_of_the_Ruined_King, target);
                        break;
                    case ItemId.Tiamat_Melee_Only:
                        if (Item.CanUseItem((int)ItemId.Tiamat_Melee_Only) && target.IsValidTarget(400)) Item.UseItem((int)ItemId.Tiamat_Melee_Only, target);
                        break;
                    case ItemId.Ravenous_Hydra_Melee_Only:
                        if (Item.CanUseItem((int)ItemId.Ravenous_Hydra_Melee_Only) && target.IsValidTarget(400)) Item.UseItem((int)ItemId.Ravenous_Hydra_Melee_Only, target);
                        break;
                    case ItemId.Titanic_Hydra:
                        if (Item.CanUseItem((int)ItemId.Titanic_Hydra) && target.IsValidTarget(Player.Instance.AttackRange)) Item.UseItem((int)ItemId.Titanic_Hydra, target);
                        break;
                }
            }
        }

        public static bool HasEnergyFor(bool q, bool w, bool e, bool r)
        {
            float totalCost = 0;

            if (q)
                totalCost += Player.Instance.Spellbook.GetSpell(SpellSlot.Q).SData.Mana;
            if (w)
                totalCost += Player.Instance.Spellbook.GetSpell(SpellSlot.W).SData.Mana;
            if (e)
                totalCost += Player.Instance.Spellbook.GetSpell(SpellSlot.E).SData.Mana;
            if (r)
                totalCost += Player.Instance.Spellbook.GetSpell(SpellSlot.R).SData.Mana;

            if (Player.Instance.Mana >= totalCost)
                return true;
            else
                return false;
        }

        private static double CalcItemsDmg(AIHeroClient target)
        {
            double result = 0d;
            foreach (var item in Player.Instance.InventoryItems)
                switch (item.Id)
                {
                    case ItemId.Lich_Bane: // LichBane
                        if (Item.CanUseItem((int)ItemId.Lich_Bane))
                            result += Player.Instance.BaseAttackDamage * 0.75 + Player.Instance.FlatMagicDamageMod * 0.5;
                        break;
                    case ItemId.Sheen: //Sheen
                        if (Item.CanUseItem((int)ItemId.Sheen))
                            result += Player.Instance.BaseAttackDamage;
                        break;
                    case ItemId.Bilgewater_Cutlass: //BwC
                        if (Item.CanUseItem((int)ItemId.Bilgewater_Cutlass))
                            result += Player.Instance.GetItemDamage(target, ItemId.Bilgewater_Cutlass);
                        break;
                    case ItemId.Hextech_Gunblade:  //Hex
                        if (Item.CanUseItem((int)ItemId.Hextech_Gunblade))
                            result += Player.Instance.GetItemDamage(target, ItemId.Hextech_Gunblade);
                        break;
                    case ItemId.Blade_of_the_Ruined_King:
                        if (Item.CanUseItem((int)ItemId.Blade_of_the_Ruined_King))
                            result += Player.Instance.GetItemDamage(target, ItemId.Blade_of_the_Ruined_King);
                        break;
                    case ItemId.Tiamat_Melee_Only:
                        if (Item.CanUseItem((int)ItemId.Tiamat_Melee_Only))
                            result += Player.Instance.GetItemDamage(target, ItemId.Tiamat_Melee_Only);
                        break;
                    case ItemId.Ravenous_Hydra_Melee_Only:
                        if (Item.CanUseItem((int)ItemId.Ravenous_Hydra_Melee_Only))
                            result += Player.Instance.GetItemDamage(target, ItemId.Ravenous_Hydra_Melee_Only);
                        break;
                    case ItemId.Titanic_Hydra:
                        if (Item.CanUseItem((int)ItemId.Titanic_Hydra))
                            result += Player.Instance.GetItemDamage(target, ItemId.Titanic_Hydra);
                        break;
                }

            return result;
        }

        public static float GetComboDamage(AIHeroClient target)
        {
            double qDamage = Player.Instance.GetSpellDamage(target, SpellSlot.Q);
            double q2Damage = Player.Instance.GetSpellDamage(target, SpellSlot.Q, DamageLibrary.SpellStages.Detonation);
            double eDamage = Player.Instance.GetSpellDamage(target, SpellSlot.E);
            double rDamage = Player.Instance.GetSpellDamage(target, SpellSlot.R);
            double hitDamage = Player.Instance.GetAutoAttackDamage(target, true);

            double totDmg = 0;

            if (SpellManager.Q.IsReady())
            {
                totDmg += qDamage;

                //if (HasBuff(target, "AkaliMota"))
                    totDmg += q2Damage + hitDamage;
            }

            if (SpellManager.E.IsReady())
                totDmg += eDamage;

            if (SpellManager.R.IsReady())
                totDmg += rDamage;

            totDmg += CalcItemsDmg(target);

            return (float)totDmg;
        }
    }

    public static class Map
    {
        public static List<Vector3> _wardSpots;

        public static void InitializeEvadeSpots()
        {
            _wardSpots = new List<Vector3>
            {
                new Vector3(7451.664f, 6538.447f, 33.74536f),
                new Vector3(8518.179f, 7240.318f, 40.60852f),
                new Vector3(8845.78f, 5213.672f, 33.61487f),
                new Vector3(11504.19f, 5433.52f, 30.58154f),
                new Vector3(11771.57f, 6313.124f, 51.80713f),
                new Vector3(12778.93f, 2197.325f, 51.68604f),
                new Vector3(7475.538f, 3373.856f, 52.57471f),
                new Vector3(3373.385f, 7560.835f, 50.81982f),
                new Vector3(1672.145f, 12495.21f, 52.83826f),
                new Vector3(2200.103f, 12940.9f, 52.83813f),
                new Vector3(4835.833f, 12076.87f, 56.44629f),
                new Vector3(7368.242f, 11604.35f, 51.2417f),
                new Vector3(7337.794f, 8310.077f, 14.54712f),
                new Vector3(6377.225f, 7541.464f, -28.97229f)
            };
        }

        public static bool IsWall(Vector2 pos)
        {
            return (NavMesh.GetCollisionFlags(pos.X, pos.Y) == CollisionFlags.Wall ||
            NavMesh.GetCollisionFlags(pos.X, pos.Y) == CollisionFlags.Building);
        }

        public static bool IsWallBetween(Vector3 start, Vector3 end)
        {
            double count = Vector3.Distance(start, end);
            for (uint i = 0; i <= count; i += 10)
            {
                Vector2 pos = start.Extend(end, i);

                if (IsWall(pos))
                    return true;
            }

            return false;
        }
    }

    public static class DamageIndicator
    {
        public delegate float DamageToUnitDelegate(AIHeroClient hero);

        private const int XOffset = 10;
        private const int YOffset = 20;
        private const int Width = 103;
        private const int Height = 8;
        public static bool Enabled = true;
        private static DamageToUnitDelegate _damageToUnit;

        //private static readonly Text TextDmg = new Text(String.Empty, new Font("Lucida Sans Unicode", 7.7F, FontStyle.Bold)); //new Text(string.Empty, new System.Drawing.Font("monospace", 11, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel));

        public static DamageToUnitDelegate DamageToUnit
        {
            get { return _damageToUnit; }

            set
            {
                if (_damageToUnit == null)
                {
                    Drawing.OnDraw += Drawing_OnDraw;
                }
                _damageToUnit = value;
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Enabled || _damageToUnit == null)
            {
                return;
            }

            foreach (var unit in ObjectManager.Get<AIHeroClient>().Where(enemy => enemy.Team != ObjectManager.Player.Team && enemy.IsValid && enemy.IsHPBarRendered))
            {
                var barPos = unit.HPBarPosition;
                var damage = _damageToUnit(unit);
                var percentHealthAfterDamage = Math.Max(0, unit.Health - damage) / unit.MaxHealth;
                var xPos = barPos.X + XOffset + Width * percentHealthAfterDamage;

                //if (damage > unit.Health)
                //{
                //    TextDmg.Position = new Vector2(barPos.X + XOffset, barPos.Y + YOffset - 13);
                //    TextDmg.TextValue = ((int)(unit.Health - damage)).ToString();
                //    TextDmg.Draw();
                //}

                Drawing.DrawLine(xPos, barPos.Y + YOffset, xPos, barPos.Y + YOffset + Height, 2, System.Drawing.Color.Lime);
            }
        }

    }
}
