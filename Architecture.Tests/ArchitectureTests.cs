using System.Reflection;
using Architecture.Tests.Rules;
using Common.Configuration;
using FluentAssertions;
using NetArchTest.Rules;

namespace Architecture.Tests
{
    public class ArchitectureTests
    {
        [Fact]
        public void GivenCommonTypes_WhenAccessedFromOtherProjects_ThenTheyAreVisible()
        {
            // Arrange
            var commonAssembly = typeof(ApiSettings).Assembly;

            // Act
            var result = Types.InAssembly(commonAssembly)
                .Should().BePublic()
                .GetResult();

            // Assert
            result.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void GivenServiceInterfaces_ThenShouldBePublicAndBeInterfacesAndStartWithI()
        {
            // Arrange
            var interfacesShouldStartWithIRule = new InterfacesShouldStartWithIRule();

            foreach (var assembly in GetAllAssemblies())
            {
                // Act
                var result = Types.InAssembly(assembly)
                    .Should().MeetCustomRule(interfacesShouldStartWithIRule)
                    .GetResult();

                // Assert
                result.IsSuccessful.Should().BeTrue();
            }
        }

        private IEnumerable<Assembly> GetAllAssemblies()
        {
            return new[]
            {
                typeof(Catalog.API.Controllers.CatalogController).Assembly,
                typeof(ApiSettings).Assembly,
                typeof(CustomerAPI.Controllers.CustomerController).Assembly,
                typeof(FeedbackAPI.Data.IFeedbackContext).Assembly,
                typeof(Notifications.NotificationsWorker).Assembly,
                typeof(OrderAPI.Controllers.OrderController).Assembly,
                typeof(OrderCommon.Data.IOrderContext).Assembly,
                typeof(OrderGrpc.Worker).Assembly,
                typeof(OrderProcessor.OrderProcessorWorker).Assembly,
                typeof(PaymentService.PaymentWorker).Assembly,
                typeof(Shipment.Worker).Assembly,
                typeof(ShoppingCartAPI.Controllers.ShoppingCartController).Assembly,
                typeof(Warehouse.WarehouseWorker).Assembly,
                typeof(WarehouseAPI.Controllers.StockItemController).Assembly,
                typeof(WarehouseCommon.Data.IWarehouseContext).Assembly,
                typeof(WebUI.Services.ICustomerService).Assembly,
                typeof(WebUIAggregatorAPI.Controllers.ProductsController).Assembly
            };
        }
    }
}