using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FoodDeliveryNetwork.Data.Migrations
{
    /// <inheritdoc />
    public partial class OwnerAdditions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourierToRestaurant_AspNetUsers_CourierId",
                table: "CourierToRestaurant");

            migrationBuilder.DropForeignKey(
                name: "FK_CourierToRestaurant_Restaurant_RestaurantId",
                table: "CourierToRestaurant");

            migrationBuilder.DropForeignKey(
                name: "FK_Dish_Restaurant_RestaurantId",
                table: "Dish");

            migrationBuilder.DropForeignKey(
                name: "FK_DispatcherToRestaurant_AspNetUsers_DispatcherId",
                table: "DispatcherToRestaurant");

            migrationBuilder.DropForeignKey(
                name: "FK_DispatcherToRestaurant_Restaurant_RestaurantId",
                table: "DispatcherToRestaurant");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_AspNetUsers_CustomerId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Restaurant_RestaurantId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Restaurant_AspNetUsers_OwnerId",
                table: "Restaurant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Restaurant",
                table: "Restaurant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Order",
                table: "Order");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DispatcherToRestaurant",
                table: "DispatcherToRestaurant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Dish",
                table: "Dish");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourierToRestaurant",
                table: "CourierToRestaurant");

            migrationBuilder.RenameTable(
                name: "Restaurant",
                newName: "Restaurants");

            migrationBuilder.RenameTable(
                name: "Order",
                newName: "Orders");

            migrationBuilder.RenameTable(
                name: "DispatcherToRestaurant",
                newName: "DispatcherToRestaurants");

            migrationBuilder.RenameTable(
                name: "Dish",
                newName: "Dishes");

            migrationBuilder.RenameTable(
                name: "CourierToRestaurant",
                newName: "CourierToRestaurants");

            migrationBuilder.RenameIndex(
                name: "IX_Restaurant_OwnerId",
                table: "Restaurants",
                newName: "IX_Restaurants_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_RestaurantId",
                table: "Orders",
                newName: "IX_Orders_RestaurantId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_CustomerId",
                table: "Orders",
                newName: "IX_Orders_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_DispatcherToRestaurant_RestaurantId",
                table: "DispatcherToRestaurants",
                newName: "IX_DispatcherToRestaurants_RestaurantId");

            migrationBuilder.RenameIndex(
                name: "IX_Dish_RestaurantId",
                table: "Dishes",
                newName: "IX_Dishes_RestaurantId");

            migrationBuilder.RenameIndex(
                name: "IX_CourierToRestaurant_RestaurantId",
                table: "CourierToRestaurants",
                newName: "IX_CourierToRestaurants_RestaurantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Restaurants",
                table: "Restaurants",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Orders",
                table: "Orders",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DispatcherToRestaurants",
                table: "DispatcherToRestaurants",
                columns: new[] { "DispatcherId", "RestaurantId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dishes",
                table: "Dishes",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourierToRestaurants",
                table: "CourierToRestaurants",
                columns: new[] { "CourierId", "RestaurantId" });

            migrationBuilder.CreateTable(
                name: "CustomerAddresses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerAddresses_AspNetUsers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OwnerApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EIK = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OwnerFullName = table.Column<string>(type: "nvarchar(92)", maxLength: 92, nullable: false),
                    OwnerEGN = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    HeadquartersFullAddress = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ApplicationStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OwnerApplications_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RestaurantOwners",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EIK = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OwnerFullName = table.Column<string>(type: "nvarchar(92)", maxLength: 92, nullable: false),
                    OwnerEGN = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    HeadquartersFullAddress = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RestaurantOwners", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RestaurantOwners_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerAddresses_CustomerId",
                table: "CustomerAddresses",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerApplications_ApplicationUserId",
                table: "OwnerApplications",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RestaurantOwners_ApplicationUserId",
                table: "RestaurantOwners",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CourierToRestaurants_AspNetUsers_CourierId",
                table: "CourierToRestaurants",
                column: "CourierId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourierToRestaurants_Restaurants_RestaurantId",
                table: "CourierToRestaurants",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_Restaurants_RestaurantId",
                table: "Dishes",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DispatcherToRestaurants_AspNetUsers_DispatcherId",
                table: "DispatcherToRestaurants",
                column: "DispatcherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DispatcherToRestaurants_Restaurants_RestaurantId",
                table: "DispatcherToRestaurants",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AspNetUsers_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Restaurants_RestaurantId",
                table: "Orders",
                column: "RestaurantId",
                principalTable: "Restaurants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_AspNetUsers_OwnerId",
                table: "Restaurants",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourierToRestaurants_AspNetUsers_CourierId",
                table: "CourierToRestaurants");

            migrationBuilder.DropForeignKey(
                name: "FK_CourierToRestaurants_Restaurants_RestaurantId",
                table: "CourierToRestaurants");

            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_Restaurants_RestaurantId",
                table: "Dishes");

            migrationBuilder.DropForeignKey(
                name: "FK_DispatcherToRestaurants_AspNetUsers_DispatcherId",
                table: "DispatcherToRestaurants");

            migrationBuilder.DropForeignKey(
                name: "FK_DispatcherToRestaurants_Restaurants_RestaurantId",
                table: "DispatcherToRestaurants");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AspNetUsers_CustomerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Restaurants_RestaurantId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Restaurants_AspNetUsers_OwnerId",
                table: "Restaurants");

            migrationBuilder.DropTable(
                name: "CustomerAddresses");

            migrationBuilder.DropTable(
                name: "OwnerApplications");

            migrationBuilder.DropTable(
                name: "RestaurantOwners");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Restaurants",
                table: "Restaurants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Orders",
                table: "Orders");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DispatcherToRestaurants",
                table: "DispatcherToRestaurants");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Dishes",
                table: "Dishes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourierToRestaurants",
                table: "CourierToRestaurants");

            migrationBuilder.RenameTable(
                name: "Restaurants",
                newName: "Restaurant");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "Order");

            migrationBuilder.RenameTable(
                name: "DispatcherToRestaurants",
                newName: "DispatcherToRestaurant");

            migrationBuilder.RenameTable(
                name: "Dishes",
                newName: "Dish");

            migrationBuilder.RenameTable(
                name: "CourierToRestaurants",
                newName: "CourierToRestaurant");

            migrationBuilder.RenameIndex(
                name: "IX_Restaurants_OwnerId",
                table: "Restaurant",
                newName: "IX_Restaurant_OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_RestaurantId",
                table: "Order",
                newName: "IX_Order_RestaurantId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CustomerId",
                table: "Order",
                newName: "IX_Order_CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_DispatcherToRestaurants_RestaurantId",
                table: "DispatcherToRestaurant",
                newName: "IX_DispatcherToRestaurant_RestaurantId");

            migrationBuilder.RenameIndex(
                name: "IX_Dishes_RestaurantId",
                table: "Dish",
                newName: "IX_Dish_RestaurantId");

            migrationBuilder.RenameIndex(
                name: "IX_CourierToRestaurants_RestaurantId",
                table: "CourierToRestaurant",
                newName: "IX_CourierToRestaurant_RestaurantId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Restaurant",
                table: "Restaurant",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Order",
                table: "Order",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DispatcherToRestaurant",
                table: "DispatcherToRestaurant",
                columns: new[] { "DispatcherId", "RestaurantId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Dish",
                table: "Dish",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourierToRestaurant",
                table: "CourierToRestaurant",
                columns: new[] { "CourierId", "RestaurantId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CourierToRestaurant_AspNetUsers_CourierId",
                table: "CourierToRestaurant",
                column: "CourierId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourierToRestaurant_Restaurant_RestaurantId",
                table: "CourierToRestaurant",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Dish_Restaurant_RestaurantId",
                table: "Dish",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DispatcherToRestaurant_AspNetUsers_DispatcherId",
                table: "DispatcherToRestaurant",
                column: "DispatcherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DispatcherToRestaurant_Restaurant_RestaurantId",
                table: "DispatcherToRestaurant",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_AspNetUsers_CustomerId",
                table: "Order",
                column: "CustomerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Restaurant_RestaurantId",
                table: "Order",
                column: "RestaurantId",
                principalTable: "Restaurant",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurant_AspNetUsers_OwnerId",
                table: "Restaurant",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
