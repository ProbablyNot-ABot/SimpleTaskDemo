## 项目简介

SimpleQuestDemo 是一个基于 Unity 的轻量级 RPG 任务系统演示项目。项目采用**管理器架构**和**事件驱动设计**，实现了完整的任务流程：**接取任务 → 完成任务 → 提交任务 → 领取奖励**。

---

## ️ 系统架构

```
┌─────────────────────────────────────────────────┐
│              GameManager                        │
│  (游戏流程控制、敌人管理、任务进度触发)          │
└─────────────────────────────────────────────────┘
           │
    ┌──────┼──────┬──────────┬──────────┐
    │      │      │          │          │
    ▼      ▼      ▼          ▼          ▼
┌────────┐ ┌────────┐ ┌────────┐ ┌────────┐ ┌────────┐
│UIManager│ │QuestManager│ │DialogManager│ │NPCManager│ │ObjectPoolManager│
│(UI 管理) │ │(任务管理) │ │(对话管理) │ │(NPC 管理) │ │(对象池) │
└────────┘ └────────┘ └────────┘ └────────┘ └────────┘
    │           │           │           │
    ▼           ▼           ▼           ▼
┌────────┐ ┌────────┐ ┌────────┐ ┌────────┐
│BasePanel│ │TaskData│ │DialogData│ │  NPC   │
└────────┘ └────────┘ └────────┘ └────────┘
```

---

## 项目结构

```
Assets/
├── Scripts/
│   ├── Manager/
│   │   ├── GameManager.cs           # 游戏管理器
│   │   ├── UIManager.cs             # UI 管理器
│   │   ├── QuestManager.cs          # 任务管理器
│   │   ├── DialogManager.cs         # 对话管理器
│   │   ├── NPCManager.cs            # NPC 管理器
│   │   └── ObjectPoolManager.cs     # 对象池管理器
│   ├── Player/
│   │   ├── PlayerMove.cs            # 玩家移动
│   │   └── SimpleBulletShooter.cs   # 子弹射击
│   ├── Enemy/
│   │   └── EnemyController.cs       # 敌人 AI
│   ├── Npc/
│   │   └── NPC.cs                   # NPC 逻辑
│   ├── UI/
│   │   ├── Quest/
│   │   │   ├── QuestListPanel.cs    # 任务列表
│   │   │   ├── QuestPanel.cs        # 任务详情
│   │   │   └── Btn_OnQuest.cs       # 任务按钮
│   │   └── Dialog/
│   │       └── DialogPanel.cs       # 对话面板
│   └── Data/
│       ├── TaskData.cs              # 任务数据
│       └── DialogData.cs            # 对话数据
├── Resources/
│   ├── UI/                          # UI 预制体
│   ├── Enemies/                     # 敌人预制体
│   └── Effects/                     # 特效预制体
└── Scenes/                          # 场景文件
```

---

## 核心功能

### 任务系统
- 多类型任务：杀敌、收集、NPC 对话
- 任务进度追踪：事件驱动，自动更新
- 任务状态管理：可接取、进行中、已完成
- 前置任务：支持任务依赖关系
- 任务奖励：道具、金币等奖励发放

### NPC 对话系统
- 树状对话：支持多分支、多选项
- 任务交互：接取/提交任务一体化
- 任务标记：NPC 头顶显示任务状态
- 条件对话：根据任务状态显示不同对话

### 战斗系统
- 子弹射击：枪口火焰、子弹轨迹、命中特效
- 对象池优化：高效管理子弹和特效
- 敌人 AI：追踪玩家、攻击、死亡
- 伤害回调：子弹命中后造成伤害

### UI 系统
- 层级管理：底层、中层、顶层、世界 UI
- 面板管理：打开/关闭/切换面板
- 任务界面：任务列表、任务详情、进度显示
- 对话界面：对话文本、选项按钮

### 性能优化
- 对象池：减少 GC，提升性能
- 事件系统：解耦模块，降低耦合
- 组件缓存：避免重复 GetComponent
- 单例模式：全局访问管理器

---

## 核心系统设计

### 任务系统 (QuestManager)

**任务状态流转：**
```
None → CanReceive → InProgress → Completed
```

**任务进度更新流程：**
```
敌人死亡
    ↓
GameManager.OnEnemyKilled(enemy)
    ↓
QuestManager.UpdateQuestProgress(target)
    ↓
触发事件 OnTaskProgressUpdated
    ↓
UI 自动更新进度
```

### 对话系统 (DialogManager)

**对话树结构：**
```csharp
DialogNode
├── nodeId
├── dialogText
├── options[]
│   ├── optionText
│   └── nextNodeId
└── nextNodeId
```

### 对象池系统 (ObjectPoolManager)

**对象生命周期：**
```
创建 → 使用 → 回收 → 复用
  ↓        ↓        ↓        ↓
Instantiate Active   Inactive Reuse
```

**Hierarchy 结构：**
```
ObjectPoolManager
└─ EnemyZombies (池容器)
   ├─ Enemy_Zombie (未激活)
   └─ Enemy_Zombie (未激活)

Canvas
└─ QuestMarks (池容器)
   ├─ QuestMark (未激活)
   └─ QuestMark (未激活)
```

---

## 技术栈

- **设计模式**：单例模式、对象池模式、观察者模式
- **数据驱动**：ScriptableObject
- **架构模式**：管理器架构、事件驱动

---
