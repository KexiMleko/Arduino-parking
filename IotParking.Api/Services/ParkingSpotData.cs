public sealed class ParkingSpotData
{
    private static readonly Lazy<ParkingSpotData> _instance =
        new Lazy<ParkingSpotData>(() => new ParkingSpotData());


    public static ParkingSpotData Instance => _instance.Value;

    public List<ParkingSpot> ParkingSpots { get; set; } = new();
}
