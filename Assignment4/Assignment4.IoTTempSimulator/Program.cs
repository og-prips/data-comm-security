using Microsoft.AspNetCore.SignalR.Client;

var hubConnection = new HubConnectionBuilder()
    .WithUrl("https://localhost:7247/temperaturehub")
    .Build();

hubConnection.Closed += async (exception) =>
{
    Console.WriteLine("Connection closed. Attempting to reconnect...");
    await Task.Delay(5000);
    try
    {
        await hubConnection.StartAsync();
        Console.WriteLine("Reconnected.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Reconnection failed: {ex.Message}");
    }
};

try
{
    await hubConnection.StartAsync();
    Console.WriteLine("Connected.");
}
catch (Exception ex)
{
    Console.WriteLine(ex.Message);
    return; // Exit the application if initial connection fails
}

Console.WriteLine("Temperature Sensor Simulator is running. Press Ctrl+C to exit.");

while (true)
{
    if (hubConnection.State != HubConnectionState.Connected)
    {
        Console.WriteLine("Connection lost. Attempting to reconnect...");
        try
        {
            await hubConnection.StartAsync();
            Console.WriteLine("Reconnected.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Reconnection failed: {ex.Message}");
        }
    }

    // Generate a random temperature value between -20 and 40 degrees Celsius
    var temperature = new Random().Next(-20, 41);
    Console.WriteLine($"Sending temperature: {temperature}°C");

    // Send the temperature value to the server via SignalR
    await hubConnection.InvokeAsync("SendTemperature", temperature);

    // Wait for 5 seconds before the next simulation
    await Task.Delay(5000);
}
