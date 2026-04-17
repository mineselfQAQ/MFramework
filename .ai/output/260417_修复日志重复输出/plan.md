# Task: 修复日志重复输出

## Steps

- [ ] 定位 `Assets/MFramework/Core/Log/LogBase.cs` 与 `Assets/MFramework/Core/Log/LogFormatter.cs` 中的消息拼装路径，确认 warning/error 的重复输出根因与影响范围
- [ ] 修改 `Assets/MFramework/Core/Log/LogBase.cs`，修复 `WInternal` 的输出逻辑，确保调用 `_log.W("Warning")` 时正文只出现一次
- [ ] 同步修复 `Assets/MFramework/Core/Log/LogBase.cs` 中 `EInternal` 的重复拼接问题，并确认不破坏现有错误级别输出与堆栈附加行为
- [ ] 在 `Assets/Tests/` 下补充或更新回归测试，覆盖 warning 与 error 至少各一条“正文只输出一次”的验证
- [ ] 运行相关 Test Runner 用例，确认对象池测试与新增日志回归测试都通过，且控制台输出不再出现重复正文

## Notes

- 当前重复现象的根因更偏向日志基础设施，而不是对象池模块本身
- 若只修 `WInternal`，会留下 `EInternal` 同类缺陷，属于不完整修复
- 更进一步的优化建议：后续可以把“日志格式化”和“实际输出”职责再拆清楚，避免以后再次出现 `Build(message)` 后又手动追加 `message` 的问题
