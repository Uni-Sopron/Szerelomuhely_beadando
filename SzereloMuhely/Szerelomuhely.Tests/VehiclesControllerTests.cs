using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SzereloMuhely.Controllers;
using SzereloMuhely.Data;
using SzereloMuhely.Models;
using System.Security.Claims;
using Xunit;

namespace SzereloMuhely.Tests
{
    public class VehiclesControllerTests
    {
        private ServiceContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ServiceContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ServiceContext(options);
        }

        private VehiclesController GetController(ServiceContext context, string role = "Admin")
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user"),
                new Claim(ClaimTypes.Role, role)
            }, "mock"));

            var controller = new VehiclesController(context);
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            return controller;
        }

        [Fact]
        public async Task Index_FiltersByOpenWorkSheets_ForNonAdmin()
        {
            // Arrange
            using var context = GetDbContext();
            var openWs = new WorkSheet { ID = 1, Title = "Open", IsOpen = true, MechanicID = "m1", RecruiterId = "r1" };
            var closedWs = new WorkSheet { ID = 2, Title = "Closed", IsOpen = false, MechanicID = "m1", RecruiterId = "r1" };
            context.WorkSheets.AddRange(openWs, closedWs);
            
            context.Vehicles.AddRange(
                new Vehicle { ID = 10, LicensePlate = "ABC-123", WorkSheetID = 1, Make = "Ford", Model = "Focus", OwnerName = "John", OwnerAddress = "Street 1" },
                new Vehicle { ID = 20, LicensePlate = "XYZ-789", WorkSheetID = 2, Make = "Opel", Model = "Astra", OwnerName = "Jane", OwnerAddress = "Street 2" }
            );
            await context.SaveChangesAsync();

            var controller = GetController(context, "Recruiter");

            // Act
            var result = await controller.Index(null, false);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Vehicle>>(viewResult.ViewData.Model);
            Assert.Single(model);
            Assert.Equal("ABC-123", model.First().LicensePlate);
        }

        [Fact]
        public async Task Index_ShowsAllForAdminEvenWhenClosed()
        {
            // Arrange
            using var context = GetDbContext();
            var ws = new WorkSheet { ID = 1, Title = "Closed", IsOpen = false, MechanicID = "m1", RecruiterId = "r1" };
            context.WorkSheets.Add(ws);
            context.Vehicles.Add(new Vehicle { ID = 1, LicensePlate = "V-1", WorkSheetID = 1, Make = "F", Model = "F", OwnerName = "O", OwnerAddress = "A" });
            await context.SaveChangesAsync();

            var controller = GetController(context, "Admin");

            // Act
            var result = await controller.Index(null, true);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<Vehicle>>(viewResult.ViewData.Model);
            Assert.Single(model);
        }
    }
}
