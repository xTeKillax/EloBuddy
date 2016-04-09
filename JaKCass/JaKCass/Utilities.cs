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

namespace JaKCass.Utilities
{
    public static class Combat
    {
        public static bool isPoisoned(Obj_AI_Base target)
        {
            return target.HasBuffOfType(BuffType.Poison);
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
    }
}
