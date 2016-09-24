using MechWars.MapElements;
using MechWars.MapElements.Orders.Actions;
using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.InGameGUI
{
    public class OrderActionButtonController : MonoBehaviour
    {
        public OrderUtilityButton cancelSelectionButton;
        public OrderUtilityButton cancelProductionButton;
        public List<OrderActionButton> buttons;

        void Update()
        {
            var selected = Globals.Spectator.InputController.SelectionMonitor.SelectedMapElements.ToList();

            // wyciagamy pojedynczy budynek, jesli taki jest zaznaczony
            Building building = null;
            if (selected.Count == 1 && selected.First() is Building)
            {
                building = (Building)selected.First();
            }

            // przycisk anulujacy produkcje (unitow/budynkow/technologii) widoczny tylko, gdy zaznaczylismy pojedynczy budynek
            bool cancelProductionButtonVisible = building != null &&
                building.orderActions.Any(oa =>
                oa is UnitProductionOrderAction ||
                oa is BuildingConstructionOrderAction ||
                oa is TechnologyDevelopmentOrderAction);
            cancelProductionButton.gameObject.SetActive(cancelProductionButtonVisible);

            // jezeli nie jest zaznaczone nic, albo sa zaznaczeni wrogowie to wszystkie buttony rozkazow sa niewidoczne
            if (selected.Empty() || selected.Any(me => me.Army != Globals.HumanArmy)
                || Globals.Spectator.InputController.CarriesOrderAction ||
                building != null && building.UnderConstruction)
            {
                foreach (var b in buttons)
                    b.gameObject.SetActive(false);
                return;
            }

            // petla ponizej sprawia ze w zbiorze orderActions jest czesc wspolna nazw akcji kazdego MapElementu
            // np. jesli zaznaczone sa Mech i Harv, Harv umie zbierac i chodzic, Mech umie atakowac i chodzic
            // czescia wspolna jest chodzenie, zostanie wiec samo chodzenie
            var orderActionNames = new HashSet<string>();
            bool first = true;
            foreach (var mapElement in selected)
            {
                if (first)
                {
                    // dla pierwszego elementu wrzucamy nazwy wszystkiech jego akcji do zbioru (po prostu)
                    orderActionNames.UnionWith(mapElement.orderActions.Select(oa => oa.gameObject.name));
                    first = false;
                }
                // a dla kazdego nastepnego bierzemy czesc wspolna nazw wszystkich jego akcji i tych nazw akcji ktore juz sa w zbiorze
                else orderActionNames.IntersectWith(mapElement.orderActions.Select(oa => oa.gameObject.name));
            }

            foreach (var b in buttons)
            {
                // sprawdzamy czy OrderAction Buttona jest w zbiorze wspolnych akcji zaznaczonych elementow
                bool active = orderActionNames.Contains(b.orderAction.gameObject.name);
                if (active)
                {
                    // oprocz tego sprawdzamy jeszcze czy spelnione sa wymagania
                    var coa = b.orderAction as BuildingConstructionOrderAction;
                    var poa = b.orderAction as UnitProductionOrderAction;
                    var doa = b.orderAction as TechnologyDevelopmentOrderAction;
                    if (coa != null)
                        active = coa.CheckRequirements(Globals.HumanArmy);
                    else if (poa != null)
                        active = poa.CheckRequirements(Globals.HumanArmy);
                    else if (doa != null)
                        active = doa.CheckRequirements(Globals.HumanArmy) &&
                            Globals.HumanArmy.Technologies.CanDevelop(doa.technology);
                }
                // jesli tak, to guzik da sie kliknac
                b.gameObject.SetActive(active);
            }
        }
    }
}