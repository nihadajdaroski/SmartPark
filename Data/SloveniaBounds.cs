namespace SmartPark.Data;

public static class SloveniaBounds
{
    public static bool Contains(double latitude, double longitude) =>
        latitude >= 45.4 && latitude <= 47.1 &&
        longitude >= 13.3 && longitude <= 16.7;
}
