using System.Collections.Generic;
using nU3.Models;

namespace nU3.Core.Repositories
{
    public interface ISecurityRepository
    {
        // Users
        List<SecurityUserDto> GetAllSecurityUsers();
        void AddSecurityUser(SecurityUserDto user, string password, List<string> deptCodes);
        void UpdateSecurityUser(SecurityUserDto user, List<string> deptCodes);
        void DeleteSecurityUser(string userId);
        bool IsUserIdExists(string userId);
        List<string> GetUserDeptCodes(string userId);

        // Roles
        List<SecurityRoleDto> GetAllRoles();
        void SyncRoles(List<SecurityRoleDto> roles);

        // Departments
        List<SecurityDeptDto> GetAllDepartments();
        void SyncDepartments(List<SecurityDeptDto> depts);

        // Permissions
        SecurityPermissionDto GetPermission(string targetType, string targetId, string progId);
        SecurityPermissionDto GetEffectivePermission(string userId, string roleCode, string progId); // New
        void SavePermission(SecurityPermissionDto permission);
    }

    public class SecurityUserDto
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
        public string DeptNames { get; set; }
    }

    public class SecurityRoleDto
    {
        public string RoleCode { get; set; }
        public string RoleName { get; set; }
        public string Description { get; set; }
    }

    public class SecurityDeptDto
    {
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
    }

    public class SecurityProgDto
    {
        public string ProgId { get; set; }
        public string ProgName { get; set; }
        public string ModuleName { get; set; }
    }

    public class SecurityPermissionDto
    {
        public string TargetType { get; set; }
        public string TargetId { get; set; }
        public string ProgId { get; set; }
        public bool CanRead { get; set; }
        public bool CanCreate { get; set; }
        public bool CanUpdate { get; set; }
        public bool CanDelete { get; set; }
        public bool CanPrint { get; set; }
        public bool CanExport { get; set; }
        public bool CanApprove { get; set; }
        public bool CanCancel { get; set; }
    }
}
