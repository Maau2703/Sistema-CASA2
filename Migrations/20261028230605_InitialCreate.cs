using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Proyecto.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Distritos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("LibSql:Autoincrement", true),
                    nombre = table.Column<string>(type: "TEXT", nullable: true),
                    ColorCinta = table.Column<string>(type: "TEXT", nullable: true),
                    Poligono = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Distritos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("LibSql:Autoincrement", true),
                    nombre = table.Column<string>(type: "TEXT", nullable: true),
                    Created_at = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoEstablecimientos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("LibSql:Autoincrement", true),
                    nombre = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoEstablecimientos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TipoReportes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("LibSql:Autoincrement", true),
                    nombre = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoReportes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Mascotas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("LibSql:Autoincrement", true),
                    Nombre = table.Column<string>(type: "TEXT", nullable: false),
                    Especie = table.Column<string>(type: "TEXT", nullable: false),
                    Edad = table.Column<string>(type: "TEXT", nullable: false),
                    Tamaño = table.Column<string>(type: "TEXT", nullable: false),
                    Color = table.Column<string>(type: "TEXT", nullable: false),
                    Caracteristica = table.Column<string>(type: "TEXT", nullable: true),
                    Foto = table.Column<string>(type: "TEXT", nullable: true),
                    Estado = table.Column<string>(type: "TEXT", nullable: true),
                    DistritoId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mascotas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Mascotas_Distritos_DistritoId",
                        column: x => x.DistritoId,
                        principalTable: "Distritos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("LibSql:Autoincrement", true),
                    nombre = table.Column<string>(type: "TEXT", nullable: true),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    celular = table.Column<string>(type: "TEXT", nullable: true),
                    ci = table.Column<string>(type: "TEXT", nullable: true),
                    Password = table.Column<string>(type: "TEXT", nullable: true),
                    fotoPerfil = table.Column<string>(type: "TEXT", nullable: true),
                    RolesId = table.Column<int>(type: "INTEGER", nullable: false),
                    DistritoId = table.Column<int>(type: "INTEGER", nullable: false),
                    Created_at = table.Column<DateOnly>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Usuarios_Distritos_DistritoId",
                        column: x => x.DistritoId,
                        principalTable: "Distritos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Establecimientos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("LibSql:Autoincrement", true),
                    nombre = table.Column<string>(type: "TEXT", nullable: true),
                    direccion = table.Column<string>(type: "TEXT", nullable: true),
                    telefono = table.Column<string>(type: "TEXT", nullable: true),
                    ubicacion = table.Column<string>(type: "TEXT", nullable: true),
                    horario = table.Column<string>(type: "TEXT", nullable: true),
                    capacidad = table.Column<int>(type: "INTEGER", nullable: false),
                    calificacion = table.Column<decimal>(type: "TEXT", nullable: false),
                    estado = table.Column<string>(type: "TEXT", nullable: true),
                    TipoEstablecimientoId = table.Column<int>(type: "INTEGER", nullable: false),
                    DistritoId = table.Column<int>(type: "INTEGER", nullable: false),
                    DistritosId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Establecimientos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Establecimientos_Distritos_DistritosId",
                        column: x => x.DistritosId,
                        principalTable: "Distritos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Establecimientos_TipoEstablecimientos_TipoEstablecimientoId",
                        column: x => x.TipoEstablecimientoId,
                        principalTable: "TipoEstablecimientos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ciudadanos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("LibSql:Autoincrement", true),
                    barrio = table.Column<string>(type: "TEXT", nullable: true),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ciudadanos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ciudadanos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Publicaciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("LibSql:Autoincrement", true),
                    titulo = table.Column<string>(type: "TEXT", nullable: true),
                    descripcion = table.Column<string>(type: "TEXT", nullable: true),
                    fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    MascotaId = table.Column<int>(type: "INTEGER", nullable: false),
                    MascotasId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Publicaciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Publicaciones_Mascotas_MascotasId",
                        column: x => x.MascotasId,
                        principalTable: "Mascotas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Publicaciones_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Rescatistas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("LibSql:Autoincrement", true),
                    UsuariosId = table.Column<int>(type: "INTEGER", nullable: false),
                    disponible = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rescatistas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rescatistas_Usuarios_UsuariosId",
                        column: x => x.UsuariosId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tramites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("LibSql:Autoincrement", true),
                    nombre = table.Column<string>(type: "TEXT", nullable: true),
                    descripcion = table.Column<string>(type: "TEXT", nullable: true),
                    requisitos = table.Column<string>(type: "TEXT", nullable: true),
                    enlace = table.Column<string>(type: "TEXT", nullable: true),
                    EstableciminetosId = table.Column<int>(type: "INTEGER", nullable: false),
                    EstablecimientosId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tramites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tramites_Establecimientos_EstablecimientosId",
                        column: x => x.EstablecimientosId,
                        principalTable: "Establecimientos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SolicitudesAdopcion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("LibSql:Autoincrement", true),
                    fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Estado = table.Column<string>(type: "TEXT", nullable: true),
                    mascotaId = table.Column<int>(type: "INTEGER", nullable: false),
                    CiudadanoId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitudesAdopcion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SolicitudesAdopcion_Ciudadanos_CiudadanoId",
                        column: x => x.CiudadanoId,
                        principalTable: "Ciudadanos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SolicitudesAdopcion_Mascotas_mascotaId",
                        column: x => x.mascotaId,
                        principalTable: "Mascotas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comentarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("LibSql:Autoincrement", true),
                    texto = table.Column<string>(type: "TEXT", nullable: true),
                    PublicacionId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsuarioId = table.Column<int>(type: "INTEGER", nullable: false),
                    usuariosId = table.Column<int>(type: "INTEGER", nullable: true),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comentarios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comentarios_Publicaciones_PublicacionId",
                        column: x => x.PublicacionId,
                        principalTable: "Publicaciones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Comentarios_Usuarios_usuariosId",
                        column: x => x.usuariosId,
                        principalTable: "Usuarios",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Reportes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("LibSql:Autoincrement", true),
                    Fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    estado = table.Column<string>(type: "TEXT", nullable: true),
                    Ubicaion = table.Column<string>(type: "TEXT", nullable: true),
                    Descripcion = table.Column<string>(type: "TEXT", nullable: true),
                    ImagenUrl = table.Column<string>(type: "TEXT", nullable: true),
                    ComentarioResolucion = table.Column<string>(type: "TEXT", nullable: true),
                    tipoReportesId = table.Column<int>(type: "INTEGER", nullable: true),
                    CiudadanoId = table.Column<int>(type: "INTEGER", nullable: true),
                    RescatistasId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reportes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reportes_Ciudadanos_CiudadanoId",
                        column: x => x.CiudadanoId,
                        principalTable: "Ciudadanos",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Reportes_Rescatistas_RescatistasId",
                        column: x => x.RescatistasId,
                        principalTable: "Rescatistas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reportes_TipoReportes_tipoReportesId",
                        column: x => x.tipoReportesId,
                        principalTable: "TipoReportes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Adopciones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("LibSql:Autoincrement", true),
                    numeroSerie = table.Column<string>(type: "TEXT", nullable: true),
                    firma = table.Column<string>(type: "TEXT", nullable: true),
                    fecha = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SolicitudId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Adopciones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Adopciones_SolicitudesAdopcion_SolicitudId",
                        column: x => x.SolicitudId,
                        principalTable: "SolicitudesAdopcion",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DenunciaAbandonos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("LibSql:Autoincrement", true),
                    denunciado = table.Column<string>(type: "TEXT", nullable: true),
                    evidencia = table.Column<string>(type: "TEXT", nullable: true),
                    ReporteId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReportesId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DenunciaAbandonos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DenunciaAbandonos_Reportes_ReportesId",
                        column: x => x.ReportesId,
                        principalTable: "Reportes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Adopciones_SolicitudId",
                table: "Adopciones",
                column: "SolicitudId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ciudadanos_UsuarioId",
                table: "Ciudadanos",
                column: "UsuarioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_PublicacionId",
                table: "Comentarios",
                column: "PublicacionId");

            migrationBuilder.CreateIndex(
                name: "IX_Comentarios_usuariosId",
                table: "Comentarios",
                column: "usuariosId");

            migrationBuilder.CreateIndex(
                name: "IX_DenunciaAbandonos_ReportesId",
                table: "DenunciaAbandonos",
                column: "ReportesId");

            migrationBuilder.CreateIndex(
                name: "IX_Establecimientos_DistritosId",
                table: "Establecimientos",
                column: "DistritosId");

            migrationBuilder.CreateIndex(
                name: "IX_Establecimientos_TipoEstablecimientoId",
                table: "Establecimientos",
                column: "TipoEstablecimientoId");

            migrationBuilder.CreateIndex(
                name: "IX_Mascotas_DistritoId",
                table: "Mascotas",
                column: "DistritoId");

            migrationBuilder.CreateIndex(
                name: "IX_Publicaciones_MascotasId",
                table: "Publicaciones",
                column: "MascotasId");

            migrationBuilder.CreateIndex(
                name: "IX_Publicaciones_UsuarioId",
                table: "Publicaciones",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Reportes_CiudadanoId",
                table: "Reportes",
                column: "CiudadanoId");

            migrationBuilder.CreateIndex(
                name: "IX_Reportes_RescatistasId",
                table: "Reportes",
                column: "RescatistasId");

            migrationBuilder.CreateIndex(
                name: "IX_Reportes_tipoReportesId",
                table: "Reportes",
                column: "tipoReportesId");

            migrationBuilder.CreateIndex(
                name: "IX_Rescatistas_UsuariosId",
                table: "Rescatistas",
                column: "UsuariosId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesAdopcion_CiudadanoId",
                table: "SolicitudesAdopcion",
                column: "CiudadanoId");

            migrationBuilder.CreateIndex(
                name: "IX_SolicitudesAdopcion_mascotaId",
                table: "SolicitudesAdopcion",
                column: "mascotaId");

            migrationBuilder.CreateIndex(
                name: "IX_Tramites_EstablecimientosId",
                table: "Tramites",
                column: "EstablecimientosId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_DistritoId",
                table: "Usuarios",
                column: "DistritoId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_RolesId",
                table: "Usuarios",
                column: "RolesId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Adopciones");

            migrationBuilder.DropTable(
                name: "Comentarios");

            migrationBuilder.DropTable(
                name: "DenunciaAbandonos");

            migrationBuilder.DropTable(
                name: "Tramites");

            migrationBuilder.DropTable(
                name: "SolicitudesAdopcion");

            migrationBuilder.DropTable(
                name: "Publicaciones");

            migrationBuilder.DropTable(
                name: "Reportes");

            migrationBuilder.DropTable(
                name: "Establecimientos");

            migrationBuilder.DropTable(
                name: "Mascotas");

            migrationBuilder.DropTable(
                name: "Ciudadanos");

            migrationBuilder.DropTable(
                name: "Rescatistas");

            migrationBuilder.DropTable(
                name: "TipoReportes");

            migrationBuilder.DropTable(
                name: "TipoEstablecimientos");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Distritos");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
