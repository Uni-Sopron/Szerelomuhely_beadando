using SzereloMuhely.Models;

namespace SzereloMuhely.Data
{
    public static class DbSeeder
    {
        public static void Initialize(ServiceContext context)
        {
            context.Database.EnsureCreated();

            if (context.WorkSheets.Any())
            {
                return;
            }

            // 1. Seed Users (Mechanics and Recruiters)
            var users = new User[]
            {
                new User { Username = "kovacs.janos", Password = "password123" }, // Mechanic ID: 1
                new User { Username = "szabo.mari", Password = "password123" },  // Recruiter ID: 2
                new User { Username = "toth.peter", Password = "password123" }   // Mechanic ID: 3
            };
            context.Users.AddRange(users);
            context.SaveChanges();

            // 2. Seed WorkSheets
            var workSheets = new WorkSheet[]
            {
                new WorkSheet
                {
                    Title = "Éves szerviz - ABC-123",
                    MechanicID = 1,
                    RecruiterName = "Szabó Mari",
                    CreatedAt = DateTime.Now.AddDays(-2),
                    Status = true
                },
                new WorkSheet
                {
                    Title = "Fékjavítás - XYZ-987",
                    MechanicID = 3,
                    RecruiterName = "Szabó Mari",
                    CreatedAt = DateTime.Now.AddDays(-5),
                    Status = false,
                    PaymentMethod = "Készpénz"
                },
                new WorkSheet
                {
                    Title = "Olajcsere - GHI-456",
                    MechanicID = 1,
                    RecruiterName = "Kovács Béla",
                    CreatedAt = DateTime.Now.AddHours(-3),
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
        }
    }
}
