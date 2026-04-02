using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 对话面板 - 只负责 UI 显示
/// </summary>
public class DialogPanel : BasePanel
{
    [Header("UI 组件")]
    [SerializeField] private Text txt_words;
    [SerializeField] private Button btn_nextstep;
    [SerializeField] private GameObject talk;
    [SerializeField] private GameObject selection;
    [SerializeField] private Button btn_selectionTemplate;  // 选项按钮模板
    
    [Header("打字效果设置")]
    [SerializeField] private float charDelay = 0.05f;
    
    // 内部状态
    private StringBuilder sb = new StringBuilder();
    private Coroutine typingCoroutine;
    private string currentText = "";
    private List<Button> optionButtons = new List<Button>();  // 存储选项按钮
    
    // 对外暴露的属性和事件
    public bool IsTyping { get; private set; }
    public event System.Action OnClick;  // 点击事件，外部订阅处理逻辑
    
    protected override void OnInit()
    {
        base.OnInit();
        
        // 如果没有在 Inspector 中赋值，尝试自动获取
        if (btn_selectionTemplate == null && selection != null)
        {
            btn_selectionTemplate = selection.GetComponentInChildren<Button>(true);
        }
        
        // 绑定按钮事件 - 只触发事件，不处理业务逻辑
        if (btn_nextstep != null)
            btn_nextstep.onClick.AddListener(OnButtonClick);
    }
    
    protected override void OnOpen(object data = null)
    {
        base.OnOpen(data);
        
        // 显示 Talk，隐藏 Selection
        if (talk != null)
            talk.SetActive(true);
        
        if (selection != null)
            selection.SetActive(false);
        
        // 如果有数据，显示对话
        if (data is string content)
        {
            Show(content);
        }
    }
    
    protected override void OnClose()
    {
        // 停止打字效果
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        
        // 通知 DialogManager 对话框已关闭
        if (DialogManager.Instance != null && DialogManager.Instance.IsDialogActive)
        {
            DialogManager.Instance.OnDialogPanelClosed();
        }
        
        base.OnClose();
    }
    
    /// <summary>
    /// 显示对话内容（外部调用）
    /// </summary>
    public void Show(string content)
    {
        if (txt_words == null) return;
        if (optionButtons.Count > 0) 
        {
            talk.SetActive(true);
            selection.SetActive(false);
            ClearOptions();
        }
        currentText = content;
        // 如果正在打字，先停止
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        
        typingCoroutine = StartCoroutine(TypeWriter(content));
    }
    
    /// <summary>
    /// 跳过打字，立即显示全部内容（外部调用）
    /// </summary>
    public void SkipTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        txt_words.text = currentText;
        IsTyping = false;
    }
    
    /// <summary>
    /// 设置是否显示 Selection（外部调用）
    /// </summary>
    public void SetSelectionVisible(bool visible)
    {
        if (selection != null)
            selection.SetActive(visible);
    }
    
    /// <summary>
    /// 显示选项按钮（外部调用）
    /// <param name="options">选项文字列表</param>
    /// <param name="onOptionSelected">选项点击事件</param>
    /// </summary>
    public void ShowOptions(List<string> options, System.Action<int> onOptionSelected)
    {
        // 清空旧按钮
        ClearOptions();
        
        // 显示 selection 容器
        if (selection != null)
            selection.SetActive(true);
        
        // 隐藏对话内容
        if (talk != null)
            talk.SetActive(false);
        
        // 动态创建按钮
        if (btn_selectionTemplate == null)
        {
            Debug.LogError("Btn_selection 模板未设置！");
            return;
        }
        
        foreach (var option in options)
        {
            // 实例化模板
            Button newButton = Instantiate(btn_selectionTemplate, selection.transform);
            newButton.gameObject.SetActive(true);
            
            // 设置按钮文字
            Text buttonText = newButton.GetComponentInChildren<Text>();
            if (buttonText != null)
            {
                buttonText.text = option;
            }
            
            // 绑定点击事件
            int index = optionButtons.Count;
            newButton.onClick.AddListener(() =>
            {
                if (onOptionSelected != null)
                {
                    onOptionSelected(index);
                }
            });
            
            // 保存引用
            optionButtons.Add(newButton);
        }
    }
    
    /// <summary>
    /// 清空选项按钮（内部调用）
    /// </summary>
    private void ClearOptions()
    {
        foreach (var button in optionButtons)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }
        optionButtons.Clear();
    }
    
    // 打字效果协程
    private IEnumerator TypeWriter(string text)
    {
        IsTyping = true;
        sb.Clear();
        txt_words.text = "";
        
        foreach (char c in text)
        {
            sb.Append(c);
            txt_words.text = sb.ToString();
            yield return new WaitForSeconds(charDelay);
        }
        
        IsTyping = false;
    }
    
    // 按钮点击 - 只触发事件，不处理业务逻辑
    private void OnButtonClick()
    {
        OnClick?.Invoke();
    }
}
