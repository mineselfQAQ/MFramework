# Task: Pool 模块迁移

## Steps

- [ ] 以 `D:\___UNITY___\MFramework_OLD\Assets\MFramework\Scripits\Runtime\Module\Pool\` 的正式对象池实现为唯一迁移基线，梳理需要保留的能力与当前框架下不再适用的旧调用方式。
- [ ] 在 `Assets/MFramework/Module/Pool/` 下规划模块文件结构，保持不新增 `.asmdef`、不改动既有模块边界，并统一到当前仓库的命名空间与代码风格。
- [ ] 迁移 `ObjectPoolContainer<T>`，保留句柄语义与 `Used` 状态管理，作为对象池回收凭据的基础类型。
- [ ] 迁移 `ObjectPool<T>`，保留预热、按需创建、回收复用的核心逻辑，并补齐边界保护，避免把旧实现中的隐式假设原样带入。
- [ ] 按当前框架重构 `MPoolManager`：不再继承 `MMonoSingleton`，改为普通类或服务类，对外承担 `WarmPool`、`SpawnObject`、`ReleaseObject` 能力。
- [ ] 为 Pool 模块设计接入方式，优先采用 `IServiceProvider + MIOCContainer` 注册对象池服务，使上层通过框架服务访问而不是通过全局单例访问。
- [ ] 明确对外 API：保留旧版以 `ObjectPoolContainer<GameObject>` 作为回收句柄的核心用法，同时调整调用入口以适配当前框架的服务获取方式。
- [ ] 在 `Assets/MFrameworkExamples/Pool/` 下补一个最小示例，验证建池、取对象、回收对象、首次自动建池等主路径能按新框架方式使用。
- [ ] 在 `Assets/Tests/` 下补最小测试，优先覆盖 `ObjectPool<T>` 的纯逻辑行为与关键边界，如空池取对象、回收后复用、重复回收保护。
- [ ] 交付前完成自检，确认模块目录、命名空间、服务接入方式、示例使用方式与旧项目能力映射都一致，并列出“保留点 / 调整点”。

## Notes

- 当前仓库已经存在 `Assets/MFramework/Module/Json/` 这种模块落位方式，所以 Pool 模块放到 `Assets/MFramework/Module/Pool/` 是合理且一致的。
- `MMonoSingleton` 被排除后，旧版 `MPoolManager.Instance` 不能直接继承保留；如果强行兼容，只会破坏当前框架风格。
- 推荐首版直接走“服务化接入”，这样后面扩展多池配置、生命周期管理、场景级释放时更自然。
- 如果后续仍希望保留“旧项目一行调用”的使用体验，可以作为第二阶段再补一个轻量静态门面，但不建议作为首版核心方案。
