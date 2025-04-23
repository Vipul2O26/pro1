using pro1.Data;
using pro1.Models;

namespace pro1.Helper;
public static class AuditHelper
{
    public static int LogLogin(AppDbContext context, int userId)
    {
        var audit = new Audit
        {
            UserID = userId,
            Login_time = DateTime.Now
        };

        context.Audits.Add(audit);
        context.SaveChanges();

        return audit.AuditID; // Return the actual primary key
    }

    public static void LogLogout(AppDbContext context, int auditId)
    {
        var audit = context.Audits.FirstOrDefault(a => a.AuditID == auditId);
        if (audit != null)
        {
            audit.Logout_time = DateTime.Now;
            context.SaveChanges();
        }
    }
}
