using System;
using nU3.Core.Context;
using nU3.Core.Repositories;
using nU3.Core.Security;
using nU3.Models;

namespace nU3.Core.Services
{
    /// <summary>
    /// 사용자 세션과 권한 정보를 바탕으로 작업 컨텍스트(WorkContext)를 생성하는 서비스입니다.
    /// </summary>
    public interface IWorkContextService
    {
        /// <summary>
        /// 현재 로그인된 사용자와 지정된 프로그램 ID를 바탕으로 실행 컨텍스트를 생성합니다.
        /// </summary>
        WorkContext CreateWorkContext(string programId);
        
        /// <summary>
        /// 특정 사용자와 역할, 프로그램에 대한 유효 권한을 조회합니다.
        /// </summary>
        ModulePermissions GetPermissions(string userId, string roleCode, int authLevel, string programId);
    }

    public class WorkContextService : IWorkContextService
    {
        private readonly IUserRepository _userRepo;
        private readonly ISecurityRepository _securityRepo;

        public WorkContextService(IUserRepository userRepo, ISecurityRepository securityRepo)
        {
            _userRepo = userRepo;
            _securityRepo = securityRepo;
        }

        public WorkContext CreateWorkContext(string programId)
        {
            var session = UserSession.Current;
            var context = new WorkContext();

            if (session != null)
            {
                var user = _userRepo.GetUserById(session.UserId);
                var roleCode = user?.RoleCode ?? "";

                context.CurrentUser = new UserInfoDto
                {
                    UserId = session.UserId,
                    UserName = session.UserName,
                    AuthLevel = session.AuthLevel,
                    RoleCode = roleCode
                };

                context.Permissions = GetPermissions(session.UserId, roleCode, session.AuthLevel, programId);
            }

            return context;
        }

        public ModulePermissions GetPermissions(string userId, string roleCode, int authLevel, string programId)
        {
            // 최고 권한자(Level 0)는 모든 권한 부여
            if (authLevel == 0)
            {
                var fullPerm = new ModulePermissions();
                fullPerm.GrantAll();
                return fullPerm;
            }

            try
            {
                var dbPerm = _securityRepo.GetEffectivePermission(userId, roleCode, programId);
                if (dbPerm != null)
                {
                    return new ModulePermissions
                    {
                        CanRead = dbPerm.CanRead,
                        CanCreate = dbPerm.CanCreate,
                        CanUpdate = dbPerm.CanUpdate,
                        CanDelete = dbPerm.CanDelete,
                        CanPrint = dbPerm.CanPrint,
                        CanExport = dbPerm.CanExport,
                        CanApprove = dbPerm.CanApprove,
                        CanCancel = dbPerm.CanCancel
                    };
                }
            }
            catch
            {
                // 로깅 등 생략 (필요 시 주입받은 로거 사용)
            }

            // 기본값은 읽기 전용
            return new ModulePermissions { CanRead = true };
        }
    }
}
