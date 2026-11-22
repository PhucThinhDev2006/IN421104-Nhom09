namespace AgriOCOP.Domain.Exceptions;
/// <summary>
/// Ngoại lệ ném ra khi cố ý chỉnh sửa / xóa nhật ký canh tác đã trong trạng thái niêm phong (QT nghiệp vụ QT02)
/// </summary>
public class FarmingLogLockedException : DomainException
{
    // Constructor 1: Cho phép truyền thông điệp lỗi tự do
    public FarmingLogLockedException(string message) : base(message) {}

    // Constructor 2: Thông báo vi phạm QT02
    public FarmingLogLockedException(Guid logId) :
        base($"Nhật ký canh tác [{logId}] đã niêm phong. Thao tác chỉnh sửa và xóa dữ liệu đã bị vô hiệu hóa (QT02.)") {}
}