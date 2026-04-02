# DialogPanel 选项按钮配置指南

## 📋 配置步骤

### 第 1 步：打开 DialogPanel 预制体

1. 在 Unity Project 窗口中导航到：`Resources/UI/DialogPanel.prefab`
2. 双击打开预制体编辑模式（或直接在 Hierarchy 中编辑）

---

### 第 2 步：确认 Selection 结构

确认 Selection 的层级结构如下：

```
DialogPanel
    ├── Talk
    │   ├── Txt_words
    │   └── Btn_nextstep
    └── Selection
        └── Btn_selection  ← 这个是模板按钮
```

**Btn_selection 应该包含：**
- Button 组件
- Image 组件
- 子物体（Text，用于显示按钮文字）

---

### 第 3 步：配置 DialogPanel 组件

1. 选中 DialogPanel GameObject（根物体）
2. 在 Inspector 中找到 `DialogPanel` 脚本组件
3. 配置以下字段：

| 字段名 | 说明 | 配置方法 |
|--------|------|----------|
| Txt Words | 显示对话文字的 Text 组件 | 拖入 Txt_words |
| Btn Nextstep | 下一步按钮 | 拖入 Btn_nextstep |
| Talk | Talk GameObject | 拖入 Talk |
| Selection | Selection GameObject | 拖入 Selection |
| **Btn Selection Template** | **选项按钮模板** | **拖入 Btn_selection** ⭐ |

---

### 第 4 步：调整 Horizontal Layout Group（可选）

Selection GameObject 上已经有 `Horizontal Layout Group` 组件，可以根据需要调整：

**推荐配置：**
```
Child Alignment: MiddleCenter  (居中)
Spacing: 20                    (按钮间距，可调整)
Child Force Expand Width: false
Child Force Expand Height: false
Child Control Width: false
Child Control Height: false
```

**Padding（内边距）：**
```
Left: 50
Right: 50
Top: 0
Bottom: 0
```

---

### 第 5 步：调整 Btn_selection 样式（可选）

如果需要修改按钮样式：

1. 选中 Btn_selection
2. 修改 Image 组件的 Color（颜色）
3. 修改 Sprite（图片）
4. 修改 RectTransform 的 SizeDelta（大小）

**默认大小：**
```
Width: 220.4
Height: 85.3
```

---

## 🧪 测试配置

### 创建测试 DialogData

1. 在 Project 窗口右键 → Create → Data → DialogData
2. 命名：`Test_OptionDialog`
3. 配置内容：

```
NPC 名称：测试 NPC

对话内容（dialogPages）：
  [0] "你好，勇敢的冒险者！"
  [1] "我这里有几个选项供你选择。"

选项（dialogOptions）：
  [0] "查看商品"
  [1] "询问任务"
  [2] "闲聊"
  [4] "离开"
```

---

### 运行测试

1. 将 DialogData 拖入场景中 NPC 组件的 `dialogData` 字段
2. 运行游戏
3. 点击 NPC → 移动到 NPC 身边
4. 显示对话："你好，勇敢的冒险者！"
5. 点击下一步 → 显示："我这里有几个选项供你选择。"
6. 点击下一步 → **显示 4 个选项按钮** ✅

---

## ✅ 验证配置是否正确

### 检查清单：

- [ ] DialogPanel 组件的 `Btn Selection Template` 字段已赋值
- [ ] Btn_selection 有 Button 组件
- [ ] Btn_selection 有 Image 组件
- [ ] Btn_selection 有子物体 Text（用于显示文字）
- [ ] Selection 有 Horizontal Layout Group 组件
- [ ] 运行游戏时，选项按钮能正确显示
- [ ] 点击选项按钮能正确触发回调

---

## 🐛 常见问题

### 问题 1：按钮不显示

**可能原因：**
- `btn_selectionTemplate` 未赋值
- Selection GameObject 未激活

**解决方法：**
```csharp
// 检查控制台是否有错误日志
// "Btn_selection 模板未设置！"
```

---

### 问题 2：按钮重叠在一起

**可能原因：**
- Horizontal Layout Group 配置错误
- Spacing 设置为 0

**解决方法：**
1. 选中 Selection GameObject
2. 检查 Horizontal Layout Group 组件
3. 调整 Spacing 为 20 或更大

---

### 问题 3：按钮文字不显示

**可能原因：**
- Btn_selection 的子物体没有 Text 组件
- Text 组件的字体配置错误

**解决方法：**
1. 展开 Btn_selection
2. 确认子物体有 Text 组件
3. 检查 Text 组件的 Font 和 Font Size

---

### 问题 4：按钮点击没反应

**可能原因：**
- Button 组件的 OnClick 事件被覆盖
- Image 的 Raycast Target 未勾选

**解决方法：**
1. 选中 Btn_selection
2. 检查 Button 组件
3. 确保 Image 组件的 Raycast Target 已勾选

---

## 🎨 样式自定义

### 修改按钮颜色

```csharp
// 在 Inspector 中修改 Btn_selection 的 Image 组件
Color: RGBA(255, 255, 255, 200)  // 半透明白色
```

### 修改按钮大小

```csharp
// 修改 RectTransform 的 SizeDelta
Width: 250
Height: 100
```

### 修改按钮间距

```csharp
// 修改 Horizontal Layout Group 的 Spacing
Spacing: 30
```

### 修改按钮排列方式

如果想垂直排列，修改 Horizontal Layout Group：
```
Child Alignment: UpperCenter
```

或者改用 Vertical Layout Group。

---

## 🚀 扩展功能

### 添加选项图标

1. 修改 DialogData，添加图标字段：
```csharp
[System.Serializable]
public class DialogOption
{
    public string optionText;
    public Sprite icon;
}
public List<DialogOption> dialogOptions;
```

2. 修改 ShowOptions 方法，同时设置图标：
```csharp
Image buttonIcon = newButton.GetComponentInChildren<Image>();
buttonIcon.sprite = options[index].icon;
```

### 添加选项条件

1. 添加前置条件字段：
```csharp
public string requiredFlag;  // 需要的前置条件
public int requiredLevel;    // 需要的等级
```

2. 在 ShowOptions 中判断条件：
```csharp
if (!CheckCondition(option))
{
    newButton.interactable = false;  // 禁用按钮
}
```

---

## 📊 配置完成后的效果

```
对话流程：
  ↓
显示第 1 页："你好，勇敢的冒险者！"
  ↓
点击下一步
  ↓
显示第 2 页："我这里有几个选项供你选择。"
  ↓
点击下一步
  ↓
显示选项按钮（水平排列）：
┌──────────────┐ ┌──────────────┐ ┌──────────────┐ ┌──────────────┐
│  查看商品    │ │  询问任务    │ │  闲聊        │ │  离开        │
└──────────────┘ └──────────────┘ └──────────────┘ └──────────────┘
  ↓
点击任意按钮
  ↓
关闭对话
```

---

## 📝 总结

配置完成后，系统会自动：
- ✅ 根据 dialogOptions 数量创建按钮
- ✅ 设置按钮文字为 dialogOptions 中的内容
- ✅ 使用 Btn_selection 的样式
- ✅ 自动水平排列按钮
- ✅ 点击按钮触发回调

如果在配置过程中遇到问题，请检查控制台错误日志和上述检查清单。
