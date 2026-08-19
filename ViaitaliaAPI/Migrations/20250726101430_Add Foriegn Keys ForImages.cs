using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace ViaitaliaAPI.Migrations
{
    public partial class AddForiegnKeysForImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Cities → Images(id)
            migrationBuilder.AddColumn<Guid>(
                name: "image_id",
                table: "Cities",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Cities_Images_image_id",
                table: "Cities",
                column: "image_id",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            // Hotels → Images(id)
            migrationBuilder.AddColumn<Guid>(
                name: "image_id",
                table: "Hotels",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Hotels_Images_image_id",
                table: "Hotels",
                column: "image_id",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            // Restaurants → Images(id)
            migrationBuilder.AddColumn<Guid>(
                name: "image_id",
                table: "Restaurants",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Restaurants_Images_image_id",
                table: "Restaurants",
                column: "image_id",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            // Beaches → Images(id)
            migrationBuilder.AddColumn<Guid>(
                name: "image_id",
                table: "Beaches",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Beaches_Images_image_id",
                table: "Beaches",
                column: "image_id",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            // AttractionPlaces → Images(id)
            migrationBuilder.AddColumn<Guid>(
                name: "image_id",
                table: "AttractionPlaces",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AttractionPlaces_Images_image_id",
                table: "AttractionPlaces",
                column: "image_id",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            // ShoppingMalls → Images(id)
            migrationBuilder.AddColumn<Guid>(
                name: "image_id",
                table: "ShoppingMalls",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingMalls_Images_image_id",
                table: "ShoppingMalls",
                column: "image_id",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
