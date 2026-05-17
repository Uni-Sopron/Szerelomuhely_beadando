using Microsoft.AspNetCore.Identity;
using SzereloMuhely.Models;

namespace SzereloMuhely.Data
{
    public static class DbSeeder
    {
        public static async Task Initialize(ServiceContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            string[] roles = { "Admin", "Mechanic", "Recruiter" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            if (context.WorkSheets.Any())
            {
                return;
            }

            // 1. Felhasználók létrehozása
            var user1 = new IdentityUser { UserName = "KovacsJanos", Email = "kovacs.janos@muhely.hu", EmailConfirmed = true };
            var user2 = new IdentityUser { UserName = "TothPeter", Email = "toth.peter@muhely.hu", EmailConfirmed = true };
            var user3 = new IdentityUser { UserName = "MesterBela", Email = "mester.bela@muhely.hu", EmailConfirmed = true };
            var user4 = new IdentityUser { UserName = "Admin", Email = "admin@muhely.hu", EmailConfirmed = true };

            await userManager.CreateAsync(user1, "Jelszo123!");
            await userManager.CreateAsync(user2, "Jelszo123!");
            await userManager.CreateAsync(user3, "Jelszo123!");
            await userManager.CreateAsync(user4, "Jelszo123!");

            await userManager.AddToRoleAsync(user1, "Mechanic");
            await userManager.AddToRoleAsync(user2, "Mechanic");
            await userManager.AddToRoleAsync(user3, "Recruiter");
            await userManager.AddToRoleAsync(user4, "Admin");

            var m1 = (await userManager.FindByEmailAsync(user1.Email))!.Id;
            var m2 = (await userManager.FindByEmailAsync(user2.Email))!.Id;
            var r = (await userManager.FindByEmailAsync(user3.Email))!.Id;

            // 2. 20 darab Munkalap létrehozása
            var workSheets = new WorkSheet[]
            {
                new WorkSheet { Title = "Időszakos nagyszerviz és vezérlés csere", MechanicID = m1, RecruiterId = r, CreatedAt = DateTime.Now.AddDays(-12), IsOpen = true },
                new WorkSheet { Title = "Komplett fékrendszer felújítás", MechanicID = m2, RecruiterId = r, CreatedAt = DateTime.Now.AddDays(-10), PaymentMethod = "Bankkártya", IsOpen = false },
                new WorkSheet { Title = "Gyors olajcsere szerviz", MechanicID = m1, RecruiterId = r, CreatedAt = DateTime.Now.AddHours(-2), IsOpen = true },
                new WorkSheet { Title = "Szezonális felkészítés és klímatöltés", MechanicID = m2, RecruiterId = r, CreatedAt = DateTime.Now.AddDays(-4), IsOpen = true },
                new WorkSheet { Title = "Kipufogórendszer javítás", MechanicID = m1, RecruiterId = r, CreatedAt = DateTime.Now.AddDays(-8), PaymentMethod = "Készpénz", IsOpen = false },
                new WorkSheet { Title = "Futómű javítás és lengéscsillapító csere", MechanicID = m2, RecruiterId = r, CreatedAt = DateTime.Now.AddDays(-3), IsOpen = true },
                new WorkSheet { Title = "Hengerfej felújítás és tömítés csere", MechanicID = m1, RecruiterId = r, CreatedAt = DateTime.Now.AddDays(-15), PaymentMethod = "Átutalás", IsOpen = false },
                new WorkSheet { Title = "Műszaki vizsgára felkészítés és vizsgáztatás", MechanicID = m2, RecruiterId = r, CreatedAt = DateTime.Now.AddDays(-1), IsOpen = true },
                new WorkSheet { Title = "Generátor felújítás és ékszíj csere", MechanicID = m1, RecruiterId = r, CreatedAt = DateTime.Now.AddDays(-6), PaymentMethod = "Bankkártya", IsOpen = false },
                new WorkSheet { Title = "Kuplung szett és kettőstömegű csere", MechanicID = m2, RecruiterId = r, CreatedAt = DateTime.Now.AddDays(-7), IsOpen = true },
                new WorkSheet { Title = "Klíma kompresszor csere és rendszer mosás", MechanicID = m1, RecruiterId = r, CreatedAt = DateTime.Now.AddDays(-5), IsOpen = true },
                new WorkSheet { Title = "Önindító javítás", MechanicID = m2, RecruiterId = r, CreatedAt = DateTime.Now.AddDays(-9), PaymentMethod = "Készpénz", IsOpen = false },
                new WorkSheet { Title = "Első kerékcsapágy csere", MechanicID = m1, RecruiterId = r, CreatedAt = DateTime.Now.AddDays(-2), IsOpen = true },
                new WorkSheet { Title = "Diagnosztika és lambdaszonda csere", MechanicID = m2, RecruiterId = r, CreatedAt = DateTime.Now.AddHours(-5), IsOpen = true },
                new WorkSheet { Title = "Hűtőrendszer szivárgás javítás", MechanicID = m1, RecruiterId = r, CreatedAt = DateTime.Now.AddDays(-11), PaymentMethod = "Bankkártya", IsOpen = false },
                new WorkSheet { Title = "Veterán autó részleges restaurálás", MechanicID = m2, RecruiterId = r, CreatedAt = DateTime.Now.AddDays(-25), IsOpen = true },
                new WorkSheet { Title = "Kormánymű felújítás", MechanicID = m1, RecruiterId = r, CreatedAt = DateTime.Now.AddDays(-4), IsOpen = true },
                new WorkSheet { Title = "Üzemanyagrendszer tisztítás és szűrő csere", MechanicID = m2, RecruiterId = r, CreatedAt = DateTime.Now.AddDays(-13), PaymentMethod = "Készpénz", IsOpen = false },
                new WorkSheet { Title = "Ablakemelő motor csere", MechanicID = m1, RecruiterId = r, CreatedAt = DateTime.Now.AddHours(-1), IsOpen = true },
                new WorkSheet { Title = "Gyújtásrendszer diagnosztika és javítás", MechanicID = m2, RecruiterId = r, CreatedAt = DateTime.Now.AddDays(-3), IsOpen = true }
            };
            context.WorkSheets.AddRange(workSheets);
            context.SaveChanges();

            // 3. 20 darab Jármű létrehozása (1:1 kapcsolat)
            var vehicles = new Vehicle[]
            {
                new Vehicle { LicensePlate = "ABC-123", Make = "Toyota", Model = "Corolla", OwnerName = "Nagy István", OwnerAddress = "Budapest, Fő utca 1.", WorkSheetID = workSheets[0].ID },
                new Vehicle { LicensePlate = "XYZ-987", Make = "Volkswagen", Model = "Golf", OwnerName = "Kiss Erzsébet", OwnerAddress = "Debrecen, Kossuth utca 10.", WorkSheetID = workSheets[1].ID },
                new Vehicle { LicensePlate = "GHI-456", Make = "Ford", Model = "Focus", OwnerName = "Varga László", OwnerAddress = "Szeged, Tisza sor 5.", WorkSheetID = workSheets[2].ID },
                new Vehicle { LicensePlate = "SWT-423", Make = "Suzuki", Model = "Swift", OwnerName = "Horváth Zoltán", OwnerAddress = "Pécs, Rákóczi út 20.", WorkSheetID = workSheets[3].ID },
                new Vehicle { LicensePlate = "MNO-321", Make = "Opel", Model = "Astra", OwnerName = "Kovács Petra", OwnerAddress = "Győr, Arany János utca 12.", WorkSheetID = workSheets[4].ID },
                new Vehicle { LicensePlate = "KLR-555", Make = "BMW", Model = "320d", OwnerName = "Szabó Márk", OwnerAddress = "Sopron, Várkerület 45.", WorkSheetID = workSheets[5].ID },
                new Vehicle { LicensePlate = "AAA-111", Make = "Audi", Model = "A4", OwnerName = "Tóth Gábor", OwnerAddress = "Kecskemét, Petőfi tér 2.", WorkSheetID = workSheets[6].ID },
                new Vehicle { LicensePlate = "BBB-222", Make = "Skoda", Model = "Octavia", OwnerName = "Molnár János", OwnerAddress = "Miskolc, Széchenyi út 8.", WorkSheetID = workSheets[7].ID },
                new Vehicle { LicensePlate = "CCC-333", Make = "Renault", Model = "Clio", OwnerName = "Farkas Anita", OwnerAddress = "Nyíregyháza, Iskola utca 14.", WorkSheetID = workSheets[8].ID },
                new Vehicle { LicensePlate = "DDD-444", Make = "Peugeot", Model = "308", OwnerName = "Németh Tamás", OwnerAddress = "Székesfehérvár, Budai út 90.", WorkSheetID = workSheets[9].ID },
                new Vehicle { LicensePlate = "EEE-555", Make = "Citroen", Model = "C4", OwnerName = "Takács Bence", OwnerAddress = "Szombathely, Jókai utca 3.", WorkSheetID = workSheets[10].ID },
                new Vehicle { LicensePlate = "FFF-666", Make = "Fiat", Model = "Punto", OwnerName = "Papp Dorina", OwnerAddress = "Zalaegerszeg, Kossuth tér 1.", WorkSheetID = workSheets[11].ID },
                new Vehicle { LicensePlate = "GGG-777", Make = "Mazda", Model = "6", OwnerName = "Simon Attila", OwnerAddress = "Veszprém, Egyetem utca 5.", WorkSheetID = workSheets[12].ID },
                new Vehicle { LicensePlate = "HHH-888", Make = "Honda", Model = "Civic", OwnerName = "Kelemen Balázs", OwnerAddress = "Eger, Dobó tér 6.", WorkSheetID = workSheets[13].ID },
                new Vehicle { LicensePlate = "III-999", Make = "Hyundai", Model = "i30", OwnerName = "Fekete Csilla", OwnerAddress = "Kaposvár, Fő utca 30.", WorkSheetID = workSheets[14].ID },
                new Vehicle { LicensePlate = "OLD-001", Make = "Mercedes-Benz", Model = "W123", OwnerName = "Gál Zsigmond", OwnerAddress = "Szentendre, Duna korzó 18.", WorkSheetID = workSheets[15].ID },
                new Vehicle { LicensePlate = "JJJ-111", Make = "Nissan", Model = "Qashqai", OwnerName = "Vass Tibor", OwnerAddress = "Tatabánya, Komáromi út 4.", WorkSheetID = workSheets[16].ID },
                new Vehicle { LicensePlate = "KKK-222", Make = "Kia", Model = "Ceed", OwnerName = "Borbély Imre", OwnerAddress = "Békéscsaba, Andrássy út 15.", WorkSheetID = workSheets[17].ID },
                new Vehicle { LicensePlate = "LLL-333", Make = "Dacia", Model = "Duster", OwnerName = "Szőke András", OwnerAddress = "Érd, Budai út 22.", WorkSheetID = workSheets[18].ID },
                new Vehicle { LicensePlate = "MMM-444", Make = "Volvo", Model = "V60", OwnerName = "Gáspár Levente", OwnerAddress = "Gödöllő, Szabadság tér 3.", WorkSheetID = workSheets[19].ID }
            };
            context.Vehicles.AddRange(vehicles);
            context.SaveChanges();

            // 4. Folyamatok, Alkatrészek és Anyagok (Óra alapú munkaidőkkel!)

            // ==================== 1. MUNKALAP: 5+ Folyamatból álló Nagyszerviz (Nyitott) ====================
            var w1_p1 = new WorkProcess { Name = "Általános állapotfelmérés és diagnosztika", Price = 15000, Duration = 1, WorkSheetID = workSheets[0].ID };
            var w1_p2 = new WorkProcess { Name = "Vezérműszíj és vízpumpa csere", Price = 45000, Duration = 4, WorkSheetID = workSheets[0].ID };
            var w1_p3 = new WorkProcess { Name = "Gyújtógyertya csere", Price = 8000, Duration = 1, WorkSheetID = workSheets[0].ID };
            var w1_p4 = new WorkProcess { Name = "Hosszbordásszíj és feszítőgörgő csere", Price = 12000, Duration = 1, WorkSheetID = workSheets[0].ID };
            var w1_p5 = new WorkProcess { Name = "Levegő-, pollen- és üzemanyagszűrő csere", Price = 10000, Duration = 1, WorkSheetID = workSheets[0].ID };
            context.WorkProcesses.AddRange(w1_p1, w1_p2, w1_p3, w1_p4, w1_p5);
            context.SaveChanges();

            context.Materials.Add(new Material { Name = "Féktisztító és zsírtalanító", Price = 1200, Quantity = 2, WorkProcessID = w1_p1.ID });
            context.Parts.Add(new Part { Name = "Vezérműszíj készlet görgőkkel", Price = 48000, Quantity = 1, WorkProcessID = w1_p2.ID });
            context.Parts.Add(new Part { Name = "Vízpumpa", Price = 18500, Quantity = 1, WorkProcessID = w1_p2.ID });
            context.Materials.Add(new Material { Name = "G12+ Hűtőfolyadék", Price = 2200, Quantity = 4, WorkProcessID = w1_p2.ID });
            context.Parts.Add(new Part { Name = "Denso Iridium gyújtógyertya", Price = 5800, Quantity = 4, WorkProcessID = w1_p3.ID });
            context.Parts.Add(new Part { Name = "Hosszbordásszíj", Price = 7200, Quantity = 1, WorkProcessID = w1_p4.ID });
            context.Parts.Add(new Part { Name = "Feszítőgörgő", Price = 11400, Quantity = 1, WorkProcessID = w1_p4.ID });
            context.Parts.Add(new Part { Name = "Levegőszűrő", Price = 4900, Quantity = 1, WorkProcessID = w1_p5.ID });
            context.Parts.Add(new Part { Name = "Pollenszűrő (aktívszenes)", Price = 6800, Quantity = 1, WorkProcessID = w1_p5.ID });
            context.Parts.Add(new Part { Name = "Üzemanyagszűrő", Price = 9200, Quantity = 1, WorkProcessID = w1_p5.ID });

            // ==================== 2. MUNKALAP: Komplett fékrendszer (Lezárt) ====================
            var w2_p1 = new WorkProcess { Name = "Első fékbetétek és tárcsák cseréje", Price = 20000, Duration = 2, WorkSheetID = workSheets[1].ID };
            var w2_p2 = new WorkProcess { Name = "Hátsó féknyergek felújítása és fékfolyadék csere", Price = 28000, Duration = 3, WorkSheetID = workSheets[1].ID };
            context.WorkProcesses.AddRange(w2_p1, w2_p2);
            context.SaveChanges();

            context.Parts.Add(new Part { Name = "Első féktárcsa (Brembo)", Price = 24000, Quantity = 2, WorkProcessID = w2_p1.ID });
            context.Parts.Add(new Part { Name = "Első fékbetét garnitúra", Price = 18500, Quantity = 1, WorkProcessID = w2_p1.ID });
            context.Materials.Add(new Material { Name = "Féktisztító spray", Price = 1500, Quantity = 3, WorkProcessID = w2_p1.ID });
            context.Parts.Add(new Part { Name = "Féknyereg javító készlet", Price = 6200, Quantity = 2, WorkProcessID = w2_p2.ID });
            context.Materials.Add(new Material { Name = "Dot4 Fékfolyadék", Price = 3800, Quantity = 1, WorkProcessID = w2_p2.ID });

            // ==================== 3. MUNKALAP: Gyors olajcsere szerviz (Nyitott) ====================
            var w3_p1 = new WorkProcess { Name = "Motorolaj és olajszűrő csere", Price = 10000, Duration = 1, WorkSheetID = workSheets[2].ID };
            context.WorkProcesses.Add(w3_p1);
            context.SaveChanges();

            context.Materials.Add(new Material { Name = "Mobil1 5W30 motorolaj", Price = 4500, Quantity = 5, WorkProcessID = w3_p1.ID });
            context.Parts.Add(new Part { Name = "Olajszűrő", Price = 3200, Quantity = 1, WorkProcessID = w3_p1.ID });

            // ==================== 4. MUNKALAP: Szezonális felkészítés (Nyitott) ====================
            var w4_p1 = new WorkProcess { Name = "Klímatöltés és gáznyomás ellenőrzés", Price = 15000, Duration = 1, WorkSheetID = workSheets[3].ID };
            var w4_p2 = new WorkProcess { Name = "Ózonos utastér fertőtlenítés", Price = 6000, Duration = 1, WorkSheetID = workSheets[3].ID };
            context.WorkProcesses.AddRange(w4_p1, w4_p2);
            context.SaveChanges();

            context.Materials.Add(new Material { Name = "R134a Klímagáz", Price = 30, Quantity = 450, WorkProcessID = w4_p1.ID });
            context.Materials.Add(new Material { Name = "Klímaolaj és UV festék", Price = 2500, Quantity = 1, WorkProcessID = w4_p1.ID });

            // ==================== 5. MUNKALAP: Kipufogó javítás (Lezárt) ====================
            var w5_p1 = new WorkProcess { Name = "Kipufogó flexibilis cső hegesztés", Price = 18000, Duration = 2, WorkSheetID = workSheets[4].ID };
            context.WorkProcesses.Add(w5_p1);
            context.SaveChanges();

            context.Parts.Add(new Part { Name = "Flexibilis cső (50x200)", Price = 8500, Quantity = 1, WorkProcessID = w5_p1.ID });
            context.Materials.Add(new Material { Name = "Hegesztőhuzal", Price = 4000, Quantity = 1, WorkProcessID = w5_p1.ID });

            // ==================== 6. MUNKALAP: Futómű javítás (Nyitott) ====================
            var w6_p1 = new WorkProcess { Name = "Első lengéscsillapítók cseréje", Price = 35000, Duration = 3, WorkSheetID = workSheets[5].ID };
            var w6_p2 = new WorkProcess { Name = "Lézeres futómű beállítás", Price = 14000, Duration = 1, WorkSheetID = workSheets[5].ID };
            context.WorkProcesses.AddRange(w6_p1, w6_p2);
            context.SaveChanges();

            context.Parts.Add(new Part { Name = "Monroe lengéscsillapító", Price = 29000, Quantity = 2, WorkProcessID = w6_p1.ID });
            context.Parts.Add(new Part { Name = "Toronycsapágy készlet", Price = 12500, Quantity = 2, WorkProcessID = w6_p1.ID });

            // ==================== 7. MUNKALAP: Hengerfej felújítás (Lezárt) ====================
            var w7_p1 = new WorkProcess { Name = "Hengerfej leszerelés és nyomáspróba", Price = 40000, Duration = 4, WorkSheetID = workSheets[6].ID };
            var w7_p2 = new WorkProcess { Name = "Szelepek becsiszolása és síkköszörülés", Price = 35000, Duration = 3, WorkSheetID = workSheets[6].ID };
            var w7_p3 = new WorkProcess { Name = "Hengerfej összeszerelés", Price = 45000, Duration = 5, WorkSheetID = workSheets[6].ID };
            context.WorkProcesses.AddRange(w7_p1, w7_p2, w7_p3);
            context.SaveChanges();

            context.Parts.Add(new Part { Name = "Hengerfej tömítés készlet", Price = 32000, Quantity = 1, WorkProcessID = w7_p3.ID });
            context.Parts.Add(new Part { Name = "Hengerfej csavar szett", Price = 14500, Quantity = 1, WorkProcessID = w7_p3.ID });

            // ==================== 8-15. MUNKALAPOK: Kisebb és közepes napi javítások ====================
            var w8_p = new WorkProcess { Name = "Műszaki átvizsgálás vizsgasorral", Price = 18000, Duration = 1, WorkSheetID = workSheets[7].ID };
            context.WorkProcesses.Add(w8_p);

            var w9_p = new WorkProcess { Name = "Generátor kiszerelés és diódahíd csere", Price = 22000, Duration = 2, WorkSheetID = workSheets[8].ID };
            context.WorkProcesses.Add(w9_p);
            context.SaveChanges();
            context.Parts.Add(new Part { Name = "Generátor diódahíd", Price = 19800, Quantity = 1, WorkProcessID = w9_p.ID });

            var w10_p = new WorkProcess { Name = "Sebességváltó le-fel és kuplung csere", Price = 55000, Duration = 6, WorkSheetID = workSheets[9].ID };
            context.WorkProcesses.Add(w10_p);
            context.SaveChanges();
            context.Parts.Add(new Part { Name = "LUK Kuplung szett kettőstömegűvel", Price = 185000, Quantity = 1, WorkProcessID = w10_p.ID });

            var w11_p = new WorkProcess { Name = "Klíma kompresszor csere", Price = 25000, Duration = 2, WorkSheetID = workSheets[10].ID };
            context.WorkProcesses.Add(w11_p);
            context.SaveChanges();
            context.Parts.Add(new Part { Name = "Klíma kompresszor", Price = 115000, Quantity = 1, WorkProcessID = w11_p.ID });

            var w12_p = new WorkProcess { Name = "Önindító felújítás", Price = 16000, Duration = 2, WorkSheetID = workSheets[11].ID };
            context.WorkProcesses.Add(w12_p);
            context.SaveChanges();
            context.Parts.Add(new Part { Name = "Önindító belső készlet", Price = 9400, Quantity = 1, WorkProcessID = w12_p.ID });

            var w13_p = new WorkProcess { Name = "Bal első kerékcsapágy csere préssel", Price = 15000, Duration = 2, WorkSheetID = workSheets[12].ID };
            context.WorkProcesses.Add(w13_p);
            context.SaveChanges();
            context.Parts.Add(new Part { Name = "SKF Kerékcsapágy", Price = 24500, Quantity = 1, WorkProcessID = w13_p.ID });

            var w14_p = new WorkProcess { Name = "Diagnosztika és Lambdaszonda csere", Price = 14000, Duration = 1, WorkSheetID = workSheets[13].ID };
            context.WorkProcesses.Add(w14_p);
            context.SaveChanges();
            context.Parts.Add(new Part { Name = "Bosch Lambdaszonda", Price = 34000, Quantity = 1, WorkProcessID = w14_p.ID });

            var w15_p = new WorkProcess { Name = "Vízhűtő radiátor csere", Price = 20000, Duration = 2, WorkSheetID = workSheets[14].ID };
            context.WorkProcesses.Add(w15_p);
            context.SaveChanges();
            context.Parts.Add(new Part { Name = "Vízhűtő radiátor", Price = 28900, Quantity = 1, WorkProcessID = w15_p.ID });

            // ==================== 16. MUNKALAP: 5+ Folyamatos Ékszerdoboz - Veterán felújítás (Nyitott) ====================
            var w16_p1 = new WorkProcess { Name = "Karosszéria rozsdamentesítés", Price = 50000, Duration = 8, WorkSheetID = workSheets[15].ID };
            var w16_p2 = new WorkProcess { Name = "Karburátor ultrahangos tisztítás", Price = 25000, Duration = 3, WorkSheetID = workSheets[15].ID };
            var w16_p3 = new WorkProcess { Name = "Szelephézag állítás", Price = 15000, Duration = 2, WorkSheetID = workSheets[15].ID };
            var w16_p4 = new WorkProcess { Name = "Egyedi fékcsövek gyártása", Price = 30000, Duration = 3, WorkSheetID = workSheets[15].ID };
            var w16_p5 = new WorkProcess { Name = "Futómű szilentek komplett cseréje", Price = 40000, Duration = 4, WorkSheetID = workSheets[15].ID };
            context.WorkProcesses.AddRange(w16_p1, w16_p2, w16_p3, w16_p4, w16_p5);
            context.SaveChanges();

            context.Materials.Add(new Material { Name = "Rozsdagátló alapozó", Price = 8500, Quantity = 2, WorkProcessID = w16_p1.ID });
            context.Parts.Add(new Part { Name = "Karburátor fúvóka készlet", Price = 14200, Quantity = 1, WorkProcessID = w16_p2.ID });
            context.Parts.Add(new Part { Name = "Szelepfedél tömítés", Price = 4300, Quantity = 1, WorkProcessID = w16_p3.ID });
            context.Materials.Add(new Material { Name = "Réz fékcső (méter)", Price = 1800, Quantity = 6, WorkProcessID = w16_p4.ID });
            context.Parts.Add(new Part { Name = "Poliuretán szilent szett", Price = 36000, Quantity = 1, WorkProcessID = w16_p5.ID });

            // ==================== 17-20. MUNKALAPOK: Befejező adatok ====================
            var w17_p = new WorkProcess { Name = "Kormánymű szimmering sor csere", Price = 45000, Duration = 4, WorkSheetID = workSheets[16].ID };
            context.WorkProcesses.Add(w17_p);

            var w18_p = new WorkProcess { Name = "Üzemanyagrendszer tisztítás", Price = 35000, Duration = 3, WorkSheetID = workSheets[17].ID };
            context.WorkProcesses.Add(w18_p);
            context.SaveChanges();
            context.Parts.Add(new Part { Name = "Gázolajszűrő", Price = 14500, Quantity = 1, WorkProcessID = w18_p.ID });

            var w19_p = new WorkProcess { Name = "Ablakemelő szerkezet csere", Price = 12000, Duration = 1, WorkSheetID = workSheets[18].ID };
            context.WorkProcesses.Add(w19_p);
            context.SaveChanges();
            context.Parts.Add(new Part { Name = "Ablakemelő mechanika motorral", Price = 26400, Quantity = 1, WorkProcessID = w19_p.ID });

            var w20_p1 = new WorkProcess { Name = "Gyújtáselosztó fedél csere", Price = 8000, Duration = 1, WorkSheetID = workSheets[19].ID };
            var w20_p2 = new WorkProcess { Name = "Gyújtáskábel készlet csere", Price = 6000, Duration = 1, WorkSheetID = workSheets[19].ID };
            context.WorkProcesses.AddRange(w20_p1, w20_p2);

            context.SaveChanges();
        }
    }
}