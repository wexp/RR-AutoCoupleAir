using Game.Messages;
using Game.State;
using HarmonyLib;
using Model;
using Model.AI;
using Network;
using System.Collections.Generic;


namespace AutoCoupleAir
{

    [HarmonyPatch(typeof(Car), "HandleCoupledChange")]
    public class AutoCoupleAir
    {
        static void Postfix(Car __instance)
        {
            AutoEngineerPersistence persistence = new AutoEngineerPersistence(__instance.KeyValueObject);
            bool flag = false;
            if (AutoAirCoupler.Settings.affectOnlyAi)
            {
                if (persistence.Orders.Enabled)
                {
                    flag = true;
                }
                else { flag = false; }
            }
            else { flag = true; }
            if (Multiplayer.Mode == ConnectionMode.Singleplayer && AutoAirCoupler.Settings.isEnabled && flag)
            {
                IEnumerable<Car> coupledCars = __instance.EnumerateCoupled();

                foreach (var car in coupledCars)
                {
                    if (car.EndGearA.IsCoupled)
                    {
                        //car.EndGearA.Anglecock.GladhandClick();
                        var othercar = car.CoupledTo(Car.LogicalEnd.A);
                        if (othercar != null)
                        {
                            StateManager.ApplyLocal(new SetGladhandsConnected(car.id, othercar.id, true));
                            car.ApplyEndGearChange(Car.LogicalEnd.A, Car.EndGearStateKey.Anglecock, 1f);
                        }
                    }
                    if (car.EndGearB.IsCoupled)
                    {
                        //car.EndGearB.Anglecock.GladhandClick();
                        var othercar = car.CoupledTo(Car.LogicalEnd.B);
                        if (othercar != null)
                        {
                            StateManager.ApplyLocal(new SetGladhandsConnected(car.id, othercar.id, true));
                            car.ApplyEndGearChange(Car.LogicalEnd.B, Car.EndGearStateKey.Anglecock, 1f);
                        }
                    }
                    if (AutoAirCoupler.Settings.affectHandbrakes)
                    {
                        StateManager.ApplyLocal(new PropertyChange(car.id, PropertyChange.Control.Handbrake, false));
                    }
                    else { continue; }
                }
            }
        }
    }
}