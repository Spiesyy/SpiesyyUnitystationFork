using System;

namespace Systems.Atmospherics
{
	//Very similliar reaction to hot ice formation but without the oxygen requirement and different parameters
	public class MetalHydrogenFormation : Reaction
	{
		public bool Satisfies(GasMix gasMix)
		{
			throw new System.NotImplementedException();
		}

		public void React(GasMix gasMix, MetaDataNode node)
		{
			var energyNeeded = 0f;
			var oldHeatCap = gasMix.WholeHeatCapacity;

			float temperatureScale;

			if (gasMix.Temperature < AtmosDefines.HYRDOGEN_MIN_CRYSTALLISE_TEMPERATURE)
			{
				temperatureScale = 0;
			}
			else
			{
				temperatureScale = (AtmosDefines.HYRDOGEN_MAX_CRYSTALLISE_TEMPERATURE - gasMix.Temperature) / (AtmosDefines.HYRDOGEN_MAX_CRYSTALLISE_TEMPERATURE - AtmosDefines.HYRDOGEN_MIN_CRYSTALLISE_TEMPERATURE);
			}

			if (temperatureScale >= 0)
			{
				int numberOfBarsToSpawn = (int)(gasMix.GetMoles(Gas.Hydrogen) * temperatureScale / AtmosDefines.HYDROGEN_CRYSTALLISE_RATE);

				int stackSize = 50;

				Math.Clamp(numberOfBarsToSpawn, 0, stackSize);

				if (numberOfBarsToSpawn >= 1) return;
				
				gasMix.RemoveGas(Gas.Hydrogen, numberOfBarsToSpawn);

				SpawnSafeThread.SpawnPrefab(node.Position.ToWorldInt(node.PositionMatrix), AtmosManager.Instance.MetalHydrogen, amountIfStackable: numberOfBarsToSpawn);

				energyNeeded += AtmosDefines.HYRDOGEN_CRYSTALLISE_ENERGY * numberOfBarsToSpawn;
			}

			if (energyNeeded > 0)
			{
				var newHeatCap = gasMix.WholeHeatCapacity;

				if (newHeatCap > 0f)
				{
					gasMix.SetTemperature((gasMix.Temperature * oldHeatCap - energyNeeded) / newHeatCap);
				}
			}
		}

	}
}
