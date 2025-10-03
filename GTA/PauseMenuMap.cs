using GTA;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DavyKager;

public class PauseMenuMap : Script
{
    private bool inPauseMenu = false;
    private int currentOption = -1;
    private int currentDestination = -1;
    private bool isInMap = false;

    private string[] pauseMenuOptions = {
        "MAP", "BRIEF", "STATS", "SETTINGS", "GAME", "ONLINE"
    };

    private string[] mapDestinations = {
        "Michael's House", "Franklin's Aunt's House", "Franklin's Vinewood Hills House", "Trevor's Trailer", "Trevor's Safehouse in Los Santos",
        "Ammu-Nation (Pillbox Hill)", "Ammu-Nation (Cypress Flats)", "Ammu-Nation (La Mesa)", "Ammu-Nation (Tataviam Mountains)", "Ammu-Nation (Chumash)",
        "Los Santos Customs (Burton)", "Los Santos Customs (La Mesa)", "Los Santos Customs (Grand Senora Desert)", "Beeker's Garage (Paleto Bay)",
        "Binco (Strawberry)", "Binco (Vespucci Canals)", "Suburban (Del Perro)", "Suburban (Harmony)", "Ponsonbys (Rockford Hills)", "Ponsonbys (Morningwood)",
        "Herr Kutz Barber (Chamberlain Hills)", "Beachcombover Barbers (Vespucci Canals)", "O'Sheas Barbers (Mirror Park)", "Bob Mulet Hair & Beauty (Rockford Hills)",
        "Blazing Tattoo (Vespucci Beach)", "The Pit (Downtown Vinewood)", "Ink Inc. (Mirror Park)", "Ink Inc. (Sandy Shores)",
        "24/7 Supermarket (Little Seoul)", "24/7 Supermarket (Downtown Vinewood)", "24/7 Supermarket (Mirror Park)", "24/7 Supermarket (Sandy Shores)", "24/7 Supermarket (Paleto Bay)",
        "Limited LTD Gasoline (Grapeseed)", "Limited LTD Gasoline (La Puerta)", "Limited LTD Gasoline (Davis)", "Rob's Liquor (Morningwood)", "Rob's Liquor (Murrieta Heights)", "Rob's Liquor (Harmony)",
        "Ten Cent Theater (Textile City)", "Tivoli Cinema (Morningwood)", "Oriental Theater (Downtown Vinewood)", "Vanilla Unicorn (Strawberry)",
        "Yellow Jack Inn (Grand Senora Desert)", "Los Santos Golf Club (Richman)", "Richman Hotel Tennis Court (Richman)", "Michael's House Tennis Court (Rockford Hills)",
        "Los Santos International Airport (LSIA)", "Vespucci Beach", "Downtown Vinewood", "Del Perro Pier", "Sandy Shores", "Paleto Bay",
        "Fort Zancudo", "Mount Chiliad", "Blaine County Savings Bank", "Altruist Cult Camp", "Playboy Mansion", "Vinewood Sign",
        "Epsilon Program Building", "Maze Bank Tower", "Los Santos Golf Club", "Car Wash (Little Seoul)", "Car Wash (Strawberry)",
        "Mission Row Police Station", "Vespucci Police Station", "La Mesa Police Station"
    };

    public PauseMenuMap()
    {
        Tick += OnTick;
        KeyDown += OnKeyDown;
        Tolk.Load(); // Load Tolk for speech
    }

    private void OnTick(object sender, EventArgs e)
    {
        DetectPauseMenu();
        if (isInMap)
        {
            HandleMapNavigation();
        }
        else if (inPauseMenu)
        {
            HandlePauseMenuNavigation();
        }
    }

    private void OnKeyDown(object sender, KeyEventArgs e)
    {
        if (!inPauseMenu) return;

        if (isInMap)
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    ChangeMapDestination(-1);
                    break;
                case Keys.Down:
                    ChangeMapDestination(1);
                    break;
                case Keys.Enter:
                    ConfirmDestination();
                    break;
                case Keys.Escape:
                    ExitMap();
                    break;
            }
        }
        else
        {
            switch (e.KeyCode)
            {
                case Keys.Up:
                    ChangePauseMenuOption(-1);
                    break;
                case Keys.Down:
                    ChangePauseMenuOption(1);
                    break;
                case Keys.Enter:
                    if (pauseMenuOptions[currentOption] == "MAP")
                    {
                        OpenMap();
                    }
                    break;
            }
        }
    }

    private void DetectPauseMenu()
    {
        if (Game.IsPaused && !inPauseMenu)
        {
            inPauseMenu = true;
            currentOption = 0;
            isInMap = false;
            Speak($"Pause menu opened. {pauseMenuOptions[currentOption]} selected.");
        }
        else if (!Game.IsPaused && inPauseMenu)
        {
            inPauseMenu = false;
            isInMap = false;
        }
    }

    private void HandlePauseMenuNavigation()
    {
        // This method can be used for additional pause menu detection logic
    }

    private void HandleMapNavigation()
    {
        // This method can be used for additional map navigation logic
    }

    private void ChangePauseMenuOption(int direction)
    {
        int newSelection = currentOption + direction;
        if (newSelection >= 0 && newSelection < pauseMenuOptions.Length)
        {
            currentOption = newSelection;
            Speak($"{pauseMenuOptions[currentOption]} selected.");
        }
    }

    private void OpenMap()
    {
        isInMap = true;
        Speak("Map opened. Use arrow keys to navigate destinations.");
        currentDestination = 0;
        Speak($"Destination: {mapDestinations[currentDestination]}");
    }

    private void ExitMap()
    {
        isInMap = false;
        Speak("Map closed. Back to pause menu.");
        Speak($"{pauseMenuOptions[currentOption]} selected.");
    }

    private void ChangeMapDestination(int direction)
    {
        int newSelection = currentDestination + direction;
        if (newSelection >= 0 && newSelection < mapDestinations.Length)
        {
            currentDestination = newSelection;
            Speak($"Destination: {mapDestinations[currentDestination]}");
        }
    }

    private void ConfirmDestination()
    {
        Speak($"{mapDestinations[currentDestination]} selected.");
        // Here you could add logic to actually set a waypoint to this destination
    }

    private void Speak(string text)
    {
        Tolk.Output(text, true);
        GTA.UI.Notification.Show(text);
    }
}