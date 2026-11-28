namespace AgriOCOP.Domain.Entities;
using AgriOCOP.Domain.Common;
using AgriOCOP.Domain.Enums;
using AgriOCOP.Domain.Exceptions;

/// <summary>
/// Entity Nhật ký canh tác đồng ruộng VietGAP
/// Quản lý các hoạt động canh tác và cơ chế 
/// niên phong chống gian lận
/// </summary>
public class FarmingLog : BaseEntity
{
    public Guid CropBatchId {get; private set;}
    public DateTime LogDate {get; private set;}
    public FarmingActivityType ActivityType {get; private set;}
    public string Description {get; private set;} = string.Empty;
    public bool IsLocked {get; private set;} = false; //Mặc định đã là false, chỉ note rõ lại
    public DateTime? LockedAt {get; private set;}
    public string? LockedBy {get; private set;}

    //Constructor bảo vệ tính đóng gói
    private FarmingLog() {}

    /// <summary>
    /// Khởi tạo một dòng nhật ký canh tác mới
    /// </summary>
    /// <param name="cropBatchId"></param>
    /// <param name="logDate"></param>
    /// <param name="activityType"></param>
    /// <param name="description"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>

    public static FarmingLog Create(Guid cropBatchId, DateTime logDate, FarmingActivityType activityType, string description)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        if (cropBatchId == Guid.Empty)
            {
                throw new ArgumentException("cropBatchId không được để trống", nameof(cropBatchId));
            }
        
        return new FarmingLog
        {
          CropBatchId = cropBatchId,
          LogDate = logDate,
          ActivityType = activityType,
          Description = description.Trim(),
          IsLocked = false,  
        };
    }

    /// <summary>
    /// Niên phong nhật ký canh tác
    /// </summary>
    /// <param name="inspectorName"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void Lock(string inspectorName)
    {
        // Ktra trạng thái - Đã niêm phong -> không cho đè thêm
        if (IsLocked)
        {
            throw new InvalidOperationException("Nhật ký canh tác đã được niêm phong từ trước.");
        }

        // Ktra dữ liệu không được null hoặc whitespace
        ArgumentException.ThrowIfNullOrWhiteSpace(inspectorName); 

        // Update status niên phong
        IsLocked = true;
        LockedAt = DateTime.UtcNow; // UTC, khớp BaseEntity
        LockedBy = inspectorName;
        MarkAsUpdated(); // thừa hưởng từ BaseEntity
    }

    /// <summary>
    /// Cập nhật thông tin nhật ký canh tác
    /// Ném ngoại lệ nếu nhật ký đã niêm phong
    /// </summary>
    /// <param name="logDate"></param>
    /// <param name="activityType"></param>
    /// <param name="description"></param>
    /// <exception cref="FarmingLogLockedException"></exception>

    public void Update(DateTime logDate, FarmingActivityType activityType, string description)
    {
        if (IsLocked)
        {
            throw new FarmingLogLockedException(Id);
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        LogDate = logDate;
        ActivityType = activityType;
        Description = description.Trim();

        MarkAsUpdated(); 
    }
}