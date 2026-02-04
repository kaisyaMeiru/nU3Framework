using System;
using Microsoft.Extensions.DependencyInjection;

namespace nU3.Core.Logic
{
    /// <summary>
    /// DI 컨테이너(IServiceProvider)를 사용하여 BizLogic 인스턴스를 생성하는 구현체입니다.
    /// ActivatorUtilities를 사용하면 해당 타입이 DI에 등록되어 있지 않아도 생성시점에
    /// 생성자 매개변수를 컨테이너에서 해결하여 인스턴스를 생성할 수 있습니다.
    /// 
    /// 사용 예:
    /// var factory = new BizLogicFactory(serviceProvider);
    /// var patientLogic = factory.Create<PatientBizLogic>();
    /// </summary>
    public class BizLogicFactory : IBizLogicFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public BizLogicFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public T Create<T>() where T : class
        {
            // ActivatorUtilities.CreateInstance는 컨테이너에서 의존성을 해결하고
            // 대상 타입의 인스턴스를 생성합니다. 타입이 DI에 등록되어 있지 않아도 작동합니다.
            return ActivatorUtilities.CreateInstance<T>(_serviceProvider);
        }
    }
}
