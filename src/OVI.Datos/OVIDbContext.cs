using Microsoft.EntityFrameworkCore;
using OVI.Modelos.Entidades;
namespace OVI.Datos;
public class OVIDbContext(DbContextOptions<OVIDbContext> options) : DbContext(options)
{
 public DbSet<Rol> Roles => Set<Rol>(); public DbSet<Institucion> Instituciones => Set<Institucion>(); public DbSet<Usuario> Usuarios => Set<Usuario>(); public DbSet<Evaluacion> Evaluaciones => Set<Evaluacion>(); public DbSet<PreguntaRIASEC> PreguntasRIASEC => Set<PreguntaRIASEC>(); public DbSet<Respuesta> Respuestas => Set<Respuesta>(); public DbSet<Resultado> Resultados => Set<Resultado>(); public DbSet<Carrera> Carreras => Set<Carrera>(); public DbSet<Recomendacion> Recomendaciones => Set<Recomendacion>(); public DbSet<FAQChatbot> FAQChatbot => Set<FAQChatbot>();
    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);
        b.Entity<Resultado>().Property(x => x.PerfilDominante).HasMaxLength(255);

        // Llaves primarias de las tablas
        b.Entity<Resultado>().Property(x => x.PerfilDominante).HasMaxLength(255);
        b.Entity<Rol>().HasKey(x => x.IdRol);
        b.Entity<Institucion>().HasKey(x => x.IdInstitucion);
        b.Entity<Usuario>().HasKey(x => x.IdUsuario);
        b.Entity<Evaluacion>().HasKey(x => x.IdEvaluacion);
        b.Entity<PreguntaRIASEC>().HasKey(x => x.IdPregunta);
        b.Entity<Respuesta>().HasKey(x => x.IdRespuesta);
        b.Entity<Resultado>().HasKey(x => x.IdResultado);
        b.Entity<Carrera>().HasKey(x => x.IdCarrera);
        b.Entity<Recomendacion>().HasKey(x => x.IdRecomendacion);
        b.Entity<FAQChatbot>().HasKey(x => x.IdFAQ);
        b.Entity<Rol>().HasIndex(x=>x.Nombre).IsUnique(); b.Entity<Usuario>().HasIndex(x=>x.Correo).IsUnique();
  b.Entity<Usuario>().HasOne(x=>x.Rol).WithMany(x=>x.Usuarios).HasForeignKey(x=>x.IdRol).OnDelete(DeleteBehavior.Restrict);
  b.Entity<Usuario>().HasOne(x=>x.Institucion).WithMany(x=>x.Usuarios).HasForeignKey(x=>x.IdInstitucion).OnDelete(DeleteBehavior.Restrict);
  b.Entity<Evaluacion>().HasOne(x=>x.Usuario).WithMany(x=>x.Evaluaciones).HasForeignKey(x=>x.IdUsuario);
  b.Entity<Respuesta>().HasOne(x=>x.Evaluacion).WithMany(x=>x.Respuestas).HasForeignKey(x=>x.IdEvaluacion).OnDelete(DeleteBehavior.Cascade);
  b.Entity<Respuesta>().HasOne(x=>x.PreguntaRIASEC).WithMany(x=>x.Respuestas).HasForeignKey(x=>x.IdPregunta).OnDelete(DeleteBehavior.Restrict);
  b.Entity<Resultado>().HasIndex(x=>x.IdEvaluacion).IsUnique(); b.Entity<Resultado>().HasOne(x=>x.Evaluacion).WithOne(x=>x.Resultado).HasForeignKey<Resultado>(x=>x.IdEvaluacion);
  b.Entity<Recomendacion>().HasOne(x=>x.Resultado).WithMany(x=>x.Recomendaciones).HasForeignKey(x=>x.IdResultado).OnDelete(DeleteBehavior.Cascade);
  b.Entity<Recomendacion>().HasOne(x=>x.Carrera).WithMany(x=>x.Recomendaciones).HasForeignKey(x=>x.IdCarrera).OnDelete(DeleteBehavior.Restrict);
  b.Entity<Recomendacion>().Property(x=>x.NivelAfinidad).HasPrecision(5,2);
  b.Entity<Rol>().HasData(new Rol{IdRol=1,Nombre="Estudiante",Descripcion="Realiza el test vocacional"},new Rol{IdRol=2,Nombre="Orientador",Descripcion="Consulta estadísticas"},new Rol{IdRol=3,Nombre="Admin",Descripcion="Administra catálogos"});
  b.Entity<Institucion>().HasData(new Institucion{IdInstitucion=1,Nombre="Institución no especificada",Estado=true});
  b.Entity<Usuario>().HasData(new Usuario{IdUsuario=1,IdRol=1,IdInstitucion=1,Nombre="Estudiante",Apellido="Demo",Correo="demo@ovi.local",PasswordHash=null,Estado=true});
  b.Entity<PreguntaRIASEC>().HasData(
   new PreguntaRIASEC{IdPregunta=1,Enunciado="Me gusta reparar objetos, herramientas o equipos.",CategoriaRIASEC="Realista",Valor=1,Estado=true},new PreguntaRIASEC{IdPregunta=2,Enunciado="Disfruto trabajar con herramientas o construir cosas.",CategoriaRIASEC="Realista",Valor=1,Estado=true},
   new PreguntaRIASEC{IdPregunta=3,Enunciado="Me interesa investigar cómo funcionan las cosas.",CategoriaRIASEC="Investigador",Valor=1,Estado=true},new PreguntaRIASEC{IdPregunta=4,Enunciado="Disfruto resolver problemas científicos o matemáticos.",CategoriaRIASEC="Investigador",Valor=1,Estado=true},
   new PreguntaRIASEC{IdPregunta=5,Enunciado="Me gusta dibujar, diseñar o crear contenido original.",CategoriaRIASEC="Artistico",Valor=1,Estado=true},new PreguntaRIASEC{IdPregunta=6,Enunciado="Prefiero actividades que permitan expresar ideas creativas.",CategoriaRIASEC="Artistico",Valor=1,Estado=true},
   new PreguntaRIASEC{IdPregunta=7,Enunciado="Me gusta enseñar, escuchar y ayudar a otras personas.",CategoriaRIASEC="Social",Valor=1,Estado=true},new PreguntaRIASEC{IdPregunta=8,Enunciado="Disfruto colaborar para resolver necesidades de una comunidad.",CategoriaRIASEC="Social",Valor=1,Estado=true},
   new PreguntaRIASEC{IdPregunta=9,Enunciado="Me interesa liderar equipos o emprender proyectos.",CategoriaRIASEC="Emprendedor",Valor=1,Estado=true},new PreguntaRIASEC{IdPregunta=10,Enunciado="Me gusta persuadir, negociar o presentar ideas.",CategoriaRIASEC="Emprendedor",Valor=1,Estado=true},
   new PreguntaRIASEC{IdPregunta=11,Enunciado="Prefiero organizar datos, documentos y procedimientos.",CategoriaRIASEC="Convencional",Valor=1,Estado=true},new PreguntaRIASEC{IdPregunta=12,Enunciado="Me gusta trabajar con números, registros y planificación.",CategoriaRIASEC="Convencional",Valor=1,Estado=true});
  b.Entity<Carrera>().HasData(
   new Carrera{IdCarrera=1,Nombre="Ingeniería en Sistemas",Descripcion="Diseño y desarrollo de soluciones tecnológicas.",Tipo="Universitaria",Estado=true},new Carrera{IdCarrera=2,Nombre="Ingeniería Industrial",Descripcion="Optimización de procesos y recursos.",Tipo="Universitaria",Estado=true},new Carrera{IdCarrera=3,Nombre="Medicina",Descripcion="Prevención, diagnóstico y atención de la salud.",Tipo="Universitaria",Estado=true},new Carrera{IdCarrera=4,Nombre="Psicología",Descripcion="Estudio de la conducta y acompañamiento humano.",Tipo="Universitaria",Estado=true},new Carrera{IdCarrera=5,Nombre="Diseño Gráfico",Descripcion="Comunicación visual y creación digital.",Tipo="Universitaria",Estado=true},new Carrera{IdCarrera=6,Nombre="Administración de Empresas",Descripcion="Gestión de organizaciones y emprendimientos.",Tipo="Universitaria",Estado=true},new Carrera{IdCarrera=7,Nombre="Técnico en Electrónica",Descripcion="Instalación y mantenimiento de sistemas electrónicos.",Tipo="Técnica",Estado=true},new Carrera{IdCarrera=8,Nombre="Técnico en Contabilidad",Descripcion="Registro y análisis de operaciones financieras.",Tipo="Técnica",Estado=true});
  b.Entity<FAQChatbot>().HasData(new FAQChatbot{IdFAQ=1,Pregunta="¿Qué es el test RIASEC?",Respuesta="Es una herramienta de exploración vocacional que organiza intereses en seis áreas. No determina por sí sola qué carrera debes estudiar.",Categoria="Test",PalabrasClave="riasec,test,perfil",Estado=true},new FAQChatbot{IdFAQ=2,Pregunta="¿Cómo elijo una carrera?",Respuesta="Compara tus intereses, habilidades, plan de estudios, costos, requisitos y oportunidades. Conversa también con un orientador.",Categoria="Carreras",PalabrasClave="elegir,carrera,estudiar",Estado=true},new FAQChatbot{IdFAQ=3,Pregunta="¿Dónde consulto becas?",Respuesta="Consulta los sitios oficiales de las universidades y las convocatorias vigentes de instituciones educativas en El Salvador.",Categoria="Becas",PalabrasClave="beca,ayuda,financiamiento",Estado=true});
 }
}
