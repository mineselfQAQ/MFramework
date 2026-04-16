# Task: Pool 模块迁移

## Steps

- [x] 以 `D:\___UNITY___\MFramework_OLD\Assets\MFramework\Scripits\Runtime\Module\Pool\` 的正式对象池实现为唯一迁移基线，梳理需要保留的能力与当前框架下不再适用的旧调用方式。 | Files: `Assets/MFramework/Module/Pool/ObjectPoolContainer.cs`, `Assets/MFramework/Module/Pool/ObjectPool.cs`, `Assets/MFramework/Module/Pool/MPoolManager.cs`
- [x] 在 `Assets/MFramework/Module/Pool/` 下规划模块文件结构，保持不新增 `.asmdef`、不改动既有模块边界，并统一到当前仓库的命名空间与代码风格。 | Files: `Assets/MFramework/Module/Pool.meta`, `Assets/MFramework/Module/Pool/ObjectPoolContainer.cs`, `Assets/MFramework/Module/Pool/ObjectPool.cs`, `Assets/MFramework/Module/Pool/MPoolManager.cs`, `Assets/MFramework/Module/Pool/PoolServiceProvider.cs`
- [x] 迁移 `ObjectPoolContainer<T>`，保留句柄语义与 `Used` 状态管理，作为对象池回收凭据的基础类型。 | Files: `Assets/MFramework/Module/Pool/ObjectPoolContainer.cs`
- [x] 迁移 `ObjectPool<T>`，保留预热、按需创建、回收复用的核心逻辑，并补齐边界保护，避免把旧实现中的隐式假设原样带入。 | Files: `Assets/MFramework/Module/Pool/ObjectPool.cs`
- [x] 按当前框架重构 `MPoolManager`：不再继承 `MMonoSingleton`，改为普通类或服务类，对外承担 `WarmPool`、`SpawnObject`、`ReleaseObject` 能力。 | Files: `Assets/MFramework/Module/Pool/MPoolManager.cs`
- [x] 为 Pool 模块设计接入方式，优先采用 `IServiceProvider + MIOCContainer` 注册对象池服务，使上层通过框架服务访问而不是通过全局单例访问。 | Files: `Assets/MFramework/Module/Pool/PoolServiceProvider.cs`, `Assets/MFrameworkExamples/Pool/MEntry.cs`
- [x] 明确对外 API：保留旧版以 `ObjectPoolContainer<GameObject>` 作为回收句柄的核心用法，同时调整调用入口以适配当前框架的服务获取方式。 | Files: `Assets/MFramework/Module/Pool/ObjectPoolContainer.cs`, `Assets/MFramework/Module/Pool/MPoolManager.cs`, `Assets/MFrameworkExamples/Pool/MEntry.cs`
- [x] 在 `Assets/MFrameworkExamples/Pool/` 下补一个最小示例，验证建池、取对象、回收对象、首次自动建池等主路径能按新框架方式使用。 | Files: `Assets/MFrameworkExamples/Pool.meta`, `Assets/MFrameworkExamples/Pool/MEntry.cs`
- [x] 在 `Assets/Tests/` 下补最小测试，优先覆盖 `ObjectPool<T>` 的纯逻辑行为与关键边界，如空池取对象、回收后复用、重复回收保护。 | Files: `Assets/Tests/Pool.meta`, `Assets/Tests/Pool/ObjectPoolTests.cs`
- [x] 交付前完成自检，确认模块目录、命名空间、服务接入方式、示例使用方式与旧项目能力映射都一致，并列出“保留点 / 调整点”。 | Files: `.ai/output/260416_Pool 模块迁移/implement.md`

## Plan Source

- Source: `D:\___UNITY___\MFramework\.ai\output\260416_Pool 模块迁移\plan.md`
- SHA256: `7F6BF7575E63E1699D5F811255ED96399BE8A5D46D4A90B3A3E5BCB35F279C1C`

## Notes

- 保留点：`ObjectPoolContainer<T>` 仍作为借出/归还句柄；`ObjectPool<T>` 仍保留预热、按需创建、回收复用三条主路径；`MPoolManager` 仍保留 `WarmPool`、`SpawnObject`、`ReleaseObject` 的旧版核心入口。
- 调整点：去除 `MMonoSingleton` 依赖，改为 `PoolServiceProvider + MIOCContainer.Default.Resolve<MPoolManager>()` 的服务化访问方式。
- 调整点：为 `ObjectPool<T>` 增加空句柄、异池句柄、重复归还保护，并显式暴露 `UnusedCount`、`UsedCount`、`TotalCount` 便于测试与诊断。
- 调整点：`MPoolManager` 现在区分“自建根节点”和“外部父节点”，避免在 `Shutdown()` 时误删外部传入的父节点，同时补充实例追踪，保证不同挂载父节点下都能统一清理。
- 未执行 Unity Editor 编译与 Test Runner 运行；当前仅完成代码审阅、`git diff --check` 与 `.meta` GUID 冲突检查。
- 当前工作区原本存在 `.ai/output/260416_Pool 模块迁移/plan.md` 删除状态，未纳入本次实现修改。

## PR Status

- Fork: Done
- Push: Done
- PR: Done
- Reason: 已推送到 `fork/ai/pool-migration`，并创建 PR：`https://github.com/mineselfQAQ/MFramework/pull/2`
