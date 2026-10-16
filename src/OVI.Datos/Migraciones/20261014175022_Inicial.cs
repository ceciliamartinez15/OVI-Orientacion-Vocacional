using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OVI.Datos.Migraciones
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Carreras",
                columns: table => new
                {
                    IdCarrera = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Tipo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Institucion = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Carreras", x => x.IdCarrera);
                });

            migrationBuilder.CreateTable(
                name: "FAQChatbot",
                columns: table => new
                {
                    IdFAQ = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Pregunta = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Respuesta = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PalabrasClave = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FAQChatbot", x => x.IdFAQ);
                });

            migrationBuilder.CreateTable(
                name: "Instituciones",
                columns: table => new
                {
                    IdInstitucion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Telefono = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Instituciones", x => x.IdInstitucion);
                });

            migrationBuilder.CreateTable(
                name: "PreguntasRIASEC",
                columns: table => new
                {
                    IdPregunta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Enunciado = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CategoriaRIASEC = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Valor = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreguntasRIASEC", x => x.IdPregunta);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    IdRol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.IdRol);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRol = table.Column<int>(type: "int", nullable: false),
                    IdInstitucion = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Estado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK_Usuarios_Instituciones_IdInstitucion",
                        column: x => x.IdInstitucion,
                        principalTable: "Instituciones",
                        principalColumn: "IdInstitucion",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_IdRol",
                        column: x => x.IdRol,
                        principalTable: "Roles",
                        principalColumn: "IdRol",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Evaluaciones",
                columns: table => new
                {
                    IdEvaluacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluaciones", x => x.IdEvaluacion);
                    table.ForeignKey(
                        name: "FK_Evaluaciones_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Respuestas",
                columns: table => new
                {
                    IdRespuesta = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEvaluacion = table.Column<int>(type: "int", nullable: false),
                    IdPregunta = table.Column<int>(type: "int", nullable: false),
                    OpcionSeleccionada = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Valor = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Respuestas", x => x.IdRespuesta);
                    table.ForeignKey(
                        name: "FK_Respuestas_Evaluaciones_IdEvaluacion",
                        column: x => x.IdEvaluacion,
                        principalTable: "Evaluaciones",
                        principalColumn: "IdEvaluacion",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Respuestas_PreguntasRIASEC_IdPregunta",
                        column: x => x.IdPregunta,
                        principalTable: "PreguntasRIASEC",
                        principalColumn: "IdPregunta",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Resultados",
                columns: table => new
                {
                    IdResultado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEvaluacion = table.Column<int>(type: "int", nullable: false),
                    PuntajeR = table.Column<int>(type: "int", nullable: false),
                    PuntajeI = table.Column<int>(type: "int", nullable: false),
                    PuntajeA = table.Column<int>(type: "int", nullable: false),
                    PuntajeS = table.Column<int>(type: "int", nullable: false),
                    PuntajeE = table.Column<int>(type: "int", nullable: false),
                    PuntajeC = table.Column<int>(type: "int", nullable: false),
                    PerfilDominante = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    FechaResultado = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resultados", x => x.IdResultado);
                    table.ForeignKey(
                        name: "FK_Resultados_Evaluaciones_IdEvaluacion",
                        column: x => x.IdEvaluacion,
                        principalTable: "Evaluaciones",
                        principalColumn: "IdEvaluacion",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recomendaciones",
                columns: table => new
                {
                    IdRecomendacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdResultado = table.Column<int>(type: "int", nullable: false),
                    IdCarrera = table.Column<int>(type: "int", nullable: false),
                    NivelAfinidad = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    Posicion = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recomendaciones", x => x.IdRecomendacion);
                    table.ForeignKey(
                        name: "FK_Recomendaciones_Carreras_IdCarrera",
                        column: x => x.IdCarrera,
                        principalTable: "Carreras",
                        principalColumn: "IdCarrera",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Recomendaciones_Resultados_IdResultado",
                        column: x => x.IdResultado,
                        principalTable: "Resultados",
                        principalColumn: "IdResultado",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Carreras",
                columns: new[] { "IdCarrera", "Descripcion", "Estado", "Institucion", "Nombre", "Tipo" },
                values: new object[,]
                {
                    { 1, "Diseño y desarrollo de soluciones tecnológicas.", true, null, "Ingeniería en Sistemas", "Universitaria" },
                    { 2, "Optimización de procesos y recursos.", true, null, "Ingeniería Industrial", "Universitaria" },
                    { 3, "Prevención, diagnóstico y atención de la salud.", true, null, "Medicina", "Universitaria" },
                    { 4, "Estudio de la conducta y acompañamiento humano.", true, null, "Psicología", "Universitaria" },
                    { 5, "Comunicación visual y creación digital.", true, null, "Diseño Gráfico", "Universitaria" },
                    { 6, "Gestión de organizaciones y emprendimientos.", true, null, "Administración de Empresas", "Universitaria" },
                    { 7, "Instalación y mantenimiento de sistemas electrónicos.", true, null, "Técnico en Electrónica", "Técnica" },
                    { 8, "Registro y análisis de operaciones financieras.", true, null, "Técnico en Contabilidad", "Técnica" }
                });

            migrationBuilder.InsertData(
                table: "FAQChatbot",
                columns: new[] { "IdFAQ", "Categoria", "Estado", "PalabrasClave", "Pregunta", "Respuesta" },
                values: new object[,]
                {
                    { 1, "Test", true, "riasec,test,perfil", "¿Qué es el test RIASEC?", "Es una herramienta de exploración vocacional que organiza intereses en seis áreas. No determina por sí sola qué carrera debes estudiar." },
                    { 2, "Carreras", true, "elegir,carrera,estudiar", "¿Cómo elijo una carrera?", "Compara tus intereses, habilidades, plan de estudios, costos, requisitos y oportunidades. Conversa también con un orientador." },
                    { 3, "Becas", true, "beca,ayuda,financiamiento", "¿Dónde consulto becas?", "Consulta los sitios oficiales de las universidades y las convocatorias vigentes de instituciones educativas en El Salvador." }
                });

            migrationBuilder.InsertData(
                table: "Instituciones",
                columns: new[] { "IdInstitucion", "Direccion", "Estado", "Nombre", "Telefono" },
                values: new object[] { 1, null, true, "Institución no especificada", null });

            migrationBuilder.InsertData(
                table: "PreguntasRIASEC",
                columns: new[] { "IdPregunta", "CategoriaRIASEC", "Enunciado", "Estado", "Valor" },
                values: new object[,]
                {
                    { 1, "Realista", "Me gusta reparar objetos, herramientas o equipos.", true, 1 },
                    { 2, "Realista", "Disfruto trabajar con herramientas o construir cosas.", true, 1 },
                    { 3, "Investigador", "Me interesa investigar cómo funcionan las cosas.", true, 1 },
                    { 4, "Investigador", "Disfruto resolver problemas científicos o matemáticos.", true, 1 },
                    { 5, "Artistico", "Me gusta dibujar, diseñar o crear contenido original.", true, 1 },
                    { 6, "Artistico", "Prefiero actividades que permitan expresar ideas creativas.", true, 1 },
                    { 7, "Social", "Me gusta enseñar, escuchar y ayudar a otras personas.", true, 1 },
                    { 8, "Social", "Disfruto colaborar para resolver necesidades de una comunidad.", true, 1 },
                    { 9, "Emprendedor", "Me interesa liderar equipos o emprender proyectos.", true, 1 },
                    { 10, "Emprendedor", "Me gusta persuadir, negociar o presentar ideas.", true, 1 },
                    { 11, "Convencional", "Prefiero organizar datos, documentos y procedimientos.", true, 1 },
                    { 12, "Convencional", "Me gusta trabajar con números, registros y planificación.", true, 1 }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "IdRol", "Descripcion", "Nombre" },
                values: new object[,]
                {
                    { 1, "Realiza el test vocacional", "Estudiante" },
                    { 2, "Consulta estadísticas", "Orientador" },
                    { 3, "Administra catálogos", "Admin" }
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "IdUsuario", "Apellido", "Correo", "Estado", "IdInstitucion", "IdRol", "Nombre", "PasswordHash" },
                values: new object[] { 1, "Demo", "demo@ovi.local", true, 1, 1, "Estudiante", null });

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_IdUsuario",
                table: "Evaluaciones",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_Recomendaciones_IdCarrera",
                table: "Recomendaciones",
                column: "IdCarrera");

            migrationBuilder.CreateIndex(
                name: "IX_Recomendaciones_IdResultado",
                table: "Recomendaciones",
                column: "IdResultado");

            migrationBuilder.CreateIndex(
                name: "IX_Respuestas_IdEvaluacion",
                table: "Respuestas",
                column: "IdEvaluacion");

            migrationBuilder.CreateIndex(
                name: "IX_Respuestas_IdPregunta",
                table: "Respuestas",
                column: "IdPregunta");

            migrationBuilder.CreateIndex(
                name: "IX_Resultados_IdEvaluacion",
                table: "Resultados",
                column: "IdEvaluacion",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Nombre",
                table: "Roles",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Correo",
                table: "Usuarios",
                column: "Correo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdInstitucion",
                table: "Usuarios",
                column: "IdInstitucion");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdRol",
                table: "Usuarios",
                column: "IdRol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FAQChatbot");

            migrationBuilder.DropTable(
                name: "Recomendaciones");

            migrationBuilder.DropTable(
                name: "Respuestas");

            migrationBuilder.DropTable(
                name: "Carreras");

            migrationBuilder.DropTable(
                name: "Resultados");

            migrationBuilder.DropTable(
                name: "PreguntasRIASEC");

            migrationBuilder.DropTable(
                name: "Evaluaciones");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "Instituciones");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
