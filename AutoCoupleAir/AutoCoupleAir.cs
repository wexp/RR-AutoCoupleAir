using Game;
using Game.Messages;
using Game.State;
using HarmonyLib;
using Model;
using Model.AI;
using RollingStock;
using System.Collections.Generic;
using System.Linq;


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
            if (AutoAirCoupler.Settings.isEnabled && flag && StateManager.CheckAuthorizedToChangeProperty(StateManager.Shared.PlayersManager.LocalPlayer.PlayerId.String, Car.EndGearStateKey.Anglecock.ToString()) && (StateManager.IsHost || TrainController.Shared.SelectedCar.id == __instance.id))
            {
                IEnumerable<Car> coupledCars = __instance.EnumerateCoupled();

                foreach (var car in coupledCars)
                {
                    if (car.EndGearA.IsCoupled)
                    {
                        car.EndGearA.Anglecock.GladhandClick();
                        var othercar = car.CoupledTo(Car.LogicalEnd.A);
                        StateManager.ApplyLocal(new SetGladhandsConnected(car.id, othercar.id, true));
                        if (car.EndGearA.IsAirConnected)
                        {
                            car.ApplyEndGearChange(Car.LogicalEnd.A, Car.EndGearStateKey.Anglecock, 1f);
                        }
                    }
                    if (car.EndGearB.IsCoupled)
                    {
                        car.EndGearB.Anglecock.GladhandClick();
                        var othercar = car.CoupledTo(Car.LogicalEnd.B);
                        StateManager.ApplyLocal(new SetGladhandsConnected(car.id, othercar.id, true));
                        if (car.EndGearA.IsAirConnected)
                        {
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