﻿using MechWars.MapElements;
using MechWars.MapElements.Orders.Actions;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;

namespace MechWars.PlayerInput
{
    public static class HoverCandidatesFilter
    {
        public static void Select(HashSet<MapElement> candidates)
        {
            candidates.RemoveWhere(me => !me.Selectable);
            if (candidates.Empty()) return;

            var armies = candidates.SelectDistinct(me => me.Army);
            var army = armies.FirstOrAnother(
                a => a == Globals.HumanArmy,
                a => a != null,
                a => a == null).Result;
            candidates.RemoveWhere(me => me.Army != army);
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

        public static void ToggleSelect(HashSet<MapElement> candidates)
        {
            candidates.RemoveWhere(me => !me.Selectable);
            if (candidates.Empty()) return;

            var selected = Globals.Spectator.InputController.SelectionMonitor.SelectedMapElements;
            Army army;
            if (!selected.Empty()) army = selected.First().Army;
            else
            {
                var armies = candidates.SelectDistinct(me => me.Army);
                army = armies.FirstOrAnother(
                    a => a == Globals.HumanArmy,
                    a => a != null,
                    a => a == null).Result;
            }
            candidates.RemoveWhere(me => me.Army != army);
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
        
        public static void Attack(HashSet<MapElement> candidates)
        {
            var selected = Globals.Spectator.InputController.SelectionMonitor.SelectedMapElements;
            if (selected.None(me => me.CanAttack))
            {
                candidates.Clear();
                return;
            }
            
            candidates.RemoveWhere(me => !me.CanBeAttacked);
            if (candidates.Empty()) return;
            
            var armies = candidates.SelectDistinct(me => me.Army);
            var army = armies.FirstOrAnother(
                a => a != null && a != Globals.HumanArmy,
                a => a == null,
                a => a == Globals.HumanArmy).Result;
            candidates.RemoveWhere(me => me.Army != army);
        }

        public static void Escort(HashSet<MapElement> candidates)
        {
            var selected = Globals.Spectator.InputController.SelectionMonitor.SelectedMapElements;
            if (selected.None(me => me.CanEscort))
            {
                candidates.Clear();
                return;
            }

            candidates.RemoveWhere(me => !me.CanBeEscorted || me.Army != Globals.HumanArmy);
        }
        
        public static void HarvestResource(HashSet<MapElement> candidates)
        {
            var selected = Globals.Spectator.InputController.SelectionMonitor.SelectedMapElements;
            if (selected.None(me => me.orderActions.Any(oa => oa is HarvestResourceOrderAction)))
            {
                candidates.Clear();
                return;
            }

            candidates.RemoveWhere(me => !(me is Resource));
        }

        internal static void LookAt(HashSet<MapElement> candidates)
        {
            candidates.RemoveWhere(me => !me.Selectable);
        }
    }
}