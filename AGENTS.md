# `AGENTS.md` — MFramework

## 项目概述
- 引擎：Unity `6000.0.61f1`（见 `ProjectSettings/ProjectVersion.txt`）。
- 渲染管线：URP（`com.unity.render-pipelines.universal` 17.0.4）。
- 典型用途：MFramework 运行时核心与扩展、示例场景与测试。

## 目录结构速览
- MFramework相关
  - `Assets/MFramework/`：框架源码
    - `Assets/MFramework/Core/`：Unity层核心
    - `Assets/MFramework/CoreEx/`：C#层核心
    - `Assets/MFramework/模块名/`：模块实现
  - `Assets/MFrameworkExamples/`：示例内容
  - `Assets/MFrameworkCSTests/`：框架无关测试
  - `Assets/Tests/`：单元测试
  - `Assets/Resources/`：Resources 资源目录
- 其它
  - `Packages/manifest.json`：包依赖清单
  - `ProjectSettings/`：项目设置

## 禁止行为（重要）

AI 代理在修改项目时 **禁止执行以下操作**：

- 删除或重新生成 `.meta` 文件（会导致 GUID 变化）。
- 随意创建新的 `.asmdef`。
- 修改 `ProjectSettings/` 下的文件，除非任务明确要求。
- 修改 `Packages/manifest.json` 中的依赖版本，除非明确说明原因。
- 移动 `Assets/MFramework/` 下已有模块的目录结构。

## 代码放置规则

新增代码**必须遵守**：

- 框架代码 → `Assets/MFramework/模块名/`
- Unity 相关底层 → `Assets/MFramework/Core/`
- 纯 C# 层 → `Assets/MFramework/CoreEx/`
- 示例代码 → `Assets/MFrameworkExamples/`
- 测试代码 → `Assets/Tests/`

## 架构层级

MFramework 分为三层：

1. Core（Unity 层）
   - MonoBehaviour
   - ScriptableObject
   - Unity API
2. CoreEx（纯 C# 层）
   - 不依赖 UnityEngine
   - 可单元测试
3. Modules（模块）
   - 基于 Core/CoreEx 构建

## asmdef 规则

- 项目已有 asmdef，不要新增新的 asmdef。
- 修改代码前确认目标 asmdef 依赖关系。
- 不要跨 asmdef 引用未声明依赖的模块。

## 通用修改流程

1. 定位相关模块
2. 确认 asmdef 依赖
3. 修改源码
4. 编译通过
5. 补充测试（如需要）

## 通用修改指南
- 优先定位到 `Assets/MFramework/` 下的源码进行修改。
- 变更前先确认影响的 `asmdef` 与模块边界，避免跨模块强耦合。
- 新增 API 时尽量补充对应测试（`Assets/Tests/` / `Assets/MFrameworkExamples/`）。
- 如果涉及项目设置或包版本变更，请在说明中明确指出具体文件与原因。

## 命令

### Agent切换

本仓库支持 **多角色 AI Agent 工作流**。
当用户使用命令开头时，AI 必须切换到对应角色执行任务。

注意事项：

- 当用户以命令开启对话时，必须首先说明身份，例如：
  `我是 Planner，负责任务规划。`
- 每个角色必须严格遵守对应规则文件：
  `.ai/agents/<role>.md`
- 角色可以在后续对话中切换，但必须明确说明。
- 如果用户没有使用命令，则默认使用 **CodeHelper** 身份分析任务。

#### `/planner <task>`

切换为 **Planner** 身份。
规则来源：
`.ai/agents/planner.md`

#### `/coder <task>`

切换为 **Coder** 身份。
规则来源：
`.ai/agents/coder.md`

#### `/reviewer <task>`

切换为 **Reviewer** 身份。
规则来源：
`.ai/agents/reviewer.md`

#### `/tester <task>`

切换为 **Tester** 身份。
规则来源：
`.ai/agents/tester.md`