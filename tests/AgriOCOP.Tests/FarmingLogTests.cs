namespace AgriOCOP.Tests;
using Xunit;
using AgriOCOP.Domain.Entities;
using AgriOCOP.Domain.Exceptions;
using AgriOCOP.Domain.Enums;

public class FarmingLogTests
{
    [Fact]
    public void Update_WhenLogIsLocked_ThrowFarmingLogLockedException()
    {
        //1. Arrandge: tạo nhật ký rồi niêm phong bằng kỹ sư kiểm định
        var cropBatchId = Guid.NewGuid();
        var logDate = new DateTime(2026, 11, 23);
        var log = FarmingLog.Create(
            cropBatchId,
            logDate,
            FarmingActivityType.PesticideSpraying,
            "Phun thuốc trừ sâu rầy sinh học liều lượng 20ml / bình" //PesticideSpraying phun thuốc bảo vệ thực vật
        );
        log.Lock("Kỹ sư Nguyễn Văn A");

        //2 & 3. Act & Asert: cố tình thao tác chỉnh sửa khi status đang niêm phong -> Kì vọng: chặn lại (QT02)
        var ex = Assert.Throws<FarmingLogLockedException>(() =>
        {
            log.Update(DateTime.UtcNow, FarmingActivityType.Fertilizing, "Cố tình chỉnh sửa nội dung");
        });

        //Kiểm tra thông điệp lỗi có nhắc đến trạng thái niêm phong
        Assert.Contains("niêm phong", ex.Message);
    }

    [Fact]
    public void Update_WhenLogIsNotLocked_UpdatesPropertiesSuccessfully()
    {
        // 1. Arrandge: Chuẩn bị nhật ký ban đầu (chưa bị khóa)
        var cropBatchId = Guid.NewGuid();
        var initiaDate = new DateTime(2026, 11, 19); //Ngày ban đầu tạo nhật ký
        var log = FarmingLog.Create(
            cropBatchId,
            new DateTime(2026, 11, 19),
            FarmingActivityType.Seeding,
            "Gieo giống ban đầu"
        );

        //Nông dân muốn cập nhật dữ liệu mới
        var newDate = new DateTime(2026, 11, 20);
        var newActivity = FarmingActivityType.Watering;
        var newDescription = "Tưới nước ấm cho hạt giống nảy nầm";

        //2. Act: Thực hiện hành vi cập nhật
        log.Update(newDate, newActivity, newDescription);

        //3. Assert: Khẳng định tất cả dữ liệu mới đã được cập nhật thành công
        Assert.Equal(newDate, log.LogDate);
        Assert.Equal(newActivity, log.ActivityType);
        Assert.Equal(newDescription, log.Description);

        //Kiểm tra thông điệp khẳng định mốc thời gian UpdateAt đã được kích hoạt (không còn là null)
        Assert.NotNull(log.UpdatedAt);
    }

    [Fact]
    public void Lock_ValidInspector_SetsLockedStatusAndCannotLockAgain()
    {
        //1. Arrange: Chuẩn bị nhật kí mới (IsLocked mặc định = false)
        var cropBatchId = Guid.NewGuid();
        var log = FarmingLog.Create(cropBatchId, DateTime.UtcNow, FarmingActivityType.Weeding, "Làm sạch cỏ dại quanh gốc");

        //2. Act: Kỹ sư kiểm tra thao tác niêm phong
        var inspector = "Kỹ sư kiểm tra Nguyễn Văn B"; // inspector: kiểm duyệt
        log.Lock(inspector);

        //3. Assert: Khẳng định trạng thái niêm phong là hợp lệ
        Assert.True(log.IsLocked);
        Assert.Equal(inspector, log.LockedBy);
        Assert.NotNull(log.LockedAt);

        //Đã khóa -> Chặn thao tác khóa đè thêm 1 lần nữa.
        Assert.Throws<InvalidOperationException>(() =>
        {
            log.Lock("Kỹ sư khác");
        });
    }
}