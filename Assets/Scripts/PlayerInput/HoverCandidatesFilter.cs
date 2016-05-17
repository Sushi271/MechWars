using MechWars.Human;
using MechWars.MapElements;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.PlayerInput
{
    public static class HoverCandidatesFilter
    {
        public static void Select(HumanPlayer player, HashSet<MapElement> candidates)
        {
            candidates.RemoveWhere(me => !me.Selectable);
            if (candidates.Empty()) return;

            var armies = candidates.SelectDistinct(me => me.army);
            var army = armies.FirstOrAnother(
                a => a == player.Army,
                a => a != null,
                a => a == null).Result;
            candidates.RemoveWhere(me => me.army != army);
            // candidates cannot be empty after this - no return

            System.Func<MapElement, bool> interMultiSelectable = me => me is Unit || me.CanAttack;
            var interMultiSelectableExist = candidates.Any(interMultiSelectable);
            if (interMultiSelectableExist)
            {
                candidates.RemoveWhereNot(interMultiSelectable);
                return;
            }

            System.Func<MapElement, bool> buildings = me => me is Building;
            var buildingsExist = candidates.Any(buildings);
            if (buildingsExist)
            {
                candidates.RemoveWhereNot(buildings);
                var buildingNames = candidates.SelectDistinct(me => me.mapElementName);
                var firstName = buildingNames.First();
                candidates.RemoveWhere(me => me.mapElementName != firstName);
                return;
            }

            var first = candidates.First();
            candidates.RemoveWhere(me => me != first);
        }

        public static void ToggleSelect(HumanPlayer player, HashSet<MapElement> candidates)
        {
            candidates.RemoveWhere(me => !me.Selectable);
            if (candidates.Empty()) return;

            var selected = player.InputController.SelectionMonitor.SelectedMapElements;
            Army army;
            if (!selected.Empty()) army = selected.First().army;
            else
            {
                var armies = candidates.SelectDistinct(me => me.army);
                army = armies.FirstOrAnother(
                    a => a == player.Army,
                    a => a != null,
                    a => a == null).Result;
            }
            candidates.RemoveWhere(me => me.army != army);
            // candidates cannot be empty after this - no return

            System.Func<MapElement, bool> interMultiSelectable = me => me is Unit || me.CanAttack;
            var interMultiSelectableExist = selected.Any(interMultiSelectable) ||
                selected.Empty() && candidates.Any(interMultiSelectable);
            if (interMultiSelectableExist)
            {
                candidates.RemoveWhereNot(interMultiSelectable);
                return;
            }

            System.Func<MapElement, bool> buildings = me => me is Building;
            var buildingsExist = selected.Any(buildings) ||
                selected.Empty() && candidates.Any(buildings);
            if (buildingsExist)
            {
                candidates.RemoveWhereNot(buildings);
                string buildingName;
                if (!selected.Empty()) buildingName = selected.First().mapElementName;
                else
                {
                    var buildingNames = candidates.SelectDistinct(me => me.mapElementName);
                    buildingName = buildingNames.First();
                }
                candidates.RemoveWhere(me => me.mapElementName != buildingName);
                return;
            }
            
            var first = selected.FirstOrDefault() ?? candidates.First();
            candidates.RemoveWhere(me => me != first);
        }
        
        public static void Attack(HumanPlayer player, HashSet<MapElement> candidates)
        {
            var selected = player.InputController.SelectionMonitor.SelectedMapElements;
            if (selected.None(me => me.CanAttack))
            {
                candidates.Clear();
                return;
            }
            
            candidates.RemoveWhere(me => !me.CanBeAttacked);
            if (candidates.Empty()) return;
            
            var armies = candidates.SelectDistinct(me => me.army);
            var army = armies.FirstOrAnother(
                a => a != null && a != player.Army,
                a => a == null,
                a => a == player.Army).Result;
            candidates.RemoveWhere(me => me.army != army);
            // candidates cannot be empty after this - no return

            System.Func<MapElement, bool> units = me => me is Unit;
            var unitsExist = candidates.Any(units);
            if (unitsExist)
            {
                candidates.RemoveWhereNot(units);
                return;
            }

            System.Func<MapElement, bool> buildings = me => me is Building;
            var buildingsExist = candidates.Any(buildings);
            if (buildingsExist)
            {
                candidates.RemoveWhereNot(buildings);
                return;
            }

            candidates.Clear();
        }

        public static void Escort(HumanPlayer player, HashSet<MapElement> candidates)
        {
            var selected = player.InputController.SelectionMonitor.SelectedMapElements;
            if (selected.None(me => me.CanEscort))
            {
                candidates.Clear();
                return;
            }

            candidates.RemoveWhere(me => !me.CanBeEscorted || me.army != player.Army);
        }

        internal static void LookAt(HumanPlayer player, HashSet<MapElement> candidates)
        {
            candidates.RemoveWhere(me => !me.Selectable);
        }
    }
}