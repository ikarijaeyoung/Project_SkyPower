using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IO
{
    /// <summary>
    /// # Summary
    /// Base class for data objects that can be saved and loaded using the data save system.
    /// This class provides a common structure for all saveable data types, including a file name property.
    /// 데이터 저장 시스템을 사용하여 저장하고 불러올 수 있는 데이터 객체의 기본 클래스.
    /// 이 클래스는 모든 저장 가능한 데이터 유형에 대해 공통 구조(파일 이름 속성 포함)를 제공한다.
    ///
    /// ## Usage:
    /// - Inherit from `SaveData` to create a custom data class that can be saved and loaded.
    /// - Use the provided `FileName` property or override it to specify a custom file name.
    /// ## 사용 방법:
    /// - `SaveData`를 상속하여 저장 및 로드할 수 있는 사용자 정의 데이터 클래스를 생성한다.
    /// - 제공된 `FileName` 속성을 사용하거나 이를 재정의하여 사용자 정의 파일 이름을 지정한다.
    ///
    /// ## Features:
    /// - Automatically sets the file name based on the class type name.
    /// - Provides a protected constructor to initialize the file name property.
    /// - Supports the serialization of properties for JSON save operations.
    /// ## 기능:
    /// - 클래스 유형 이름을 기반으로 파일 이름을 자동으로 설정한다.
    /// - 파일 이름 속성을 초기화하기 위한 보호된 생성자를 제공한다.
    /// - JSON 저장 작업을 위해 속성의 직렬화를 지원한다.
    ///
    /// ## Notes for Inheriting Classes:
    /// - All fields or properties that need to be saved must be marked as serializable.
    ///   Use `[SerializeField]` for private fields or `[field: SerializeField]` for auto-properties.
    /// - Ensure that all serializable fields or properties have data types supported by Unity's `JsonUtility`.
    ///   Custom types should either be serializable or provide explicit conversion to/from supported types.
    /// - Avoid using complex types like dictionaries, and consider using lists or arrays instead, as `JsonUtility` does not support them.
    /// - Remember to initialize all properties or fields properly in the constructor to avoid null reference issues during serialization.
    /// - You can define custom constructors with parameters in derived classes, but you must also explicitly declare a parameterless constructor.
    ///   This ensures compatibility with Unity's serialization system, which requires a parameterless constructor for deserialization.
    /// ## 파생 클래스에서의 주의점:
    /// - 저장해야 하는 모든 필드나 속성은 직렬화 가능하도록 설정해야 한다.
    ///   프라이빗 필드는 `[SerializeField]`, 자동 프로퍼티는 `[field: SerializeField]`를 사용한다.
    /// - 직렬화할 모든 필드나 속성은 Unity의 `JsonUtility`에서 지원하는 데이터 타입이어야 한다.
    ///   사용자 정의 타입은 직렬화 가능하거나 지원되는 타입으로 명시적 변환을 제공해야 한다.
    /// - 딕셔너리 같은 복잡한 타입 사용을 피하고, 대신 리스트나 배열을 사용하는 것이 좋다. `JsonUtility`는 이를 지원하지 않는다.
    /// - 직렬화 과정에서 널 참조 오류를 피하기 위해 모든 속성이나 필드를 생성자에서 올바르게 초기화해야 한다.
    /// - 파생 클래스에서 인자를 받는 생성자를 정의할 수 있지만, 반드시 인자를 받지 않는 생성자도 명시적으로 선언해야 한다.
    ///   이는 Unity의 직렬화 시스템이 역직렬화할 때 매개변수가 없는 생성자를 필요로 하기 때문이다.
    /// </summary>
    public class SaveData
    {
        /// <summary>
        /// Gets the name of the file to be used for saving data.
        /// The file name is automatically set to the type name of the derived class.
        /// 데이터를 저장하는 데 사용할 파일 이름을 가져온다.
        /// 파일 이름은 파생 클래스의 유형 이름으로 자동 설정된다.
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        /// Protected constructor that initializes the file name property.
        /// 파생 클래스가 이 생성자를 호출하여 파일 이름 속성을 초기화할 수 있다.
        /// </summary>
        protected SaveData()
        {
            FileName = this.GetType().ToString();
        }

    }
}
