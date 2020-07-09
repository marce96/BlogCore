using Microsoft.EntityFrameworkCore.Migrations;

namespace BlogCore.AccesoDatos.Migrations
{
    public partial class spGetCategorias : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string procedimiento = @"CREATE PROCEDURE spGetCategorias 
                                    AS
                                    BEGIN

                                        SELECT* FROM Categoria
                                    END";
            migrationBuilder.Sql(procedimiento);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            string procedimiento = @"DROP PROCEDURE spGetCategorias";
            migrationBuilder.Sql(procedimiento);
        }
    }
}
