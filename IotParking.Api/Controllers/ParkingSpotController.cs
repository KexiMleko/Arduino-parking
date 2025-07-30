using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/[controller]")]
public class ParkingSpotController : ControllerBase
{
    [HttpGet("get-free")]
    public IActionResult GetFreeSpots()
    {
        var res = ParkingSpotData.Instance.ParkingSpots.Where(x => x.Slobodan == true);
        return Ok(res);
    }
    [HttpGet("get-all")]
    public IActionResult GetAllSpots()
    {
        var res = ParkingSpotData.Instance.ParkingSpots;
        return Ok(res);
    }
}
