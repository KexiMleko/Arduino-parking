using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/[controller]")]
public class SyncController : ControllerBase
{
    [HttpPost]
    public IActionResult UpdateParkingSpots(List<ParkingSpot> parkingSpots)
    {
        ParkingSpotData.Instance.ParkingSpots = parkingSpots;
        return Ok();
    }
}
