La base se crea y actualiza con las migraciones de Entity Framework Core en `src/OVI.Datos/Migraciones` (`Inicial`, `AmpliarPerfilDominante`, `AgregarAreasCarrera`, `EliminarRolOrientador`); son la fuente de verdad del esquema y de los datos semilla.

`01_crear_base.sql` es auxiliar: solo crea la base vacía `OrientacionVocacionalDB` si no existe. No ejecutar scripts de creación de tablas sobre una base ya migrada. Ver `docs/MANUAL_INSTALACION.md`.
