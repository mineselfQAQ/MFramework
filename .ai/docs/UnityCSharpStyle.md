# Unity C# 编写规范（通用版）

本规范适用于通用 Unity 项目，可根据项目规模和团队习惯做适当裁剪。

## 命名规范
- 脚本文件名与类名一致，使用 `PascalCase`，例如 `PlayerController.cs`。
- 类型（class/struct/interface/enum/delegate）使用 `PascalCase`。
- 接口以 `I` 前缀开头，例如 `IMovable`。
- 公有方法、属性、事件使用 `PascalCase`。
- 参数、局部变量使用 `camelCase`。
- 私有字段使用 `_camelCase`，例如 `_moveSpeed`。
- 常量使用 `PascalCase`，例如 `MaxHealth`。
- 枚举成员使用 `PascalCase`。
- Unity 回调方法保持引擎约定命名，例如 `Awake`、`Start`、`Update`、`OnEnable`。

## 目录结构建议
- `Assets/Scripts/Runtime`：运行时代码。
- `Assets/Scripts/Editor`：Editor 代码（需在 Unity Editor 下编译）。
- `Assets/Tests`：测试代码与测试资源。

## 类结构顺序
推荐类内成员顺序如下：
1. 常量与静态字段
2. 序列化字段（`[SerializeField]`）与私有字段
3. 公有字段与属性
4. Unity 生命周期回调（`Awake`/`Start`/`OnEnable`/`Update` 等）
5. 公有 API
6. 私有方法与辅助方法

## Unity 约定与实践
- 能序列化的私有字段用 `[SerializeField]` 替代 `public` 字段。
- 避免在 `Update`、`LateUpdate` 中频繁分配内存（例如 `new`、`LINQ`）。
- 优先通过引用缓存组件（`GetComponent` 只在初始化时调用）。
- 避免在运行时使用 `GameObject.Find`/`FindObjectOfType` 等查找接口。

## 反模式清单
- 滥用 `public` 字段导致封装性下降。
- 在 `Awake`/`Start` 中堆积过多初始化逻辑。
- 随意使用单例导致生命周期与依赖关系混乱。
