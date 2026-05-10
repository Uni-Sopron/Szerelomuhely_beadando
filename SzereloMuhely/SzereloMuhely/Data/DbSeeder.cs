using Microsoft.AspNetCore.Identity;
using SzereloMuhely.Models;

namespace SzereloMuhely.Data
{
    public static class DbSeeder
    {
        public static async Task Initialize(ServiceContext context, UserManager<IdentityUser> userManager)
        {
            context.Database.EnsureCreated();

            if (context.WorkSheets.Any())
            {
                return;
            }

            // 1. Felhasználók létrehozása Identity-vel
            // Megnézzük, létezik-e már a teszt felhasználó
            var mechanicEmail = "kovacs.janos@muhely.hu";
            var mechanic2Email = "toth.peter@muhely.hu";
            var recruiterEmail = "mester.bela@muhely.hu";
            var adminEmail = "admin@muhely.hu";

            var mechanicName = "Kovács János";
            var mechanic2Name = "Tóth Péter";
            var recruiterName = "Mester Béla";
            var adminName = "Admin";

            if (await userManager.FindByEmailAsync(mechanicEmail) == null)
            {
                var user1 = new IdentityUser { UserName = mechanicName, Email = mechanicEmail, EmailConfirmed = true };
                var user2 = new IdentityUser { UserName = mechanic2Name, Email = mechanic2Email, EmailConfirmed = true };
                var user3 = new IdentityUser { UserName = recruiterName, Email = recruiterEmail, EmailConfirmed = true };
                var user4 = new IdentityUser { UserName = adminName, Email = adminEmail, EmailConfirmed = true };

                await userManager.CreateAsync(user1, "Password123!");
                await userManager.CreateAsync(user2, "Password123!");
                await userManager.CreateAsync(user3, "Password123!");
                await userManager.CreateAsync(user4, "Password123!");
            }

            // Lekérjük a generált ID-kat az adatbázisból a munkalapokhoz
            var mechanic1 = await userManager.FindByEmailAsync(mechanicEmail);
            var mechanic2 = await userManager.FindByEmailAsync(mechanic2Email);
            var recruiter = await userManager.FindByEmailAsync(recruiterEmail);
            var admin = await userManager.FindByEmailAsync(adminEmail);

            // 2. Seed WorkSheets
            var workSheets = new WorkSheet[]
            {
                new WorkSheet
                {
                    Title = "Éves szerviz - ABC-123",
                    MechanicID = mechanic1!.Id,
                    RecruiterName = recruiterName,
                    CreatedAt = DateTime.Now.AddDays(-2),
                    Status = true
                },
                new WorkSheet
                {
                    Title = "Fékjavítás - XYZ-987",
                    MechanicID = mechanic2!.Id,
                    RecruiterName = recruiterName,
                    CreatedAt = DateTime.Now.AddDays(-5),
                    Status = false
                },
                new WorkSheet
                {
                    Title = "Olajcsere - GHI-456",
                    MechanicID = mechanic1.Id,
                    RecruiterName = recruiterName,
                    CreatedAt = DateTime.Now.AddHours(-3),
                    Status = true
                },
                new WorkSheet
                {
                    Title = "Kerékcsere - SWT-423",
                    MechanicID = mechanic1.Id,
                    RecruiterName = recruiterName,
                    CreatedAt = DateTime.Now.AddDays(-5),
                    Status = true
                }
            };
            context.WorkSheets.AddRange(workSheets);
            context.SaveChanges();

            // 3. Seed Vehicles (1:1 with WorkSheets in this model)
            var vehicles = new Vehicle[]
            {
                new Vehicle
                {
                    LicensePlate = "ABC-123", Make = "Toyota", Model = "Corolla",
                    OwnerName = "Nagy István", OwnerAddress = "Budapest, Fő utca 1.",
                    WorkSheetID = workSheets[0].ID
                },
                new Vehicle
                {
                    LicensePlate = "XYZ-987", Make = "Volkswagen", Model = "Golf",
                    OwnerName = "Kiss Erzsébet", OwnerAddress = "Debrecen, Kossuth utca 10.",
                    WorkSheetID = workSheets[1].ID
                },
                new Vehicle
                {
                    LicensePlate = "GHI-456", Make = "Ford", Model = "Focus",
                    OwnerName = "Varga László", OwnerAddress = "Szeged, Tisza sor 5.",
                    WorkSheetID = workSheets[2].ID
                },
                new Vehicle
                {
                    LicensePlate = "SWT-423", Make = "Suzuki", Model = "Swift",
                    OwnerName = "Horváth Zoltán", OwnerAddress = "Pécs, Rákóczi út 20.",
                    WorkSheetID = workSheets[3].ID
                }
            };
            context.Vehicles.AddRange(vehicles);
            context.SaveChanges();

            // 4. Seed WorkProcesses, Materials, and Parts
            // For WorkSheet 1 (Open)
            var wp1 = new WorkProcess
            {
                Name = "Általános átvizsgálás",
                Price = 15000,
                Duration = 60,
                WorkSheetID = workSheets[0].ID
            };
            context.WorkProcesses.Add(wp1);
            context.SaveChanges();

            context.Materials.Add(new Material { Name = "Tisztítófolyadék", Price = 1200, Quantity = 2, WorkProcessID = wp1.ID });

            // For WorkSheet 2 (Closed)
            var wp2 = new WorkProcess
            {
                Name = "Fékbetét csere",
                Price = 20000,
                Duration = 120,
                WorkSheetID = workSheets[1].ID
            };
            context.WorkProcesses.Add(wp2);
            context.SaveChanges();

            context.Parts.Add(new Part { Name = "Első fékbetét készlet", Price = 35000, Quantity = 1, WorkProcessID = wp2.ID });
            context.Materials.Add(new Material { Name = "Féktisztító", Price = 2500, Quantity = 1, WorkProcessID = wp2.ID });

            // For WorkSheet 3 (Open)
            var wp3 = new WorkProcess
            {
                Name = "Olajszerviz",
                Price = 10000,
                Duration = 45,
                WorkSheetID = workSheets[2].ID
            };
            context.WorkProcesses.Add(wp3);
            context.SaveChanges();

            context.Materials.Add(new Material { Name = "Motorolaj 5W30", Price = 4500, Quantity = 5, WorkProcessID = wp3.ID });
            context.Parts.Add(new Part { Name = "Olajszűrő", Price = 3800, Quantity = 1, WorkProcessID = wp3.ID });

            context.SaveChanges();
            var wp4 = new WorkProcess
            {
                Name = "Kerékcsere",
                Price = 10000,
                Duration = 1,
                WorkSheetID = workSheets[3].ID
            };
            var vp5 = new WorkProcess
            {
                Name = "Új gumik felrakása",
                Price = 5000,
                Duration = 1,
                WorkSheetID = workSheets[3].ID
            };
            context.WorkProcesses.Add(wp4);
            context.WorkProcesses.Add(vp5);
            context.SaveChanges();

            context.Materials.Add(new Material { Name = "Kerékcsavar", Price = 1000, Quantity = 4, WorkProcessID = wp4.ID });
            context.Parts.Add(new Part { Name = "Gumiabroncs", Price = 30000, Quantity = 4, WorkProcessID = vp5.ID });
            context.SaveChanges();
        }
    }
}
