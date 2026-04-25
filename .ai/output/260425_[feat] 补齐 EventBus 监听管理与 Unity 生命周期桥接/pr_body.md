# Task

[feat] 补齐 EventBus 监听管理与 Unity 生命周期桥接

## 变更摘要

- 为 `MEventBus` 补充字符串事件和泛型 `IEvent` 事件的 `Remove` / `Clear` / 全量 `Clear`。
- 为 `RegisterSafe` 建立原始 handler 到 Safe 包装 handler 的映射，支持使用原始 handler 移除 Safe 注册。
- 新增 `UnityLifecycleEventProvider`、`UnityLifecycleEventDispatcher` 与强类型 Unity 生命周期事件。
- 新增 EventBus 功能测试与 Unity 生命周期 Dispatcher 最小验证测试。

## 涉及文件

- `Assets/MFramework/CoreEx/Event/MEventBus.cs`
- `Assets/MFramework/Module/Event/UnityLifecycleEvents.cs`
- `Assets/MFramework/Module/Event/UnityLifecycleEventDispatcher.cs`
- `Assets/MFramework/Module/Event/UnityLifecycleEventProvider.cs`
- `Assets/Tests/Features/Event/EventBusFeatureTests.cs`
- `Assets/Tests/Features/Event/UnityLifecycleEventDispatcherTests.cs`
- `.ai/output/260425_[feat] 补齐 EventBus 监听管理与 Unity 生命周期桥接/implement.md`

## 测试结果

- Unity EditMode：`12/12 Passed`
- 命令要点：Unity `6000.0.61f1`，`-runTests -testPlatform EditMode`，未使用 `-quit`，避免 Test Runner 下一帧启动前退出。

## 风险说明

- `Remove` 对同一原始 handler 的普通注册和 Safe 注册会一并移除，避免 Safe 包装监听残留。
- 生命周期桥接使用显式 Provider 持有 `MEventBus`，没有迁移旧静态 `MEventSystem` 兼容层。
- Unity 首次导入产生过 `Packages` / `ProjectSettings` 自动改动，已恢复，最终 PR 不包含这些文件。
