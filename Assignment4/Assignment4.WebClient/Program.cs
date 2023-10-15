var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

var app = builder.Build();

// CSP f�r att begr�nsa k�llor f�r webbinneh�ll, till�ter anslutning i form av HTTPS och WSS fr�n APIet
app.Use(async (context, next) =>
{
    string csp =
        "default-src 'self'; " +
        "connect-src 'self' https://localhost:7205 wss://localhost:7205 wss://localhost:44304/Assignment4.WebClient/; " +
        "script-src 'self'; " +
        "style-src 'self'; " +
        "font-src 'self'; " +
        "img-src 'self'; " +
        "frame-src 'self'";

    // L�gg till CSP-headers i HTTP-svaret.
    context.Response.Headers.Add("Content-Security-Policy", csp);

    await next();
});

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
