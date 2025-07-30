using System.IO.Ports;
using System.Text;
using Newtonsoft.Json;

public class WebPollingService
{
    private System.Timers.Timer pollingTimer;
    private SerialPort serialPort;
    private readonly List<ParkingSpot> parkings = new()
    {
        new ParkingSpot { Naziv = "Parking 1", Latitude = 44.7866, Longitude = 20.4489, Slobodan = false },
        new ParkingSpot { Naziv = "Parking 2", Latitude = 44.7854, Longitude = 20.4732, Slobodan = false }
    };

    public void Start()
    {
        InitializeSerialPort();
        StartPollingTimer();
    }

    public void Stop()
    {
        pollingTimer?.Stop();
        serialPort?.Close();
    }

    private void StartPollingTimer()
    {
        pollingTimer = new System.Timers.Timer(1000);
        pollingTimer.Elapsed += (s, e) => PerformPolling();
        pollingTimer.AutoReset = true;
        pollingTimer.Start();
    }

    private void PerformPolling()
    {
        try
        {
            using HttpClient client = new HttpClient();
            var url = "http://192.168.0.12:5050/api/sync";
            string json = JsonConvert.SerializeObject(parkings);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = client.PostAsync(url, content).Result;
            response.EnsureSuccessStatusCode();

            Console.WriteLine("Parking spots sent.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending to server: {ex.Message}");
        }
    }

    private void InitializeSerialPort()
    {
        try
        {
            if (SerialPort.GetPortNames().Contains("COM4"))
            {
                serialPort = new SerialPort("COM4", 9600);
                serialPort.DataReceived += SerialPort_DataReceived;
                serialPort.Open();
                Console.WriteLine("Successfully opened COM4.");
            }
            else
            {
                Console.WriteLine("COM4 not found.");
            }
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine("Port COM4 is in use.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error opening COM4: {ex.Message}");
        }
    }

    private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
    {
        try
        {
            string data = serialPort.ReadLine().Trim();
            Console.WriteLine($"Received: {data}");

            string[] parkingStatuses = data.Split(',');

            foreach (string status in parkingStatuses)
            {
                string[] parts = status.Split(':');
                if (parts.Length == 2)
                {
                    int parkingIndex = int.Parse(parts[0]) - 1;
                    if (parkingIndex >= 0 && parkingIndex < parkings.Count)
                    {
                        parkings[parkingIndex].Slobodan = parts[1] == "SLOBODAN";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Serial read error: {ex.Message}");
        }
    }
}
