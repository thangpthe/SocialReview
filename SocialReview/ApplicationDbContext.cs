using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialReview.Models;
using System.Security.AccessControl;

namespace SocialReview
{
    // 1. KẾ THỪA TỪ IDENTITYDBCONTEXT (dùng 'User' và 'int' tùy chỉnh)
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        // 2. CÁC DBSET TÙY CHỈNH
        //
        // KHÔNG CẦN 'public DbSet<User> Users { get; set; }'
        // (Vì nó đã được kế thừa từ IdentityDbContext)
        //
        public DbSet<Company> Companies { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Reaction> Reactions { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // 3. (RẤT QUAN TRỌNG) GHI ĐÈ OnModelCreating ĐỂ MAP TÊN BẢNG
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    // BẮT BUỘC gọi 'base.OnModelCreating(modelBuilder)'
        //    // để Identity tự cấu hình các mối quan hệ của nó
        //    base.OnModelCreating(modelBuilder);

        //    // --- BÁO CHO IDENTITY DÙNG TÊN BẢNG CỦA BẠN ---

        //    // Map lớp 'User' (kế thừa từ IdentityUser) tới bảng 'Users'
        //    modelBuilder.Entity<User>(entity =>
        //    {
        //        entity.ToTable(name: "Users");
        //    });

        //    // Map các lớp MẶC ĐỊNH của Identity tới tên bảng tùy chỉnh
        //    modelBuilder.Entity<IdentityRole<int>>(entity =>
        //    {
        //        entity.ToTable(name: "Roles"); // Thay vì 'AspNetRoles'
        //    });
        //    modelBuilder.Entity<IdentityUserRole<int>>(entity =>
        //    {
        //        entity.ToTable("UserRoles"); // Thay vì 'AspNetUserRoles'
        //    });
        //    modelBuilder.Entity<IdentityUserClaim<int>>(entity =>
        //    {
        //        entity.ToTable("UserClaims"); // Thay vì 'AspNetUserClaims'
        //    });
        //    modelBuilder.Entity<IdentityUserLogin<int>>(entity =>
        //    {
        //        entity.ToTable("UserLogins"); // Thay vì 'AspNetUserLogins'
        //    });
        //    modelBuilder.Entity<IdentityRoleClaim<int>>(entity =>
        //    {
        //        entity.ToTable("RoleClaims"); // Thay vì 'AspNetRoleClaims'
        //    });
        //    modelBuilder.Entity<IdentityUserToken<int>>(entity =>
        //    {
        //        entity.ToTable("UserTokens"); // Thay vì 'AspNetUserTokens'
        //    });

        //    // (Thêm các cấu hình Fluent API khác của bạn ở đây nếu cần)
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map bảng Identity
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
            });
            modelBuilder.Entity<IdentityRole<int>>().ToTable("Roles");
            modelBuilder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            modelBuilder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");

            // ⚠️ Fix lỗi Multiple Cascade Paths
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Product)
                .WithMany()
                .HasForeignKey(r => r.ProductID)
                .OnDelete(DeleteBehavior.Cascade);

            // Nếu có Reports, Comments, Reactions... cũng nên thêm tương tự:
            modelBuilder.Entity<Report>()
                .HasOne(r => r.Reporter)
                .WithMany()
                .HasForeignKey(r => r.ReporterID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(c=>c.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Reaction>()
                .HasOne(c => c.User)
                .WithMany(c=>c.Reactions)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}

