namespace Proyecto.Helpers;

public static class PermissionHelper
{
    /// <summary>
    /// Verifica si un usuario tiene permiso para editar o eliminar un reporte.
    /// </summary>
    public static bool CanManageReport(string? userRole, int? usuarioCiudadanoId, int? reporteCiudadanoId)
    {
        // 1. Si es Admin o Rescatista, siempre tiene permiso
        if (userRole?.ToLower() == "administrador" || userRole?.ToLower() == "rescatista")
        {
            return true;
        }

        // 2. Si es ciudadano, solo tiene permiso si él creó el reporte
        if (usuarioCiudadanoId.HasValue && reporteCiudadanoId.HasValue)
        {
            return usuarioCiudadanoId.Value == reporteCiudadanoId.Value;
        }

        // 3. Si no cumple nada, no tiene permiso
        return false;
    }
}