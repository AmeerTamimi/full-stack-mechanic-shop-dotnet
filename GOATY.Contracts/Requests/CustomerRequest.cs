namespace GOATY.Contracts.Requests
{
    public sealed record CustomerRequest(
        string FirstName,
        string LastName,
        string Phone,
        string Email,
        string Address,
        List<VehicleRequest> Vehicles
    );

    public sealed record VehicleRequest(
        Guid Id,
        string Brand,
        string Model,
        int Year,
        string LicensePlate
    );
}