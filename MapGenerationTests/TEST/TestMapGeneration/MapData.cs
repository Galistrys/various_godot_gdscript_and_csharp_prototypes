public class MapData
{
    public float[,] HeightData { get; private set; }
    public float[,] HeatData { get; private set; }
    public float[,] MoistureData { get; private set; }

    public float MinHeight { get; set; }
    public float MaxHeight { get; set; }

    public float MinHeat { get; set; }
    public float MaxHeat { get; set; }

    public float MinMoisture { get; set; }
    public float MaxMoisture { get; set; }

    public MapData(int size)
    {
        HeightData = new float[size, size];
        HeatData = new float[size, size];
        MoistureData = new float[size, size];

        MinHeight = float.MaxValue;
        MaxHeight = float.MinValue;
        MinHeat = float.MaxValue;
        MaxHeat = float.MinValue;
        MinMoisture = float.MaxValue;
        MaxMoisture = float.MinValue;
    }
}