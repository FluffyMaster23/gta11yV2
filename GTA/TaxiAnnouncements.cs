using GTA;
using GTA.Native;
using GTA.Math;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DavyKager;

public class TaxiAnnouncements : Script
{
    private bool taxiCalled = false;
    private bool taxiDispatched = false;
    private bool taxiArrived = false;
    private bool inTaxiMenu = false;
    private int currentSelection = -1;
    private Random random = new Random();

    private List<string> taxiDestinations = new List<string>
    {
        "Downtown Vinewood",
        "Vespucci Beach",
        "Los Santos International Airport",
        "Del Perro Pier",
        "Sandy Shores",
        "Paleto Bay",
        "Rockford Hills",
        "Mirror Park",
        "Little Seoul",
        "Strawberry",
        "Davis",
        "Chamberlain Hills"
    };

    public TaxiAnnouncements()
    {
        Tick += OnTick;
        KeyDown += OnKeyDown;
        Tolk.Load(); // Load Tolk for speech
    }

    private void OnTick(object sender, EventArgs e)
    {
        DetectTaxiCall();
        DetectTaxiDispatch();
        DetectTaxiArrival();
        DetectTaxiEntry();
        DetectTaxiFare();
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (!inTaxiMenu) return;

        switch (e.KeyCode)
        {
            case Keys.Up:
                ChangeDestination(-1);
                break;
            case Keys.Down:
                ChangeDestination(1);
                break;
            case Keys.Enter:
                ConfirmDestination();
                break;
            case Keys.Escape:
                ExitTaxiMenu();
                break;
        }
    }

    private void DetectTaxiCall()
    {
        // Check if player has called a taxi (simplified detection)
        if (Function.Call<bool>(Hash.IS_WAYPOINT_ACTIVE) && !taxiCalled)
        {
            Vehicle nearbyTaxi = World.GetClosestVehicle(Game.Player.Character.Position, 50f, VehicleHash.Taxi);
            if (nearbyTaxi != null && nearbyTaxi.Driver != null && !nearbyTaxi.Driver.IsPlayer)
            {
                taxiCalled = true;
                Speak("Taxi requested. Waiting for dispatch confirmation.");
            }
        }
    }

    private void DetectTaxiDispatch()
    {
        if (!taxiCalled || taxiDispatched) return;

        foreach (Vehicle taxi in World.GetNearbyVehicles(Game.Player.Character.Position, 500))
        {
            if (taxi.Model == VehicleHash.Taxi && taxi.Driver != null && !taxi.Driver.IsPlayer)
            {
                taxiDispatched = true;
                Speak("Your taxi is on its way. Please wait.");
                return;
            }
        }
    }

    private void DetectTaxiArrival()
    {
        if (!taxiCalled || !taxiDispatched || taxiArrived) return;

        foreach (Vehicle taxi in World.GetNearbyVehicles(Game.Player.Character.Position, 20))
        {
            if (taxi.Model == VehicleHash.Taxi && taxi.Speed < 1f && taxi.Driver != null && !taxi.Driver.IsPlayer)
            {
                taxiArrived = true;
                Speak("Your taxi has arrived. Enter the vehicle.");
                return;
            }
        }
    }

    private void DetectTaxiEntry()
    {
        if (!taxiArrived) return;

        if (Game.Player.Character.IsInVehicle() && Game.Player.Character.CurrentVehicle.Model == VehicleHash.Taxi)
        {
            Speak("You are now in the taxi. Please select your destination using arrow keys.");
            inTaxiMenu = true;
            currentSelection = 0;
            Speak($"Destination: {taxiDestinations[currentSelection]}");
            
            // Random taxi driver dialogue
            Wait(2000);
            TaxiDriverTalk();
            
            ResetTaxiState();
        }
    }

    private void DetectTaxiFare()
    {
        if (!Game.Player.Character.IsInVehicle() || taxiArrived) return;

        // Check if player just exited a taxi
        if (Game.Player.Character.LastVehicle != null && 
            Game.Player.Character.LastVehicle.Model == VehicleHash.Taxi)
        {
            // Simulate fare calculation based on distance traveled
            int fare = random.Next(15, 150); // Random fare between $15-$150
            int balance = Game.Player.Money;

            Speak($"Your ride cost {fare} dollars. You have {balance} dollars remaining.");
            CheckLowBalance();
            
            // Reset state when exiting taxi
            taxiArrived = false;
        }
    }

    private void ChangeDestination(int direction)
    {
        int newSelection = currentSelection + direction;
        if (newSelection >= 0 && newSelection < taxiDestinations.Count)
        {
            currentSelection = newSelection;
            Speak($"Destination: {taxiDestinations[currentSelection]}");
        }
    }

    private void ConfirmDestination()
    {
        Speak($"Navigating to {taxiDestinations[currentSelection]}.");
        AutoDriveToDestination();
        inTaxiMenu = false; // Exit menu state
    }

    private void ExitTaxiMenu()
    {
        Speak("Destination selection cancelled.");
        inTaxiMenu = false;
    }

    private void AutoDriveToDestination()
    {
        if (Game.Player.Character.IsInVehicle())
        {
            Vehicle taxi = Game.Player.Character.CurrentVehicle;
            
            // Try to get waypoint first
            Vector3 waypoint = World.WaypointPosition;
            if (waypoint != Vector3.Zero)
            {
                Function.Call(GTA.Native.Hash.TASK_VEHICLE_DRIVE_TO_COORD_LONGRANGE, 
                    taxi.Driver.Handle, taxi.Handle, waypoint.X, waypoint.Y, waypoint.Z, 
                    25f, 262144, 5.0f);
                Speak("Taxi will drive to your waypoint.");
            }
            else
            {
                // Use predefined destination coordinates (simplified)
                Vector3 destination = GetDestinationCoordinates(taxiDestinations[currentSelection]);
                if (destination != Vector3.Zero)
                {
                    Function.Call(GTA.Native.Hash.TASK_VEHICLE_DRIVE_TO_COORD_LONGRANGE, 
                        taxi.Driver.Handle, taxi.Handle, destination.X, destination.Y, destination.Z, 
                        25f, 262144, 5.0f);
                    Speak($"Taxi is driving to {taxiDestinations[currentSelection]}.");
                }
            }
        }
    }

    private Vector3 GetDestinationCoordinates(string destination)
    {
        // Simplified coordinate mapping - in a real implementation, you'd have a proper database
        switch (destination)
        {
            case "Downtown Vinewood":
                return new Vector3(293f, 180f, 104f);
            case "Vespucci Beach":
                return new Vector3(-1394f, -1020f, 13f);
            case "Los Santos International Airport":
                return new Vector3(-1037f, -2674f, 13f);
            case "Del Perro Pier":
                return new Vector3(-1850f, -1231f, 13f);
            case "Sandy Shores":
                return new Vector3(1556f, 3686f, 34f);
            case "Paleto Bay":
                return new Vector3(-448f, 6019f, 31f);
            default:
                return Vector3.Zero;
        }
    }

    private void TaxiDriverTalk()
    {
        string[] dialogue = {
            "Hey, you from around here?",
            "Long night, huh?",
            "You hear about that crazy stuff happening downtown?",
            "Traffic's been terrible lately.",
            "Nice weather we're having.",
            "Been driving this route for years.",
            "Hope you don't mind the music.",
            "Let me know if you need the AC adjusted."
        };

        int randomIndex = random.Next(dialogue.Length);
        Speak($"Driver says: {dialogue[randomIndex]}");
    }

    private void CheckLowBalance()
    {
        int balance = Game.Player.Money;
        if (balance < 100)
        {
            Speak("Warning: You have less than 100 dollars left!");
        }
    }

    private void ResetTaxiState()
    {
        taxiCalled = false;
        taxiDispatched = false;
        taxiArrived = false;
    }

    private void Speak(string text)
    {
        Tolk.Output(text, true);
        GTA.UI.Notification.PostTicker(text, false);
    }
}