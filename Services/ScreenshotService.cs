using Microsoft.Extensions.Logging;
using PuppeteerSharp;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ScreenshotWebApp.Services
{
    public class ScreenshotService
    {
        private readonly ILogger<ScreenshotService> _logger;

        public ScreenshotService(ILogger<ScreenshotService> logger)
        {
            _logger = logger;
        }

        public async Task TakeScreenshotAsync()
        {
            try
            {
                _logger.LogInformation("Screenshot alma işlemi başladı: {time}", DateTime.Now);

                var launchOptions = new LaunchOptions
                {
                    Headless = true, // Prod ortamda genellikle true olur
                    Args = new[] { "--no-sandbox", "--disable-setuid-sandbox" }
                };

                using var browser = await Puppeteer.LaunchAsync(launchOptions);
                using var page = await browser.NewPageAsync();

                //  Genişlik ve yükseklik ayarı 
                await page.SetViewportAsync(new ViewPortOptions
                {
                    Width = 1445,
                    Height = 1440
                });

                //  Kullanıcı tarayıcı bilgisi ve dil
                await page.SetUserAgentAsync("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/123.0.0.0 Safari/537.36");

                await page.SetExtraHttpHeadersAsync(new Dictionary<string, string>
                {
                    ["Accept-Language"] = "tr-TR,tr;q=0.9"
                });

                //  Kayıt klasörü
                string folderPath = Path.Combine(AppContext.BaseDirectory, "Screenshots");
                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                string filename = $"screenshot_{DateTime.Now:yyyyMMdd_HHmmss}.png";
                string fullPath = Path.Combine(folderPath, filename);

                //  Sayfaya git
                await page.GoToAsync(
                    "https://www.tcmb.gov.tr/wps/wcm/connect/tr/tcmb+tr/main+page+site+area/bugun",
                    WaitUntilNavigation.Networkidle0
                );

                

                //  Ekran görüntüsü al
                await page.ScreenshotAsync(fullPath, new ScreenshotOptions { FullPage = true });

                _logger.LogInformation("Ekran görüntüsü kaydedildi: {path}", fullPath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Screenshot alma sırasında hata oluştu.");
            }
        }
    }
}
