using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ArduinoParking
{
    public partial class MainWindow : Window
    {
        private SerialPort serialPort;
        private List<Parking> parkings = new List<Parking>
        {
            new Parking
            {
                Naziv = "Parking 1",
                Latitude = 44.7866,
                Longitude = 20.4489,
                Slobodan = false
            },
            new Parking
            {
                Naziv = "Parking 2",
                Latitude = 44.7854,
                Longitude = 20.4732,
                Slobodan = false
            }
        };

        public MainWindow()
        {
            InitializeComponent();
            InitializeSerialPort();
        }

        private void InitializeSerialPort()
        {
            try
            {
                // Prvo zatvorite sve postojeće konekcije
                if (SerialPort.GetPortNames().Contains("COM4"))
                {
                    using (SerialPort testPort = new SerialPort("COM4"))
                    {
                        testPort.Close();
                    }
                }

                serialPort = new SerialPort("COM4", 9600);
                serialPort.DataReceived += SerialPort_DataReceived;
                serialPort.Open();
                MessageBox.Show("Uspešno otvoren port COM4");
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Port je trenutno zauzet. Zatvorite druge aplikacije koje koriste COM4.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Greška pri otvaranju porta: {ex.Message}");
            }
        }

        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                string data = serialPort.ReadLine().Trim();
                System.Diagnostics.Debug.WriteLine($"Primljeni podaci: {data}"); // Debug output

                string[] parkingStatuses = data.Split(',');

                Dispatcher.Invoke(() => {
                    foreach (string status in parkingStatuses)
                    {
                        string[] parts = status.Split(':');
                        if (parts.Length == 2)
                        {
                            int parkingIndex = int.Parse(parts[0]) - 1;
                            parkings[parkingIndex].Slobodan = parts[1] == "SLOBODAN";
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => {
                    MessageBox.Show($"Greška pri čitanju podataka: {ex.Message}");
                });
            }
        }

        private void BtnNadjiParking_Click(object sender, RoutedEventArgs e)
        {
            wrapPanelParkings.Children.Clear();

            foreach (var parking in parkings)
            {
                if (parking.Slobodan)
                {
                    Button parkingKartica = new Button
                    {
                        Content = $"{parking.Naziv}\nStatus: Slobodan",
                        Style = (Style)Resources["ParkingButtonStyle"],
                        Tag = parking
                    };

                    parkingKartica.Click += ParkingKartica_Click;
                    wrapPanelParkings.Children.Add(parkingKartica);
                }
            }
        }

        private void ParkingKartica_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var parking = button.Tag as Parking;

            string googleMapsUrl = $"https://www.google.com/maps?q={parking.Latitude},{parking.Longitude}";
            Process.Start(new ProcessStartInfo(googleMapsUrl) { UseShellExecute = true });
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }
        }
    }

}