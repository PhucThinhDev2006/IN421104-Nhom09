namespace AgriOCOP.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using AgriOCOP.Domain.Entities;

public class AgriOCOPDbContext : DbContext
{
    public AgriOCOPDbContext(DbContextOptions<AgriOCOPDbContext>options) : base(options) { }

    public DbSet<Cooperative> Cooperatives => Set<Cooperative>();
    public DbSet<Farm> Farms => Set<Farm>();
    public DbSet<CropBatch> CropBatches => Set<CropBatch>();
    public DbSet<FarmingLog> FarmingLogs => Set<FarmingLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        //Cấu hình bảng FarmingLog  trong CSDL PostgreSQL
        modelBuilder.Entity<FarmingLog>(entity =>
        {
            //1. Đặt tên bảng trong CSDL
            entity.ToTable("FarmingLogs");

            //2. Khóa chính (PK)
            entity.HasKey(e => e.Id);

            //3. Ràng buộc các cột dữ liệu
            entity.Property(e => e.LogDate)
                  .IsRequired();
            
            entity.Property(e => e.Description)
                  .IsRequired()
                  .HasMaxLength(500); //Giới hạn 500 kí tự
            
            //Chuyển đổi Enum thành chuỗi chữ
            entity.Property(e => e.ActivityType)
                  .HasConversion<string>()
                  .HasMaxLength(50);
            
            entity.Property(e => e.IsLocked)
                  .HasDefaultValue(false);
            
            entity.Property(e => e.LockedBy)
                  .HasMaxLength(100);

            //4. Thiết lập khóa ngoại (FK), liên kết tới CropBatch
            entity.HasOne<CropBatch>()
                  .WithMany()
                  .HasForeignKey(e => e.CropBatchId)
                  .OnDelete(DeleteBehavior.Restrict); //Restrict: Thao tác xóa lô mùa vụ vô hiệu hóa khi đã có nhật ký canh tác ghi nhận
        });
    }
}