using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraWaitForm;
using nU3.Core.Services;

namespace nU3.Core.UI.Shell
{
    /// <summary>
    /// 진행 표시와 함께 비동기 작업을 실행하기 위한 헬퍼 클래스입니다.
    /// 대기(웨이트) 폼, 진행 표시(퍼센트), 취소 지원을 일관된 방식으로 제공합니다.
    /// 폼 소유자(owner)를 전달하면 해당 폼을 부모로 하여 대화형 대기창을 표시합니다.
    /// </summary>
    public static class AsyncOperationHelper
    {
        /// <summary>
        /// 대기 폼을 표시하면서 비동기 작업을 실행합니다.
        /// 호출자는 작업을 CancellationToken으로 취소할 수 있으며, 진행률(IProgress<int>)를 통해 퍼센트를 보고할 수 있습니다.
        /// waitForm은 작업이 끝나면 자동으로 닫힙니다.
        /// </summary>
        /// <typeparam name="T">작업이 반환하는 결과 타입</typeparam>
        /// <param name="owner">대기 폼의 소유자 폼(부모)</param>
        /// <param name="caption">대기 폼에 표시할 문구</param>
        /// <param name="operation">실제 실행할 비동기 작업. 인자로 CancellationToken과 IProgress<int>를 받습니다.</param>
        /// <param name="allowCancel">사용자가 취소 버튼을 사용할 수 있는지 여부</param>
        /// <returns>작업이 반환한 결과를 포함하는 Task</returns>
        public static async Task<T> ExecuteWithWaitAsync<T>(
            Form owner,
            string caption,
            Func<CancellationToken, IProgress<int>, Task<T>> operation,
            bool allowCancel = true)
        {
            using var cts = new CancellationTokenSource();
            using var waitForm = new ProgressWaitForm(caption, allowCancel, cts);

            T result = default!;
            Exception? exception = null;

            // 소유 폼을 부모로 하여 대기 폼 표시
            waitForm.Show(owner);

            try
            {
                // 진행률 콜백: UI 스레드에서 폼이 살아있으면 진행률 갱신
                var progress = new Progress<int>(percent =>
                {
                    if (!waitForm.IsDisposed)
                        waitForm.UpdateProgress(percent);
                });

                result = await operation(cts.Token, progress);
            }
            catch (OperationCanceledException)
            {
                // 사용자가 취소한 경우 예외를 그대로 상위로 전달
                throw;
            }
            catch (Exception ex)
            {
                // 예외를 보관해 폼을 닫은 뒤 다시 던집니다.
                exception = ex;
            }
            finally
            {
                if (!waitForm.IsDisposed)
                    waitForm.Close();
            }

            if (exception != null)
                throw exception;

            return result;
        }

        /// <summary>
        /// 항목 수준의 상세 진행 정보를 표시하는 폼과 함께 비동기 작업을 실행합니다.
        /// 작업은 BatchOperationProgress를 통해 총 항목 개수, 완료 항목 수, 현재 항목, 퍼센트 등을 보고할 수 있습니다.
        /// </summary>
        /// <typeparam name="T">작업이 반환하는 결과 타입</typeparam>
        /// <param name="owner">대기 폼의 소유자 폼</param>
        /// <param name="caption">대기 폼에 표시할 문구</param>
        /// <param name="operation">실제 실행할 비동기 작업. CancellationToken과 IProgress<BatchOperationProgress>를 받습니다.</param>
        /// <param name="allowCancel">사용자가 취소 버튼을 사용할 수 있는지 여부</param>
        /// <returns>작업이 반환한 결과를 포함하는 Task</returns>
        public static async Task<T> ExecuteWithProgressAsync<T>(
            Form owner,
            string caption,
            Func<CancellationToken, IProgress<BatchOperationProgress>, Task<T>> operation,
            bool allowCancel = true)
        {
            using var cts = new CancellationTokenSource();
            using var progressForm = new DetailedProgressForm(caption, allowCancel, cts);

            T result = default!;
            Exception? exception = null;

            // 소유 폼을 부모로 하여 상세 진행 폼 표시
            progressForm.Show(owner);

            try
            {
                // 항목 단위 진행률 업데이트 핸들러
                var progress = new Progress<BatchOperationProgress>(p =>
                {
                    if (!progressForm.IsDisposed)
                        progressForm.UpdateProgress(p);
                });

                result = await operation(cts.Token, progress);
            }
            catch (OperationCanceledException)
            {
                // 취소 시 상위로 전달
                throw;
            }
            catch (Exception ex)
            {
                exception = ex;
            }
            finally
            {
                if (!progressForm.IsDisposed)
                    progressForm.Close();
            }

            if (exception != null)
                throw exception;

            return result;
        }

        /// <summary>
        /// 진행 표시 없이 간단한 대기만 필요한 비동기 작업을 실행합니다.
        /// 내부적으로 ExecuteWithWaitAsync(T) 오버로드를 호출합니다.
        /// </summary>
        public static async Task<T> ExecuteWithWaitAsync<T>(
            Form owner,
            string caption,
            Func<CancellationToken, Task<T>> operation,
            bool allowCancel = true)
        {
            return await ExecuteWithWaitAsync(owner, caption,
                async (ct, progress) => await operation(ct),
                allowCancel);
        }

        /// <summary>
        /// 반환값이 없는 비동기 작업을 대기 폼과 함께 실행합니다.
        /// 내부적으로 제네릭 오버로드를 사용하여 Task를 래핑합니다.
        /// </summary>
        public static async Task ExecuteWithWaitAsync(
            Form owner,
            string caption,
            Func<CancellationToken, IProgress<int>, Task> operation,
            bool allowCancel = true)
        {
            await ExecuteWithWaitAsync<object>(owner, caption,
                async (ct, progress) =>
                {
                    await operation(ct, progress);
                    return null!;
                },
                allowCancel);
        }
    }

    /// <summary>
    /// 진행 바가 있는 간단한 대기 폼입니다.
    /// - 캡션(문구)을 표시하고 퍼센트 기반의 진행률을 업데이트합니다.
    /// - 취소 버튼을 제공하여 작업을 취소할 수 있습니다(설정된 경우).
    /// - UI 스레드 안전성을 위해 InvokeRequired 체크를 수행합니다.
    /// </summary>
    public class ProgressWaitForm : XtraForm
    {
        private readonly LabelControl _labelCaption;
        private readonly ProgressBarControl _progressBar;
        private readonly SimpleButton _btnCancel;
        private readonly CancellationTokenSource _cts;
        private readonly bool _allowCancel;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="caption">표시할 문구</param>
        /// <param name="allowCancel">취소 버튼 표시 여부</param>
        /// <param name="cts">취소를 트리거할 CancellationTokenSource</param>
        public ProgressWaitForm(string caption, bool allowCancel, CancellationTokenSource cts)
        {
            _cts = cts;
            _allowCancel = allowCancel;

            this.Text = "처리 중...";
            this.Size = new System.Drawing.Size(400, 150);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;
            this.ShowInTaskbar = false;

            _labelCaption = new LabelControl
            {
                Text = caption,
                Location = new System.Drawing.Point(20, 20),
                AutoSizeMode = LabelAutoSizeMode.None,
                Size = new System.Drawing.Size(360, 20)
            };

            _progressBar = new ProgressBarControl
            {
                Location = new System.Drawing.Point(20, 50),
                Size = new System.Drawing.Size(360, 25),
                Properties = { ShowTitle = true, PercentView = true }
            };

            _btnCancel = new SimpleButton
            {
                Text = "취소",
                Location = new System.Drawing.Point(150, 90),
                Size = new System.Drawing.Size(100, 30),
                Visible = allowCancel
            };
            _btnCancel.Click += (s, e) =>
            {
                // 취소 요청: 버튼 비활성화 및 텍스트 변경으로 UX 제공
                _cts.Cancel();
                _btnCancel.Enabled = false;
                _btnCancel.Text = "취소 중...";
            };

            this.Controls.Add(_labelCaption);
            this.Controls.Add(_progressBar);
            this.Controls.Add(_btnCancel);
        }

        /// <summary>
        /// 진행률(0-100)을 업데이트합니다. UI 스레드에서 호출되지 않았을 경우 Invoke로 안전하게 호출합니다.
        /// </summary>
        public void UpdateProgress(int percent)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<int>(UpdateProgress), percent);
                return;
            }

            _progressBar.Position = Math.Min(100, Math.Max(0, percent));
        }

        /// <summary>
        /// 캡션 문구를 변경합니다. UI 스레드 안전 처리 포함.
        /// </summary>
        public void UpdateCaption(string caption)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateCaption), caption);
                return;
            }

            _labelCaption.Text = caption;
        }

        /// <summary>
        /// 폼이 닫힐 때 사용자가 닫는 동작이면 취소 토큰을 트리거합니다(취소 허용 시).
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && _allowCancel)
            {
                _cts.Cancel();
            }
            base.OnFormClosing(e);
        }
    }

    /// <summary>
    /// 항목 수준의 진행 정보를 표시하는 상세 진행 폼입니다.
    /// - 총 항목 수, 완료 항목 수, 현재 항목, 전체 퍼센트 등을 한눈에 보여줍니다.
    /// - 각 단계별로 설명 텍스트를 업데이트할 수 있습니다.
    /// - 취소 버튼을 통해 처리 취소를 요청할 수 있습니다.
    /// </summary>
    public class DetailedProgressForm : XtraForm
    {
        private readonly LabelControl _labelCaption;
        private readonly LabelControl _labelStatus;
        private readonly LabelControl _labelCurrentItem;
        private readonly ProgressBarControl _progressBar;
        private readonly SimpleButton _btnCancel;
        private readonly CancellationTokenSource _cts;
        private readonly bool _allowCancel;

        /// <summary>
        /// 생성자
        /// </summary>
        /// <param name="caption">표시할 문구</param>
        /// <param name="allowCancel">취소 버튼 표시 여부</param>
        /// <param name="cts">취소 토큰 소스</param>
        public DetailedProgressForm(string caption, bool allowCancel, CancellationTokenSource cts)
        {
            _cts = cts;
            _allowCancel = allowCancel;

            this.Text = "처리 중...";
            this.Size = new System.Drawing.Size(450, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;
            this.ShowInTaskbar = false;

            _labelCaption = new LabelControl
            {
                Text = caption,
                Location = new System.Drawing.Point(20, 15),
                AutoSizeMode = LabelAutoSizeMode.None,
                Size = new System.Drawing.Size(400, 20),
                Appearance = { Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold) }
            };

            _labelStatus = new LabelControl
            {
                Text = "준비 중...",
                Location = new System.Drawing.Point(20, 45),
                AutoSizeMode = LabelAutoSizeMode.None,
                Size = new System.Drawing.Size(400, 20)
            };

            _progressBar = new ProgressBarControl
            {
                Location = new System.Drawing.Point(20, 75),
                Size = new System.Drawing.Size(400, 25),
                Properties = { ShowTitle = true, PercentView = true }
            };

            _labelCurrentItem = new LabelControl
            {
                Text = "",
                Location = new System.Drawing.Point(20, 110),
                AutoSizeMode = LabelAutoSizeMode.None,
                Size = new System.Drawing.Size(400, 20),
                Appearance = { ForeColor = System.Drawing.Color.Gray }
            };

            _btnCancel = new SimpleButton
            {
                Text = "취소",
                Location = new System.Drawing.Point(175, 140),
                Size = new System.Drawing.Size(100, 30),
                Visible = allowCancel
            };
            _btnCancel.Click += (s, e) =>
            {
                _cts.Cancel();
                _btnCancel.Enabled = false;
                _btnCancel.Text = "취소 중...";
                _labelStatus.Text = "취소 요청됨...";
            };

            this.Controls.Add(_labelCaption);
            this.Controls.Add(_labelStatus);
            this.Controls.Add(_progressBar);
            this.Controls.Add(_labelCurrentItem);
            this.Controls.Add(_btnCancel);
        }

        public void UpdateProgress(nU3.Core.Services.BatchOperationProgress progress)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<nU3.Core.Services.BatchOperationProgress>(UpdateProgress), progress);
                return;
            }

            _progressBar.Position = progress.PercentComplete;
            _labelStatus.Text = $"처리 중... ({progress.CompletedItems}/{progress.TotalItems})";
            _labelCurrentItem.Text = progress.CurrentItem ?? "";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && _allowCancel)
            {
                _cts.Cancel();
            }
            base.OnFormClosing(e);
        }
    }

    /// <summary>
    /// 불확정(progress indeterminate)형 대기 폼입니다(마퀴 스타일).
    /// - 진행률이 수치로 제공되지 않을 때 사용합니다.
    /// - 취소 버튼은 선택적으로 표시됩니다.
    /// </summary>
    public class MarqueeWaitForm : XtraForm
    {
        private readonly LabelControl _labelCaption;
        private readonly MarqueeProgressBarControl _progressBar;
        private readonly SimpleButton _btnCancel;
        private readonly CancellationTokenSource? _cts;

        public MarqueeWaitForm(string caption, CancellationTokenSource? cts = null)
        {
            _cts = cts;

            this.Text = "처리 중...";
            this.Size = new System.Drawing.Size(350, 120);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ControlBox = false;
            this.ShowInTaskbar = false;

            _labelCaption = new LabelControl
            {
                Text = caption,
                Location = new System.Drawing.Point(20, 15),
                AutoSizeMode = LabelAutoSizeMode.None,
                Size = new System.Drawing.Size(300, 20)
            };

            _progressBar = new MarqueeProgressBarControl
            {
                Location = new System.Drawing.Point(20, 45),
                Size = new System.Drawing.Size(300, 20)
            };

            _btnCancel = new SimpleButton
            {
                Text = "취소",
                Location = new System.Drawing.Point(125, 75),
                Size = new System.Drawing.Size(100, 25),
                Visible = _cts != null
            };
            _btnCancel.Click += (s, e) =>
            {
                _cts?.Cancel();
                _btnCancel.Enabled = false;
                _btnCancel.Text = "취소 중...";
            };

            this.Controls.Add(_labelCaption);
            this.Controls.Add(_progressBar);
            this.Controls.Add(_btnCancel);
        }

        public void UpdateCaption(string caption)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(UpdateCaption), caption);
                return;
            }
            _labelCaption.Text = caption;
        }
    }
}
