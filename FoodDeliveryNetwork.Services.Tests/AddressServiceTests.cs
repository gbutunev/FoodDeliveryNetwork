using FoodDeliveryNetwork.Data;
using FoodDeliveryNetwork.Data.Models;
using FoodDeliveryNetwork.Services.Data;
using FoodDeliveryNetwork.Services.Data.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FoodDeliveryNetwork.Services.Tests
{
    public class AddressServiceTests
    {
        private DbContextOptions<ApplicationDbContext> dbOptions;
        private Mock<ApplicationDbContext> dbContextMock;

        private Mock<UserManager<ApplicationUser>> userManagerMock;

        private Mock<RoleManager<IdentityRole<Guid>>> roleManagerMock;

        private IAddressService addressService;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "AddressServiceTests" + Guid.NewGuid().ToString())
                .Options;

            dbContextMock = new Mock<ApplicationDbContext>(dbOptions);

            userManagerMock = new Mock<UserManager<ApplicationUser>>(
                Mock.Of<IUserStore<ApplicationUser>>(), null, null, null, null, null, null, null, null);

            roleManagerMock = new Mock<RoleManager<IdentityRole<Guid>>>(
                Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);

            addressService = new AddressService(dbContextMock.Object, userManagerMock.Object);
        }

        [SetUp]
        public void CreateDbContext()
        {
            List<CustomerAddress> addresses = new List<CustomerAddress>()
            {
                new CustomerAddress { CustomerId = Guid.Parse("b48b9cf9-b8fe-46b6-9206-545d14525fc6"), Address = "123 Main St" },
                new CustomerAddress { CustomerId = Guid.Parse("b48b9cf9-b8fe-46b6-9206-545d14525fc6"), Address = "456 Main St" },
                new CustomerAddress { CustomerId = Guid.Parse("b48b9cf9-b8fe-46b6-9206-545d14525fc6"), Address = "789 Main St" },
            };

            var customerAddressesQueryable = addresses.AsQueryable();
            var customerAddressesMock = new Mock<DbSet<CustomerAddress>>();

            customerAddressesMock.As<IQueryable<CustomerAddress>>().Setup(m => m.Provider)
                .Returns(customerAddressesQueryable.Provider);
            customerAddressesMock.As<IQueryable<CustomerAddress>>().Setup(m => m.Expression)
                .Returns(customerAddressesQueryable.Expression);
            customerAddressesMock.As<IQueryable<CustomerAddress>>().Setup(m => m.ElementType)
                .Returns(customerAddressesQueryable.ElementType);
            customerAddressesMock.As<IQueryable<CustomerAddress>>().Setup(m => m.GetEnumerator())
                .Returns(customerAddressesQueryable.GetEnumerator());

            dbContextMock.Setup(db => db.CustomerAddresses).Returns(customerAddressesMock.Object);
        }



        [Test]
        public async Task AddressExistsAsync_AddressExists_ReturnsTrue()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var address = "123 Main St";

            // Act
            var result = await addressService.AddressExistsAsync(userId, address);

            // Assert
            Assert.True(result);
        }
    }

    //public class AddressServiceTests
    //{
    //    private DbContextOptions<ApplicationDbContext> dbOptions;
    //    private ApplicationDbContext dbContext;

    //    private UserManager<ApplicationUser> userManager;
    //    private RoleManager<IdentityRole<Guid>> roleManager;

    //    private IAddressService addressService;

    //    [OneTimeSetUp]
    //    public async Task OneTimeSetUp()
    //    {
    //        dbOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
    //            .UseInMemoryDatabase("FoodDeliveryNetworkTestInMemory" + Guid.NewGuid().ToString())
    //            .Options;
    //        dbContext = new ApplicationDbContext(this.dbOptions);

    //        dbContext.Database.EnsureCreated();

    //        roleManager = new RoleManager<IdentityRole<Guid>>(
    //            new RoleStore<IdentityRole<Guid>,
    //            ApplicationDbContext, Guid>(dbContext),
    //            null,
    //            null,
    //            null,
    //            null);

    //        userManager = new UserManager<ApplicationUser>(
    //            new UserStore<ApplicationUser, IdentityRole<Guid>, ApplicationDbContext, Guid>(dbContext),
    //            null,
    //            new PasswordHasher<ApplicationUser>(),
    //            null,
    //            null,
    //            null,
    //            null,
    //            null,
    //            null);

    //        roleManager.AddRoles();
    //        await Users.AddUsers(userManager, roleManager);

    //        addressService = new AddressService(dbContext, userManager);

    //    }

    //    [SetUp]
    //    public void Setup()
    //    {
    //    }

    //    [Test]
    //    public void Test1()
    //    {
    //        Assert.Pass();
    //    }
    //}
}