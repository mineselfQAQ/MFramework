# Task: [feat] 补齐 EventBus 监听管理与 Unity 生命周期桥接

## Steps

- [x] 梳理当前 `Assets/MFramework/CoreEx/Event/MEventBus.cs` 的字符串事件与泛型 `IEvent` 事件结构，确认新增 Remove / Clear API 不破坏现有 `Register`、`RegisterSafe`、`Publish` 行为。 | Files: `Assets/MFramework/CoreEx/Event/MEventBus.cs`
- [x] 为字符串事件补充监听移除能力：支持移除指定 `Action`，并支持清空指定 `eventName` 下的全部监听。 | Files: `Assets/MFramework/CoreEx/Event/MEventBus.cs`, `Assets/Tests/Features/Event/EventBusFeatureTests.cs`
- [x] 为泛型 `IEvent` 事件补充监听移除能力：支持移除指定 `Action<TEvent>`，并支持清空指定 `TEvent` 类型下的全部监听。 | Files: `Assets/MFramework/CoreEx/Event/MEventBus.cs`, `Assets/Tests/Features/Event/EventBusFeatureTests.cs`
- [x] 补充全量清理能力，用于清空 `MEventBus` 内部全部字符串事件与泛型事件监听，便于模块关闭或测试隔离。 | Files: `Assets/MFramework/CoreEx/Event/MEventBus.cs`, `Assets/Tests/Features/Event/EventBusFeatureTests.cs`
- [x] 处理 `RegisterSafe` 与 Remove 的关系：明确 Safe 包装后的 handler 是否可被原始 handler 移除；若不能无歧义支持，应在实现中选择可维护方案并补充说明。 | Files: `Assets/MFramework/CoreEx/Event/MEventBus.cs`, `Assets/Tests/Features/Event/EventBusFeatureTests.cs`
- [x] 在 `Assets/MFramework/Module/Event/` 下尝试新增 `UnityLifecycleEventProvider`，通过 `IServiceProvider` 接入当前框架生命周期，不新增 `.asmdef`。 | Files: `Assets/MFramework/Module/Event/UnityLifecycleEventProvider.cs`
- [x] 在 `Assets/MFramework/Module/Event/` 下尝试新增 `UnityLifecycleEventDispatcher : MonoBehaviour`，负责把 `Awake`、`Start`、`Update`、`FixedUpdate`、`LateUpdate`、`OnApplicationFocus`、`OnApplicationPause`、`OnApplicationQuit` 转发为事件。 | Files: `Assets/MFramework/Module/Event/UnityLifecycleEventDispatcher.cs`
- [x] 设计 Unity 生命周期事件的数据类型，优先使用当前 `MEventBus` 的强类型 `IEvent` 模式，而不是照搬旧 `BuiltInEvent` 双字典结构。 | Files: `Assets/MFramework/Module/Event/UnityLifecycleEvents.cs`
- [x] 评估 Unity 生命周期桥接效果：若与当前 DI 生命周期冲突、需要隐式全局对象、或引入过重样板代码，则不添加该尝试项，并在实现记录中说明原因。 | Files: `Assets/MFramework/Module/Event/UnityLifecycleEventProvider.cs`, `.ai/output/260425_[feat] 补齐 EventBus 监听管理与 Unity 生命周期桥接/implement.md`
- [x] 补充 `[feat]` 测试或验证：至少覆盖字符串事件 Remove / Clear、泛型事件 Remove / Clear、全量 Clear；如保留 Unity 生命周期桥接，则补充最小验证脚本或测试辅助组件。 | Files: `Assets/Tests/Features/Event/EventBusFeatureTests.cs`, `Assets/Tests/Features/Event/UnityLifecycleEventDispatcherTests.cs`
- [x] 编译检查，确认不修改 `ProjectSettings/`、`Packages/manifest.json`，不新增 `.asmdef`，不删除或重新生成 `.meta`。 | Files: `.ai/output/260425_[feat] 补齐 EventBus 监听管理与 Unity 生命周期桥接/implement.md`
- [x] 在实现记录中说明本次没有整体迁移旧 `MEventSystem` 的原因，以及旧 `MonoSingleton` 生命周期与当前 DI Provider 生命周期的差异。 | Files: `.ai/output/260425_[feat] 补齐 EventBus 监听管理与 Unity 生命周期桥接/implement.md`

## Plan Source

- Source: `D:\___UNITY___\MFramework\.ai\output\260425_[feat] 补齐 EventBus 监听管理与 Unity 生命周期桥接\plan.md`
- SHA256: `9259CA2211F752CCA149F8BB4094CC75B5BD8274B17F593BB599CF40C4C42D68`

## Notes

- 实际编辑工作区：`E:\___AI___\MFramework_AI_260425_EventBus`。该目录按用户授权作为独立干净 AI 工作目录，从固定 fork clone 后基于 `upstream/main` 创建 `260425-feat-eventbus-lifecycle` 分支。
- `MEventBus` 保持原有字符串事件与强类型 `IEvent` 双结构；新增 `Remove(string, Action)`、`Clear(string)`、`Remove<TEvent>`、`Clear<TEvent>`、`Clear()`，未改变原有 `Register`、`RegisterSafe`、`Publish` 调用方式。
- 字符串 `Publish` 改为快照遍历，避免派发过程中移除监听导致集合修改异常；泛型 `Publish` 原本已使用快照。
- `RegisterSafe` 现在保存原始 handler 到 safe 包装 handler 的映射，因此调用 `Remove` 时可传入原始 handler 移除 Safe 注册。若同一个 handler 同时普通注册和 Safe 注册，`Remove` 会移除该 handler 关联的两类监听，避免留下隐式包装监听。
- 保留 Unity 生命周期桥接：Provider 以 `IServiceProvider` 接入框架生命周期，持有显式 `MEventBus`，创建隐藏 Dispatcher 对象转发 Unity 回调；没有引入静态全局 `MEventSystem`，也没有新增 `.asmdef`。`Awake` 通过先创建 inactive GameObject、初始化 Dispatcher 后再激活来降低漏发风险。
- 本次没有整体迁移旧 `MEventSystem`：旧实现是静态入口与 `MonoSingleton` 生命周期，倾向隐式全局对象和跨模块静态状态；当前框架服务生命周期以 `IServiceProvider.Register/Initialize/Shutdown/Unregister` 为边界，更适合显式 Provider 持有 dispatcher 与 event bus，便于测试隔离和模块关闭时清理。
- Unity BatchMode 测试命令需要去掉 `-quit`，否则当前 Unity 6000.0.61f1 + Test Framework 1.6.0 会在 Test Runner 下一帧启动前退出。测试结果：EditMode `12/12 Passed`，包含 EventBus 新增测试、UnityLifecycleEventDispatcher 测试和既有 Pool 测试。
- Unity 首次导入曾自动修改 `Packages/manifest.json`、`Packages/packages-lock.json`、`ProjectSettings/EditorBuildSettings.asset`、`ProjectSettings/ShaderGraphSettings.asset`，已恢复；最终变更不包含这些文件。

## PR Status

- Fork: Done
- Push: Done
- PR: Done
- Reason: 已推送 `260425-feat-eventbus-lifecycle` 到固定 fork，并创建 PR `https://github.com/mineselfQAQ/MFramework/pull/4`。PR 正文通过 UTF-8 文件 `.ai/output/260425_[feat] 补齐 EventBus 监听管理与 Unity 生命周期桥接/pr_body.md` 传入 `gh pr create --body-file`。
