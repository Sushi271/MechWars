using MechWars.MapElements.Orders.Actions.Args;
using MechWars.PlayerInput;
using System.Collections.Generic;
using UnityEngine;

namespace MechWars.MapElements.Orders.Actions
{
    public interface IOrderAction : IMouseBehaviourDeterminant
    {
        bool IsAttack { get; }
        IOrder CreateOrder(MapElement orderExecutor, OrderActionArgs args);
    }
}
