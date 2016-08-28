using MechWars.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MechWars.InGameGUI
{
    public class OrderActionButtonController : MonoBehaviour
    {
        public List<OrderActionButton> buttons;

        void Update()
        {
            var selected = Globals.Spectator.InputController.SelectionMonitor.SelectedMapElements;
            // jezeli nie jest zaznaczone nic, albo sa zaznaczeni wrogowie to wszystkie buttony rozkazow sa niewidoczne
            if (selected.Empty() || selected.Any(me => me.Army != Globals.HumanArmy)
                || Globals.Spectator.InputController.CarriesOrderAction)
            {
                foreach (var b in buttons)
                    b.gameObject.SetActive(false);
                return;
            }

            // petla ponizej sprawia ze w zbiorze orderActions jest czesc wspolna akcji kazdego MapElementu
            // np. jesli zaznaczone sa Mech i Harv, Harv umie zbierac i chodzic, Mech umie atakowac i chodzic
            // czescia wspolna jest chodzenie, zostanie wiec samo chodzenie
            var orderActionTypes = new HashSet<System.Type>();
            bool first = true;
            foreach (var mapElement in selected)
            {
                if (first)
                {
                    // dla pierwszego elementu wrzucamy jego wszystkie akcje do zbioru (po prostu)
                    orderActionTypes.UnionWith(mapElement.orderActions.Select(oa => oa.GetType()));
                    first = false;
                }
                // a dla kazdego nastepnego bierzemy czesc wspolna jego akcji i tych akcji ktore juz sa w zbiorze
                else orderActionTypes.IntersectWith(mapElement.orderActions.Select(oa => oa.GetType()));
            }

            foreach (var b in buttons)
            {
                // sprawdzamy czy OrderAction Buttona jest w zbiorze wspolnych akcji zaznaczonych elementow
                bool active = orderActionTypes.Contains(b.orderAction.GetType());
                // jesli tak, to guzik da sie kliknac
                b.gameObject.SetActive(active);
            }
        }
    }
}