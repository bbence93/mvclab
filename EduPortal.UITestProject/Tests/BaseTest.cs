using AngleSharp;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace EduPortal.UITestProject.Tests;

public class BaseTest : IDisposable
{
    protected IWebDriver Driver { get; private set; }
    protected string _baseUrl;

    public BaseTest()
    {
        new DriverManager().SetUpDriver(new ChromeConfig());

        var envUrl = Environment.GetEnvironmentVariable("EDUPORTAL_URL");
        _baseUrl = string.IsNullOrEmpty(envUrl)
            ? "http://192.168.1.79:8080" // Fallback (Otthoni fix IP)
            : envUrl; // CI/CD-bõl jön

        var options = new ChromeOptions();
        options.AddArgument("--start-maximized");
        // Initialize WebDriver (e.g., ChromeDriver, FirefoxDriver, etc.)
        Driver = new ChromeDriver(options);

        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
    }
    public void Dispose()
    {
        Driver?.Quit();
        Driver?.Dispose();
    }
}
    public static class TestDataHelper
    {
    // A Connection Stringet ideális esetben appsettings.json-ból olvasnánk,
    // de most a példa kedvéért ideírom (írd át a sajátodra!):
    private static readonly string ConnectionString = TestConfiguration.GetConnectionString();

    // Egy fix Hash a "Password123!" jelszóhoz (ASP.NET Core Identity V3 formátum)
    // Így nem kell a jelszó hash-elési logikát újradefiniálni a tesztben.
    private const string FixedPasswordHash = "AQAAAAEAACcQAAAAEH8Z..."; // Ez csak példa, lentebb adok egy mûködõt

        public static void CreateUser(string username, string password)
        {
            // 1. SQL INSERT parancs
            // Figyeld meg: a FirstName, LastName, DOB mezõket mi találjuk ki itt helyben!
            string query = @"
                INSERT INTO AspNetUsers (
                    Id, 
                    UserName, NormalizedUserName, 
                    Email, NormalizedEmail, 
                    EmailConfirmed, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount,
                    PasswordHash, SecurityStamp, ConcurrencyStamp,
                    FirstName, LastName, DateOfBirth
                )
                VALUES (
                    @Id, 
                    @UserName, @NormalizedUserName, 
                    @Email, @NormalizedEmail, 
                    1, 0, 0, 1, 0,
                    @PasswordHash, NEWID(), NEWID(),
                    @FirstName, @LastName, @DateOfBirth
                )";

            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (var command = new SqlCommand(query, connection))
                {
                    // 2. Paraméterek feltöltése
                    // GUID generálása
                    command.Parameters.AddWithValue("@Id", Guid.NewGuid());

                    // Username kezelése
                    command.Parameters.AddWithValue("@UserName", username);
                    command.Parameters.AddWithValue("@NormalizedUserName", username.ToUpper());

                    // Email generálása a username-bõl
                    command.Parameters.AddWithValue("@Email", $"{username}@test.com");
                    command.Parameters.AddWithValue("@NormalizedEmail", $"{username.ToUpper()}@TEST.COM");

                    // JELSZÓ KEZELÉSE:
                    // A legegyszerûbb, ha a paraméterben kapott 'password'-öt figyelmen kívül hagyjuk,
                    // és mindig egy ismert hash-t szúrunk be, amihez tudjuk a jelszót (pl. "Password123!").
                    // Ha dinamikus jelszót akarsz, ahhoz kellene az ASP.NET Identity könyvtár.
                    command.Parameters.AddWithValue("@PasswordHash", GetHashForPassword(password));

                    // A TE EGYEDI MEZÕID (Dummy adatok):
                    command.Parameters.AddWithValue("@FirstName", "Test");
                    command.Parameters.AddWithValue("@LastName", "User");
                    // Figyelj, hogy valid dátum legyen (pl. 18+ éves)
                    command.Parameters.AddWithValue("@DateOfBirth", new DateTime(1990, 1, 1));

                    command.ExecuteNonQuery();
                }
            }
        }

        // Segédmetódus a jelszóhoz
        private static string GetHashForPassword(string password)
        {
            // TRÜKK: Teszteléshez a legegyszerûbb, ha mindig ugyanazt a jelszót várjuk el,
            // vagy beégetünk egy mûködõ hash-t a "Password123!"-hoz.
            // Ez egy valid ASP.NET Core Identity hash a "Password123!" stringhez:
            if (password == "Password123!")
            {
                return "AQAAAAEAACcQAAAAEExhgA5... (ide egy valódi hosszú hash kell)";
            }

            // Ha komolyan dinamikus jelszót akarsz, akkor be kell húzni a 
            // Microsoft.AspNetCore.Identity csomagot és használni a PasswordHasher<T>-t.
            // De kezdetnek tedd egyszerûvé: mindig Password123!-al hozd létre a tesztusert.
            return "AQAAAAIAAYagAAAAEP0/4..."; // "Password123!" hash
        }
    }

public static class TestConfiguration
{
    private static IConfiguration _configuration;

    // Static konstruktor: csak egyszer fut le, amikor elõször használod
    static TestConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // A bin mappa
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            // Ez nagyon fontos: a környezeti változók felülírják a JSON-t!
            .AddEnvironmentVariables();

        _configuration = builder.Build();
    }

    public static string GetConnectionString()
    {
        return _configuration.GetConnectionString("DefaultConnection");
    }

    public static string GetBaseUrl()
    {
        return _configuration["TestSettings:BaseUrl"];
    }
}

