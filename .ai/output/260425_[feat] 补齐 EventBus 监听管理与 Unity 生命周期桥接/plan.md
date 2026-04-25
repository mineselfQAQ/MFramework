# Task: [feat] 补齐 EventBus 监听管理与 Unity 生命周期桥接

## Steps

- [ ] 梳理当前 `Assets/MFramework/CoreEx/Event/MEventBus.cs` 的字符串事件与泛型 `IEvent` 事件结构，确认新增 Remove / Clear API 不破坏现有 `Register`、`RegisterSafe`、`Publish` 行为。
- [ ] 为字符串事件补充监听移除能力：支持移除指定 `Action`，并支持清空指定 `eventName` 下的全部监听。
- [ ] 为泛型 `IEvent` 事件补充监听移除能力：支持移除指定 `Action<TEvent>`，并支持清空指定 `TEvent` 类型下的全部监听。
- [ ] 补充全量清理能力，用于清空 `MEventBus` 内部全部字符串事件与泛型事件监听，便于模块关闭或测试隔离。
- [ ] 处理 `RegisterSafe` 与 Remove 的关系：明确 Safe 包装后的 handler 是否可被原始 handler 移除；若不能无歧义支持，应在实现中选择可维护方案并补充说明。
- [ ] 在 `Assets/MFramework/Module/Event/` 下尝试新增 `UnityLifecycleEventProvider`，通过 `IServiceProvider` 接入当前框架生命周期，不新增 `.asmdef`。
- [ ] 在 `Assets/MFramework/Module/Event/` 下尝试新增 `UnityLifecycleEventDispatcher : MonoBehaviour`，负责把 `Awake`、`Start`、`Update`、`FixedUpdate`、`LateUpdate`、`OnApplicationFocus`、`OnApplicationPause`、`OnApplicationQuit` 转发为事件。
- [ ] 设计 Unity 生命周期事件的数据类型，优先使用当前 `MEventBus` 的强类型 `IEvent` 模式，而不是照搬旧 `BuiltInEvent` 双字典结构。
- [ ] 评估 Unity 生命周期桥接效果：若与当前 DI 生命周期冲突、需要隐式全局对象、或引入过重样板代码，则不添加该尝试项，并在实现记录中说明原因。
- [ ] 补充 `[feat]` 测试或验证：至少覆盖字符串事件 Remove / Clear、泛型事件 Remove / Clear、全量 Clear；如保留 Unity 生命周期桥接，则补充最小验证脚本或测试辅助组件。
- [ ] 编译检查，确认不修改 `ProjectSettings/`、`Packages/manifest.json`，不新增 `.asmdef`，不删除或重新生成 `.meta`。
- [ ] 在实现记录中说明本次没有整体迁移旧 `MEventSystem` 的原因，以及旧 `MonoSingleton` 生命周期与当前 DI Provider 生命周期的差异。

## Notes

- 必做项：`MEventBus` 的 Remove / Clear。
- 尝试项：`UnityLifecycleEventProvider + UnityLifecycleEventDispatcher`，若验证不佳允许不添加，但必须记录原因。
- 不建议保留旧版 `MEventSystem.AddListener(...)` 静态兼容层，除非后续明确有旧代码兼容需求。
- 旧项目 Event 模块实际路径为 `D:\___UNITY___\MFramework_OLD\Assets\MFramework\Scripits\Runtime\Module\Event\`，用户最初提到的 `EventMoudle` 与实际路径不完全一致。
