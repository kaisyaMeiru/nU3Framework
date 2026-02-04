using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using nU3.Models;

namespace nU3.Bootstrapper
{
    /// <summary>
    /// 업데이트 진행 표시 폼(로직 클래스).
    /// 
    /// 주요 역할:
    /// - UI 초기화 과정은 디자이너 코드(InitializeComponent)로 분리되어 있음
    /// - 각 단계별 진행 상태(현재 진행 컴포넌트, 전체 표시 등)를 이벤트 처리로 갱신
    /// 
    /// 주요 로직 책임:
    /// - ComponentLoader가 보낸 이벤트를 받아 UI의 상태를 표시
    /// - 사용자는 이 창을 통해 업데이트 진행 현황을 확인하고, 필요 시 취소할 수 있음
    /// - UI 갱신은 백그라운드 스레드 InvokeRequired 체크를 준수함
    /// </summary>
    public partial class UpdateProgressForm : Form
    {
        private bool _canClose = false;

        /// <summary>
        /// 생성자: 디자이너에서 생성된 InitializeComponent를 호출하여 컨트롤을 초기화합니다.
        /// </summary>
        public UpdateProgressForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 패널 크기 변경 시 하단 패널 내 버튼 위치를 조정합니다.
        /// 버튼이 패널의 중앙에 위치하도록 재계산합니다.
        /// 디자이너에서 폰트 스케일링을 정확히 의도대로 안 할 수 있기 때문입니다.
        /// </summary>
        private void PanelBottom_Resize(object? sender, EventArgs e)
        {
            // 버튼을 패널 중앙에 배치합니다.
            _btnCancel.Location = new Point(
                (_pnlBottom.Width - _btnCancel.Width) / 2,
                (_pnlBottom.Height - _btnCancel.Height) / 2);
        }

        /// <summary>
        /// 업데이트 대상 정보를 ListView에 초기화합니다.
        /// UI 스레드에서 ListView를 직접 조작해야 하므로 InvokeRequired를 체크합니다.
        /// </summary>
        /// <param name="updates">업데이트가 필요한 컴포넌트 목록</param>
        public void InitializeUpdateList(List<ComponentUpdateInfo> updates)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<List<ComponentUpdateInfo>>(InitializeUpdateList), updates);
                return;
            }

            _listView.Items.Clear();

            foreach (var update in updates)
            {
                var item = new ListViewItem("대기")
                {
                    Tag = update.ComponentId
                };
                item.SubItems.Add(update.ComponentName ?? update.ComponentId);
                item.SubItems.Add(update.ServerVersion);
                item.SubItems.Add($"{update.FileSize / 1024.0:N0} KB");
                item.SubItems.Add(GetComponentTypeText(update.ComponentType));

                _listView.Items.Add(item);
            }

            _lblStatus.Text = $"{updates.Count}개 컴포넌트 업데이트 필요";
        }

        /// <summary>
        /// ComponentLoader로부터 전달된 진행 이벤트를 받아 UI를 갱신합니다.
        /// PercentComplete로 ProgressBar를 채우고 현재 단계(다운로드/설치 등)에 따라 상태 메시지를 표시합니다.
        /// </summary>
        /// <param name="args">업데이트 진행 이벤트 인자</param>
        public void UpdateProgress(ComponentUpdateEventArgs args)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ComponentUpdateEventArgs>(UpdateProgress), args);
                return;
            }

            _progressBar.Value = Math.Min(100, Math.Max(0, args.PercentComplete));

            switch (args.Phase)
            {
                case UpdatePhase.Checking:
                    _lblStatus.Text = "업데이트 확인 중...";
                    break;

                case UpdatePhase.Downloading:
                    _lblStatus.Text = $"다운로드 중... ({args.CurrentIndex}/{args.TotalCount})";
                    _lblDetail.Text = args.ComponentName ?? args.ComponentId;
                    UpdateListItemStatus(args.ComponentId, "다운로드 중");
                    break;

                case UpdatePhase.Installing:
                    _lblStatus.Text = $"설치 중... ({args.CurrentIndex}/{args.TotalCount})";
                    _lblDetail.Text = args.ComponentName ?? args.ComponentId;
                    UpdateListItemStatus(args.ComponentId, "설치 중");
                    break;

                case UpdatePhase.Completed:
                    _lblStatus.Text = "업데이트 완료!";
                    _lblDetail.Text = "";
                    _btnCancel.Text = "완료";
                    _canClose = true;
                    break;

                case UpdatePhase.Failed:
                    UpdateListItemStatus(args.ComponentId, "실패");
                    break;
            }
        }

        /// <summary>
        /// 개별 컴포넌트의 완료 상태를 표시합니다.
        /// </summary>
        /// <param name="componentId">컴포넌트 ID</param>
        /// <param name="success">성공 여부</param>
        public void MarkComponentCompleted(string componentId, bool success)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<string, bool>(MarkComponentCompleted), componentId, success);
                return;
            }

            UpdateListItemStatus(componentId, success ? "완료" : "실패");
        }

        /// <summary>
        /// 전체 업데이트 작업의 최종 결과를 화면에 표시합니다.
        /// 성공 시 Shell 실행 버튼으로 변경, 실패 시 실패 항목을 강조합니다.
        /// </summary>
        /// <param name="result">업데이트 결과 객체</param>
        public void ShowResult(ComponentUpdateResult result)
        {
            if (InvokeRequired)
            {
                Invoke(new Action<ComponentUpdateResult>(ShowResult), result);
                return;
            }

            _progressBar.Value = 100;
            _lblDetail.Text = "";
            _canClose = true;

            if (result.Success)
            {
                _lblStatus.Text = $"성공: {result.Message}";
                _lblStatus.ForeColor = Color.Green;
                _btnCancel.Text = "Shell 실행";
            }
            else
            {
                _lblStatus.Text = $"실패: {result.Message}";
                _lblStatus.ForeColor = Color.Orange;
                _btnCancel.Text = "닫기";

                foreach (var (componentId, _) in result.FailedComponents)
                {
                    UpdateListItemStatus(componentId, "실패");
                }
            }
        }

        /// <summary>
        /// 업데이트가 없을 경우 UI를 갱신합니다.
        /// </summary>
        public void ShowNoUpdates()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(ShowNoUpdates));
                return;
            }

            _progressBar.Value = 100;
            _lblStatus.Text = "모든 컴포넌트가 최신 상태입니다.";
            _lblStatus.ForeColor = Color.Green;
            _lblDetail.Text = "";
            _btnCancel.Text = "Shell 실행";
            _canClose = true;
        }

        /// <summary>
        /// 리스트뷰의 특정 항목 텍스트를 갱신합니다.
        /// </summary>
        private void UpdateListItemStatus(string componentId, string status)
        {
            foreach (ListViewItem item in _listView.Items)
            {
                if (item.Tag?.ToString() == componentId)
                {
                    item.Text = status;
                    item.EnsureVisible();
                    break;
                }
            }
        }

        /// <summary>
        /// ComponentType 열거형을 사용자 친화적인 텍스트로 변환합니다.
        /// 필요 시 다국어화(Localization) 확장 포인트를 제공하세요.
        /// </summary>
        private static string GetComponentTypeText(ComponentType type)
        {
            return type switch
            {
                ComponentType.FrameworkCore => "Core",
                ComponentType.SharedLibrary => "Library",
                ComponentType.Executable => "EXE",
                ComponentType.Plugin => "Plugin",
                ComponentType.Resource => "Resource",
                ComponentType.Configuration => "Config",
                _ => "Other"
            };
        }

        /// <summary>
        /// 취소/닫기 버튼 클릭 이벤트 핸들러
        /// - 업데이트가 진행 중이면 정말 종료할지 확인 대화상자를 띄웁니다.
        /// - 완료 상태이면 DialogResult를 OK로 설정하여 호출자에게 알립니다.
        /// </summary>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            if (_canClose)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                if (MessageBox.Show("업데이트를 중단하시겠습니까?\n일부 컴포넌트가 설치되지 않을 수 있습니다.",
                    "중단 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            }
        }

        /// <summary>
        /// 사용자가 창을 닫으려 할 때 호출되는 폼 종료 처리
        /// 진행 중이면 닫기를 막고 취소 로직을 대신 수행합니다.
        /// </summary>
        private void UpdateProgressForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!_canClose && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                BtnCancel_Click(sender, e);
            }
        }

        private void UpdateProgressForm_Load(object sender, EventArgs e)
        {

        }

        private void _lblDetail_Click(object sender, EventArgs e)
        {

        }
    }
}