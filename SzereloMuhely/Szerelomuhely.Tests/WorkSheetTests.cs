using SzereloMuhely.Models;
using Xunit;

namespace SzereloMuhely.Tests
{
    public class WorkSheetTests
    {
        [Fact]
        public void TotalAmount_Calculation_IsCorrect()
        {
            // Arrange
            var workSheet = new WorkSheet();
            var workProcess = new WorkProcess
            {
                Price = 5000, // Munkadíj
                Materials = new List<Material>
                {
                    new Material { Price = 1000, Quantity = 2 }, // 2000
                    new Material { Price = 500, Quantity = 1 }   // 500
                },
                Parts = new List<Part>
                {
                    new Part { Price = 3000, Quantity = 1 }      // 3000
                }
            };
            workSheet.WorkProcesses.Add(workProcess);

            // Act
            var total = workSheet.TotalAmount;

            // Assert
            // 5000 (wp) + 2000 (m1) + 500 (m2) + 3000 (p1) = 10500
            Assert.Equal(10500, total);
        }

        [Fact]
        public void TotalAmount_WithMultipleProcesses_IsCorrect()
        {
            // Arrange
            var workSheet = new WorkSheet();
            
            var wp1 = new WorkProcess { Price = 2000 };
            var wp2 = new WorkProcess { Price = 3000 };
            
            workSheet.WorkProcesses.Add(wp1);
            workSheet.WorkProcesses.Add(wp2);

            // Act
            var total = workSheet.TotalAmount;

            // Assert
            Assert.Equal(5000, total);
        }

        [Fact]
        public void TotalAmount_EmptyWorkSheet_ReturnsZero()
        {
            // Arrange
            var workSheet = new WorkSheet();

            // Act
            var total = workSheet.TotalAmount;

            // Assert
            Assert.Equal(0, total);
        }
    }
}
