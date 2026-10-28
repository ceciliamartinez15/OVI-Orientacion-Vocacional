using Microsoft.EntityFrameworkCore;
using OVI.Datos;
using OVI.Modelos.Entidades;
namespace OVI.Logica.Servicios;
public record RespuestaTest(int IdPregunta, int Valor);
public record ResultadoVocacional(Resultado Resultado, List<(Carrera Carrera, decimal Afinidad)> Carreras);
public class ServicioVocacional(OVIDbContext db)
{
 private static readonly string[] Categorias=["Realista","Investigador","Artistico","Social","Emprendedor","Convencional"];
 public async Task<ResultadoVocacional> CalcularAsync(int idUsuario, List<RespuestaTest> respuestas)
 {
  var preguntas=await db.PreguntasRIASEC.Where(p=>p.Estado).ToDictionaryAsync(p=>p.IdPregunta);
  if(respuestas.Count!=preguntas.Count || respuestas.Any(r=>!preguntas.ContainsKey(r.IdPregunta) || r.Valor<1 || r.Valor>5) || respuestas.Select(r=>r.IdPregunta).Distinct().Count()!=respuestas.Count) throw new InvalidOperationException("Las respuestas del test no son válidas.");
  var usuarioExiste=await db.Usuarios.AnyAsync(u=>u.IdUsuario==idUsuario && u.Estado); if(!usuarioExiste) throw new InvalidOperationException("No se encontró el estudiante.");
  var evaluacion=new Evaluacion{IdUsuario=idUsuario,FechaInicio=DateTime.UtcNow,FechaFin=DateTime.UtcNow,Estado="Finalizada"}; db.Evaluaciones.Add(evaluacion); await db.SaveChangesAsync();
  foreach(var r in respuestas) db.Respuestas.Add(new Respuesta{IdEvaluacion=evaluacion.IdEvaluacion,IdPregunta=r.IdPregunta,Valor=r.Valor,OpcionSeleccionada=r.Valor switch{1=>"Nada",2=>"Poco",3=>"Algo",4=>"Mucho",_=>"Muchísimo"}});
  var scores=Categorias.ToDictionary(c=>c,c=>respuestas.Where(r=>preguntas[r.IdPregunta].CategoriaRIASEC==c).Sum(r=>r.Valor));
  var max=scores.Values.DefaultIfEmpty(0).Max(); var dominant=string.Join("-",Categorias.Where(c=>scores[c]==max));
  var resultado=new Resultado{IdEvaluacion=evaluacion.IdEvaluacion,PuntajeR=scores["Realista"],PuntajeI=scores["Investigador"],PuntajeA=scores["Artistico"],PuntajeS=scores["Social"],PuntajeE=scores["Emprendedor"],PuntajeC=scores["Convencional"],PerfilDominante=dominant,FechaResultado=DateTime.UtcNow}; db.Resultados.Add(resultado); await db.SaveChangesAsync();
  var carreras=await db.Carreras.Where(c=>c.Estado).ToListAsync(); var recomendaciones=carreras.Select(c=>(Carrera:c, Afinidad: Afinidad(c,scores))).OrderByDescending(x=>x.Afinidad).Take(3).ToList(); int pos=1;
  foreach(var rec in recomendaciones) db.Recomendaciones.Add(new Recomendacion{IdResultado=resultado.IdResultado,IdCarrera=rec.Carrera.IdCarrera,NivelAfinidad=rec.Afinidad,Posicion=pos++}); await db.SaveChangesAsync();
  return new ResultadoVocacional(resultado,recomendaciones);
 }
 private static decimal Afinidad(Carrera c, Dictionary<string,int> s){string[] keys=c.Nombre switch {"Ingeniería en Sistemas"=>["Investigador","Convencional","Realista"],"Ingeniería Industrial"=>["Investigador","Emprendedor","Convencional"],"Medicina"=>["Investigador","Social"],"Psicología"=>["Social","Investigador"],"Diseño Gráfico"=>["Artistico","Emprendedor"],"Administración de Empresas"=>["Emprendedor","Convencional","Social"],"Técnico en Electrónica"=>["Realista","Investigador"],"Técnico en Contabilidad"=>["Convencional","Investigador"],_=>["Social"]}; var sum=keys.Sum(k=>s[k]); var max=Math.Max(1,keys.Sum(k=>s.Values.DefaultIfEmpty(1).Max())); return Math.Round(Math.Min(100m,100m*sum/Math.Max(max,keys.Length*5)),2); }
}
