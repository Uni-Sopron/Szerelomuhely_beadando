using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using SzereloMuhely.Controllers;
using SzereloMuhely.Data;
using SzereloMuhely.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace SzereloMuhely.Tests
{
    public class WorkSheetsControllerTests
    {
        private (ServiceContext, ApplicationDbContext) GetDbContexts()
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var options1 = new DbContextOptionsBuilder<ServiceContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .UseInternalServiceProvider(serviceProvider)
                .Options;
            
            var options2 = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            return (new ServiceContext(options1), new ApplicationDbContext(options2));
        }

        private WorkSheetsController GetController(ServiceContext context, ApplicationDbContext identityContext, string role = "Admin", string userId = "test-user")
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            }, "mock"));

            var controller = new WorkSheetsController(context, identityContext, null); 
            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
            return controller;
        }

        [Fact]
        public async Task Index_FiltersOpenWorkSheetsByDefault()
        {
            // Arrange
            var (context, identityContext) = GetDbContexts();
            context.WorkSheets.AddRange(
                new WorkSheet { Title = "Open 1", IsOpen = true, MechanicID = "m1", RecruiterId = "r1" },
                new WorkSheet { Title = "Closed 1", IsOpen = false, MechanicID = "m1", RecruiterId = "r1" }
            );
            await context.SaveChangesAsync();

            var controller = GetController(context, identityContext, "Mechanic", "m1");

            // Act
            var result = await controller.Index(null, null, false);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<WorkSheet>>(viewResult.ViewData.Model);
            Assert.Single(model);
            Assert.True(model.First().IsOpen);
        }

        [Fact]
        public async Task Index_ShowsAllWorkSheetsWhenRequestedByAdmin()
        {
            // Arrange
            var (context, identityContext) = GetDbContexts();
            context.WorkSheets.AddRange(
                new WorkSheet { Title = "Open 1", IsOpen = true, MechanicID = "m1", RecruiterId = "r1" },
                new WorkSheet { Title = "Closed 1", IsOpen = false, MechanicID = "m2", RecruiterId = "r1" }
            );
            await context.SaveChangesAsync();

            var controller = GetController(context, identityContext, "Admin");

            // Act
            var result = await controller.Index(null, null, true);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<WorkSheet>>(viewResult.ViewData.Model);
            Assert.Equal(2, model.Count());
        }

        [Fact]
        public async Task Edit_ReturnsBadRequest_WhenWorkSheetIsClosed()
        {
            // Arrange
            var (context, identityContext) = GetDbContexts();
            var closedWorkSheet = new WorkSheet { ID = 1, Title = "Closed", IsOpen = false, MechanicID = "m1", RecruiterId = "r1" };
            context.WorkSheets.Add(closedWorkSheet);
            await context.SaveChangesAsync();

            var controller = GetController(context, identityContext, "Admin");

            // Act
            var result = await controller.Edit(1);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Lezárt munkalap nem módosítható.", badRequestResult.Value);
        }

        [Fact]
        public async Task Index_SearchFiltersResultsCorrectily()
        {
            // Arrange
            var (context, identityContext) = GetDbContexts();
            context.WorkSheets.AddRange(
                new WorkSheet { Title = "Brake Repair", IsOpen = true, MechanicID = "m1", RecruiterId = "r1" },
                new WorkSheet { Title = "Oil Change", IsOpen = true, MechanicID = "m1", RecruiterId = "r1" }
            );
            await context.SaveChangesAsync();

            var controller = GetController(context, identityContext, "Admin");

            // Act
            var result = await controller.Index("Brake", null, true);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<WorkSheet>>(viewResult.ViewData.Model);
            Assert.Single(model);
            Assert.Contains("Brake", model.First().Title);
        }
    }
}
