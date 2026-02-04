using System;

namespace nU3.Core.Logic
{
    /// <summary>
    /// 비즈니스 로직 인스턴스를 생성하기 위한 팩토리 인터페이스입니다.
    /// DI 컨테이너와 통합하여 의존성 주입을 자동으로 처리할 수 있도록 합니다.
    /// 
    /// 목적:
    /// - 각 BizLogic 클래스를 일일이 DI에 등록하지 않고도 필요한 시점에 생성할 수 있습니다.
    /// - 테스트 환경에서 목(MOCK) 주입을 용이하게 합니다.
    /// </summary>
    public interface IBizLogicFactory
    {
        /// <summary>
        /// 지정한 비즈니스 로직 타입의 인스턴스를 생성하여 반환합니다.
        /// 제네릭 T는 클래스 타입이어야 합니다.
        /// </summary>
        /// <typeparam name="T">생성할 BizLogic 타입</typeparam>
        /// <returns>생성된 BizLogic 인스턴스</returns>
        T Create<T>() where T : class;
    }
}
