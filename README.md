# csharp-dll-test

nodejs-electron 환경에서 C# dll 로드 테스트를 위해 제작하였습니다.

# 환경

## C#

- IDE: visual studio 2019
- Newtonsoft.Json 설치 필요

설치: [https://www.newtonsoft.com/json](https://www.newtonsoft.com/json)

참고: [https://www.newtonsoft.com/json/help/html/SerializeObject.htm](https://www.newtonsoft.com/json/help/html/SerializeObject.htm)

## Electron

일렉트론은 크로스플랫폼(win, mac, linux) 데스크탑 앱을 제작하는 도구입니다.

참고: [https://electronjs.org](https://electronjs.org)

- nodejs 10: 코어 개발
- electron 5: OS API
- vue.js 2.5: 화면 개발(프론트엔드)
- electron-edge-js: C# dll 로드를 위해 추가

# 테스트 클래스 작성

**Class1.cs**  
```c#
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Test_CreateDLL
{
    public class inputData
    {
        public int a { get; set; }
        public int b { get; set; }
        public string c { get; set; }
    }
    public class Account
    {
        public string Email { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public IList<string> Roles { get; set; }
        public int RcvData { get; set; }
        public object input { get; set; }
    }

    public class dllTest
    {
        public async Task<object> get(object input)
        {
            Account account = new Account
            {
                Email = "james@example.com",
                Active = true,
                CreatedDate = new DateTime(2013, 1, 20, 0, 0, 0, DateTimeKind.Utc),
                Roles = new List<string>
                {
                    "User",
                    "Admin",
                    "abcd"
                },
                RcvData = (int)input, // 숫자가 들어왔다고 가정할 경우
                input = input
            };
            return account;
        }
        public async Task<object> add(object input)
        {
          // input = { a: 1, b: 3, c: "str" } 
            string json = JsonConvert.SerializeObject(input, Formatting.Indented);
            inputData ipd = JsonConvert.DeserializeObject<inputData>(json);
            Account account = new Account
            {
                Email = "james@example.com",
                Active = true,
                CreatedDate = new DateTime(2013, 1, 20, 0, 0, 0, DateTimeKind.Utc),
                Roles = new List<string>
                {
                    "User",
                    "Admin",
                    "abcd"
                },
                RcvData = 12345 + ipd.a, // 풀어진 값으로 연산이 가능한 것을 테스트
                input = input
            };
            return account;
        }
        
    }
}
```

빌드하게되면 Debug/WpfControlLibrary1.dll 이 생성됩니다.

add와 get 함수를 만들 었습니다.

생성된 클래스를 그대로 리턴하면 electron에서는 javascript object(json) 형식으로 받아집니다.

만약 electron 에서 javascript object를 인자로 전달할 경우 변환이 필요합니다.

object -> string -> class

# nodejs에서 로드 후 호출

```javascript
const edge = require('electron-edge-js')

const baseDll = './WpfControlLibrary1.dll'
const name = 'Test_CreateDLL.dllTest' // 네임스페이스와 사용할 클래스
const test1 = edge.func({
  assemblyFile: baseDll,
  typeName: name,
  methodName: 'get' // 해당 클래스 안의 메쏘드
})

test1(1234, function (error, result) {
  console.log('test1 result')
  if (error) return console.error(error.message)
  console.log(result)
  console.log(result.Email) // 멤버의 값 확인
})

const test2 = edge.func({
  assemblyFile: baseDll,
  typeName: name,
  methodName: 'add'
})

test2({ a: 1, b: 3, c: "str" }, function (error, result) {
  console.log('test2 result')
  if (error) return console.error(error.message)
  console.log(result)
})
```

제대로 값들이 들어오는 것이 확인 되었습니다.

# 정리

- 함수 선언은 이런식으로 해야합니다.  
public async Task<object> get(object input)

- 클래스를 리턴하면 일렉트론에서는 javacript object 형식으로 들어옵니다.
