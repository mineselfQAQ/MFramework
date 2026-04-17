# Task: 修复日志重复输出

## Steps

- [x] 定位 `Assets/MFramework/Core/Log/LogBase.cs` 与 `Assets/MFramework/Core/Log/LogFormatter.cs` 中的消息拼装路径，确认 warning/error 的重复输出根因与影响范围 | Files: `Assets/MFramework/Core/Log/LogBase.cs`, `Assets/MFramework/Core/Log/LogFormatter.cs`
- [x] 修改 `Assets/MFramework/Core/Log/LogBase.cs`，修复 `WInternal` 的输出逻辑，确保调用 `_log.W("Warning")` 时正文只出现一次 | Files: `Assets/MFramework/Core/Log/LogBase.cs`
- [x] 同步修复 `Assets/MFramework/Core/Log/LogBase.cs` 中 `EInternal` 的重复拼接问题，并确认不破坏现有错误级别输出与堆栈附加行为 | Files: `Assets/MFramework/Core/Log/LogBase.cs`
- [x] 在 `Assets/Tests/` 下补充或更新回归测试，覆盖 warning 与 error 至少各一条“正文只输出一次”的验证 | Files: `Assets/Tests/LogDuplicateOutputTests.cs`, `Assets/Tests/LogDuplicateOutputTests.cs.meta`, `Assets/Tests/Tests.asmdef`, `Assets/MFramework/Module/MFramework.Module.asmdef`, `Assets/MFramework/Module/MFramework.Module.asmdef.meta`
- [x] 运行相关 Test Runner 用例，确认对象池测试与新增日志回归测试都通过，且控制台输出不再出现重复正文 | Files: `Assets/Tests/Pool/ObjectPoolTests.cs`, `Assets/Tests/LogDuplicateOutputTests.cs`, `.ai/output/260417_修复日志重复输出/test-results.xml`, `.ai/output/260417_修复日志重复输出/unity-test.log`

## Plan Source

- Source: `D:\___UNITY___\MFramework\.ai\output\260417_修复日志重复输出\plan.md`
- SHA256: `583CCE05A600E01AFA60103ECF30D340365CFD7568EDBB5BACB08A0A54EAE3E5`

## Notes

- 测试使用 `E:\___SOFTWARE___\Unity\Editor\Unity 6000.0.61f1\Editor\Unity.exe` 执行 `-runTests -runSynchronously -testPlatform EditMode -assemblyNames Tests`，结果为 `Passed`，共 5 条用例全部通过。
- 为了让计划中的对象池测试可执行，额外修复了预先存在的测试程序集依赖断链问题：恢复历史上已存在的 `Assets/MFramework/Module/MFramework.Module.asmdef`，并将 `Assets/Tests/Tests.asmdef` 的悬空 GUID 引用更新为当前实际存在的程序集。
- Unity 批处理测试会临时改写 `Packages/manifest.json`、`Packages/packages-lock.json` 以及部分项目设置文件；这些文件的内容已回退到 `HEAD`，未纳入本次任务变更范围。

## PR Status

- Fork: Skipped
- Push: Skipped
- PR: Skipped
- Reason: 本次仅校验了本地 `fork` remote 配置并创建任务分支，没有在本轮实际执行 fork 创建；同时未执行 push 与 PR 流程。
