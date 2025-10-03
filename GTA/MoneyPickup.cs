using GTA;
using GTA.Native;
using System;
using DavyKager;

public class MoneyPickup : Script
{
    private int lastCash = 0;
    private DateTime lastCashTime = DateTime.MinValue;

    public MoneyPickup()
    {
        Tick += OnTick; // Runs every frame
        lastCash = Game.Player.Money; // Initialize with current money
    }

    private void OnTick(object sender, EventArgs e)
    {
        CheckForMoneyChange();
    }

    private void CheckForMoneyChange()
    {
        int currentCash = Game.Player.Money;
        
        // Check if money increased (indicating pickup)
        if (currentCash > lastCash)
        {
            int difference = currentCash - lastCash;
            
            // Prevent multiple announcements within a short time frame
            if ((DateTime.Now - lastCashTime).TotalMilliseconds > 500)
            {
                AnnounceMoneyPickup(difference);
                lastCashTime = DateTime.Now;
            }
        }
        
        lastCash = currentCash;
    }

    private void AnnounceMoneyPickup(int amount)
    {
        string message = $"You picked up {amount} dollars.";
        
        try
        {
            Tolk.Output(message, false); // Speak the amount (false = don't interrupt previous speech)
        }
        catch (Exception ex)
        {
            // Fallback to notification if Tolk fails
            GTA.UI.Notification.PostTicker($"Cash pickup: ${amount}", false);
        }
        
        // Optional visual notification
        GTA.UI.Notification.PostTicker($"${amount}", false);
    }
}