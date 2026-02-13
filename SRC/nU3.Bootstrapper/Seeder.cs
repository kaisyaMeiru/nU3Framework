using System;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using nU3.Connectivity;
using nU3.Core.Interfaces;

namespace nU3.Bootstrapper
{
    /// <summary>
    /// 개발/테스트 환경에서 로컬 데이터베이스의 초기(더미) 데이터를 생성하는 유틸리티 클래스입니다.
    /// </summary>
    public class Seeder
    {
        private readonly IDBAccessService _db;

        /// <summary>
        /// Seeder 생성자
        /// </summary>
        /// <param name="db">DB 접근 서비스</param>
        public Seeder(IDBAccessService db)
        {
            _db = db;
        }

        /// <summary>
        /// 기본 모듈 및 메뉴 데이터를 시드(DB 삽입)합니다.
        /// </summary>
        public void SeedDummyData()
        {
            // 애플리케이션 베이스 디렉토리(실행 경로) 확인
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // 시드할 대상 DLL 파일명 및 우선 탐색 경로(현재 위치)
            string dllFileName = "nU3.Modules.ADM.AD.Deployer.dll";
            string dummyDllPath = Path.Combine(baseDir, dllFileName);

            // 현재 위치에 DLL이 없으면 개발 환경 프로젝트의 Debug 출력 경로를 시도
            if (!File.Exists(dummyDllPath))
            {
                dummyDllPath = Path.GetFullPath(Path.Combine(
                    baseDir,
                     "..", "..", "..",
                    "Modules", "ADM", "nU3.Modules.ADM.AD.Deployer",
                    "bin", "Debug",
                    dllFileName));
            }

            // 시드용 DLL 파일이 없으면 시드 작업을 건너뜀
            if (!File.Exists(dummyDllPath))
            {
                FileLogger.Warning($"시드용 DLL을 찾을 수 없습니다: {dummyDllPath}. 검색 작업을 건너뜁니다.");
                return;
            }

            // 파일 해시 및 크기 계산
            string hash = CalculateFileHash(dummyDllPath);
            long size = new FileInfo(dummyDllPath).Length;

            // DB 작업 시작 (IDBAccessService 사용)
            try
            {
                // 1) 모듈 마스터 정보 생성
                string sqlModule = @"
                    INSERT OR REPLACE INTO SYS_MODULE_MST (MODULE_ID, CATEGORY, SUBSYSTEM, MODULE_NAME, FILE_NAME)
                    VALUES ('MOD_ADM_AD_DEPLOYER', 'ADM', 'AD', 'ADM AD Deployer Module', 'nU3.Modules.ADM.AD.Deployer.dll')";
                _db.ExecuteNonQuery(sqlModule);

                // 2) 모듈 버전 정보(초기 메타데이터) 생성
                string sqlVersion = @"
                    INSERT OR REPLACE INTO SYS_MODULE_VER (MODULE_ID, VERSION, FILE_HASH, FILE_SIZE, STORAGE_PATH, DEPLOY_DESC, DEL_DATE)
                    VALUES ('MOD_ADM_AD_DEPLOYER', '1.0.0.0', @hash, @size, @path, 'Seed', NULL)";
                
                var verParams = new Dictionary<string, object>
                {
                    { "@hash", hash },
                    { "@size", size },
                    { "@path", dummyDllPath }
                };
                _db.ExecuteNonQuery(sqlVersion, verParams);

                FileLogger.Info("ADM AD Deployer 기본 데이터 시드 완료.");

                // 3) 프로그램(인스턴스/화면) 정보 생성
                string sqlProg = @"
                    INSERT OR REPLACE INTO SYS_PROG_MST (PROG_ID, MODULE_ID, CLASS_NAME, PROG_NAME, AUTH_LEVEL, IS_ACTIVE, PROG_TYPE)
                    VALUES ('ADM_AD_DEPLOYER', 'MOD_ADM_AD_DEPLOYER', 'nU3.Modules.ADM.AD.Deployer.DeployerWorkControl', 'ADM AD Deployer', 1, 'Y', 1)";
                _db.ExecuteNonQuery(sqlProg);

                // 4) 메뉴 라이브러리 업데이트 및 기본 메뉴 추가
                string sqlCount = "SELECT COUNT(*) FROM SYS_MENU";
                int count = Convert.ToInt32(_db.ExecuteScalarValue(sqlCount) ?? 0);
                
                if (count == 0)
                {
                    _db.ExecuteNonQuery("INSERT INTO SYS_MENU (MENU_ID, MENU_NAME, SORT_ORD) VALUES ('ROOT_ADM', 'ADM System', 10)");
                    _db.ExecuteNonQuery("INSERT INTO SYS_MENU (MENU_ID, PARENT_ID, MENU_NAME, PROG_ID, SORT_ORD) VALUES ('NODE_AD_DEPLOY', 'ROOT_ADM', 'AD Deployer', 'ADM_AD_DEPLOYER', 10)");
                    FileLogger.Info("기본 메뉴 시드 완료.");
                }
            }
            catch (Exception ex)
            {
                FileLogger.Error("Seeder 데이터 입력 중 오류 발생", ex);
            }
        }

        /// <summary>
        /// 파일의 SHA256 해시를 사용하여 소문자 16진수 문자열로 반환합니다.
        /// </summary>
        /// <param name="filePath">해시를 계산할 파일의 전체 경로</param>
        /// <returns>소문자 16진수 문자열 해시</returns>
        private string CalculateFileHash(string filePath)
        {
            using (var sha256 = SHA256.Create())
            using (var stream = File.OpenRead(filePath))
            {
                var hash = sha256.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}