using System;
using System.IO;
using System.Security.Cryptography;
using System.Data.SQLite;
using nU3.Data;

namespace nU3.Bootstrapper
{
    /// <summary>
    /// ����/����� ȯ�濡�� ���� �����ͺ��̽��� ����(����) �����͸� �����ϴ� ��ƿ��Ƽ Ŭ�����Դϴ�.
    /// 
    /// ����:
    /// - �����ڰ� ���ÿ��� ���ø����̼��� ������ �� �ʿ��� �ּ����� ���/���α׷�/�޴� �����͸� �ڵ����� ����Ͽ�
    ///   ���� ������ �Է� ���̵� ����� Ȯ���� �� �ֵ��� �����ϴ�.
    /// - � ȯ�濡���� ȣ������ �ʵ��� �����ϼ���. (���� Program.cs�� DEBUG ���忡���� ȣ���մϴ�.)
    /// 
    /// ���� ���:
    /// 1. ���� ��ġ �Ǵ� ������Ʈ ����� ��¿��� ������ DLL�� ã��
    /// 2. ������ SHA256 �ؽÿ� ũ�⸦ ���
    /// 3. ���� SQLite DB�� ��� ��Ÿ������ �� ���� ������ INSERT OR REPLACE�� ����
    /// 4. �⺻ �޴� �׸��� ����ִٸ� �޴��� ����
    /// </summary>
    public class Seeder
    {
        private readonly LocalDatabaseManager _dbManager;

        /// <summary>
        /// Seeder ������
        /// </summary>
        /// <param name="dbManager">���� DB ������ ���� LocalDatabaseManager �ν��Ͻ�</param>
        public Seeder(LocalDatabaseManager dbManager)
        {
            _dbManager = dbManager;
        }

        /// <summary>
        /// ���� ��� �� ���� ������(DB ���ڵ�)�� �����մϴ�.
        /// 
        /// ����:
        /// - ��� DLL�� �������� ������ �õ� �۾��� �ǳʶݴϴ�.
        /// - �� �޼���� �ַ� ���� ���Ǹ� ���� �����Ǹ� � ȯ�濡���� �������� ���ʽÿ�.
        /// </summary>
        public void SeedDummyData()
        {
            // ���ø����̼� ���� ���̽� ���丮(������ ��� bin ����) Ȯ��
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            // �õ��� ���� DLL ���ϸ��� �켱 Ž�� ���(������ ��ġ)
            string dllFileName = "nU3.Modules.ADM.AD.Deployer.dll";
            string dummyDllPath = Path.Combine(baseDir, dllFileName);

            // ���� ��ġ�� DLL�� ������ ���� ȯ�� ������Ʈ�� Debug ���� ��� ��θ� �õ�
            if (!File.Exists(dummyDllPath))
            {
                dummyDllPath = Path.GetFullPath(Path.Combine(
                    baseDir,
                     "..", "..", "..",
                    "Modules", "ADM", "nU3.Modules.ADM.AD.Deployer",
                    "bin", "Debug",
                    dllFileName));
            }

            // ���� DLL ������ ������ ������ �õ� �۾��� �ǳʶ�
            if (!File.Exists(dummyDllPath))
            {
                FileLogger.Warning($"시드용 DLL을 찾을 수 없습니다: {dummyDllPath}. 검색 작업을 건너뜁니다.");
                return;
            }

            // ���� �ؽ� �� ũ�� ���
            string hash = CalculateFileHash(dummyDllPath);
            long size = new FileInfo(dummyDllPath).Length;

            // SQLite ���� ���ڿ��� ����Ͽ� DB�� ����
            using (var conn = new SQLiteConnection(_dbManager.GetConnectionString()))
            {
                conn.Open();

                // 1) ��� ������ ���� ����
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"
                        INSERT OR REPLACE INTO SYS_MODULE_MST (MODULE_ID, CATEGORY, SUBSYSTEM, MODULE_NAME, FILE_NAME)
                        VALUES ('MOD_ADM_AD_DEPLOYER', 'ADM', 'AD', 'ADM AD Deployer Module', 'nU3.Modules.ADM.AD.Deployer.dll')";
                    cmd.ExecuteNonQuery();

                    // 2) ��� ����(���� ��Ÿ������) ����
                    cmd.CommandText = @"
                        INSERT OR REPLACE INTO SYS_MODULE_VER (MODULE_ID, VERSION, FILE_HASH, FILE_SIZE, STORAGE_PATH, DEPLOY_DESC, DEL_DATE)
                        VALUES ('MOD_ADM_AD_DEPLOYER', '1.0.0.0', @hash, @size, @path, 'Seed', NULL)";

                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@hash", hash);
                    cmd.Parameters.AddWithValue("@size", size);
                    cmd.Parameters.AddWithValue("@path", dummyDllPath);
                    cmd.ExecuteNonQuery();
                }

                FileLogger.Info("ADM AD Deployer 기본 데이터 시드 완료.");

                // 3) ���α׷�(���ν���/ȭ��) ���� ���� ����
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = @"
                        INSERT OR REPLACE INTO SYS_PROG_MST (PROG_ID, MODULE_ID, CLASS_NAME, PROG_NAME, AUTH_LEVEL, IS_ACTIVE, PROG_TYPE)
                        VALUES ('ADM_AD_DEPLOYER', 'MOD_ADM_AD_DEPLOYER', 'nU3.Modules.ADM.AD.Deployer.DeployerWorkControl', 'ADM AD Deployer', 1, 'Y', 1);
                    ";
                    cmd.ExecuteNonQuery();
                }

                // 4) �޴� ���̺��� ��������� �⺻ �޴��� �߰�
                using (var cmd = new SQLiteCommand(conn))
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM SYS_MENU";
                    int count = Convert.ToInt32(cmd.ExecuteScalar());
                    if (count == 0)
                    {
                        cmd.CommandText = @"
                            INSERT INTO SYS_MENU (MENU_ID, MENU_NAME, SORT_ORD) VALUES ('ROOT_ADM', 'ADM System', 10);
                            INSERT INTO SYS_MENU (MENU_ID, PARENT_ID, MENU_NAME, PROG_ID, SORT_ORD) VALUES ('NODE_AD_DEPLOY', 'ROOT_ADM', 'AD Deployer', 'ADM_AD_DEPLOYER', 10);
                        ";
                        cmd.ExecuteNonQuery();
                        FileLogger.Info("기본 메뉴 시드 완료.");
                    }
                }
            }
        }

        /// <summary>
        /// ������ SHA256 �ؽø� ����Ͽ� �ҹ��� 16�� ���ڿ��� ��ȯ�մϴ�.
        /// </summary>
        /// <param name="filePath">�ؽø� ����� ������ ��ü ���</param>
        /// <returns>�ҹ��� 16�� ���ڿ� ������ SHA256 �ؽ�</returns>
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
