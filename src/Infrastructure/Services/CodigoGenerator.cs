namespace LostPeople.Infrastructure.Services;

public static class CodigoGenerator
{
    private static readonly Random _random = new();

    public static string GenerarCodigoSeguimiento()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var codigo = new char[5];
        for (int i = 0; i < 5; i++)
            codigo[i] = chars[_random.Next(chars.Length)];
        return $"LP-{new string(codigo)}";
    }

    public static string GenerarCodigoVerificacion()
    {
        return _random.Next(100000, 999999).ToString();
    }
}
