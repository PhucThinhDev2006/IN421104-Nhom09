namespace AgriOCOP.Domain.Enums;
/// <summary>
/// Quy trình canh tác VietGAP
/// </summary>
public enum FarmingActivityType
{
    /// <summary>Gieo giống</summary>
    Seeding = 1,

    /// <summary>Bón phân</summary>
    Fertilizing = 2,

    /// <summary>Tưới cây</summary>
    Watering = 3,

    /// <summary>Phun thuốc BVTV</summary>
    PesticideSpraying = 4,

    /// <summary>Làm cỏ</summary>
    Weeding = 5,

    /// <summary>Tỉa cành</summary>
    Pruning = 6,
}