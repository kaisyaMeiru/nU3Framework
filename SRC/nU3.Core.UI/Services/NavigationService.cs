using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using nU3.Core.Interfaces;
using nU3.Core.Logging;
using nU3.Core.UI.Helpers;
using nU3.Core.UI.Interfaces;
using nU3.Core.Services;
using nU3.Core.Events;
using nU3.Core.Context;

namespace nU3.Core.UI.Services
{
    public interface INavigationService
    {
        void Initialize(IShellView shellView);
        Task OpenProgramAsync(string programId, string? displayName = null, WorkContext? context = null);
    }

    public class NavigationService : INavigationService
    {
        private readonly ModuleLoaderService _moduleLoader;
        private readonly IWorkContextService _workContextService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IServiceProvider _serviceProvider;
        private IShellView? _shellView;

        public NavigationService(
            ModuleLoaderService moduleLoader,
            IWorkContextService workContextService,
            IEventAggregator eventAggregator,
            IServiceProvider serviceProvider)
        {
            _moduleLoader = moduleLoader;
            _workContextService = workContextService;
            _eventAggregator = eventAggregator;
            _serviceProvider = serviceProvider;
        }

        public void Initialize(IShellView shellView)
        {
            _shellView = shellView;
        }

        public async Task OpenProgramAsync(string programId, string? displayName = null, WorkContext? context = null)
        {
            if (_shellView == null) return;

            // 이미 열려있는 경우
            if (_shellView.IsProgramOpen(programId))
            {
                _shellView.ActivateProgram(programId);
                
                // [추가] 이미 열려있더라도 새로운 컨텍스트(인자)가 들어오면 업데이트 수행
                if (context != null)
                {
                    LogManager.Debug($"[Navigation] '{programId}' 이미 열려있음. 전달된 인자로 컨텍스트 업데이트 시도.", "Navigation");
                    // 실제 업데이트 로직은 IShellView 등을 통해 구현 필요 (필요 시 확장)
                }
                return;
            }

            try
            {
                var type = await _moduleLoader.GetProgramTypeAsync(programId).ConfigureAwait(false);
                if (type == null) return;

                (_shellView as Control)?.SafeInvoke(() => 
                {
                    var control = CreateControl(type, programId, context);
                    _shellView.ShowContent(control, programId, displayName);
                });
            }
            catch (Exception ex)
            {
                LogManager.Error($"프로그램 '{programId}' 실행 실패", "Navigation", ex);
            }
        }

        private Control CreateControl(Type type, string programId, WorkContext? externalContext)
        {
            // DI 컨테이너를 사용하여 인스턴스 생성
            var control = (Control)ActivatorUtilities.CreateInstance(_serviceProvider, type);

            if (control is BaseWorkControl wc)
            {
                // [중요] 전역 이벤트 버스 할당
                wc.EventBus = _eventAggregator;

                // 1. 기본 작업 컨텍스트 생성 (사용자 정보 및 DB 권한 로드)
                var context = _workContextService.CreateWorkContext(programId);

                // 2. 외부에서 전달된 인자(WorkContext)가 있으면 병합
                if (externalContext != null)
                {
                    LogManager.Debug($"[Navigation] '{programId}'에 외부 컨텍스트(인자) 주입 중.", "Navigation");
                    
                    if (externalContext.CurrentPatient != null) context.CurrentPatient = externalContext.CurrentPatient;
                    if (externalContext.CurrentExam != null) context.CurrentExam = externalContext.CurrentExam;
                    if (externalContext.CurrentAppointment != null) context.CurrentAppointment = externalContext.CurrentAppointment;
                    
                    // 커스텀 파라미터 병합
                    if (externalContext.CustomData != null)
                    {
                        foreach (var kv in externalContext.CustomData)
                        {
                            context.SetParameter(kv.Key, kv.Value);
                        }
                    }
                }

                // 컨트롤에 컨텍스트 주입
                wc.InitializeContext(context);
            }

            control.Dock = DockStyle.Fill;
            return control;
        }
    }
}
